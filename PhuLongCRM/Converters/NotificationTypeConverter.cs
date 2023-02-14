using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.Converters
{
    public class NotificationTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            NotificationType type = (NotificationType)value;

            if (type == NotificationType.Project)
            {
                return "ConstructionProgess.png";
            }
            else if (type == NotificationType.PhaseLaunch)
            {
                return "newProject.png";
            }
            else if (type == NotificationType.QueueCancel)
            {
                return "newProject.png";
            }
            else if (type == NotificationType.QueueSuccess)
            {
                return "newProject.png";
            }
            else if (type == NotificationType.QueueRefunded)
            {
                return "newProject.png";
            }
            else if (type == NotificationType.MatchUnit)
            {
                return "paymentSuccess.png";
            }
            else //QueueSuccess
            {
                return "newProject.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
