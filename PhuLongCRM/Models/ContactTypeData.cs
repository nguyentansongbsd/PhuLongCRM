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
                new OptionSet("100000000","Khách hàng"), // khach hanfg Customer
                new OptionSet("100000001","Cộng tác viên"), // cong tac vien Collaborator
                new OptionSet("100000002","Người ủy quyền"), // người ủy quyền Authorized
                new OptionSet("100000003","Người đại diện pháp lý"), //Legal Representative
            };
        }

        public static OptionSet GetContactTypeById(string Id)
        {
            return ContactTypes().SingleOrDefault(x => x.Val == Id);
        }
    }
}
