using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartExpenseApp.Models
{
    public class Onboarding
    {
        public Onboarding(string imageString)
        {
            ImageName = imageString;
        }
        private string _imageName;

        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }
    }
}
