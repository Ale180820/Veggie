using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public User searchUsers = new User();
        public User actualUser = new User();
        public Entry actualConversation = new Entry();
        public List<Contacts> contacts = new List<Contacts>();
        public List<SendMessage> messages = new List<SendMessage>();
        public List<Message> findMessages = new List<Message>();
        public List<Conversation> conversations = new List<Conversation>();
        public int idUser;
        public int conversationId;
    }

    
}
