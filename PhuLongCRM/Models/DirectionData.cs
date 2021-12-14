using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhuLongCRM.Models
{
    public class DirectionData
    {
        public static OptionSet GetDiretionById(string diretionId)
        {
            var diretion = Directions().Single(x=>x.Val == diretionId);
            return diretion;
        }

        public static List<OptionSetFilter> Directions()
        {
            var directions = new List<OptionSetFilter>();
            directions.Add(new OptionSetFilter {Val = "100000000",Label= "Đông" });
            directions.Add(new OptionSetFilter { Val = "100000001", Label = "Tây" });
            directions.Add(new OptionSetFilter { Val = "100000002", Label = "Nam" });
            directions.Add(new OptionSetFilter { Val = "100000003", Label = "Bắc" });
            directions.Add(new OptionSetFilter { Val = "100000004", Label = "Tây bắc" });
            directions.Add(new OptionSetFilter { Val = "100000005", Label = "Đông bắc" });
            directions.Add(new OptionSetFilter { Val = "100000006", Label = "Tây nam" });
            directions.Add(new OptionSetFilter { Val = "100000007", Label = "Đông nam" });
            return directions;
        }
    }
}
