using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartExpenseApp.Models;
//using SmartExpenseApp.Views;

namespace SmartExpenseApp.ViewModels
{
    public class OnboardingViewModel
    {
        public OnboardingViewModel()
        {
            ImageCollection.Add(new Onboarding("carousel_1.png"));
            ImageCollection.Add(new Onboarding("carousel_1.png"));
            ImageCollection.Add(new Onboarding("carousel_1.png"));

        }
        private List<Onboarding> imageCollection = new List<Onboarding>();
        public List<Onboarding> ImageCollection
        {
            get { return imageCollection; }
            set { imageCollection = value; }
        }
    }
}
