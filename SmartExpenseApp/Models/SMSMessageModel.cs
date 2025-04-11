namespace SmartExpenseApp.Models
{
    public class SMSMessageModel
    {
        public string MessageId { get; set; }
        public string ThreadId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}