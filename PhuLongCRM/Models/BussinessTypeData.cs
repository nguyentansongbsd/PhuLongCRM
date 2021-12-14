using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class BussinessTypeData
    {
        public static List<OptionSet> BussinessTypes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Khách hàng"),
                new OptionSet("100000001","Đối tác"),
                new OptionSet("100000002","Đại lý"),
                new OptionSet("100000003","Chủ đầu tư"),
            };
        }
        public static OptionSet GetBussinessTypeById(string Id)
        {
            return BussinessTypes().SingleOrDefault(x => x.Val == Id);
        }
    }
}
