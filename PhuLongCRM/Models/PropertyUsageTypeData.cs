using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class PropertyUsageTypeData
    {
        public static OptionSet GetPropertyUsageTypeById(string Id)
        {
            return PropertyUsageTypes().SingleOrDefault(x => x.Val == Id);
        }
        public static List<OptionSet> PropertyUsageTypes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("1","Condo"),
                new OptionSet("2","Chung cư"),
                new OptionSet("3","Nhà phố"),
            };
        }
    }
}
