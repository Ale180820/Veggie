using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeggieBack.Models;

namespace VeggieAPI.Services {
    public class Storage {

        private static Storage _instance = null;

        //Method for instance
        public static Storage Instance{
            get{
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }

        public Conversation actualConversation = new Conversation();  
    }
}
