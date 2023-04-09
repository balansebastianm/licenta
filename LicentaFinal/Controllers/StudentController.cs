using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace LicWeb.Controllers
{
    public class StudentController : Controller
    {
        [Authorize(Roles = "student")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
