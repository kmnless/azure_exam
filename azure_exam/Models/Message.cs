namespace azure_exam.Models
{
    public class Message
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public string? FileUrl { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
