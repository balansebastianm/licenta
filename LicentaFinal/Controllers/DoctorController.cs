using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace LicWeb.Controllers
{
    [Authorize(Roles = "doctor")]
    public class DoctorController : Controller
    {
        private readonly IWebHostEnvironment Environment;
        private readonly IAdeverintaRepository _adeverintaRepository;
        public DoctorController(IWebHostEnvironment environment
            ,IAdeverintaRepository adeverintaRepository)
        {
            Environment = environment;
            _adeverintaRepository = adeverintaRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(AdeverintaViewModel adeverintaViewModel)
        {
            if (ModelState.IsValid)
            {
                string wwwPath = this.Environment.WebRootPath;
                string contentPath = this.Environment.ContentRootPath;

                string path = Path.Combine(this.Environment.WebRootPath, "uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var PKEY = adeverintaViewModel.CheiePrivata;
                List<string> uploadedPKEY = new List<string>();
                string PKEYFileName = $@"{Guid.NewGuid()}.txt"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, PKEYFileName), FileMode.Create))
                {
                    PKEY.CopyTo(stream);
                    uploadedPKEY.Add(PKEYFileName);
                    ViewBag.Message += PKEYFileName + ",";
                    stream.Close();
                }
                var adeverinta = adeverintaViewModel.Adeverinta;
                List<string> uploadedAdeverinta = new List<string>();
                string adeverintaFileName = $@"{Guid.NewGuid()}.txt"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, adeverintaFileName), FileMode.Create))
                {
                    adeverinta.CopyTo(stream);
                    uploadedAdeverinta.Add(adeverintaFileName);
                    ViewBag.Message += adeverintaFileName + ",";
                    stream.Close();
                }
                CertificateAuthority CA = new CertificateAuthority();
                string signature = CA.SignData(adeverintaFileName, PKEYFileName);
                var adeverintaToDb = new Adeverinta()
                {
                    EncryptedData = signature,
                    PathToAdeverinta = adeverintaFileName,
                    EmailStudent = adeverintaViewModel.EmailStudent,
                    StartDate = adeverintaViewModel.MotivareDin,
                    EndDate = adeverintaViewModel.MotivarePana
                };
                _adeverintaRepository.Add(adeverintaToDb);
                _adeverintaRepository.Save();
            }
            return View();
        }
    }
}
