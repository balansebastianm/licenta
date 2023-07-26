using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.Repositories;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace LicWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ICheieRepository _cheieRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IProfesorRepository _profesorRepository;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context,
            IUserRepository userRepository,
            IDoctorRepository doctorRepository,
            ICheieRepository cheieRepository,
            IStudentRepository studentRepository,
            IProfesorRepository profesorRepository)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _cheieRepository = cheieRepository;
            _studentRepository = studentRepository;
            _profesorRepository = profesorRepository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (User.Identity.IsAuthenticated)
            {
                Debug.WriteLine("------------------------");
                Debug.WriteLine(User.Identity.Name);
#pragma warning disable CS8604 // Possible null reference argument.
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);
#pragma warning restore CS8604 // Possible null reference argument.
                string role = string.Join(", ", res2);
                if (role == "profesor")
                    return RedirectToAction("Index", "Profesor");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
                else if (role == "doctor")
                    return RedirectToAction("Index", "Doctor");
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null && user.StatusCont == 2)
            {
                //Email bun, verificam parola
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Parola);
                if (passwordCheck)
                {
                    //Parola corecta, redirectionare
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Parola, false, false);
                    if (result.Succeeded)
                    {
                        var res2 = await _userManager.GetRolesAsync(user);
                        string role = string.Join(", ", res2);
                        if (role == "profesor")
                            return RedirectToAction("Index", "Profesor");
                        else if (role == "admin")
                            return RedirectToAction("Index", "Admin");
                        else if (role == "student")
                            return RedirectToAction("Index", "Student");
                        else if (role == "doctor")
                            return RedirectToAction("Index", "Doctor");
                    }
                }
                else
                {
                    TempData["Error"] = "Parola gresita.";
                }
            }
            else if (user != null && (user.StatusCont == 0 || user.StatusCont == 1))
            {
                TempData["Error"] = "Contul nu a fost activat inca.";
                return View(loginViewModel);
            }
            else if (user != null && user.StatusCont == -1)
                //Utilizatorul este blocat
                TempData["Error"] = "Contul a fost blocat.";
            else
            {
                TempData["Error"] = "Contul nu a fost gasit.";
            }
            return View(loginViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ForgottenPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);
#pragma warning restore CS8604 // Possible null reference argument.
                string role = string.Join(", ", res2);
                if (role == "profesor")
                    return RedirectToAction("Index", "Profesor");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
                else if (role == "doctor")
                    return RedirectToAction("Index", "Doctor");
            }
            var response = new ForgottenPasswordViewModel();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> ForgottenPassword(ForgottenPasswordViewModel forgottenPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(forgottenPassword);
            }

            var user = await _userManager.FindByEmailAsync(forgottenPassword.Email);
            //Email gasit, trimitem mail
            if (user != null && user.StatusCont == 2)
            {
                TempData["Succes"] = "Email trimis.";
                return View(forgottenPassword);
            }
            else if (user != null && user.StatusCont == 1 || user.StatusCont == 0)
            {
                TempData["Eroare"] = "Contul nu a fost activat inca.";
                return View(forgottenPassword);
            }
            else if (user != null && user.StatusCont == -1)
            {
                TempData["Eroare"] = "Contul a fost blocat.";
                return View(forgottenPassword);
            }
            TempData["Eroare"] = "Contul nu exista.";
            return View(forgottenPassword);
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);

                string role = string.Join(", ", res2);
                if (role == "profesor")
                    return RedirectToAction("Index", "Profesor");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
                else if (role == "doctor")
                    return RedirectToAction("Index", "Doctor");
            }
            var response = new RegisterViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            string id = _userRepository.GetIdByToken(registerViewModel.CodInregistrare);
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {

                user.Nume = registerViewModel.Nume;
                user.Prenume = registerViewModel.Prenume;
                user.Localitate = registerViewModel.Localitate;
                user.Judet = registerViewModel.Judet;
                string userName = user.Nume + " " + user.Prenume;
                user.UserName = userName;
                user.TokenInregistrare = "";
                user.StatusCont = 1;
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, registerViewModel.Parola);
                _userRepository.Save();
                CertificateAuthority CA = new CertificateAuthority();
                CA.GenerateCSR(user.Email, user.Localitate, user.Judet);
                string PvPem = "C:\\licenta\\LicentaFinal\\Certificate-Requests\\private-key-" + user.Email + ".pem";
                CA.EncryptFile(registerViewModel.Parola, PvPem, user.Email);
                var Cheie = new Cheie()
                {
                    CheiePublica = CA.GetPkeyRegister(user.Email),
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1)
                };
                var newCheieResponse = _cheieRepository.Add(Cheie);
                var newDoctor = new Doctor()
                {
                    DoctorUserId = user.Id,
                    Parafa = registerViewModel.Parafa,
                    IdCheiePublica = Cheie.Id
                };
                var newDoctorResponse = _doctorRepository.Add(newDoctor);
                if (newDoctorResponse && newCheieResponse)
                {
                    _doctorRepository.Save();
                    _cheieRepository.Save();
                }
                TempData["Succes"] = "Inregistrare completa, in scurt timp vei fi contactat pe email";
                return View(registerViewModel);

            }
            else
            {
                TempData["Error"] = "Token invalid.";
                return View(registerViewModel);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> RegisterStudent()
        {
#pragma warning disable CS8604 // Possible null reference argument.

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);
#pragma warning restore CS8604 // Possible null reference argument.
                string role = string.Join(", ", res2);
                if (role == "profesor")
                    return RedirectToAction("Index", "Profesor");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
                else if (role == "doctor")
                    return RedirectToAction("Index", "Doctor");
            }
            var response = new RegisterViewModel();
            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> RegisterStudentAsync(RegisterViewModel registerViewModelStud)
        {
            //create user
            var user = new User();
            user.Nume = registerViewModelStud.Nume;
            user.Prenume = registerViewModelStud.Prenume;
            user.Localitate = registerViewModelStud.Localitate;
            user.Judet = registerViewModelStud.Judet;
            user.TokenInregistrare = "";
            user.StatusCont = 2;
            user.Email = registerViewModelStud.Email;
            var newUserResponse = await _userManager.CreateAsync(user, registerViewModelStud.Parola);
            Console.WriteLine(newUserResponse.ToString());

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);

            }
            //create student
            var student = new Student();
            student.StudentUserId = user.Id;
            student.NumarMatricol = registerViewModelStud.NrMatricol;
            student.IdSpecializare = registerViewModelStud.IdSpecializare;
            student.AnDeStudii = registerViewModelStud.AnStudii;
            student.ModulStudii = registerViewModelStud.ModulStudii;
            _studentRepository.Add(student);
            _studentRepository.Save();
            return View(registerViewModelStud);
        }
        [HttpGet]
        public async Task<IActionResult> RegisterProfesor()
        {
#pragma warning disable CS8604 // Possible null reference argument.

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var res2 = await _userManager.GetRolesAsync(user);
#pragma warning restore CS8604 // Possible null reference argument.
                string role = string.Join(", ", res2);
                if (role == "profesor")
                    return RedirectToAction("Index", "Profesor");
                else if (role == "admin")
                    return RedirectToAction("Index", "Admin");
                else if (role == "student")
                    return RedirectToAction("Index", "Student");
                else if (role == "doctor")
                    return RedirectToAction("Index", "Doctor");
            }
            var response = new RegisterViewModel();
            return View(response);
        }



        [HttpPost]
        public async Task<IActionResult> RegisterProfesor(RegisterViewModel registerViewModel)
        {
            //create user
            var user = new User();
            user.Nume = registerViewModel.Nume;
            user.UserName = registerViewModel.Email;
            user.Prenume = registerViewModel.Prenume;
            user.Localitate = registerViewModel.Localitate;
            user.Judet = registerViewModel.Judet;
            user.TokenInregistrare = "";
            user.StatusCont = 2;
            user.Email = registerViewModel.Email;
            var newUserResponse = await _userManager.CreateAsync(user, registerViewModel.Parola);
            Debug.WriteLine("========================================");
            Debug.WriteLine(newUserResponse.ToString());

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Profesor);

            }
            //create student
            var profesor = new Profesor();
            profesor.Nomenclatura = registerViewModel.Nomenclatura;
            profesor.ProfesorUserId = user.Id;
            _profesorRepository.Add(profesor);
            _profesorRepository.Save();
            return View(registerViewModel);
        }
    }
}
