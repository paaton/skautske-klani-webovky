using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scout_game.Models
{
    // defines team class
    [FirestoreData]
    public class Team
    {

        [FirestoreProperty]
        public string Name { get; set; } // team name
        [FirestoreProperty]
        public string User { get; set; } // koo name
        [FirestoreProperty]
        public string Game { get; set; } // game
        [FirestoreProperty]
        public string Tag { get; set; } // koo discord tag
        [FirestoreProperty]
        public string Login_username { get; set; } // koo login 
        [FirestoreProperty]
        public int Match_group { get; set; } // group in match
        [FirestoreProperty]
        public int ACT_Id { get; set; } // activation ID
        public string Doc_id { get; set; } //id of document in database - not transfered in database as property
        [FirestoreProperty]
        public ArrayList Members { get; set; } // list of team members
        [FirestoreProperty]
        public int points { get; set; } // point of team - no used - explained in match controller under * - CZ only
    }
}
