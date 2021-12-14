using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class HandoverCoditionMinimumData
    {
        public static OptionSet GetHandoverCoditionMinimum(string Id)
        {
            return HandoverCoditionMinimums().SingleOrDefault(x => x.Val == Id);
        }
        public static List<OptionSet> HandoverCoditionMinimums()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Bare-shell"),
                new OptionSet("100000001","Basic Finished"),
                new OptionSet("100000002","Fully Finished"),
                new OptionSet("100000003","Fully Finished the outside and Bare-shell the inside"),
                new OptionSet("100000004","Add On Option"),
            };
        }
    }
}
