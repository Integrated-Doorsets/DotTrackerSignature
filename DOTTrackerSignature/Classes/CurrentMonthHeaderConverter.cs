using System;
using System.Globalization;
using System.Windows.Data;


namespace IdslTracker
{
    [ValueConversion(typeof(bool), typeof(bool))]
    class CurrentMonthHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().EndsWith(" Accruals Tracker"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
