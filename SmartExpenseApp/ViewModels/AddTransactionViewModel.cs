using SmartExpenseApp.Data;
using SmartExpenseApp.Models;
using Syncfusion.Maui.Buttons;

namespace SmartExpenseApp.ViewModels
{
    public class AddTransactionViewModel: BaseViewModel
    {
        private List<SfSegmentItem> segmentItems;

        public string[] Categories { get; set; }
        public string[] SourceList { get; set; }

        private readonly SmartExpenseAppDatabase _database;
        private Transaction transaction;

        public Transaction Transaction
        {
            get => transaction;
            set => SetProperty(ref transaction, value);
        }

        public AddTransactionViewModel(SmartExpenseAppDatabase database)
        {
            _database = database;

            segmentItems = new List<SfSegmentItem>()
            {
                new SfSegmentItem() {  ImageSource="expense_icon.png", Text="Expense" },
                new SfSegmentItem() { ImageSource="income_icon.png", Text="Income" },
                new SfSegmentItem() { ImageSource ="icon_scan_document.png", Text="Scan" },
            };

            Categories =
            [
                "Travel",
                "Food",
                "Shopping",
                "Health",
                "Education",
                "Others"
            ];

            SourceList =
            [
                "SBI",
                "BOB",
                "ICICI",
                "Cash"
            ];
        }

        public List<SfSegmentItem> SegmentItems
        {
            get { return segmentItems; }
            set { segmentItems = value; }
        }

        public async Task InitializeAsync(int transactionId)
        {
            Transaction = await _database.GetTransactionByIdAsync(transactionId);
        }
    }
}