using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scout_game.Models
{
    // defines match class
    [FirestoreData]
    public class Match
    {
        [FirestoreProperty]
        public string Losser { get; set; } 
        [FirestoreProperty]
        public string Winner { get; set; }
        [FirestoreProperty]
        public bool Active { get; set; } // defines status of match - approved / not-yet approved by looser
        [FirestoreProperty]
        public string Game { get; set; }
    }
}
