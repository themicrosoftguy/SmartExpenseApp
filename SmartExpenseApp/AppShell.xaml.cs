using SmartExpenseApp.Views;

namespace SmartExpenseApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes
            Routing.RegisterRoute("signup", typeof(SignUp));
            Routing.RegisterRoute("login", typeof(Login));
            Routing.RegisterRoute("account", typeof(Account));
            Routing.RegisterRoute("transactiondetails", typeof(TransactionDetails));
            Routing.RegisterRoute("addtransactionpage", typeof(AddTransactionPage));
        }
    }
}