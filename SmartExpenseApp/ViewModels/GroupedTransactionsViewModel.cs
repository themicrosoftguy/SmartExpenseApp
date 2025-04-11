using CommunityToolkit.Maui.Core.Extensions;
using SmartExpenseApp.Data;
using SmartExpenseApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartExpenseApp.ViewModels
{
    public class GroupedTransactionsViewModel : INotifyPropertyChanged
    {
        bool includeEmptyGroups;
        private ObservableCollection<Transaction> transactions;
        private readonly SmartExpenseAppDatabase _smartExpenseAppDatabase;

        public ObservableCollection<Transaction> Transactions
        {
            get => transactions;
            set
            {
                transactions = value;
                OnPropertyChanged();
            }
        }

        private Transaction _selectedTransaction;

        public Transaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                if (_selectedTransaction != value)
                {
                    _selectedTransaction = value;
                    OnPropertyChanged(nameof(SelectedTransaction));

                    // Navigate to Transaction Details when an item is selected
                    if (_selectedTransaction != null)
                    {
                        TransactionSelectedCommand.Execute(_selectedTransaction);
                    }
                }
            }
        }

        public ObservableCollection<TransactionGroup> GroupedTransactions { get; private set; }
            = new ObservableCollection<TransactionGroup>();

        public ICommand TransactionSelectedCommand { get; }

        public GroupedTransactionsViewModel(SmartExpenseAppDatabase smartExpenseAppDatabase, bool emptyGroups = false)
        {
            _smartExpenseAppDatabase = smartExpenseAppDatabase;
            includeEmptyGroups = emptyGroups;
            Transactions = new ObservableCollection<Transaction>();
                                                
            GetGroupedTransactionList();

            TransactionSelectedCommand = new Command<Transaction>(async (transaction) => await NavigateToDetails(transaction));
        }

        private async Task LoadTransactions()
        {
            var transactions = await _smartExpenseAppDatabase.GetTransactionsAsync();

            Transactions.Clear();
            GroupedTransactions.Clear();

            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }

        public void GetGroupedTransactionList()
        {
            Task.Run(async () => await LoadTransactions()).Wait();

            GroupedTransactions = Transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderByDescending(g => new DateTime(g.Key.Year, g.Key.Month, 1))
                .Select(g => new TransactionGroup(GetGroupName(g.Key.Year, g.Key.Month), g.OrderByDescending(t => t.Date).ToObservableCollection()))
                .ToObservableCollection();

            OnPropertyChanged(nameof(GroupedTransactions));
        }

        private string GetGroupName(int year, int month)
        {
            return new DateTime(year, month, 1).ToString("MMMM yyyy");
        }

        private async Task NavigateToDetails(Transaction transaction)
        {
            if (transaction != null)
            {
                await Shell.Current.GoToAsync($"transactiondetails?transactionId={transaction.ID}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}