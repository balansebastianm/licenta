using LicWeb.Repositories;
using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection.Emit;
using System.Diagnostics;
using System;
using System.IO;
namespace LicWeb.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IUserRepository userRepository;
        private readonly ApplicationDbContext context;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IProfesorRepository _proffessorRepository;
        private readonly ISeminarRepository _seminarRepository;
        private readonly IAdeverintaRepository _adeverintaRepository;

        public AdminController(UserManager<User> userManager,
            IUserRepository userRepository,
            ApplicationDbContext context,
            IDoctorRepository doctorRepository,
            ICourseRepository courseRepository,
            ISeminarRepository seminarRepository,
            IProfesorRepository profesorRepository,
            IAdeverintaRepository adeverintaRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.context = context;
            _doctorRepository = doctorRepository;
            _courseRepository = courseRepository;
            _seminarRepository = seminarRepository;
            _proffessorRepository = profesorRepository;
            _adeverintaRepository = adeverintaRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GestioneazaUtilizatori()
        {
            var users = userManager.Users;

            return View(users);
        }
        public async Task<IActionResult> GestioneazaInregistrari()
        {
            var users = await userRepository.GetAll();
            List<User> resultUser = new List<User>();
            foreach (var user in users)
            {
                var userViewModel = new User()
                {
                    Id = user.Id,
                    Email = user.Email,
                    CNP = user.CNP,
                    SerieBuletin = user.SerieBuletin,
                    UserName = user.UserName,
                    StatusCont = user.StatusCont
                };
                resultUser.Add(userViewModel);
            }
            return View(resultUser);
        }
        public async Task<IActionResult> GestioneazaMaterii()
        {
            var courses = await _courseRepository.GetAll();

            List<CourseViewModel> resultCourse = new List<CourseViewModel>();
            foreach (var course in courses)
            {
                try
                {
                    Debug.WriteLine(course.ProfesorCursId);
                    var profCurs = await _proffessorRepository.GetByIdAsync(course.ProfesorCursId);
                    var IdProfLab = _seminarRepository.GetProfByAssocCourse(course.ProfesorCursId);
                    var profLab = await _proffessorRepository.GetByIdAsync(IdProfLab);
                    string profCursId = profCurs.ProfesorUserId;
                    string profLabId = profLab.ProfesorUserId;
                    var GetNameCourse = await userRepository.GetByIdAsync(profCursId);
                    var GetNameLab = await userRepository.GetByIdAsync(profLabId);
                    string profCursName = GetNameCourse.UserName;
                    string profLabName = GetNameLab.UserName;
                    var courseViewModel = new CourseViewModel()
                    {
                        Id = course.Id,
                        CourseName = course.CourseName,
                        ProfCursName = profCursName,
                        ProfLabName = profLabName
                    };
                    resultCourse.Add(courseViewModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }

            return View(resultCourse);
        }
        [HttpPost]
        public IActionResult ModificaMaterie(int id)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ModificaMaterie(MaterieViewModel materieViewModel)
        {
            var course = await _courseRepository.GetByIdAsync(materieViewModel.Id);
            if (course != null)
            {
                course.CourseName = materieViewModel.NumeMaterie;
                course.DayOfWeek = materieViewModel.ZiuaSaptamaniiCurs;
                var reference = new DateTime(2023, 1, 1);
                TimeOnly timeOnly = materieViewModel.StartTimeCurs;
                reference += timeOnly.ToTimeSpan();
                course.TimeOfDay = reference;
                course.ProfesorCursId = materieViewModel.ProfesorCursId;
                var seminar = await _seminarRepository.GetByCursId(materieViewModel.Id);
                if (seminar != null)
                {
                    seminar.ProfesorSeminarId = materieViewModel.ProfesorSeminarId;
                    seminar.DayOfWeek = materieViewModel.ZiuaSaptamaniiLabSeminar;
                    var reference2 = new DateTime(2023, 1, 1);
                    TimeOnly timeOnly2 = materieViewModel.StartTimeLabSeminar;
                    reference2 += timeOnly2.ToTimeSpan();
                    seminar.TimeOfDay = reference2;
                    _seminarRepository.Save();
                    _courseRepository.Save();
                    TempData["Succes"] = "Cursul a fost actualizat cu succes.";
                }
                else
                {
                    TempData["Erorr"] = "A intervenit o eroare.";
                }

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View();
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {

                    string PathToCertificate = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Valid Certificates\\certificat-" + user.Email + ".der";
                    System.IO.File.Delete(PathToCertificate);
                    return RedirectToAction("GestioneazaUtilizatori");
                }
                return View("GestioneazaUtilizatori");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DenyUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.StatusCont = -1;
                userRepository.Save();
                MailManager m = new MailManager();
                m.SendMail(user.Email,
                    "Inregistrare Platforma",
                    "Cererea dumneavoastra de inregistrare a fost respinsa.\n",
                    "Administrator",
                    "Sistem",
                    "Utilizator",
                    "Invitat");
                string PathToPrivateKey = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\private-key-" + user.Email + ".pem";
                string PathToPublicKey = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\public-key-" + user.Email + ".pem";
                string PathToCertificate = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\certificat-" + user.Email + ".der";
                string PathToCSR = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\certificat-" + user.Email + ".csr";
                System.IO.File.Delete(PathToPrivateKey);
                System.IO.File.Delete(PathToPublicKey);
                System.IO.File.Delete(PathToCertificate);
                System.IO.File.Delete(PathToCSR);
                return RedirectToAction("GestioneazaInregistrari");
            }
            else
            {
                return RedirectToAction("GestioneazaInregistrari");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AllowUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {

                CertificateAuthority CA = new CertificateAuthority();
                CA.GenerateCertFromCSR(user.Email);


                MailManager m = new MailManager();
                string PathToPrivateKey = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\private-key-" + user.Email + ".pem";
                string PathToPublicKey = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\public-key-" + user.Email + ".pem";
                string PathToCertificate = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\certificat-" + user.Email + ".der";
                string PathToCSR = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Certificate Requests\\" + user.Email + ".csr";
                string MoveCertificate = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\Valid Certificates\\certificat-" + user.Email + ".der";
                m.SendMail(user.Email,
                "Inregistrare Platforma",
                "Cererea dumneavoastra de inregistrare a fost aprobata. Certificatul si perechea de chei au fost atasate.\n",
                "Administrator",
                "Sistem",
                "Utilizator",
                "Invitat",
                PathToPrivateKey,
                PathToPublicKey,
                PathToCertificate
                );

                var newDoctor = new DoctorFamilie()
                {
                    CheiePublica = CA.GetPkeyRegister(user.Email),
                    DoctorUserId = user.Id
                };

                var newDoctorResponse = _doctorRepository.Add(newDoctor);
                if (newDoctorResponse)
                {
                    user.StatusCont = 2;
                    userRepository.Save();

                }
                System.IO.File.Delete(PathToPrivateKey);
                System.IO.File.Delete(PathToPublicKey);
                System.IO.File.Delete(PathToCSR);
                System.IO.File.Move(PathToCertificate, MoveCertificate);
                return RedirectToAction("GestioneazaInregistrari");

            }
            else
            {
                return RedirectToAction("GestioneazaInregistrari");
            }
        }
        [HttpGet]
        public ActionResult AdaugaMaterie()
        {
            var response = new MaterieViewModel();
            return View(response);
        }

        [HttpPost]
        public IActionResult AdaugaMaterie(MaterieViewModel materieViewModel)
        {
            var reference = new DateTime(2023, 1, 1);
            TimeOnly timeOnly = materieViewModel.StartTimeCurs;
            reference += timeOnly.ToTimeSpan();
            var course = new Course()
            {
                CourseName = materieViewModel.NumeMaterie,
                DayOfWeek = materieViewModel.ZiuaSaptamaniiCurs,
                TimeOfDay = reference,
                ProfesorCursId = materieViewModel.ProfesorCursId
            };
            var newCourseResponse = _courseRepository.Add(course);
            if (newCourseResponse)
            {
                _courseRepository.Save();
            }
            var reference2 = new DateTime(2023, 1, 1);
            TimeOnly timeOnly2 = materieViewModel.StartTimeLabSeminar;
            reference2 += timeOnly2.ToTimeSpan();
            var find = _courseRepository.GetIdByCourseName(course.CourseName);
            var seminar = new Seminar()
            {
                ProfesorSeminarId = materieViewModel.ProfesorSeminarId,
                DayOfWeek = materieViewModel.ZiuaSaptamaniiLabSeminar,
                TimeOfDay = reference2,
                IdCursAsociat = course.Id
            };

            var newSeminarResponse = _seminarRepository.Add(seminar);
            if (newCourseResponse && newSeminarResponse)
            {
                _seminarRepository.Save();
                TempData["Succes"] = "Materia a fsot adaugata cu succes";
            }
            else
            {
                TempData["Error"] = "A intervenit o eroare";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Invita()
        {
            var response = new InvitaViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Invita(InvitaViewModel invitaViewModel)
        {
            string email, regcode;
            var user = await userManager.FindByEmailAsync(invitaViewModel.Email);
            if (user != null && user.StatusCont != 2)
            {
                TempData["Error"] = "Utilizator deja invitat.";
                return View(invitaViewModel);
            }
            else if (user != null && user.StatusCont == 2)
            {
                TempData["Error"] = "Utilizator exista deja.";
                return View(invitaViewModel);
            }
            else
            {
                Random rd = new Random();
                const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                char[] chars = new char[7];
                for (int i = 0; i < 7; i++)
                {
                    chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
                }
                email = invitaViewModel.Email;
                regcode = new string(chars);

                TempData["Succes"] = "Utilizatorul a fost invitat cu succes.";
                MailManager m = new MailManager();
                m.SendMail(email,
                    "Inregistrare Platforma",
                    "Folositi urmatorul cod pentru inregistrare:\n",
                    regcode,
                    "Administrator",
                    "Sistem",
                    "Utilizator",
                    "Invitat");
                Debug.WriteLine(regcode);
                var newUser = new User()
                {
                    Email = email,
                    UserName = email,
                    TokenInregistrare = regcode,
                    StatusCont = 0

                };
                var newUserResponse = await userManager.CreateAsync(newUser, "temppassword");
                Console.WriteLine(newUserResponse.ToString());

                if (newUserResponse.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, UserRoles.Doctor);

                }
                return View(invitaViewModel);
            }
        }
        public async Task<ActionResult> GestioneazaAdeverinte()
        {
            var adeverinte = await _adeverintaRepository.GetAll();
                return View(adeverinte);
        }
        [HttpPost]
        public async Task<ActionResult> RespingeAdeverinta(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            adeverinta.CurrentStatus = -1;
            _adeverintaRepository.Save();
            return RedirectToAction("GestioneazaAdeverinte");
        }
        [HttpPost]
        public async Task<ActionResult> AprobaAdeverinta(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            adeverinta.CurrentStatus = 1;
            _adeverintaRepository.Save();
            return RedirectToAction("GestioneazaAdeverinte");
        }
        public async Task<IActionResult> Download(int id)
        {
            var adeverinta = await _adeverintaRepository.GetByIdAsync(id);
            var path = "C:\\Users\\Sebi\\source\\repos\\LicentaFinal\\LicentaFinal\\wwwroot\\uploads\\" + adeverinta.PathToAdeverinta;
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
