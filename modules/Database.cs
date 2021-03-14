using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Google.Cloud.Firestore;
using Discord;
using Discord.Commands;
using Scout_game.Models;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace Scout_game.modules
{
    public class Database // biggest class in project - provides acces to database, we used it to almost everything that wanted acces to database
    {

        public DocumentReference docRef;
        public FirestoreDb db = FirestoreDb.Create("firestore_project_name"); // creates instance of db

        public async Task AddTeam(Team team) // add team to database
        {
            docRef = db.Collection("Team").Document();
            await docRef.SetAsync(team);
        }

        public async Task Create_activator(int id, string author) // create activator
        {
            docRef = db.Collection("activators").Document(Convert.ToString(id));
            Dictionary<string, object> docData = new Dictionary<string, object>
            {
                { "is_done", false },
                { "author", author }
            };
            await docRef.SetAsync(docData);
        }
        public async Task Activate_account(string Activator, ICommandContext context) //create activation request
        {
            DocumentReference docRef = db.Collection("activators").Document(Activator); // find activator
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(); // get snaphot of it
            IUser author = context.Message.Author; // gets author of dicord command
            if (snapshot.Exists) // check existence
            {

                Dictionary<string, object> load = snapshot.ToDictionary(); //convert snapshot to dictionary pairs
                foreach (KeyValuePair<string, object> pair in load) // 
                {
                    if (pair.Key == "author") // i know there are better and safer ways, but I'm not experienced developer, and it works fine, so who cares :)
                    {
                        if (pair.Value.ToString() == context.Message.Author.Username) // check if sender is koo of team -- security issue - people can change their discord name
                        {
                            // gets team with that activator - again not safe beware more teams with one activator - that can happen
                            Query pas = db.Collection("Team").WhereEqualTo("ACT_Id", Convert.ToInt32(Activator)); 
                            QuerySnapshot pas_snap = await pas.GetSnapshotAsync();
                            Team team;
                            IUser user = context.User;
                            foreach (DocumentSnapshot doc in pas_snap.Documents) 
                            {
                                team = doc.ConvertTo<Team>();
                                if (team.User == context.Message.Author.Username) // check if it is right team -- trying to fix security issue from line 53
                                { 
                                    IRole grole;
                                    await UserExtensions.SendMessageAsync(author, "Tvoje heslo je: " + doc.Id); // sends password of team
                                    if (team.Game == "LoL")
                                    {
                                        grole = context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414960289054770");
                                    }
                                    else
                                    {
                                        grole = context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414757482397797");
                                    }
                                    await (user as IGuildUser).AddRoleAsync(grole); //asign game role to user
                                }
                                
                            }
                            var role = context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804672968424030268"); // get koo role
                            await (user as IGuildUser).AddRoleAsync(role); //asign koo role to user
                            await docRef.DeleteAsync(); // delete activator object in database - on start all teams with existing activator will be removed
                            await context.Channel.SendMessageAsync("A je to, do soukromých zpráv jsme ti poslali heslo, kterým se můžes přihlásit na našich stránkách, děkujeme ti za registraci v Skautském herním klání");


                        }
                        else
                        {
                            Console.WriteLine(pair.Value + " a " + author.Username + " doesn't match"); // in case of bad user requests 
                            await context.Channel.SendMessageAsync("jméno člověka, který registroval tým, se s tvým neschoduje, aktivační příkaz musí poslat koordinátor týmu");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Document {0} does not exist!", snapshot.Id);
                await context.Channel.SendMessageAsync("tento aktivační kód buď neexistuje, nebo už byl použit"); // in case of nonexisting activation number
            }

        }

        public async Task<Team> Login(string user, string password) // provides login to webpage, if retuns null - bad login 
        {
            if (password != null || user != null) // check existence of properties
            {
                // gets document - your password is your database document ID - try to not hate me, i thought it would be good idea
                DocumentReference docRef = db.Collection("Team").Document(password); 
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists) // check existence of document 
                {

                    Team data = snapshot.ConvertTo<Team>();
                    if (data.Members.Count == 1) // members array must have some data in it on writing it in db
                    {
                        data.Members = null;
                    }
                    if (data.Login_username == user) // check valid username
                    {
                        return data; // returns team object
                    }
                    else
                    {
                        return null; 
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        public async Task<Team> GetTeamById(string Id) // gets team object by his ID, never used in release
        {
            DocumentReference docRef = db.Collection("Team").Document(Id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.ConvertTo<Team>();
        }
        public async Task<List<Team>> GetAllTeams(string Game) // get list of all teams registered in game
        {
            if (Game == "CS-GO" || Game == "LoL")
            {
                Query Teamsquery = db.Collection("Team").WhereEqualTo("Game", Game); // gets collection
                QuerySnapshot Teamssnap = await Teamsquery.GetSnapshotAsync();
                List<Team> Teams = new List<Team>(); // new list of teams
                foreach (DocumentSnapshot document in Teamssnap.Documents)
                {
                    Team temp = document.ConvertTo<Team>();
                    temp.Doc_id = document.Id;
                    Teams.Add(temp);// adds team to list that will be returned
                }
                return Teams;

            }
            else { return null; } // returns null in case of nonexisting game provided
        }
        public async Task EditTeam(string Id, Team team) // rewrite any team with another by ID, never used in release
        {
            DocumentReference docRef = db.Collection("Team").Document(Id);
            await docRef.SetAsync(team);
        }


        public async Task Set_colabs(string Id, string team1, string team2, string team3, string team4, string Game) // set members of team with ID
        {
            DocumentReference docRef = db.Collection("Team").Document(Id);
            ArrayList colabs = new ArrayList // create list of members
            {
                team1,
                team2,
                team3,
                team4
            };
            await docRef.UpdateAsync("Members", colabs); // edit team members

            // ADD members to list of users in database - used to give roles to users

            Dictionary<string, object> member1 = new Dictionary<string, object>
            {
                { "Name", team1.Replace(" #", "#") } // repaires spaces between name and tag
            };
            await db.Collection(Game + "_users").AddAsync(member1); // sends it to db

            Dictionary<string, object> member2 = new Dictionary<string, object>
            {
                { "Name", team2.Replace(" #", "#") }
            };
            await db.Collection(Game + "_users").AddAsync(member2);

            Dictionary<string, object> member3 = new Dictionary<string, object>
            {
                { "Name", team3.Replace(" #", "#") }
            };
            await db.Collection(Game + "_users").AddAsync(member3);

            Dictionary<string, object> member4 = new Dictionary<string, object>
            {
                { "Name", team4.Replace(" #", "#") }
            };
            await db.Collection(Game + "_users").AddAsync(member4);

            // is there more efficient way to do this?
        }
        public async Task Check_member(string name, string game, ICommandContext Context) // add role to members
        {
            Query IS = db.Collection(game + "_users").WhereEqualTo("Name", name); // find user
            QuerySnapshot ISSnap = await IS.GetSnapshotAsync();
            if (ISSnap.Count == 0) // is user allowed to get role?
            {
                await Context.Channel.SendMessageAsync("vypadá to, že tě nemáme v systému, pro přidání role tě musí tvůj koordinátor týmu zapsat jako spoluhráče na webu");
            }
            else
            {
                IRole role;
                IGuildUser user = (Context.User as IGuildUser);
                if (game == "CS-GO") // add role based on game
                {
                    role = Context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414757482397797");
                }
                else
                {
                    role = Context.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == "804414960289054770");
                }
                await user.AddRoleAsync(role); // asign role
                await Context.Channel.SendMessageAsync("Role nastaveny, vítej v turnaji"); // send welcome message
            }

        }



        public async Task<List<Team>> GetTeamsOfGroup(int Group) // get list og teams in group
        {
            Query Teamquery = db.Collection("Team").WhereEqualTo("Match_group", Group);
            QuerySnapshot Teamssnap = await Teamquery.GetSnapshotAsync();
            List<Team> Group_list = new List<Team>();
            foreach (DocumentSnapshot document in Teamssnap.Documents)
            {
                Team temp = document.ConvertTo<Team>();
                temp.Doc_id = document.Id;
                Group_list.Add(temp);
            }
            if (Group_list.Count == 0)
            {
                return null; // in case of empty group
            }
            else
            {
                return Group_list; // return list
            }
        }
        //-------------------------------------------------------------------------------match system---------------------------------------------------------------------------------------

        // gets ID by name - another security issue, but who cares, it was scout tournament, and its better then putting opponent ID to html or js
        public async Task<string> GetTeamIdbyName(string Name, string Game)  
        {
            Query query = db.Collection("Team").WhereEqualTo("Game", Game).WhereEqualTo("Name", Name);
            QuerySnapshot snap = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in snap.Documents)
            {
                return doc.Id; // return ID
            }
            return null; // in case of no team found
        }
        public async Task Edit_Group(string Id, int Group) // edit match group of team with ID, and new group provided - never used in release
        {
            DocumentReference docRef = db.Collection("Team").Document(Id);
            await docRef.UpdateAsync("Match_group", Group);
        }
        public async Task CreateWinRequest(string WinnerId, string LoserId, string game) // creates new match in db
        {
            Match match = new Match
            {
                Active = true, // true means that match is created, but note yet approved by loser
                Winner = WinnerId,
                Losser = LoserId,
                Game = game
                
            };
            await db.Collection("Match").AddAsync(match); // send to db
        }
        public async Task<string> Checkmatchstate(string WinnerId, string LoserId, string game) // check state of match in db - called by "User/Matches.cshtml" view.
        {
            Query query = db.Collection("Match").WhereEqualTo("Game", game).WhereEqualTo("Winner", WinnerId).WhereEqualTo("Losser", LoserId); // get match
            QuerySnapshot snap = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in snap.Documents) 
            {
                Match match = doc.ConvertTo<Match>();
                if (match.Active)  // check state
                { return "active"; } 
                else
                { return "done"; }

            }
            return null; // returns null, if match dont exist
        }
        public async Task Losse(string WinnerId, string LoserId, string game) // approve lose by loser
        {
            Query query = db.Collection("Match").WhereEqualTo("Game", game).WhereEqualTo("Winner", WinnerId).WhereEqualTo("Losser", LoserId); // gets query
            QuerySnapshot snap = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in snap.Documents) // approve lose of all docs - should be only one
            {
                DocumentReference docref = db.Collection("Match").Document(doc.Id);
                await docref.UpdateAsync("Active", false); //update Active parameter
            }
        }
        public async Task Ipoints(string Id, int points) // increment points of winner - didnt work, explanation in "Controllers\MatchController.cs" 
        {
            DocumentReference docRef = db.Collection("Team").Document(Id);
            await docRef.UpdateAsync("points", FieldValue.Increment(points)); //update points field in team doc at db

        }
        public async Task<List<List<Team>>> Get_Result(string Game) // get current result of matches
        {
            Query query = db.Collection("Team").WhereEqualTo("Game", Game); // get teams
            QuerySnapshot snap = await query.GetSnapshotAsync();
            List<List<Team>> Ret = new List<List<Team>>(); // list of lists of teams -- will be returned
            List<Team> templist = new List<Team>();
            
             // creates new Team class for each team
            foreach (DocumentSnapshot doc in snap.Documents)
            {
                Team t = doc.ConvertTo<Team>();
                QuerySnapshot point = await db.Collection("Match").WhereEqualTo("Winner", doc.Id).WhereEqualTo("Active", false).GetSnapshotAsync();
                t.points = point.Count; //set points by actual number of won matches - repair for method IPoints
                
                templist.Add(t);
            }
            List<Team> group1 = new List<Team>(); // new lists for each group
            List<Team> group2 = new List<Team>();
            List<Team> group3 = new List<Team>();
            List<Team> group4 = new List<Team>();
            List<Team> group5 = new List<Team>();
            List<Team> group6 = new List<Team>();
            List<Team> group7 = new List<Team>();
            List<Team> group8 = new List<Team>();
            foreach (Team team in templist)
            {
                if (team.Match_group == 1) // adds to lists by match group
                {
                    group1.Add(team); 
                }
                if (team.Match_group == 2)
                {
                    group2.Add(team);
                }
                if (team.Match_group == 3)
                {
                    group3.Add(team);
                }
                if (team.Match_group == 4)
                {
                    group4.Add(team);
                }
                if (team.Match_group == 5)
                {
                    group5.Add(team);
                }
                if (team.Match_group == 6)
                {
                    group6.Add(team);
                }
                if (team.Match_group == 7)
                {
                    group7.Add(team);
                }
                if (team.Match_group == 8)
                {
                    group8.Add(team);
                }

            }
            Ret.Add(group1); // add lists to final list
            Ret.Add(group2);
            Ret.Add(group3);
            Ret.Add(group4);
            Ret.Add(group5);
            Ret.Add(group6);
            Ret.Add(group7);
            Ret.Add(group8);
            return Ret;

            // I think there must be more effective way to do this, but i can't find it
        }

        /*------------------------------------------------------------------------stats_system-----------------------------------------------------------------------------*/
        public async Task<Dictionary<string, int>> NumberofTeams() // statistics for admin team - never used in release
        {
            Query query = db.Collection("Team").WhereEqualTo("Game", "CS-GO");
            QuerySnapshot snap = await query.GetSnapshotAsync();
            Query queryL = db.Collection("Team").WhereEqualTo("Game", "LoL");
            QuerySnapshot snapL = await queryL.GetSnapshotAsync();
            return new Dictionary<string, int>
            {
                {"LoL", snapL.Count },
                {"CS", snap.Count }
            };
        }
    }

}
