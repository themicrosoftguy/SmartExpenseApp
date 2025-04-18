using MvvmHelpers.Commands;
using SmartExpenseApp.Data;
using SmartExpenseApp.Models;
using SmartExpenseApp.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static SmartExpenseApp.Utilities.SmartExpenseEnums;
using MauiCommand = Microsoft.Maui.Controls.Command;

namespace SmartExpenseApp.ViewModels
{
    public class HomeScreenViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private ObservableCollection<Transaction> transactions;
        private List<Transaction> filteredTransactionsList;
        private readonly SmartExpenseAppDatabase _smartExpenseAppDatabase;
        private bool isLoading;
        public ObservableCollection<SMSMessageModel> SMSMessages { get; } = new ObservableCollection<SMSMessageModel>();

        public ObservableCollection<string> Months { get; set; }

        public ObservableCollection<Transaction> FilteredTransactions { get; set; }

        public int TabViewCurrentSelectedIndex { get; set; }

        public ICommand ReadSMSCommand { get; }

        public ICommand NavigateToProfileCommand { get; }

        public ICommand NavigateToTransactionsListCommand { get; }

        public ObservableCollection<Transaction> Transactions
        {
            get => transactions;
            set
            {
                transactions = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
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
        public double Balance
        {
            get => balance;
            set => SetProperty(ref balance, value);
        }

        private void UpdateBalance()
        {
            Balance = TotalCreditTransactionsAmount - TotalDebitTransactionsAmount;
        }

        public HomeScreenViewModel(SmartExpenseEnums.TransactionsFilterEnums transactionsFilter, SmartExpenseAppDatabase smartExpenseAppDatabase)
        {
            _smartExpenseAppDatabase = smartExpenseAppDatabase;

            FilteredTransactions = new ObservableCollection<Transaction>();
            Transactions = new ObservableCollection<Transaction>();
            ReadSMSCommand = new AsyncCommand<int>(async (parameter) => await ReadSMSMessagesAsync(parameter));

            NavigateToProfileCommand = new MauiCommand(async () => await NavigateToProfilePage());

            NavigateToTransactionsListCommand = new MauiCommand(async () => await Shell.Current.GoToAsync("///transactions"));

            filteredTransactionsList = new List<Transaction>();
        }

        private async Task NavigateToProfilePage()
        {
            await Shell.Current.GoToAsync("///profile");
        }

        public void GetFilteredTransactionList(int tabViewCurrentSelectedIndex)
        {
            Task.Run(async () => await LoadTransactions()).Wait();

            TotalCreditTransactionsAmount = 0;
            TotalDebitTransactionsAmount = 0;

            foreach (var transaction in transactions)
            {
                UpdateTransactionAmounts(transaction.TransactionType, transaction.Amount);
                FilteredTransactions.Add(transaction);
            }

            if (tabViewCurrentSelectedIndex == 0)
            {
                FilterItems(SmartExpenseEnums.TransactionsFilterEnums.Today);
            }
            else if (tabViewCurrentSelectedIndex == 1)
            {
                FilterItems(SmartExpenseEnums.TransactionsFilterEnums.Week);
            }
            else if (tabViewCurrentSelectedIndex == 2)
            {
                FilterItems(SmartExpenseEnums.TransactionsFilterEnums.Month);
            }

            OnPropertyChanged(nameof(FilteredTransactions));
        }

        private async Task LoadTransactions()
        {
            var transactions = await _smartExpenseAppDatabase.GetTransactionsAsync();

            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }

        // Method to Filter Data
        public void FilterItems(SmartExpenseEnums.TransactionsFilterEnums transactionsFilterEnums)
        {
            FilteredTransactions.Clear();

            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(1).AddTicks(-1); // Today with maximum time of the day

            switch (transactionsFilterEnums)
            {
                case SmartExpenseEnums.TransactionsFilterEnums.Today:
                    filteredTransactionsList = Transactions.Where(
                        item => item.Date >= startDate && item.Date <= endDate).OrderByDescending(item => item.Date).ToList();
                    break;

                case SmartExpenseEnums.TransactionsFilterEnums.Week:
                    filteredTransactionsList = Transactions.Where(
                        item => item.Date >= DateTime.Today.AddDays(-7) && item.Date <= endDate).OrderByDescending(item => item.Date).ToList();
                    break;

                case SmartExpenseEnums.TransactionsFilterEnums.Month:
                    var firstDayOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);
                    filteredTransactionsList = Transactions.Where(
                        item => item.Date >= firstDayOfMonth && item.Date <= lastDayOfMonth).OrderByDescending(item => item.Date).ToList();
                    break;

                default:
                    filteredTransactionsList = Transactions.Where(
                        item => item.Date >= startDate && item.Date <= endDate).OrderByDescending(item => item.Date).ToList();
                    break;
            }

            FilteredTransactions = new ObservableCollection<Transaction>(filteredTransactionsList);

            OnPropertyChanged(nameof(FilteredTransactions));
        }

        private async Task ReadSMSMessagesAsync(int smsCount)
        {

            #region "UNCOMMENT THIS"

            //            var res = await CheckAndRequestSMSPermission();
            //            if (res.Equals(PermissionStatus.Granted))
            //            {
            //#if ANDROID
            //                string INBOX = "content://sms/inbox";
            //                string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
            //                Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);

            //                string sortOrder = string.Concat("date DESC LIMIT ", smsCount);
            //                Android.Database.ICursor cursor = Platform.CurrentActivity.ContentResolver.Query(uri, reqCols, null, null, sortOrder);

            //                if (cursor.MoveToFirst())
            //                {
            //                    SMSMessages.Clear();
            //                    TotalCreditTransactionsAmount = 0;
            //                    TotalDebitTransactionsAmount = 0;

            //                    do
            //                    {
            //                        var message = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));

            //                        if (MessageHasKeywords(message))
            //                        {
            //                            var smsMessage = new SMSMessageModel
            //                            {
            //                                MessageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0])),
            //                                ThreadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1])),
            //                                Address = cursor.GetString(cursor.GetColumnIndex(reqCols[2])),
            //                                Name = cursor.GetString(cursor.GetColumnIndex(reqCols[3])),
            //                                Date = ConvertEpochToDateTime(cursor.GetString(cursor.GetColumnIndex(reqCols[4]))),
            //                                Message = cursor.GetString(cursor.GetColumnIndex(reqCols[5])),
            //                                Type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]))
            //                            };

            //                            SMSMessages.Add(smsMessage);
            //                        }                      

            //                    } while (cursor.MoveToNext());

            //                    //await _smartExpenseAppDatabase.DeleteAllTransactionsAsync();

            //                    await _smartExpenseAppDatabase.TransformAndSaveSMSMessagesAsync(SMSMessages);
            //                }
            //#endif
            //            }

            #endregion

            GenerateStaticSMSMessagesData();

            await _smartExpenseAppDatabase.DeleteAllTransactionsAsync();
            await _smartExpenseAppDatabase.TransformAndSaveSMSMessagesAsync(SMSMessages);
        }

        #region "DELETE THIS"

        private void GenerateStaticSMSMessagesData()
        {
            var random = new Random();

            // Generate messages for today
            for (int i = 0; i < 2; i++)
            {
                SMSMessages.Add(new SMSMessageModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ThreadId = Guid.NewGuid().ToString(),
                    Address = $"Sender{i}",
                    Name = $"Sender Name {i}",
                    Date = DateTime.Now.AddMinutes(-i * 10).ToString("g"),
                    Message = GenerateRandomMessage(random),
                    Type = "1"
                });
            }

            // Generate messages for this week
            for (int i = 0; i < 4; i++)
            {
                SMSMessages.Add(new SMSMessageModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ThreadId = Guid.NewGuid().ToString(),
                    Address = $"Sender{i + 10}",
                    Name = $"Sender Name {i + 10}",
                    Date = DateTime.Now.AddDays(-i % 7).ToString("g"),
                    Message = GenerateRandomMessage(random),
                    Type = "1"
                });
            }

            // Generate messages for this month
            for (int i = 0; i < 10; i++)
            {
                SMSMessages.Add(new SMSMessageModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ThreadId = Guid.NewGuid().ToString(),
                    Address = $"Sender{i + 30}",
                    Name = $"Sender Name {i + 30}",
                    Date = DateTime.Now.AddDays(-i % 30).ToString("g"),
                    Message = GenerateRandomMessage(random),
                    Type = "1"
                });
            }

            // Generate messages for the previous month
            for (int i = 0; i < 5; i++)
            {
                SMSMessages.Add(new SMSMessageModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ThreadId = Guid.NewGuid().ToString(),
                    Address = $"Sender{i + 50}",
                    Name = $"Sender Name {i + 50}",
                    Date = DateTime.Now.AddMonths(-1).AddDays(-i).ToString("g"),
                    Message = GenerateRandomMessage(random),
                    Type = "1"
                });
            }

            // Generate messages for two months ago
            for (int i = 0; i < 5; i++)
            {
                SMSMessages.Add(new SMSMessageModel
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ThreadId = Guid.NewGuid().ToString(),
                    Address = $"Sender{i + 55}",
                    Name = $"Sender Name {i + 55}",
                    Date = DateTime.Now.AddMonths(-2).AddDays(-i).ToString("g"),
                    Message = GenerateRandomMessage(random),
                    Type = "1"
                });
            }
        }

        private string GenerateRandomMessage(Random random)
        {
            // Randomly decide if the message is a credit or debit transaction
            bool isCredit = random.Next(0, 2) == 0;

            if (isCredit)
            {
                // Generate a random credit transaction message
                double amount = Math.Round(random.NextDouble() * 10000, 2); // Random amount between 0 and 10,000
                string name = GenerateRandomName(random);
                return $"Dear Customer, INR {amount} credited to your A/c No XX0320 on {DateTime.Now:dd/MM/yyyy} through NEFT by {name}.";
            }
            else
            {
                // Generate a random debit transaction message
                double amount = Math.Round(random.NextDouble() * 10000, 2); // Random amount between 0 and 10,000
                string name = GenerateRandomName(random);
                return $"Dear UPI user A/C X0320 debited by {amount} on date {DateTime.Now:ddMMMyy} trf to {name} Refno {random.Next(100000000, 999999999)}.";
            }
        }

        private string GenerateRandomName(Random random)
        {
            // List of random names
            var names = new[] { "ABC", "XYZ Corp", "John Doe", "Jane Smith", "F3 Two", "QuickPay", "PayPal", "Amazon", "Google", "Microsoft" };
            return names[random.Next(names.Length)];
        }

        #endregion

        private void UpdateTransactionAmounts(TransactionType transaction, string amount)
        {
            if (transaction == TransactionType.Income)
            {
                TotalCreditTransactionsAmount += double.Parse(amount);
                OnPropertyChanged(nameof(TotalCreditTransactionsAmount));
                OnPropertyChanged(nameof(Balance));
            }
            else
            {
                TotalDebitTransactionsAmount += double.Parse(amount);
                OnPropertyChanged(nameof(TotalDebitTransactionsAmount));
                OnPropertyChanged(nameof(Balance));
            }
        }

        private async Task<PermissionStatus> CheckAndRequestSMSPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Sms>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Sms>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.Sms>();
            return status;
        }

        private string ConvertEpochToDateTime(string epochString)
        {
            if (long.TryParse(epochString, out long epoch))
            {
                var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(epoch);
                return dateTimeOffset.ToString("g"); // You can customize the format as needed
            }
            return string.Empty;
        }

        private bool MessageHasKeywords(string message)
        {
            var keywords = new[] { "credited to your A/c No XX0320", "A/C X0320 debited by" }; // Replace with actual keywords

            //var keywords = new[] { "Acct XX035 is credited with" }; // Replace with actual keywords

            foreach (var keyword in keywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsDebitTransaction(string message, out double amount)
        {
            var debitKeywords = new[] { "A/C X0320 debited by" }; // Replace with actual debit keywords
            return ContainsKeywordsAndExtractAmount(message, debitKeywords, out amount);
        }

        private bool IsCreditTransaction(string message, out double amount)
        {
            var creditKeywords = new[] { "credited to your A/c No XX0320" }; // Replace with actual credit keywords
            return ContainsKeywordsAndExtractAmount(message, creditKeywords, out amount);
        }

        private bool ContainsKeywordsAndExtractAmount(string message, string[] keywords, out double amount)
        {
            amount = 0;
            foreach (var keyword in keywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(message, @"INR \s\d+(\.\d{1,2})?");
                    if (match.Success)
                    {
                        var amountString = match.Value.Replace("INR ", "").Trim();
                        amount = double.Parse(amountString);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}