using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class FollowUpGroup
    {
        public static List<StatusCodeModel> FollowUpGroupData()
        {
            return new List<StatusCodeModel>()
            {   
                new StatusCodeModel("100000001","CCR","#FDC206"),
                new StatusCodeModel("100000002","FIN","#06CF79"),
                new StatusCodeModel("100000000","S&M","#06CF79"),
                new StatusCodeModel("0","","#333333"),
            };
        }

        public static StatusCodeModel GetFollowUpGroupById(string id)
        {
            return FollowUpGroupData().SingleOrDefault(x => x.Id == id);
        }
    }
}
