using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class TypeIdCardData
    {
        public static List<OptionSet> TypeIdCards()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","CMND"),
                new OptionSet("100000001","CCCD"),
                new OptionSet("100000002","Passport"),
            };
        }

        public static OptionSet GetTypeIdCardById(string Id)
        {
            return TypeIdCards().SingleOrDefault(x => x.Val == Id);
        }
    }
}
