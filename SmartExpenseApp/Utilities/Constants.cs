namespace SmartExpenseApp.Utilities
{
    public static class Constants
    {
        public const string DatabaseFilename = "SmartExpenseAppLocalDb.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public const string SyncfusionLicenseKey = "Ngo9BigBOggjHTQxAR8/V1NMaF5cXmBCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXxccXRcR2FdUUVyWEo=";

        public const int SMSMessagesMaxFetchCount = 300;

        public static readonly Dictionary<string, string> CategoryMappings = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            // Travel
            { "Uber", "Travel" },
            { "Ola", "Travel" },
            { "Cab", "Travel" },
            { "Taxi", "Travel" },
            { "Auto", "Travel" },
            { "Bus", "Travel" },
            { "Train", "Travel" },
            { "IRCTC", "Travel" },
            { "Flight", "Travel" },
            { "Airlines", "Travel" },
            { "Booking.com", "Travel" },
            { "MakeMyTrip", "Travel" },
            { "Yatra", "Travel" },
            { "Goibibo", "Travel" },
            { "Fuel", "Travel" },
            { "Petrol", "Travel" },
            { "Diesel", "Travel" },
            { "Railways", "Travel" },

            // Food
            { "Zomato", "Food" },
            { "Swiggy", "Food" },
            { "Dominos", "Food" },
            { "McDonalds", "Food" },
            { "KFC", "Food" },
            { "Cafe", "Food" },
            { "Restaurant", "Food" },
            { "Dining", "Food" },
            { "Foodpanda", "Food" },
            { "Pizza", "Food" },
            { "Meal", "Food" },
            { "Sweets", "Food" },
            { "Vadapav", "Food" },
            { "Bakery", "Food" },
            { "Bakers", "Food" },

            // Shopping
            { "Amazon", "Shopping" },
            { "Flipkart", "Shopping" },
            { "Myntra", "Shopping" },
            { "Ajio", "Shopping" },
            { "Snapdeal", "Shopping" },
            { "BigBasket", "Shopping" },
            { "Dmart", "Shopping" },
            { "Groceries", "Shopping" },
            { "Shopping", "Shopping" },
            { "Supermarket", "Shopping" },
            { "Market", "Shopping" },
            { "Clothing", "Shopping" },
            { "Reliance", "Shopping" },
            { "Max", "Shopping" },
            { "Zudio", "Shopping" },

            // Health
            { "Pharmacy", "Health" },
            { "Medicine", "Health" },
            { "Hospital", "Health" },
            { "Clinic", "Health" },
            { "Medical", "Health" },
            { "Doctor", "Health" },
            { "Apollo", "Health" },
            { "Lab Test", "Health" },
            { "Health", "Health" },
            { "Med", "Health" },

            // Education
            { "School", "Education" },
            { "College", "Education" },
            { "Tuition", "Education" },
            { "Exam", "Education" },
            { "Fee", "Education" },
            { "Course", "Education" },
            { "Online Class", "Education" },
            { "BYJU'S", "Education" },
            { "Udemy", "Education" },
            { "Coursera", "Education" },

            // Others (default or general mappings)
            { "Gift", "Others" },
            { "Donation", "Others" },
            { "Recharge", "Others" },
            { "Subscription", "Others" },
            { "Netflix", "Others" },
            { "Spotify", "Others" },
            { "YouTube", "Others" },
            { "Insurance", "Others" },
            { "Loan", "Others" },
            { "Tax", "Others" },
            { "EMI", "Others" },
            { "Wallet", "Others" },
            { "Transfer", "Others" },
            { "Credit", "Others" },
            { "Debit", "Others" },
        };
    }
}