using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace azure_exam.Models
{
    public class Chat
    {
        public string id { get; set; }
        public string chatId { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public DateTime CreatedAt { get; set; }
    }

}
