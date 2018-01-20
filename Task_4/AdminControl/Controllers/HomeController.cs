using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminControl.Models;
using Microsoft.Extensions.Configuration;
using AdminControl.Data;

namespace AdminControl.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; set; }
        private ApplicationDbContext db;



        public HomeController(IConfiguration config, ApplicationDbContext context)
        {
            Configuration = config;
            db = context;
        }

        public IActionResult Index(string flag)
        {
            ViewData["Flag"] = flag;
            AccountDbContext dbContext = new AccountDbContext();
            var allusers = dbContext.Accounts.ToList();
            ViewData["Users"] = allusers;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
