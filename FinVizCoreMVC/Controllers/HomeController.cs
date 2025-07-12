using Microsoft.AspNetCore.Mvc;

namespace FinVizCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var expiryStr = HttpContext.Session.GetString("TokenExpiry");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expiryStr))
            {
                return RedirectToAction("SignIn", "Employee");
            }

            var expiry = DateTime.Parse(expiryStr);
            if (DateTime.UtcNow > expiry)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("SignIn", "Employee");
            }

            ViewBag.Token = token;
            return View();
        }

    }
}
