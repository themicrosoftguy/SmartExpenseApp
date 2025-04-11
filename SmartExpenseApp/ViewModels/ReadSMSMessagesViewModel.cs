using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SmartExpenseApp.Models;

namespace SmartExpenseApp.ViewModels
{
    public class ReadSMSMessagesViewModel : BaseViewModel
    {
        public ObservableCollection<SMSMessageModel> SMSMessages { get; } = new ObservableCollection<SMSMessageModel>();

        private double totalDebitTransactionsAmount;
        public double TotalDebitTransactionsAmount
        {
            get => totalDebitTransactionsAmount;
            set => SetProperty(ref totalDebitTransactionsAmount, value);
        }

        private double totalCreditTransactionsAmount;
        public double TotalCreditTransactionsAmount
        {
            get => totalCreditTransactionsAmount;
            set => SetProperty(ref totalCreditTransactionsAmount, value);
        }

        public Command<string> ReadSMSCommand { get; }

        public ReadSMSMessagesViewModel()
        {
            ReadSMSCommand = new Command<string>(async (parameter) => await ReadSMSMessagesAsync(parameter));
        }

        private async Task ReadSMSMessagesAsync(string smsCount)
        {
            var res = await CheckAndRequestSMSPermission();

            if (res.Equals(PermissionStatus.Granted))
            {
#if ANDROID
                string INBOX = "content://sms/inbox";
                string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
                Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);

                string sortOrder = string.Concat("date DESC LIMIT ", smsCount);
                Android.Database.ICursor cursor = Platform.CurrentActivity.ContentResolver.Query(uri, reqCols, null, null, sortOrder);

                if (cursor.MoveToFirst())
                {
                    SMSMessages.Clear();
                    TotalCreditTransactionsAmount = 0;
                    TotalDebitTransactionsAmount = 0;

                    do
                    {
                        var message = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));

                        if (MessageHasKeywords(message))
                        {
                            var smsMessage = new SMSMessageModel
                            {
                                MessageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0])),
                                ThreadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1])),
                                Address = cursor.GetString(cursor.GetColumnIndex(reqCols[2])),
                                Name = cursor.GetString(cursor.GetColumnIndex(reqCols[3])),
                                Date = ConvertEpochToDateTime(cursor.GetString(cursor.GetColumnIndex(reqCols[4]))),
                                Message = cursor.GetString(cursor.GetColumnIndex(reqCols[5])),
                                Type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]))
                            };

                            SMSMessages.Add(smsMessage);

                            if (IsDebitTransaction(message, out double amount))
                            {
                                TotalDebitTransactionsAmount += amount;
                                //DebitTransactions.Add(smsMessage);
                            }
                            else if (IsCreditTransaction(message, out amount))
                            {
                                TotalCreditTransactionsAmount += amount;
                                //CreditTransactions.Add(smsMessage);
                            }
                        }                      

                    } while (cursor.MoveToNext());
                }
#endif
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