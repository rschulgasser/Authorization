using Authorization.Data;
using Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Authorization; Integrated Security=true;";
        public IActionResult Index()
        {
            DataBase data = new(_connectionString);
            HomePageVM vm = new();
            vm.Ads = data.GetAllAds();

            if (User.Identity.IsAuthenticated)
            {
                vm.AdIds = data.GetListOfAdIdsByEmail(User.Identity.Name);
            }
                return View(vm);
        }
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            DataBase dataBase = new(_connectionString);
            dataBase.DeleteAd(id);
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            DataBase dataBase = new(_connectionString);
            dataBase.AddAd(ad, User.Identity.Name);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult NewAd()
        {

            return View();
        }




    }
}
