using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class CaseResolutionType
    {
        public static List<OptionSet> CaseResolutionTypeData()
        {
            return new List<OptionSet>()
            {
                new OptionSet("5", "Problem Solved"),
                new OptionSet("1000", "Information Provided"),
            };
        }
    }
}
