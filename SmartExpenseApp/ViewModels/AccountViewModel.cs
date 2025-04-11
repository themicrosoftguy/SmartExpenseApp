using System.Collections.ObjectModel;

namespace SmartExpenseApp.ViewModels
{
    public class AccountViewModel
    {
        public ObservableCollection<string> AccountTypes { get; set; }

        public AccountViewModel()
        {
            AccountTypes = new ObservableCollection<string>
            {
                "Bank",
                "Source",
            };
        }
    }
}