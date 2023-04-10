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
        string UserName;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext context,
            IUserRepository userRepository,
            IDoctorRepository doctorRepository)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                Debug.WriteLine(User.Identity.Name);
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
                        UserName = user.Email;
                    }
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
            string id = "";
            id = _userRepository.GetIdByToken(registerViewModel.CodInregistrare);
            System.Diagnostics.Debug.WriteLine(id);
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.UserName = registerViewModel.NumeComplet;
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, registerViewModel.Parola);

                user.CNP = registerViewModel.CNP;
                user.SerieBuletin = registerViewModel.SerieBuletin;
                user.Localitate = registerViewModel.Localitate;
                user.Judet = registerViewModel.Judet;
                user.TokenInregistrare = "";
                user.StatusCont = 1;
                _userRepository.Save();
                CertificateAuthority CA = new CertificateAuthority();
                CA.GenerateCSR(user.Email, user.Localitate, user.Judet);
                _doctorRepository.Save();
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

    }
}
