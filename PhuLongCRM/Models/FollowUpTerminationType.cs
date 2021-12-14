using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class FollowUpTerminationType
    {
        public static List<StatusCodeModel> FollowUpTerminationTypeData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000002","Change Contract Type","#FDC206"),
                new StatusCodeModel("100000001","Forfeiture Refund","#06CF79"),
                new StatusCodeModel("100000000","Key-in Error","#03ACF5"),
                new StatusCodeModel("0","","#333333"),
            };
        }

        public static StatusCodeModel GetFollowUpTerminationTypeById(string id)
        {
            return FollowUpTerminationTypeData().SingleOrDefault(x => x.Id == id);
        }
    }
}
