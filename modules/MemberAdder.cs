namespace Scout_game.modules
{
    // test of adding members automatically - never used for bad functionality - bot dont have cached every user in current guild, and downloading them would take too much time, RAM and cpu.


    /*
    public class MemberAdder
    {
        public async Task CSGO(ICommandContext Context, string mention)
        {
            if ((Context.User as IGuildUser).RoleIds.FirstOrDefault(x => x == 804414757482397797) != 0)
            {
                if ((Context.User as IGuildUser).RoleIds.FirstOrDefault(z => z == 804672968424030268) != 0)
                {
                    int indexend = mention.Length;
                    string ID = mention.Substring(3, indexend - 4);
                    await Context.Channel.SendMessageAsync("stahování informací o uživateli...");
                    if (await Context.Guild.GetUserAsync(769135545456132106) == null);
                    { await Context.Guild.DownloadUsersAsync();}    
                    IGuildUser user = await Context.Guild.GetUserAsync(Convert.ToUInt64(ID));
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414960289054770");
                    if (user == null)
                    {
                        Console.WriteLine(ID + "is not avaiable");
                        await Context.Channel.SendMessageAsync("vypadá, to že je neějaký problém na naší straně, kontaktuj prosím admina, nebo to zkus později");
                    }
                    else
                    { 
                        await user.AddRoleAsync(role);
                        await Context.Channel.SendMessageAsync($"uživatel {user.Username} byl přidán ");
                    }
                        
                }
                else
                {
                    await Context.Channel.SendMessageAsync("pro přidání musíš být koordinátor týmu");
                }

            }
            else
            {
                await Context.Channel.SendMessageAsync("Na tohle nemáš právo");
            }


        }
        public async Task LoL(ICommandContext Context, string mention)
        {
            if ((Context.User as IGuildUser).RoleIds.FirstOrDefault(x => x == 804414960289054770) != 0)
            {
                if ((Context.User as IGuildUser).RoleIds.FirstOrDefault(z => z == 804672968424030268) != 0)
                {
                    int indexend = mention.Length;
                    string ID = mention.Substring(3, indexend - 4);
                    await Context.Channel.SendMessageAsync("stahování informací o uživateli...");
                    if (await Context.Guild.GetUserAsync(769135545456132106) == null) ;
                    { 
                        await Context.Guild.DownloadUsersAsync();
                        await Task.Delay(5000);
                    }
                    IGuildUser user = await Context.Guild.GetUserAsync(Convert.ToUInt64(ID));
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414960289054770");
                    if (user == null)
                    {
                        Console.WriteLine(ID + "is not avaiable");
                    }
                    else
                    { 
                        await user.AddRoleAsync(role);
                        await Context.Channel.SendMessageAsync($"uživatel {user.Username} byl přidán ");
                    }
                    

                }
                else
                {
                    await Context.Channel.SendMessageAsync("pro přidání musíš být koordinátor týmu");
                }

            }
            else
            {
                await Context.Channel.SendMessageAsync("Na toto nemáš právo");
            }
        }
    }*/
}
