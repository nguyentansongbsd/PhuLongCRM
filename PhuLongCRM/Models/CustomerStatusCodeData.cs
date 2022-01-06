using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Models
{
    public class CustomerStatusCodeData
    {
        public static List<StatusCodeModel> CustomerStatusCode()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000000",Language.chinh_thuc,"#2FCC71"),
                new StatusCodeModel("1",Language.tiem_nang,"#04A8F4"),
                new StatusCodeModel("0","","#FFFFFF"),
            };
        }

        public static StatusCodeModel GetCustomerStatusCodeById(string Id)
        {
            return CustomerStatusCode().SingleOrDefault(x => x.Id == Id);
        }
    }
}
