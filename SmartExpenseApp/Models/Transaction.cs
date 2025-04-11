using SQLite;
using static SmartExpenseApp.Utilities.SmartExpenseEnums;

namespace SmartExpenseApp.Models
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Icon { get; set; }
        public string Amount { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }    
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string Sender { get; set; }
        public TransactionType TransactionType { get; set; }
        public int IsManual { get; set; }
    }
}