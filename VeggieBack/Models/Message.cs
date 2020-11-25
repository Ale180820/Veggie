using System;

namespace VeggieBack.Models {
    public class Message {

        public string _id { get; set; }
        public string sendingUser { get; set; }
        public string receivingUser { get; set; }
        public DateTime messageInformation { get; set; }
        public bool typeMessage { get; set; }

        public Message() { }

    }
}
