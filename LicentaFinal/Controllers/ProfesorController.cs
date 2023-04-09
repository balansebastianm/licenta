using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicWeb.Controllers
{
    public class ProfesorController : Controller
    {
        [Authorize(Roles ="profesor")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
