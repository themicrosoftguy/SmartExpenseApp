using SmartExpenseApp.Data;
using SmartExpenseApp.ViewModels;
using static SmartExpenseApp.Utilities.SmartExpenseEnums;
using Transaction = SmartExpenseApp.Models.Transaction;

namespace SmartExpenseApp.Views;

[QueryProperty(nameof(TransactionId), "transactionId")]
public partial class AddTransactionPage : ContentPage
{
    SmartExpenseAppDatabase database;

    private readonly AddTransactionViewModel viewModel;

    public string TransactionId { get; set; }

    public AddTransactionPage(SmartExpenseAppDatabase smartExpenseAppDatabase)
	{
		InitializeComponent();

        database = smartExpenseAppDatabase;
        viewModel = new AddTransactionViewModel(database);
        BindingContext = viewModel;
    }

    private void ExpenseButton_Clicked(object sender, EventArgs e)
    {
        this.ExpenseTransactionDateTimePicker.IsOpen = true;
    }

    private void TransactionTypeSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        if (e.NewIndex == 0)
        {
            AddTransactionPageGrid.BackgroundColor = Color.FromArgb("#FF3C4A");
        }

        else if (e.NewIndex == 1)
        {
            AddTransactionPageGrid.BackgroundColor = Color.FromArgb("#00A86B");
        }
    }

    private void TransactionDateTimePicker_OkButtonClicked(object sender, EventArgs e)
    {
        DateEntry.Text = ExpenseTransactionDateTimePicker.SelectedDate.ToString();
        ExpenseTransactionDateTimePicker.IsOpen = false;
    }

    private void TransactionDateTimePicker_CancelButtonClicked(object sender, EventArgs e)
    {
        ExpenseTransactionDateTimePicker.IsOpen = false;
        //DateEntry.Text = "";
    }

    private async void AddTransactionButton_Clicked(object sender, EventArgs e)
    {
        // Validate that all fields contain values
        if (string.IsNullOrWhiteSpace(AmountEntry.Text) ||
            string.IsNullOrWhiteSpace(TitleEntry.Text) ||
            string.IsNullOrWhiteSpace(DateEntry.Text) ||
            CategoryPicker.SelectedItem == null ||
            SourcePicker.SelectedItem == null)
        {
            await DisplayAlert("Validation Error", "Please fill in all fields.", "OK");
            return;
        }

        TransactionType transactionType; //= TransactionTypeSegmentedControl.SelectedIndex == 0 ? TransactionType.Expense : TransactionType.Income;

        switch (TransactionTypeSegmentedControl.SelectedIndex)
        {
            case 0:
                transactionType = TransactionType.Expense;
                break;
            case 1:
                transactionType = TransactionType.Income;
                break;
            case 2:
                transactionType = TransactionType.Scan;
                break;
            default:
                transactionType = TransactionType.Expense;
                break;
        }

        Transaction transaction;

        if(int.TryParse(TransactionId, out int id))
        {
            transaction = await database.GetTransactionByIdAsync(id);
            transaction.Amount = AmountEntry.Text;
            transaction.Title = TitleEntry.Text;
            transaction.Date = Convert.ToDateTime(DateEntry.Text);
            transaction.Category = CategoryPicker.SelectedItem?.ToString();
            transaction.Source = SourcePicker.SelectedItem?.ToString();
        }
        else
        {
            transaction = new Transaction
            {
                Amount = AmountEntry.Text,
                Title = TitleEntry.Text,
                Date = Convert.ToDateTime(DateEntry.Text),
                Category = CategoryPicker.SelectedItem?.ToString(),
                Source = SourcePicker.SelectedItem?.ToString(),
                TransactionType = transactionType,
                IsManual = 1
            };
        }

        App.Database.SaveTransactionAsync(transaction);
        DisplayAlert("Success", "Transaction Saved!", "OK");

        ResetAllFields(TransactionTypeSegmentedControl.SelectedIndex);
    }

    private void ResetAllFields(int? transactionTypeSelectedIndex)
    {
        // Reset all fields
        AmountEntry.Text = string.Empty;
        TitleEntry.Text = string.Empty;
        DateEntry.Text = string.Empty;
        CategoryPicker.SelectedItem = null;
        SourcePicker.SelectedItem = null;
        TransactionTypeSegmentedControl.SelectedIndex = transactionTypeSelectedIndex;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await Task.Yield();


        if (int.TryParse(TransactionId, out int id))
        {
            await viewModel.InitializeAsync(id);
            Title = "Edit Transaction"; // Change the page title to "Edit Transaction"
            AddTransactionButton.Text = "Update Transaction"; // Change button text to "Update Transaction"

            var transaction = await database.GetTransactionByIdAsync(id);

            if (transaction.IsManual == 0)
            {
                AmountEntry.IsEnabled = false;
                //TitleEntry.IsEnabled = true;
                DateEntry.IsEnabled = false;
                DescriptionEditor.IsEnabled = false;
                CategoryPicker.IsEnabled = true;
                SourcePicker.IsEnabled = true;
            }
            else
            {
                AmountEntry.IsEnabled = true;
                TitleEntry.IsEnabled = true;
                DateEntry.IsEnabled = true;
                DescriptionEditor.IsEnabled = true;
                CategoryPicker.IsEnabled = true;
                SourcePicker.IsEnabled = true;
            }
        }
        else
        {
            Title = "Add Transaction"; // Default title for adding a new transaction
            AddTransactionButton.Text = "Add Transaction"; // Default button text for adding a new transaction
        }
    }
}