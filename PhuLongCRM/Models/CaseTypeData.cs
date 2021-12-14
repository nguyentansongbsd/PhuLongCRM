using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class CaseTypeData
    {
        public static List<OptionSet> CasesData()
        {
            return new List<OptionSet>()
            {
                new OptionSet("1","Câu hỏi"),
                new OptionSet("2","Vấn đề"),
                new OptionSet("3","Yêu cầu"),
                 new OptionSet("0",""),
            };
        }

        public static OptionSet GetCaseById(string id)
        {
            return CasesData().SingleOrDefault(x => x.Val == id);
        }
    }
}