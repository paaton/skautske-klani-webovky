using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scout_game.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Scout_game.modules;

namespace Scout_game.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // load main page
        public IActionResult Index()
        {
            return View();
        }
        

        //error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //registration form
        public IActionResult Reg(string error )
        {
            ViewBag.error = error;
            return View();
        }
        
        
        //registration POST data receiver - also returns a new page for activation
        [HttpPost]
        public async Task<IActionResult> Create(Team team /*new team object*/)
        {
            Random rand = new Random();
            //creates random 6-digit int for activation
            int activ = rand.Next(111111, 999999);

           
            if (team.Name != null && team.Game != null && team.Login_username != null && team.Tag != null && team.User != null) //form fill check
            {
                team.ACT_Id = activ; // puts in data that has not been filled with form
                team.Match_group = 0;
                team.Members = new System.Collections.ArrayList
                {
                    "None"
                };
                // end of data inserting
                await new Database().AddTeam(team); //create new team at database
                await new Database().Create_activator(activ, team.User); //create new activate listener at database

                ViewBag.activator = Convert.ToString(activ); //put activator in viewbag for use in returned view
                return View();
            }
            else
            {
                 //back the form with error in case of empty property 
                return RedirectToAction("Reg", "Home", new { error = "nejspíše jsi něco nevyplnil, zkus to prosím znovu " });
            }
                
        }
        
    }
}