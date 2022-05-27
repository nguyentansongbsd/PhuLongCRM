using PhuLongCRM.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class DiscountSpecialModel
    {
        public Guid bsd_discountspecialid { get; set; }
        public string bsd_name { get; set; }
        public decimal bsd_percentdiscount { get; set; }
        public string percentdiscount_format { get { return StringFormatHelper.FormatPercent(bsd_percentdiscount) + "%"; } }
        public decimal bsd_totalamount { get; set; }
        public string totalamount_format { get { return StringFormatHelper.FormatCurrency(bsd_totalamount) + " đ"; } }
        public string statuscode { get; set; }
        public string statuscode_format { get { return statuscode != string.Empty ? DiscountSpecialStatus.GetDiscountSpecialStatusById(statuscode)?.Name : null; } }
        public string statuscode_color { get { return statuscode != string.Empty ? DiscountSpecialStatus.GetDiscountSpecialStatusById(statuscode)?.Background : "#f1f1f1"; } }
    }
    public class DiscountSpecialStatus
    {
        public static List<StatusCodeModel> DiscountSpecialStatusData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("1","Active","#06CF79"),
                new StatusCodeModel("100000000","Approved","#03ACF5"),
                new StatusCodeModel("100000001","Reject","#FDC206"),
                new StatusCodeModel("100000002","Canceled","#03ACF5"),
                new StatusCodeModel("2","Inactive","#FDC206"),
                new StatusCodeModel("0","","#f1f1f1")
            };
        }

        public static StatusCodeModel GetDiscountSpecialStatusById(string id)
        {
            return DiscountSpecialStatusData().SingleOrDefault(x => x.Id == id);
        }
    }
}
