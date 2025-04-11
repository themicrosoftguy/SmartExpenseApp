using Microsoft.Maui.Controls;

namespace SmartExpenseApp.Views
{
    public partial class Profile : ContentPage
    {
        public Profile()
        {
            InitializeComponent();
        }

        // Event handler for Account button
        private void OnAccountClicked(object sender, EventArgs e)
        {
            // Navigate to Account page or show account information
            DisplayAlert("Account", "Account button clicked", "OK");
            // Navigation Example:
            // await Navigation.PushAsync(new AccountPage());
        }

        // Event handler for Settings button
        private void OnSettingsClicked(object sender, EventArgs e)
        {
            // Navigate to Settings page or show settings
            DisplayAlert("Settings", "Settings button clicked", "OK");
            // Navigation Example:
            // await Navigation.PushAsync(new SettingsPage());
        }

        // Event handler for Export Data button
        private void OnExportDataClicked(object sender, EventArgs e)
        {
            // Trigger export data functionality
            DisplayAlert("Export Data", "Export Data button clicked", "OK");
        }

        // Event handler for Logout button
        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Perform logout operation
            DisplayAlert("Logout", "Logging out...", "OK");
            // Example: Clear session, navigate to login screen, etc.
        }
    }
}

