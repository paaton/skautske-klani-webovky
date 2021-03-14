using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Scout_game.modules;

namespace Scout_game.modules
{
    public class Discord_commands : ModuleBase
    {
        [Command("activate")] //activate team
        public async Task Activation([Remainder]string code)
        {
            if (Context.Channel.Id == 804414560462307408) // check discord channel
            {
                await new Database().Activate_account(code, Context); // provide activation code, and message context to another block of code, with open database connection
            }
            else
            {
                await Context.Channel.SendMessageAsync("Tento command je podporován jen v channelu #aktivace");
            }
        }

        [Command("add")]
        public async Task Roles([Remainder] string game) //role adder
        {
            string name = Context.User.Username + "#" + Context.User.Discriminator; 
            await Context.Channel.SendMessageAsync(name); // log for testing, we left it in production for good look, and check that bot received request
            if (game != "CS-GO" && game != "LoL") // check typo of message remainder
            {
                await Context.Channel.SendMessageAsync("Tento Command podporuje jen hry CS-GO, nebo Lol, a jeho správné použití je: !add CS-GO, nebo !add LoL Zadal jsi: !add " + game);
                
            }
            else 
            {
                await new Database().Check_member(name, game, Context); // send it to code block with open database connection
            }
                
        }

    }
}
