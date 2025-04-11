using System.Globalization;

namespace SmartExpenseApp.Converters
{
    public class CategoryToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string category)
            {
                return category switch
                {
                    "Travel" => "icon_transaction_travel.png",
                    "Food" => "icon_transaction_food.png",
                    "Shopping" => "icon_transaction_shopping.png",
                    "Health" => "icon_transaction_health.png",
                    "Education" => "icon_transaction_education.png",
                    "Others" => "icon_transaction_others.png",
                    _ => "icon_transaction_missing.png"
                };
            }
            return "icon_transaction_missing.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
