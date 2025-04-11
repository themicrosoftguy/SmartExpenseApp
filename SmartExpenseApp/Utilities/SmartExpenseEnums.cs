namespace SmartExpenseApp.Utilities
{
    public static class SmartExpenseEnums
    {
        public enum TransactionsFilterEnums
        {
            Today,
            Week,
            Month
        }

        public enum TransactionType
        {
            Expense,
            Income,
            Scan
        }

        public enum TransactionCategory
        {
            Travel,
            Food,
            Shopping,
            Health,
            Education,
            Others
        }

        public enum TransactionSource
        {
            SBI,
            BOB,
            ICICI,
            Cash
        }
    }   
}