using PhuLongCRM.Resources;
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
                new StatusCodeModel("100000002", Language.ful_change_contract_type_type,"#FDC206"), //ful_change_contract_type_type
                new StatusCodeModel("100000001",Language.ful_forfeiture_refund_type,"#06CF79"),//ful_forfeiture_refund_type
                new StatusCodeModel("100000000",Language.ful_key_in_error_type,"#03ACF5"),//ful_key_in_error_type
            };
        }

        public static StatusCodeModel GetFollowUpTerminationTypeById(string id)
        {
            return FollowUpTerminationTypeData().SingleOrDefault(x => x.Id == id);
        }
    }
}
