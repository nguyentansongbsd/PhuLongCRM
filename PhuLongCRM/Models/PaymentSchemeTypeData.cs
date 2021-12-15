using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class PaymentSchemeTypeData
    {
        public static List<OptionSet> PaymentSchemeTypes()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Giữ nguyên"),
                new OptionSet("100000001","Gộp đầu"),
                new OptionSet("100000002","Gộp cuối"),
            };
        }
        public static OptionSet GetPaymentSchemeTypeById(string Id)
        {
            return PaymentSchemeTypes().SingleOrDefault(x => x.Val == Id);
        }
    }
}
