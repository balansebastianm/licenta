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
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        public StudentController(IAdeverintaRepository adeverintaRepository, 
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            IAttendanceRepository attendanceRepository)
        {
            _adeverintaRepository = adeverintaRepository;
            _enrollmentRepository = enrollmentRepository;
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
            adeverinta.CurrentStatus = 2;
            _adeverintaRepository.Save();
            var enrollments = await _enrollmentRepository.GetAll();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var studentId = _studentRepository.GetIdByUID(userId);
            var attendances = await _attendanceRepository.GetAll();
            foreach (var enrollment in enrollments)
            {
                if(enrollment.StudentId == studentId)
                {
                    foreach(var attendance in attendances)
                    {
                        if (attendance.EnrollmentId == enrollment.Id && attendance.AttendanceDateTime >= adeverinta.StartDate && attendance.AttendanceDateTime <= adeverinta.EndDate)
                        {
                            attendance.Present = true;
                        }
                    }
                }
            }
            _attendanceRepository.Save();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Download(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            var path = "D:\\licenta\\LicentaFinal\\wwwroot\\" + adeverinta.PathToAdeverinta;
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
