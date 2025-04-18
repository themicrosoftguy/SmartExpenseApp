using CommunityToolkit.Maui.Core.Extensions;
using SmartExpenseApp.Data;
using SmartExpenseApp.Models;
using Syncfusion.Maui.Buttons;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static SmartExpenseApp.Utilities.SmartExpenseEnums;

namespace SmartExpenseApp.ViewModels
{
    public class GraphAndTrendsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private ObservableCollection<Transaction> transactions;
        private readonly SmartExpenseAppDatabase _smartExpenseAppDatabase;

        private bool isSplineAreaChartVisible;
        private bool isCircularChartVisible;
        private int selectedSegmentIndex;
        private int selectedTransactionTypeIndex;

        public bool IsSplineAreaChartVisible
        {
            get => isSplineAreaChartVisible;
            set
            {
                isSplineAreaChartVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsCircularChartVisible
        {
            get => isCircularChartVisible;
            set
            {
                isCircularChartVisible = value;
                OnPropertyChanged();
            }
        }

        public int SelectedSegmentIndex
        {
            get => selectedSegmentIndex;
            set
            {
                selectedSegmentIndex = value;
                OnPropertyChanged();
                UpdateChartVisibility();
            }
        }

        public int SelectedTransactionTypeIndex
        {
            get => selectedTransactionTypeIndex;
            set
            {
                selectedTransactionTypeIndex = value;
                OnPropertyChanged();
                UpdateFilteredTransactions();
            }
        }

        public List<SfSegmentItem> SegmentedItems { get; set; }

        public ObservableCollection<Transaction> Transactions
        {
            get => transactions;
            set
            {
                transactions = value;
                OnPropertyChanged();
            }
        }

        private double totalDebitTransactionsAmount;

        public double TotalDebitTransactionsAmount
        {
            get => totalDebitTransactionsAmount;
            set
            {
                SetProperty(ref totalDebitTransactionsAmount, value);
                UpdateBalance();
            }
        }

        private double totalCreditTransactionsAmount;

        public double TotalCreditTransactionsAmount
        {
            get => totalCreditTransactionsAmount;
            set
            {
                SetProperty(ref totalCreditTransactionsAmount, value);
                UpdateBalance();
            }
        }

        private double balance;

        public double Balance { get => balance; set => SetProperty(ref balance, value); }

        private double totalTransactionsAmount;

        public double TotalTransactionsAmount
        {
            get => totalTransactionsAmount;
            set { SetProperty(ref totalTransactionsAmount, value); }
        }

        private ObservableCollection<Transaction> filteredTransactions;

        public ObservableCollection<Transaction> FilteredTransactions
        {
            get => filteredTransactions;
            set { SetProperty(ref filteredTransactions, value); }
        }

        //private ObservableCollection<Grouping<string, Transaction>> groupedTransactionsByCategory;
        //public ObservableCollection<Grouping<string, Transaction>> GroupedTransactionsByCategory
        //{
        //    get => groupedTransactionsByCategory;
        //    set
        //    {
        //        groupedTransactionsByCategory = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public ObservableCollection<TransactionCategoryGroup> GroupedCategoryTransactions { get; private set; }
        //    = new ObservableCollection<TransactionCategoryGroup>();

        private ObservableCollection<TransactionCategoryGroup> groupedCategoryTransactions;

        public ObservableCollection<TransactionCategoryGroup> GroupedCategoryTransactions
        {
            get => groupedCategoryTransactions;
            set
            {
                groupedCategoryTransactions = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshTransactionsCommand { get; }

        //public ICommand ToggleGroupCommand { get; }


        public ICommand ToggleGroupCommand => new Command<TransactionCategoryGroup>(
            (item) =>
            {
                if(item.GroupIcon == "icon_down")
                {
                    item.Clear();
                    item.GroupIcon = "icon_up";
                } else
                {
                    var recordsTobeAdded = FilteredTransactions.Where(f => f.Category == item.Category).ToList();
                    item.AddRange(recordsTobeAdded);
                    item.GroupIcon = "icon_down";
                }
            });

        public GraphAndTrendsViewModel(SmartExpenseAppDatabase smartExpenseAppDatabase)
        {
            _smartExpenseAppDatabase = smartExpenseAppDatabase;
            InitializeImageCollection();

            // Initialize default chart visibility
            IsSplineAreaChartVisible = true;
            IsCircularChartVisible = false;

            Transactions = new ObservableCollection<Transaction>();
            FilteredTransactions = new ObservableCollection<Transaction>();

            // Command to refresh transactions
            RefreshTransactionsCommand = new Command(async () => await LoadTransactions(TransactionType.Expense));

            //ToggleGroupCommand = new Command<TransactionCategoryGroup>(ToggleGroup);

            //Task.Run(async () => await LoadTransactions()).Wait();
        }

        private void InitializeImageCollection()
        {
            this.SegmentedItems = new List<SfSegmentItem>
            {
                new SfSegmentItem() { ImageSource = "icon_line_chart.png", ImageSize = 21.00 },
                new SfSegmentItem() { ImageSource = "icon_chart.png", ImageSize = 21.00 }
            };
        }

        public async Task LoadTransactions(TransactionType transactionType)
        {
            //await _smartExpenseAppDatabase.DeleteAllTransactionsAsync();

            var transactionsList = await _smartExpenseAppDatabase.GetTransactionsAsync();

            Transactions.Clear();

            Transactions = transactionsList.ToObservableCollection<Transaction>();

            // Calculate the total transactions amount
            TotalTransactionsAmount = transactions
                .Where(t => t.TransactionType == transactionType)
                .Sum(t => double.TryParse(t.Amount, out var amount) ? amount : 0);

            // Group transactions by category and set TotalTransactionsAmount for each group
            //GroupedCategoryTransactions = new ObservableCollection<TransactionCategoryGroup>(
            //    transactions
            //        .GroupBy(t => t.Category)
            //        .Select(g => new TransactionCategoryGroup(g.Key, new ObservableCollection<Transaction>(g))
            //        {
            //            TotalTransactionsAmount = TotalTransactionsAmount
            //        })
            //);


            //var filteredTransactions = transactions
            //    .Where(t => t.TransactionType == transactionType)
            //    .OrderByDescending(t => t.Date)
            //    .ToList();

            //Transactions.Clear();
            //foreach (var transaction in filteredTransactions)
            //{
            //    UpdateTransactionAmounts(transaction.TransactionType, transaction.Amount);
            //    Transactions.Add(transaction);
            //}

            UpdateFilteredTransactions();

            if(transactionType == TransactionType.Income)
            {
                TotalTransactionsAmount = TotalCreditTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            } else
            {
                TotalTransactionsAmount = TotalDebitTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            }
        }

        public void UpdateChartVisibility()
        {
            if(SelectedSegmentIndex == 0)
            {
                IsSplineAreaChartVisible = true;
                IsCircularChartVisible = false;
            } else if(SelectedSegmentIndex == 1)
            {
                IsSplineAreaChartVisible = false;
                IsCircularChartVisible = true;
            }

            // Refresh transactions based on the selected chart
            var transactionType = SelectedTransactionTypeIndex == 0 ? TransactionType.Expense : TransactionType.Income;

            //Task.Run(async () => await LoadTransactions(transactionType));

            UpdateGroupedTransactionsByCategory();

            if(transactionType == TransactionType.Income)
            {
                TotalTransactionsAmount = TotalCreditTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            } else
            {
                TotalTransactionsAmount = TotalDebitTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            }
        }

        private void UpdateTransactionAmounts(TransactionType transaction, string amount)
        {
            if(transaction == TransactionType.Income)
            {
                TotalCreditTransactionsAmount += double.Parse(amount);
                OnPropertyChanged(nameof(TotalCreditTransactionsAmount));
                OnPropertyChanged(nameof(Balance));
            } else
            {
                TotalDebitTransactionsAmount += double.Parse(amount);
                OnPropertyChanged(nameof(TotalDebitTransactionsAmount));
                OnPropertyChanged(nameof(Balance));
            }
        }

        private void UpdateFilteredTransactions()
        {
            var transactionType = SelectedTransactionTypeIndex == 0 ? TransactionType.Expense : TransactionType.Income;

            // Clear the existing items in the FilteredTransactions collection
            FilteredTransactions.Clear();
            TotalCreditTransactionsAmount = 0;
            TotalDebitTransactionsAmount = 0;

            // Add the filtered transactions to the existing collection
            foreach(var transaction in Transactions.Where(t => t.TransactionType == transactionType))
            {
                UpdateTransactionAmounts(transaction.TransactionType, transaction.Amount);
                FilteredTransactions.Add(transaction);
            }

            OnPropertyChanged(nameof(FilteredTransactions));

            if(transactionType == TransactionType.Income)
            {
                TotalTransactionsAmount = TotalCreditTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            } else
            {
                TotalTransactionsAmount = TotalDebitTransactionsAmount;
                OnPropertyChanged(nameof(TotalTransactionsAmount));
            }

            //UpdateGroupedTransactionsByCategory();

            //UpdateChartVisibility();
        }

        private void UpdateBalance() { Balance = TotalCreditTransactionsAmount - TotalDebitTransactionsAmount; }

        private void UpdateGroupedTransactionsByCategory()
        {
            GroupedCategoryTransactions = FilteredTransactions
                .GroupBy(t => t.Category)
                //.Select(g => new Grouping<string, Transaction>(g.Key, g.ToList()))
                .Select(
                    g => new TransactionCategoryGroup(
                        g.Key,
                        g.OrderByDescending(t => t.Category).ToObservableCollection()))
                .ToObservableCollection();

            //GroupedCategoryTransactions = new ObservableCollection<Grouping<string, Transaction>>(grouped);

            //GroupedCategoryTransactions = new ObservableCollection<TransactionCategoryGroup>(
            //    FilteredTransactions
            //.GroupBy(t => t.Category)
            //        .Select(
            //            g => new TransactionCategoryGroup(g.Key, new ObservableCollection<Transaction>(g))
            //                {
            //                    TotalTransactionsAmount = TotalTransactionsAmount
            //                }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }
}
