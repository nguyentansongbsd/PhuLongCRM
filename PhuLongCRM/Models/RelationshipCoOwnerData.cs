using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class RelationshipCoOwnerData
    {
        public static List<OptionSet> RelationshipData()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Vợ chồng"),
                new OptionSet("100000001","Con"),
                new OptionSet("100000002","Cha mẹ"),
                new OptionSet("100000003","Bạn"),
                new OptionSet("100000004","Khác"),
            };
        }
        public static OptionSet GetRelationshipById(string id)
        {
            return RelationshipData().SingleOrDefault(x => x.Val == id);
        }
    }
}
