using SmartExpenseApp.Models;
using SmartExpenseApp.Data;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartExpenseApp.ViewModels
{
    public class TransactionDetailsViewModel : BaseViewModel
    {
        private readonly SmartExpenseAppDatabase _database;
        private Transaction transaction;

        public Transaction Transaction
        {
            get => transaction;
            set => SetProperty(ref transaction, value);
        }

        public ICommand EditTransactionCommand { get; }

        public TransactionDetailsViewModel(SmartExpenseAppDatabase database)
        {
            _database = database;
            EditTransactionCommand = new Command(async () => await NavigateToAddDetails());
        }

        public async Task LoadTransactionDetails(int transactionId)
        {
            Transaction = await _database.GetTransactionByIdAsync(transactionId);
        }

        private async Task NavigateToAddDetails()
        {
            if (Transaction != null)
            {
                await Shell.Current.GoToAsync($"addtransactionpage?transactionId={Transaction.ID}");
            }
        }
    }
}