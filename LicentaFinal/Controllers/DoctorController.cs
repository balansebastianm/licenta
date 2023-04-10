using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.Repositories;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace LicWeb.Controllers
{
    [Authorize(Roles = "doctor")]
    
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment Environment;
        private readonly IAdeverintaRepository _adeverintaRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IDoctorRepository _doctorRepository;
        public DoctorController(IWebHostEnvironment environment
            ,IAdeverintaRepository adeverintaRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            IDoctorRepository doctorRepository, 
            ApplicationDbContext context)
        {
            Environment = environment;
            _adeverintaRepository = adeverintaRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _doctorRepository = doctorRepository;
            _context = context;
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
                string PKEYFileName = $@"{Guid.NewGuid()}.pem"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, PKEYFileName), FileMode.Create))
                {
                    PKEY.CopyTo(stream);
                    uploadedPKEY.Add(PKEYFileName);
                    ViewBag.Message += PKEYFileName + ",";
                    stream.Close();
                }
                var adeverinta = adeverintaViewModel.Adeverinta;
                List<string> uploadedAdeverinta = new List<string>();
                string adeverintaFileName = $@"{Guid.NewGuid()}.pdf"; ;
                using (FileStream stream = new FileStream(Path.Combine(path, adeverintaFileName), FileMode.Create))
                {
                    adeverinta.CopyTo(stream);
                    uploadedAdeverinta.Add(adeverintaFileName);
                    ViewBag.Message += adeverintaFileName + ",";
                    stream.Close();
                }
                CertificateAuthority CA = new CertificateAuthority();
                string signature = CA.SignData(adeverintaFileName, PKEYFileName);
                _userManager.GetUserAsync(HttpContext.User);
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Debug.WriteLine("USER ID:" + userId);
                var DocId = _doctorRepository.GetByUID(userId);
                Debug.WriteLine("DOCTOR ID:" + DocId.Id);
                int passed;
                var getPath = _context.Adeverinte.FirstOrDefault(b => b.DoctorId == DocId.Id);
                var getStudentId = _userRepository.GetIdByEmail(adeverintaViewModel.EmailStudent);
                if (CA.VerifySignature(DocId.CheiePublica, signature, adeverintaFileName))
                {
                    passed = 1;
                }
                else
                {
                    passed = 0;
                }
                var adeverintaToDb = new Adeverinta()
                {
                    EncryptedData = signature,
                    PathToAdeverinta = adeverintaFileName,
                    IdStudent = getStudentId,
                    StartDate = adeverintaViewModel.MotivareDin,
                    EndDate = adeverintaViewModel.MotivarePana,
                    DoctorId = DocId.Id,
                    Passed = passed
                };
                _adeverintaRepository.Add(adeverintaToDb);
                _adeverintaRepository.Save();
            }
            return View();
        }
    }
}
