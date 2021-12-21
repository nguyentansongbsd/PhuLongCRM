using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class CustomerStatusCodeData
    {
        public static List<StatusCodeModel> CustomerStatusCode()
        {
            return new List<StatusCodeModel>()
            {
                 new StatusCodeModel("100000000","Chính thức","#2FCC71"),
                new StatusCodeModel("1","Tiềm năng","#04A8F4"),
                new StatusCodeModel("0","","#FFFFFF"),
            };
        }

        public static StatusCodeModel GetCustomerStatusCodeById(string Id)
        {
            return CustomerStatusCode().SingleOrDefault(x => x.Id == Id);
        }
    }
}
