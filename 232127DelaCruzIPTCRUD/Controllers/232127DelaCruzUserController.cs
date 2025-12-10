using Microsoft.AspNetCore.Mvc;
using _232127DelaCruzIPTCRUD.Models;
using _232127DelaCruzIPTCRUD.Services;

namespace _232127DelaCruzIPTCRUD.Controllers
{
    public class _232127DelaCruzUserController : Controller
    {
        private readonly _232127DelaCruzUserService _userService;

        public _232127DelaCruzUserController(_232127DelaCruzUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if already logged in
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "_232127DelaCruzDashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(int userid, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter user ID and password";
                return View();
            }

            var user = await _userService.AuthenticateAsync(userid, password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.USERID);
                HttpContext.Session.SetString("UserName", $"{user.FNAME} {user.LNAME}");
                return RedirectToAction("Dashboard", "_232127DelaCruzDashboard");
            }

            ViewBag.Error = "Invalid user ID or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
