using System;
using System.Globalization;
using System.Windows.Data;

namespace BankASystem.Services
{
    public class PassportValueCodingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return ViewModelBase.isManager ? value : "**** ******";
            }
            return value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
