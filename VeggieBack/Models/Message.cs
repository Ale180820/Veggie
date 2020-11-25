using System;

namespace VeggieBack.Models {
    public class Message {

        public string sendingUser { get; set; }
        public string receivingUser { get; set; }
        public DateTime messageInformation { get; set; }
        public byte statusMessage { get; set; }

        public Message() { }

    }
}
