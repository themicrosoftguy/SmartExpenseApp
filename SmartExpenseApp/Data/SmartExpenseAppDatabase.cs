using SmartExpenseApp.Models;
using SmartExpenseApp.Utilities;
using SQLite;
using System.Text.RegularExpressions;
using static SmartExpenseApp.Utilities.SmartExpenseEnums;

namespace SmartExpenseApp.Data
{
    public class SmartExpenseAppDatabase
    {
        SQLiteAsyncConnection database;

        private double totalDebitTransactionsAmount;
        public double TotalDebitTransactionsAmount
        {
            get => totalDebitTransactionsAmount;
            set
            {
                totalDebitTransactionsAmount = value;
                UpdateBalance();
            }
        }

        private double totalCreditTransactionsAmount;
        public double TotalCreditTransactionsAmount
        {
            get => totalCreditTransactionsAmount;
            set
            {
                totalCreditTransactionsAmount = value;
                UpdateBalance();
            }
        }

        private double balance;
        public double Balance
        {
            get => balance;
            set => balance = value;
        }

        private void UpdateBalance()
        {
            Balance = TotalCreditTransactionsAmount - TotalDebitTransactionsAmount;
        }

        public SmartExpenseAppDatabase(string dbPath)  
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Transaction>().Wait();
        }

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await database.Table<Transaction>().ToListAsync();
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await database.Table<Transaction>()
                            .Where(t => t.ID == id) // Filter by TransactionId
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveTransactionAsync(Transaction item)
        {
            if (item.TransactionType == TransactionType.Income)
            {
                TotalCreditTransactionsAmount += double.Parse(item.Amount);
            }
            else if (item.TransactionType == TransactionType.Expense)
            {
                TotalDebitTransactionsAmount += double.Parse(item.Amount);
            }

            if (item.ID != 0)
            {
                return await database.UpdateAsync(item);
            }
            else
            {
                return await database.InsertAsync(item);
            }
        }

        public async Task<int> DeleteTransactionAsync(Transaction item)
        {
            return await database.DeleteAsync(item);
        }

        public async Task<int> DeleteAllTransactionsAsync()
        {
            return await database.ExecuteAsync("DELETE FROM [Transaction] where IsManual = 0");
        }

        public async Task<int> AddSMSMessageTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            return await database.InsertAllAsync(transactions);
        }

        public async Task<int> TransformAndSaveSMSMessagesAsync(IEnumerable<SMSMessageModel> smsMessages)
        {
            var transactions = new List<Transaction>();

            TotalCreditTransactionsAmount = 0;
            TotalDebitTransactionsAmount = 0;
            Balance = 0;

            foreach (var sms in smsMessages)
            {
                var messageExists = await TransactionExistsAsync(DateTime.Parse(sms.Date), sms.Address);
                if (messageExists)
                {
                    continue; // Skip if the transaction already exists
                }

                var transactionType = FetchTransactionType(sms.Message);
                var title = ExtractTitleFromMessage(sms.Message);

                var transaction = new Transaction
                {
                    Title = title,
                    Description = sms.Message,
                    Amount = ExtractAmountFromMessage(sms.Message, transactionType),
                    Date = DateTime.Parse(sms.Date),
                    Category = FetchCategoryFromTitle(title),
                    Sender = sms.Address,
                    TransactionType = transactionType,
                    IsManual = 0
                };

                transactions.Add(transaction);
            }

            return await AddSMSMessageTransactionsAsync(transactions);
        }

        private string ExtractAmountFromMessage(string message, TransactionType transactionType)
        {
            Match match = null;

            if (transactionType == TransactionType.Income)
            {
                match = Regex.Match(message, @"INR \s*\d+(\.\d{1,2})?");
            }
            else
            {
                match = Regex.Match(message, @"debited by \s*\d+(\.\d{1,2})?");
            }

            string amount = string.Empty;

            if (match.Success && transactionType == TransactionType.Income)
            {
                amount = match.Value.Replace("INR", "").Trim();
                TotalCreditTransactionsAmount += double.Parse(amount);
                return amount;
            }
            else if (match.Success && transactionType == TransactionType.Expense)
            {
                amount = match.Value.Replace("debited by", "").Trim();
                TotalDebitTransactionsAmount += double.Parse(amount);
                return amount;
            }

            return "0";
        }

        private TransactionType FetchTransactionType(string message)
        {
            if (message.Contains("credited", StringComparison.OrdinalIgnoreCase))
            {
                return TransactionType.Income;
            }
            else if (message.Contains("debited", StringComparison.OrdinalIgnoreCase))
            {
                return TransactionType.Expense;
            }

            return TransactionType.Expense;
        }

        private string ExtractTitleFromMessage(string message)
        {
            Match match = null;

            string pattern = @"(?:at|to|from|via)\s+(.+?)\s+Refno";
            match = Regex.Match(message, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            
            return "0";
        }

        public async Task<bool> TransactionExistsAsync(DateTime date, string sender)
        {
            var existingTransaction = await database.Table<Transaction>()
                .Where(t => t.Date == date && t.Sender == sender)
                .FirstOrDefaultAsync();

            return existingTransaction != null;
        }

        private string FetchCategoryFromTitle(string title)
        {
            foreach (var mapping in Constants.CategoryMappings)
            {
                if (title.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return mapping.Value;
                }
            }

            return "Others"; // Default category if no match is found
        }
    }
}