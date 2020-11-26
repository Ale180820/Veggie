using System.Collections.Generic;
using VeggieBack.Models;

namespace Veggie.Services {
    public class Storage {
        //Instance element
        private static Storage _instance = null;

        //Method for instance
        public static Storage Instance {
            get {
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }
        public User searchUser = new User();
        public List<Message> messages = new List<Message>();
        public List<Conversation> conversations = new List<Conversation>();
        public int idUser = new int();
    }

    
}
