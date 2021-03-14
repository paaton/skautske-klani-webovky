using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scout_game.modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scout_game.Controllers
{
    public class MatchController : Controller
    {
        //create request for match
        public async Task<ActionResult> Handle_request(string data)
        {
            string LosserId = await new Database().GetTeamIdbyName(data, HttpContext.Session.GetString("Team_Game")); //not safe, beware multiple teams with same name
            //creates new not aproached match in database
            await new Database().CreateWinRequest(HttpContext.Session.GetString("Team_Id"), LosserId, HttpContext.Session.GetString("Team_Game")); 
            return RedirectToAction("Matches", "User"); //refresh page with matches
        }
        public async Task<ActionResult> Aproach_lose(string data)
        {
            string WinnerId = await new Database().GetTeamIdbyName(data, HttpContext.Session.GetString("Team_Game")); //not safe, beware multiple teams with same name
            await new Database().Losse(WinnerId, HttpContext.Session.GetString("Team_Id"), HttpContext.Session.GetString("Team_Game")); //sets match status to done
            await new Database().Ipoints(WinnerId, 1); //adds points to team - didnt work*
            return RedirectToAction("Matches", "User");
        }
    }
}
// *CZ - týmům byly body přiřazovány i několikrát, pravděpodobně, kvůli tomu, že hráči podávali win request vícekrát, 
// klikli na tlačítko i 10x, místo toho aby počkali než se stránka načte -- nezabezpečené proti multiclicku
// řešením byl samostatný program, který smazal všechny doublérské zápasy před vyhlášením skupin
