using SmartExpenseApp.Data;
using SmartExpenseApp.Utilities;
using SmartExpenseApp.ViewModels;

namespace SmartExpenseApp.Views;

public partial class TransactionsListPage : ContentPage
{
    private GroupedTransactionsViewModel viewModel;
    SmartExpenseAppDatabase database;

    public TransactionsListPage(SmartExpenseAppDatabase smartExpenseAppDatabase)
	{
		InitializeComponent();

  //      BindingContext = new TransactionViewModel();
		//BindingContext = new HomeScreenViewModel(SmartExpenseEnums.TransactionsFilterEnums.Today, smartExpenseAppDatabase);
        BindingContext = viewModel = new GroupedTransactionsViewModel(smartExpenseAppDatabase);

        database = smartExpenseAppDatabase;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var transactions = await database.GetTransactionsAsync();

        if (transactions.Count > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                viewModel.GetGroupedTransactionList();
            });
        }
    }
}