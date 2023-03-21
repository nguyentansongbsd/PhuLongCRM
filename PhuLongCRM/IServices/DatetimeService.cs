using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.IServices
{
    public class DatetimeService : IDatetimeService
    {
        public string TimeAgo(DateTime date)
        {
            var currentDate = DateTime.Now;
            TimeSpan timeSince = currentDate.Subtract(date);
            if (timeSince.TotalSeconds < 60)
                return "Vài giây trước";// "Just now";
            if (timeSince.TotalSeconds < 120)
                return "1 phút trước";// "1 minute ago";
            if (timeSince.TotalMinutes < 60)
                return string.Format("{0} phút trước", timeSince.Minutes);
            if (timeSince.TotalMinutes < 120)
                return "1 giờ trước";
            if (timeSince.TotalHours < 12)
                return string.Format("{0} giờ trước", timeSince.Hours);
            if (timeSince.TotalHours < 24)
                return "Hôm qua lúc " + date.ToString("HH:mm");
            if (timeSince.TotalDays < 7)
                return string.Format("Cách đây {0} ngày", timeSince.Days);
            if (timeSince.TotalDays < 14)
                return "Tuần trước";
            if (timeSince.TotalDays < 21)
                return "Cách đây 2 tuần";
            if (timeSince.TotalDays < 28)
                return "Cách đây 3 tuần";
            if (timeSince.TotalDays < 60)
                return "Tháng trước";
            if (timeSince.TotalDays < 365)
                return string.Format("Cách đây {0} tháng", Math.Round(timeSince.TotalDays / 30));
            if (timeSince.TotalDays < 730)
                return "Năm trước";
            return string.Format("Cách đây {0} năm", Math.Round(timeSince.TotalDays / 365));
        }
    }
}
