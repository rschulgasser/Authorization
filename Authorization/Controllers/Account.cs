using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Data;
using Authorization.Models;

namespace Authorization.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Authorization; Integrated Security=true;";
      
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var repo = new DataBase(_connectionString);
            repo.AddUser(user, password);

            return Redirect("home/login");
        }

        public IActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
             var repo = new DataBase(_connectionString);
            var user = repo.Login(email, password);
            if (user == null)
            {
                TempData["message"] = "Invalid login!";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>
            {
              
                new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/index");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
        public IActionResult MyAccount()
        {
            DataBase data = new(_connectionString);
            HomePageVM vm = new();
            vm.Ads = data.GetAdsForUser(User.Identity.Name);
            return View(vm);
        }

    }
}

