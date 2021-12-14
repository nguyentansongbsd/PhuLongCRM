using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class CustomerGroupData
    {
        public static List<OptionSet> CustomerGroups()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Nhà ở/ Căn hộ"),
                new OptionSet("100000001","Nghỉ dưỡng"),
                new OptionSet("100000002","Chưa xác định"),
            };
        }

        public static OptionSet GetCustomerGroupById(string Id)
        {
            return CustomerGroups().SingleOrDefault(x => x.Val == Id);
        }
    }
}
