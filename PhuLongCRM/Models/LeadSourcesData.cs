using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class LeadSourcesData
    {
        public static List<OptionSet> GetListSources()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Newspapers"),
                new OptionSet("100000001","Staff Phu Long"),
                new OptionSet("100000002","Trademark"),
                new OptionSet("100000003","Hotline"),
                new OptionSet("100000004","The current of the position"),
                new OptionSet("100000005","Old customers have bought referrals"),
                new OptionSet("1","Quảng cáo"),
                new OptionSet("2","Nhân viên giới thiệu"),
                new OptionSet("3","Giới thiệu bên ngoài"),
                new OptionSet("4","Đối tác"),
                new OptionSet("5","Quan hệ công chúng"),
                new OptionSet("6","Hội thảo"),
                new OptionSet("7","Triển lãm thương mại"),
                new OptionSet("8","Trang Web"),
                new OptionSet("9","Truyền miệng"),
                new OptionSet("10","Khác"),
            };
        }

        public static OptionSet GetLeadSourceById(string Id)
        {
            return GetListSources().SingleOrDefault(x => x.Val == Id);
        }
    }
}
