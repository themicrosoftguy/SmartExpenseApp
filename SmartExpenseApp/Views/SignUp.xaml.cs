using SmartExpenseApp.Data;
using SmartExpenseApp.Models;
using System.Diagnostics;

namespace SmartExpenseApp.Views;

public partial class SignUp : ContentPage
{

    private readonly SmartExpenseAppDatabase _database;

    public SignUp(SmartExpenseAppDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("///login");
    }

    private async void SignupButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(NameEntry.Text) || string.IsNullOrEmpty(MobileEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
            {
                await DisplayAlert("Information", "Please fill all the fields", "Ok");
                return;
            }

            // Check if the email already exists
            var existingUser = await _database.GetUserByEmailAsync(EmailEntry.Text);
            if (existingUser != null)
            {
                await DisplayAlert("Error", "An account with this email already exists.", "Ok");
                return;
            }

            // Save the user data
            var user = new User
            {
                Name = NameEntry.Text,
                Mobile = MobileEntry.Text,
                Email = EmailEntry.Text,
                Password = PasswordEntry.Text
            };

            await _database.SaveUserAsync(user);

            await DisplayAlert("Success", "Account created successfully!", "Ok");
            await Shell.Current.GoToAsync("//home");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message, "Error in SignUp");
            await DisplayAlert("Error", "An error occurred while creating the account. Please try again.", "Ok");
        }
        
    }
}