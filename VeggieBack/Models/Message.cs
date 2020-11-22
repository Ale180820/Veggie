using System;

namespace VeggieBack.Models {
    public class Message {

        private static int codeMessage = 0;
        public int _id { get; set; }
        public string sendingUser { get; set; }
        public string receivingUser { get; set; }
        public DateTime messageInformation { get; set; }
        public byte statusMessage { get; set; }

        public Message() { }

        public int generateId() {
            codeMessage++;
            return codeMessage;
        }



    }
}
