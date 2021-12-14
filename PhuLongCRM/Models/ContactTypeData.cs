using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class ContactTypeData
    {
        public static List<OptionSet> ContactTypes() {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Customer"),
                new OptionSet("100000001","Collaborator"),
                new OptionSet("100000002","Authorized"),
                new OptionSet("100000003","Legal Representative"),
            };
        }

        public static OptionSet GetContactTypeById(string Id)
        {
            return ContactTypes().SingleOrDefault(x => x.Val == Id);
        }
    }
}
