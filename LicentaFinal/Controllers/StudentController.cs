using LicWeb.Interfaces;
using LicWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace LicWeb.Controllers
{
    public class StudentController : Controller
    {
        private readonly IAdeverintaRepository _adeverintaRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPrezentaRepository _attendanceRepository;
        public StudentController(IAdeverintaRepository adeverintaRepository, 
            IStudentRepository studentRepository,
            IPrezentaRepository attendanceRepository)
        {
            _adeverintaRepository = adeverintaRepository;
            _studentRepository = studentRepository;
            _attendanceRepository = attendanceRepository;
        }
        [Authorize(Roles = "student")]

        public async Task<IActionResult> Index()
        {
            var adeverinte = await _adeverintaRepository.GetAll();
            return View(adeverinte);
        }
        [HttpPost]
        public async Task<ActionResult> RespingeAdeverinta(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            adeverinta.CurrentStatus = -2;
            _adeverintaRepository.Save();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AprobaAdeverinta(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            adeverinta.CurrentStatus = 3;
            _adeverintaRepository.Save();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Download(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            var path = "C:\\licenta\\LicentaFinal\\wwwroot\\uploads\\" + adeverinta.PathToAdeverinta;
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return File(memory, "text/plain", Path.GetFileName(path));
        }
    }
}
