using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class CaseBillableTime
    {
        public static List<OptionSet> CaseBillableTimeData()
        {
            return new List<OptionSet>()
            {
                new OptionSet("1", "1 Phút"),
                new OptionSet("15", "15 Phút"),
                new OptionSet("30", "30 Phút"),
                new OptionSet("45", "45 Phút"),
                new OptionSet("60", "1 Giờ"),
                new OptionSet("90", "1.5 Giờ"),
                new OptionSet("120", "2 Giờ"),
                new OptionSet("150", "2.5 Giờ"),
                 new OptionSet("180", "3 Giờ"),
                new OptionSet("210", "3.5 Giờ"),
                 new OptionSet("240", "4 Giờ"),
                new OptionSet("270", "4.5 Giờ"),
                 new OptionSet("300", "5 Giờ"),
                new OptionSet("330", "5.5 Giờ"),
                 new OptionSet("360", "6 Giờ"),
                new OptionSet("390", "6.5 Giờ"),
                 new OptionSet("420", "7 Giờ"),
                new OptionSet("450", "7.5 Giờ"),
                new OptionSet("480", "8 Giờ"),
                new OptionSet("1440", "1 Ngày"),
                new OptionSet("2880", "2 Ngày"),
                new OptionSet("4320", "3 Ngày"),
            };
        }
    }
}
