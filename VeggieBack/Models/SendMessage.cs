using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VeggieBack.Models {
    public class SendMessage {
        public Message messageSend { get; set; }
        public int idConversation { get; set; }
        public bool typeMessage { get; set; }
        IFormFile fileSend { get; set; }

    }
}
