using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Models {
    public class Message {

        public int _id { get; set; }
        public UserMessage sendingUser { get; set; }
        public UserMessage receivingUser { get; set; }
        public DateTime messageInformation { get; set; }
        public byte statusMessage { get; set; }

        public Message() { }
    }
}
