using System;
using System.Collections.Generic;

namespace VeggieBack.Models {

    public class Conversation {

        public static int codeConversation = 0;
        public int _id { get; set; }
        public User userOne { get; set; }
        public User userTwo { get; set; }
        public int firstKey { get; set; }
        public int secondKey { get; set; }
        public Dictionary<string, Message> messages { get; set; }

        public Conversation() {
            var rand = new Random();
            codeConversation = codeConversation + 100 + rand.Next(0, 100000);
            this._id = codeConversation;
        }

    }
}
