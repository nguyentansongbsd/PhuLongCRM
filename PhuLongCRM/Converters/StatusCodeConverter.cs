using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.Converters
{
    public class StatusCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "1")
            {
                return "Mới"; // New
            }
            else if ((string)value == "2")
            {
                return "Đã Liên Hệ"; // Contacted
            }
            else if ((string)value == "3")
            {
                return "Đã Xác Nhận";
            }
            else if ((string)value == "4")
            {
                return "Mất Khách Hàng"; // Lost
            }
            else if ((string)value == "5")
            {
                return "Không Liên Hệ Được"; // Cannot Contact
            }
            else if ((string)value == "6")
            {
                return "Không Quan Tâm";  //No Longer Interested
            }
            else if ((string)value == "7")
            {
                return "Đã hủy"; // Canceled
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
