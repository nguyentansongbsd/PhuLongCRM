using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class ContractTypeData
    {
        public static List<OptionSet> ContractTypes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Cho thuê dài hạn"),
                new OptionSet("100000001","Hợp đồng mua bán trong nước"),
                new OptionSet("100000002","HĐMB nước ngoài"),
            };
        }
        public static OptionSet GetContractTypeById(string id)
        {
            return ContractTypes().SingleOrDefault(x => x.Val == id);
        }
    }
}
