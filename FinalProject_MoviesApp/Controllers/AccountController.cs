using FinalProject_MoviesApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProject_MoviesApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext db;

        public AccountController(DataContext db)
        {
            this.db = db;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var credentials = db.Users.Where(p => p.Email == username && p.Password == password).FirstOrDefault();

            if (credentials != null)
            {
                var identity = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                    }, CookieAuthenticationDefaults.AuthenticationScheme
                    );

                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            TempData["SuccessMessage"] = "You're successfuly registered!!!";
            return RedirectToAction("Login");
        }

    }
}
