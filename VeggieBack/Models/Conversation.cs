using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Models {

    public class Conversation {

        public User userOne { get; set; }
        public User userTwo { get; set; }
        public Dictionary<string, Message> messages { get; set; }

        public Conversation() { }

    }
}
