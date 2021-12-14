using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class CaseStatusCodeData
    {
        public static List<OptionSet> CaseStatusData()
        {
            return new List<OptionSet>()
            {
                new OptionSet("1", "Đang xử lý"),
                new OptionSet("2", "Đang chờ"),
                new OptionSet("3", "Đang chờ thông tin chi tiết"),
                new OptionSet("4", "Nghiên cứu"),
                new OptionSet("5", "Vấn đề đã được giải quyết"),
                new OptionSet("1000", "Cung cấp thông tin"),
                new OptionSet("6", "Đã hủy"),
                new OptionSet("2000", "Hợp nhất"),
                 new OptionSet("0", "")

            };
        }

        public static OptionSet GetCaseStatusCodeById(string id)
        {
            return CaseStatusData().SingleOrDefault(x => x.Val == id);
        }
    }
}
