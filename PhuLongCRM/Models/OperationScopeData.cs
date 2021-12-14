using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class OperationScopeData
    {
        public static List<OptionSet> OperationScopes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Real Estate"),
                new OptionSet("100000001","Finance"),
                new OptionSet("100000002","Education"),
            };
        }

        public static OptionSet GetOperationScopeById(string Id)
        {
            return OperationScopes().SingleOrDefault(x => x.Val == Id);
        }
    }
}
