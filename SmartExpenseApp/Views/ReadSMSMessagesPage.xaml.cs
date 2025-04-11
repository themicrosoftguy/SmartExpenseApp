using SmartExpenseApp.ViewModels;

namespace SmartExpenseApp.Views;

public partial class ReadSMSMessagesPage : ContentPage
{
	public ReadSMSMessagesPage()
	{
		InitializeComponent();
        BindingContext = new ReadSMSMessagesViewModel();
    }

    private async void ReadSMSButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is ReadSMSMessagesViewModel viewModel)
        {
            viewModel.ReadSMSCommand.Execute(SMSCountEntry.Text);
        }
    }
}