using System.Collections.ObjectModel;

namespace SmartExpenseApp.Models
{
    public class TransactionGroup : ObservableCollection<Transaction>
    {
        public string Date { get; private set; }

        public TransactionGroup(string date, ObservableCollection<Transaction> transactions) : base(transactions)
        {
            Date = date;
        }
    }
}
