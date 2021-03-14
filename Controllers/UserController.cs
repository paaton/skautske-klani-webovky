using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scout_game.Models;
using Scout_game.modules;



namespace Scout_game.Controllers
{
    public class UserController : Controller
    {
        
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Team_Id") != null) //check logged in
            {
                return RedirectToAction("account");
            }
            return View();
        }

        public IActionResult Account()
        {
            if (HttpContext.Session.GetString("Team_Id") == null) //check logged in
            {
                TempData["error"] = "nejdříve se prosím přihlaš";
                return RedirectToAction("index");
            }

            return View();
        }
        public IActionResult Matches()
        {
            if (HttpContext.Session.GetString("Team_Id") == null) //check logged in
            {
                TempData["error"] = "nejdříve se prosím přihlaš";
                return RedirectToAction("index");
            }
            
            
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }


        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) // login service
        {
            
            
            
            Team team = await new Database().Login(username, password); //create login request - return data, if credentials are right
            if (team == null) //in case of error
            {
                TempData["error"] = "něco se pokazilo, zadal jsi to dobře?"; 
                return RedirectToAction("Index"); //return login page with error
            }
            else
            {
                HttpContext.Session.SetString("Team_name", team.Name); // start of inserting basic team info in session
                HttpContext.Session.SetString("Team_Id", password);
                HttpContext.Session.SetString("User_name", team.Login_username);
                HttpContext.Session.SetString("user_nick", team.User);
                HttpContext.Session.SetString("user_tag", team.Tag);
                HttpContext.Session.SetString("Team_name", team.Name);
                HttpContext.Session.SetInt32("Team_Group", team.Match_group);
                HttpContext.Session.SetString("Team_Game", team.Game); // end of inserting basic team info in session
                if (team.Members != null)
                { 
                    HttpContext.Session.SetString("member", Convert.ToString(team.Members.Count)); // if members exist, then put them in session too
                }
                return RedirectToAction("account");
            }
        }

        [Route("logout")]
        [HttpGet] //never referenced, or used - not wanted - login is only temporary
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); //clear session
            return RedirectToAction("Index"); // return login
        }
        [HttpPost] //members adder - in first login, or when session string "member" is null
        public async Task<IActionResult> Add_members(string Name1, string Name2, string Name3, string Name4)
        {
            if (Name1 != null || Name2 != null || Name3 != null || Name4 != null) //check existence of properties
            {
                HttpContext.Session.SetString("member1", Name1); // puts members of team in session in case,
                HttpContext.Session.SetString("member2", Name2); // that they are wanted
                HttpContext.Session.SetString("member3", Name3);
                HttpContext.Session.SetString("member4", Name4);
                HttpContext.Session.SetString("member", Convert.ToString(4)); // sets session string "member" for check of members existence
                 // insert members of team in database
                await new Database().Set_colabs(HttpContext.Session.GetString("Team_Id"), Name1, Name2, Name3, Name4, HttpContext.Session.GetString("Team_Game"));
            }
            else
            {
                TempData["error"] = "Pro odeslání musíš vyplnit všechny 4 členy"; //in case of null property 
            }
 
            

            return RedirectToAction("account"); // returns refreshed main page
        }
        


    }
}

