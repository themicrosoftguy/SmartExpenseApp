using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartExpenseApp.ViewModels
{
    public class TransactionViewModel
    {
        public ObservableCollection<string> TimeFilter { get; set; }

        public TransactionViewModel()
        {
            TimeFilter = new ObservableCollection<string>
            {
                "Today",
                "Week",
                "Month",
                "Year",
            };
        }
    }
}
