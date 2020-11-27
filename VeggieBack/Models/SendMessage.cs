using Microsoft.AspNetCore.Http;

namespace VeggieBack.Models {


    public class SendMessage {

        public Message messageSend { get; set; }
        public int idConversation { get; set; }
        public bool typeMessage { get; set; }
        File fileSend { get; set; }

    }
}
