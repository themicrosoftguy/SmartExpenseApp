namespace SmartExpenseApp.Views;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("signup");
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Validate credentials
        if(string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
        {
            await DisplayAlert("Information", "Please enter your username and password", "OK");
            return;
        }
        else if (EmailEntry.Text.Equals("abc@gmail.com") && PasswordEntry.Text.Equals("pass@123"))
        {
            await Shell.Current.GoToAsync("//home");
        }
        else
        {
            await DisplayAlert("Information", "Incorrect username or password", "OK");
            return;
        }
    }

}