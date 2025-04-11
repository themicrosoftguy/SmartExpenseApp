using MvvmHelpers.Commands;
using SmartExpenseApp.Data;
using SmartExpenseApp.Utilities;
using SmartExpenseApp.ViewModels;

namespace SmartExpenseApp.Views;

public partial class HomeScreen : ContentPage
{
    private HomeScreenViewModel viewModel;
    SmartExpenseAppDatabase database;

    public HomeScreen(SmartExpenseAppDatabase smartExpenseAppDatabase)
    {
        InitializeComponent();

        BindingContext = viewModel =
            new HomeScreenViewModel(SmartExpenseEnums.TransactionsFilterEnums.Today, smartExpenseAppDatabase);

        database = smartExpenseAppDatabase;
    }

    private void tabView_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.TabView.TabSelectionChangedEventArgs e)
    {
        viewModel.TabViewCurrentSelectedIndex = e.NewIndex;

        viewModel.GetFilteredTransactionList(viewModel.TabViewCurrentSelectedIndex);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        MainThread.BeginInvokeOnMainThread(
            async () =>
            {
                viewModel.IsBusy = true;

                viewModel.TotalCreditTransactionsAmount = 0;
                viewModel.TotalDebitTransactionsAmount = 0;
                viewModel.Balance = 0;

                await ((AsyncCommand<int>)viewModel.ReadSMSCommand).ExecuteAsync(Constants.SMSMessagesMaxFetchCount);
                //database.DeleteAllTransactionsAsync();

                viewModel.GetFilteredTransactionList(viewModel.TabViewCurrentSelectedIndex);

                viewModel.IsBusy = false;
            });
    }
}