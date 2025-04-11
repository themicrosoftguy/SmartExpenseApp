using SmartExpenseApp.Data;
using SmartExpenseApp.ViewModels;

namespace SmartExpenseApp.Views
{
    [QueryProperty(nameof(ID), "transactionId")]
    public partial class TransactionDetails : ContentPage
    {
        private readonly TransactionDetailsViewModel viewModel;

        public string ID { get; set; }

        public TransactionDetails(SmartExpenseAppDatabase database)
        {
            InitializeComponent();
            viewModel = new TransactionDetailsViewModel(database);
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (int.TryParse(ID, out int id))
            {
                await viewModel.LoadTransactionDetails(id);
            }
        }
    }
}