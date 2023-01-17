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
    }
}
