using MvvmHelpers.Commands;
using SmartExpenseApp.Data;
using SmartExpenseApp.ViewModels;
using Syncfusion.Maui.Toolkit.TabView;
using System.Diagnostics;

namespace SmartExpenseApp.Views;

public partial class GraphAndTrends : ContentPage
{
    private GraphAndTrendsViewModel viewModel;
    SmartExpenseAppDatabase database;

    public GraphAndTrends(SmartExpenseAppDatabase smartExpenseAppDatabase)
    {
        InitializeComponent();

        BindingContext = viewModel = new GraphAndTrendsViewModel(smartExpenseAppDatabase);

        database = smartExpenseAppDatabase;

        // Set the BindingContext for the CenterView of the DoughnutSeries
        SetCenterViewBindingContext();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        MainThread.BeginInvokeOnMainThread(
            async () =>
            {
                try
                {
                    viewModel.IsBusy = true;

                    viewModel.TotalCreditTransactionsAmount = 0;
                    viewModel.TotalDebitTransactionsAmount = 0;
                    viewModel.TotalTransactionsAmount = 0;
                    viewModel.Balance = 0;

                    //await ((AsyncCommand<int>)viewModel.ReadSMSCommand).ExecuteAsync(Constants.SMSMessagesMaxFetchCount);
                    //database.DeleteAllTransactionsAsync();

                    //viewModel.GetFilteredTransactionList(viewModel.TabViewCurrentSelectedIndex);

                    await viewModel.LoadTransactions(Utilities.SmartExpenseEnums.TransactionType.Expense);

                    viewModel.IsBusy = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An Error occurred on {0} page. Error: {1}", nameof(GraphAndTrends),  ex.Message);
                }
            });
    }

    private void TabViewTransactionType_SelectionChanged(
        object sender,
        Syncfusion.Maui.Toolkit.TabView.TabSelectionChangedEventArgs e)
    {
        viewModel.SelectedTransactionTypeIndex = e.NewIndex;
    }

    private void ChartTypeSegmentedControl_SelectionChanged(
        object sender,
        Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    { 
        viewModel.SelectedSegmentIndex = (int)e.NewIndex; 
    }

    private void SetCenterViewBindingContext()
    {
        

        if (CategoryDoughnutSeries.CenterView is View centerView)
        {
            centerView.BindingContext = BindingContext;
        }
    }
}