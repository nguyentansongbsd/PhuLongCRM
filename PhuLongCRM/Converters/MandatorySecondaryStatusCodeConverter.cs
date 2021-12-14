using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.Converters
{
    public class MandatorySecondaryStatusCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                switch ((int)value)
                {
                    case 1: 
                        return "Nháp"; // Active
                    case 100000000: 
                        return "Đang áp dụng"; // Applying
                    case 100000001: 
                        return "Hủy"; //Cancel
                    case 2: 
                        return "Vô hiệu lực"; // Inactive 
                    default:
                        return "";
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
