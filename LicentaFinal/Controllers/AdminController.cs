using LicWeb.Repositories;
using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMaterieRepository _courseRepository;
        private readonly IProfesorRepository _proffessorRepository;
        private readonly IAdeverintaRepository _adeverintaRepository;
        private readonly IOrarRepository _orarRepository;
        private readonly ISpecializareRepository _specializareRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMaterieRepository _materieRepository;
        private readonly IPrezentaRepository _prezentaRepository;
        private readonly ISituatieFinalaRepository _situatieFinalaRepository;

        public AdminController(UserManager<User> userManager,
            IUserRepository userRepository,
            ApplicationDbContext context,
            IDoctorRepository doctorRepository,
            IMaterieRepository courseRepository,
            IProfesorRepository profesorRepository,
            IAdeverintaRepository adeverintaRepository,
            IOrarRepository orarRepository,
            ISpecializareRepository specializareRepository,
            IStudentRepository studentRepository,
            IMaterieRepository materieRepository,
            IPrezentaRepository prezentaRepository,
            ISituatieFinalaRepository situatieFinalaRepository)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.context = context;
            _doctorRepository = doctorRepository;
            _courseRepository = courseRepository;
            _proffessorRepository = profesorRepository;
            _adeverintaRepository = adeverintaRepository;
            _orarRepository = orarRepository;
            _specializareRepository = specializareRepository;
            _studentRepository = studentRepository;
            _materieRepository = materieRepository;
            _prezentaRepository = prezentaRepository;
            _situatieFinalaRepository = situatieFinalaRepository;
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
                    Debug.WriteLine(course.IdProfCurs);
                    var profCurs = await _proffessorRepository.GetByIdAsync(course.IdProfCurs);
                    var profLab = await _proffessorRepository.GetByIdAsync(course.IdProfSeminar);
                    string profCursId = profCurs.ProfesorUserId;
                    string profLabId = profLab.ProfesorUserId;  
                    var GetNameCourse = await userRepository.GetByIdAsync(profCursId);
                    var GetNameLab = await userRepository.GetByIdAsync(profLabId);
                    string profCursName = GetNameCourse.Nume;
                    string profLabName = GetNameLab.Prenume;
                    var courseViewModel = new CourseViewModel()
                    {
                        Id = course.Id,
                        CourseName = course.NumeMaterie,
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
                course.NumeMaterie = materieViewModel.NumeMaterie;
                course.NrCredite = materieViewModel.NrCredite;
                course.AnStudii = materieViewModel.AnStudii;
                course.ModulStudii = materieViewModel.ModulStudii;
                course.IdProfCurs = materieViewModel.ProfesorCursId;
                course.IdProfSeminar = materieViewModel.ProfesorSeminarId;
                course.ProcentajPrezentaCurs = materieViewModel.ProcentajPrezCurs;
                course.ProcentajPrezentaSeminar = materieViewModel.ProcentajPrezSeminar;
                _courseRepository.Save();

                var orar = await _orarRepository.GetByMaterieIdAsync(materieViewModel.Id);
                orar.ZiCurs = materieViewModel.ZiuaSaptamaniiCurs;
                orar.ZiSeminar = materieViewModel.ZiuaSaptamaniiSeminar;
                var reference = new DateTime(2023, 1, 1);
                TimeOnly timeOnly = materieViewModel.StartTimeCurs;
                reference += timeOnly.ToTimeSpan();
                orar.InceputCurs = reference;
                reference = new DateTime(2023, 1, 1);
                timeOnly = materieViewModel.EndTimeCurs;
                reference += timeOnly.ToTimeSpan();
                orar.FinalCurs = reference;
                reference = new DateTime(2023, 1, 1);
                timeOnly = materieViewModel.StartTimeSeminar;
                reference += timeOnly.ToTimeSpan();
                orar.InceputSeminar = reference;
                reference = new DateTime(2023, 1, 1);
                timeOnly = materieViewModel.EndTimeSeminar;
                reference += timeOnly.ToTimeSpan();
                orar.FinalSeminar = reference;
                _orarRepository.Save();
                TempData["Erorr"] = "Succes";
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
                string PathToPrivateKey = user.Email + ".zip";
                string PathToPublicKey = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\public-key-" + user.Email + ".pem";
                string PathToCertificate = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\certificat-" + user.Email + ".der";
                string PathToCSR = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\certificat-" + user.Email + ".csr";
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
                string PathToPrivateKey = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\" + user.Email + ".zip";
                string PathToPublicKey = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\public-key-" + user.Email + ".pem";
                string PathToCertificate = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\certificat-" + user.Email + ".der";
                string PathToCSR = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\" + user.Email + ".csr";
                string MoveCertificate = "C:\\licenta\\LicentaFinal\\Valid Certificates\\certificat-" + user.Email + ".der";
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
                user.StatusCont = 2;
                userRepository.Save();
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
        public IActionResult AdaugaMaterie()
        {
            Debug.WriteLine("IN GET ==============================");
            var response = new MaterieViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AdaugaMaterie(MaterieViewModel materieViewModel)
        {
            Debug.WriteLine("IN POST ==============================");
            //adauga materia
            var course = new Materie()
            {
                NumeMaterie = materieViewModel.NumeMaterie,
                NrCredite = materieViewModel.NrCredite,
                IdSpecializare = materieViewModel.IdSpecializare,
                AnStudii = materieViewModel.AnStudii,
                ModulStudii = materieViewModel.ModulStudii,
                IdProfCurs = materieViewModel.ProfesorCursId,
                IdProfSeminar = materieViewModel.ProfesorSeminarId,
                ProcentajPrezentaCurs = materieViewModel.ProcentajPrezCurs,
                ProcentajPrezentaSeminar = materieViewModel.ProcentajPrezSeminar,
            };
            _courseRepository.Add(course);
            _courseRepository.Save();
            //stabileste-o in orar
            var orar = new Orar();
            orar.ZiCurs = materieViewModel.ZiuaSaptamaniiCurs;
            orar.ZiSeminar = materieViewModel.ZiuaSaptamaniiSeminar;
            var reference = new DateTime(2023, 1, 1);
            TimeOnly timeOnly = materieViewModel.StartTimeCurs;
            reference += timeOnly.ToTimeSpan();
            orar.InceputCurs = reference;
            reference = new DateTime(2023, 1, 1);
            timeOnly = materieViewModel.EndTimeCurs;
            reference += timeOnly.ToTimeSpan();
            orar.FinalCurs = reference;
            reference = new DateTime(2023, 1, 1);
            timeOnly = materieViewModel.StartTimeSeminar;
            reference += timeOnly.ToTimeSpan();
            orar.InceputSeminar = reference;
            reference = new DateTime(2023, 1, 1);
            timeOnly = materieViewModel.EndTimeSeminar;
            reference += timeOnly.ToTimeSpan();
            orar.FinalSeminar = reference;
            orar.IdMaterie = course.Id;
            _orarRepository.Add(orar);
            _orarRepository.Save();
            //creeaza tabel prezente si situatie finala

            var studenti = await _studentRepository.GetAll();
            if (studenti != null)
            {
                foreach (var student in studenti)
                {
                    if (student.IdSpecializare == course.IdSpecializare && student.AnDeStudii == course.AnStudii && student.ModulStudii == course.ModulStudii)
                    {
                        //prezenta
                        var prezenta = new Prezenta();
                        Debug.WriteLine("STUDENT ID: ");
                        Debug.WriteLine(student.Id);
                        Debug.WriteLine("COURSE ID: ");
                        Debug.WriteLine(course.Id);
                        prezenta.IdStudent = student.Id;
                        prezenta.IdMaterie = course.Id;
                        Debug.WriteLine("====================================");
                        prezenta.Prezenta_C1 = 0;
                        prezenta.Prezenta_C2 = 0;
                        prezenta.Prezenta_C3 = 0;
                        prezenta.Prezenta_C4 = 0;
                        prezenta.Prezenta_C5 = 0;
                        prezenta.Prezenta_C6 = 0;
                        prezenta.Prezenta_C7 = 0;
                        prezenta.Prezenta_S1 = 0;
                        prezenta.Prezenta_S2 = 0;
                        prezenta.Prezenta_S3 = 0;
                        prezenta.Prezenta_S4 = 0;
                        prezenta.Prezenta_S5 = 0;
                        prezenta.Prezenta_S6 = 0;
                        prezenta.Prezenta_S7 = 0;

                        var result = _prezentaRepository.Add(prezenta);
                        if (!result)
                        {

                            Debug.WriteLine(result.ToString());
                        }
                        else
                        {
                            _prezentaRepository.Save();
                        }

                        //situatie
                        var situatie = new SituatieFinala();
                        situatie.IdStudent = student.Id;
                        situatie.IdMaterie = course.Id;
                        situatie.NotaSeminar = 0;
                        situatie.NotaCurs = 0;
                        situatie.NotaSumativ = 0;
                        situatie.Medie = 0;
                        situatie.EsteRestant = 0;
                        _situatieFinalaRepository.Add(situatie);
                        _situatieFinalaRepository.Save();
                    }
                }
            }
              
            TempData["Succes"] = "Materia a fost adaugata";

            return RedirectToAction("AdaugaMaterie");

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
                var newUser = new User()
                {
                    Email = email,
                    UserName = email,
                    TokenInregistrare = regcode,
                    StatusCont = 0

                };
                var newUserResponse = await userManager.CreateAsync(newUser, "temppassword");

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
            //semnare digitala
            CertificateAuthority CA = new CertificateAuthority();
            string signature = CA.SignData(adeverinta.PathToAdeverinta, "RootCert-private.pem", 1);
            adeverinta.SemnaturaUniversitate = signature;
            adeverinta.CurrentStatus = 2;
            _adeverintaRepository.Save();
            return RedirectToAction("GestioneazaAdeverinte");
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
        [HttpGet]
        public ActionResult AdaugaSpecializare()
        {
            var response = new AdaugaSpecializareViewModel();
            return View(response);
        }

        [HttpPost]
        public Task<IActionResult> AdaugaSpecializare(AdaugaSpecializareViewModel adaugaSpecializare)
        {
            var specializare = new Specializare();
            specializare.NumeSpecializare = adaugaSpecializare.NumeSpecializare;
            var resultAdauga = _specializareRepository.Add(specializare);
            if (resultAdauga)
            {
                TempData["Succes"] = "Specializarea a fost adaugata";
            }
            else
            {
                TempData["Error"] = "Specializarea nu a putut fi adaugata";
            }
            return Task.FromResult<IActionResult>(View(adaugaSpecializare));
        }
    }
}
