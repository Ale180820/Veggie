using System;

namespace VeggieBack.Models {
    public class Message {

        public static int codeMessage = 0;
        public int _id { get; set; }
        public string sendingUser { get; set; }
        public string receivingUser { get; set; }
        public string message { get; set; }
        public DateTime messageTime { get; set; }
        public bool typeMessage { get; set; } //True texto, false es file. 

        public Message() {
            var rand = new Random();
            codeMessage = codeMessage + 100 + rand.Next(0, 100000);
            this._id = codeMessage;
        }

    }
}
