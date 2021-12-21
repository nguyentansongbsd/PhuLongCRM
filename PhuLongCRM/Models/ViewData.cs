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

        public static string GetViewByIds(string ids)
        {
            List<string> list = new List<string>();
            string[] Ids = ids.Split(',');
            foreach (var item in Ids)
            {
                var i = GetViewById(item);
                if (i != null)
                    list.Add(i.Label);
            }
            return string.Join(", ", list);
        }

        public static List<OptionSetFilter> Views()
        {
            return new List<OptionSetFilter>() {
                new OptionSetFilter(){ Val="100000000",Label="Thành phố"},
                new OptionSetFilter(){Val="100000001", Label="Bể bơi" },  // hồ bơi
                new OptionSetFilter(){ Val="100000002",Label="Công viên"},
                new OptionSetFilter(){ Val="100000003",Label="Mặt tiền"},
                new OptionSetFilter(){ Val="100000004",Label="Sân vườn"},
                new OptionSetFilter(){ Val="100000006",Label="Xa lộ"},
                new OptionSetFilter(){ Val="100000007",Label="Hồ"}, //lake
                new OptionSetFilter(){ Val="100000008",Label="Sông"},
                new OptionSetFilter(){ Val="100000009",Label="Biển"},
                new OptionSetFilter(){ Val="100000010",Label="01 mặt thoáng"}, 
                new OptionSetFilter(){ Val="100000011",Label="02 mặt thoáng"},
                new OptionSetFilter(){ Val="100000012",Label="Hồ bơi"}, // pool

            };
        }
    }
}
