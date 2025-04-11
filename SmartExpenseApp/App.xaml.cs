using SmartExpenseApp.Data;
using SmartExpenseApp.Utilities;

namespace SmartExpenseApp
{
    public partial class App : Application
    {
        private static SmartExpenseAppDatabase _database;

        public static SmartExpenseAppDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    string dbPath = Constants.DatabasePath;
                    _database = new SmartExpenseAppDatabase(dbPath);
                }
                return _database;
            }
        }

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Constants.SyncfusionLicenseKey);

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}