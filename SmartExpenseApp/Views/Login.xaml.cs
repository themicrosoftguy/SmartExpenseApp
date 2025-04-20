using SmartExpenseApp.Data;
using System.Diagnostics;

namespace SmartExpenseApp.Views;

public partial class Login : ContentPage
{
    private readonly SmartExpenseAppDatabase _database;

    public Login(SmartExpenseAppDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("signup");
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Validate credentials
            if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Information", "Please enter your username and password", "OK");
                return;
            }

            // Query the database for the user
            var user = await _database.GetUserByEmailAsync(EmailEntry.Text);

            if (user != null && user.Password == PasswordEntry.Text)
            {
                // Login successful
                await DisplayAlert("Success", "Login successful!", "OK");
                await Shell.Current.GoToAsync("//home");
            }
            else
            {
                // Login failed
                await DisplayAlert("Error", "Incorrect email or password", "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message, "Error in Login");

            await DisplayAlert("Error", "An error occurred while logging in. Please try again.", "OK");
        }        
    }
}