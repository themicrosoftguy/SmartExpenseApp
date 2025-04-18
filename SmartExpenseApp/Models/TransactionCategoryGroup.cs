using MvvmHelpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartExpenseApp.Models
{
    public class TransactionCategoryGroup : ObservableRangeCollection<Transaction>, INotifyPropertyChanged
    {
        public string Category { get; private set; }

        private string groupIcon = "icon_down";
        public string GroupIcon
        {
            get => groupIcon;
            set => SetProperty(ref groupIcon, value);
        }

        public double TotalAmount => this.Sum(transaction => double.TryParse(transaction.Amount, out var amount) ? amount : 0);

        public TransactionCategoryGroup(string category, ObservableCollection<Transaction> transactions) : base(transactions)
        {
            Category = category;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
