using PhuLongCRM.Helper;
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
        public Guid promotion_project_id { get; set; }
        public string bsd_type { get; set; }
        public Guid project_id { get; set; }
        public string project_name { get; set; } // tên dự án
        public int bsd_priority { get; set; }

        private string _image;
        public string image { get { return _image; } set { _image = value; OnPropertyChanged(nameof(image)); } }

        private decimal _promotion_values;
        public decimal promotion_values { get => _promotion_values; set { _promotion_values = value; OnPropertyChanged(nameof(promotion_values)); } }
        public string bsd_values_format { get => StringFormatHelper.FormatCurrency(promotion_values); }

        private string _promotion_description;
        public string promotion_description { get => _promotion_description; set { _promotion_description = value; OnPropertyChanged(nameof(promotion_description)); } }
        public string promotion_phaseslaunch_name { get; set; }
    }
}
