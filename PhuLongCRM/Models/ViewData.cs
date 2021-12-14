using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class ViewData
    {
        public static OptionSetFilter GetViewById(string viewId)
        {
            var view = Views().SingleOrDefault(x => x.Val == viewId);
            return view;
        }

        public static List<OptionSetFilter> Views()
        {
            return new List<OptionSetFilter>() {
                new OptionSetFilter(){ Val="100000000",Label="Thành phố"},
                new OptionSetFilter(){Val="100000001", Label="Hồ bơi" },
                new OptionSetFilter(){ Val="100000002",Label="Công viên"},
                new OptionSetFilter(){ Val="100000003",Label="Mặt tiền"},
                new OptionSetFilter(){ Val="100000004",Label="Garden"},
                new OptionSetFilter(){ Val="100000006",Label="High Way"},
                new OptionSetFilter(){ Val="100000007",Label="Lake"},
                new OptionSetFilter(){ Val="100000008",Label="River"},
                new OptionSetFilter(){ Val="100000009",Label="Sea"},
                new OptionSetFilter(){ Val="100000010",Label="01 Side Open"},
                new OptionSetFilter(){ Val="100000011",Label="02 Side Open"},
                new OptionSetFilter(){ Val="100000012",Label="Pool"},

            };
        }
    }
}
