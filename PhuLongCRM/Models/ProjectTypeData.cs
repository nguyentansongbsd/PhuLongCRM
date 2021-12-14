using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class ProjectTypeData
    {
        public static OptionSet GetProjectType(string projectType)
        {
            return ProjectTypes().SingleOrDefault(x => x.Val == projectType);
        }
        public static List<OptionSet> ProjectTypes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("false","Nhà ở"),
                new OptionSet("true","Thương mại"),
            };
        }
    }
}
