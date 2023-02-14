using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class NewsModel : BaseViewModel
    {
        public Guid bsd_newsid { get; set; }
        public string bsd_name { get; set; }
        public string bsd_image { get; set; }
        public string bsd_url { get; set; }
        public string bsd_thumnail { get; set; }
        public Guid promotion_id { get; set; }
        public string promotion_name { get; set; }
        public DateTime promotion_startdate { get; set; }
        public DateTime promotion_enddate { get; set; }
        public string promotion_project_name { get; set; }
        public string bsd_type { get; set; }
        public Guid project_id { get; set; }
        public string project_name { get; set; } // tên dự án
        public int bsd_priority { get; set; }
    }
}
