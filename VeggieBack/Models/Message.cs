using System;

namespace VeggieBack.Models {
    public class Message {

        /// <summary>
        /// Method that will call the interface method
        /// </summary>

        public static int codeMessage = 0;
        public int _id { get; set; }
        public string sendingUser { get; set; }
        public string receivingUser { get; set; }
        public string message { get; set; }
        public DateTime messageTime { get; set; }

        public Message() {
            var rand = new Random();
            codeMessage = codeMessage + 100 + rand.Next(0, 100000);
            this._id = codeMessage;
        }

    }
}
