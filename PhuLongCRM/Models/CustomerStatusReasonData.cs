using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class CustomerStatusReasonData
    {
        public static List<OptionSet> CustomerStatusReasons()
        {
            return new List<OptionSet>()
            {
                new OptionSet("1","Tiềm năng"),
                new OptionSet("100000000","Chính thức"),
            };
        }

        public static OptionSet GetCustomerStatusReasonById(string Id)
        {
            return CustomerStatusReasons().SingleOrDefault(x => x.Val == Id);
        }
    }
}
