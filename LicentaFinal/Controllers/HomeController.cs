using LicWeb.Data;
using LicWeb.Interfaces;
using LicWeb.Models;
using LicWeb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
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
            return View();
        }

    }
}