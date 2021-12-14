using System;
namespace PhuLongCRM.Models
{
    public class DiscountChildOptionSet : OptionSet
    {
        public decimal bsd_amount { get; set; }
        public decimal bsd_percentage { get; set; }
        public string new_type { get; set; }
        public DateTime? bsd_startdate { get; set; }
        public DateTime? bsd_enddate { get; set; }
        public DateTime createdon { get; set; }

        public bool IsEnableChecked { get; set; }

        private string _itemColor = "#444444";
        public string ItemColor { get => _itemColor; set { _itemColor = value; OnPropertyChanged(nameof(ItemColor)); } }
        public bool IsExpired
        {
            get
            {
                if (bsd_startdate.HasValue && bsd_enddate.HasValue)
                {
                    bsd_startdate = bsd_startdate.Value.ToLocalTime();
                    bsd_enddate = bsd_enddate.Value.ToLocalTime();
                    if ( DateTime.Now.Date > bsd_enddate.Value.Date && DateTime.Now.Date > bsd_startdate.Value.Date)
                    {
                        ItemColor = "#ff0000";
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsNotApplied
        {
            get
            {
                if (bsd_startdate.HasValue && bsd_enddate.HasValue)
                {
                    bsd_startdate = bsd_startdate.Value.ToLocalTime();
                    bsd_enddate = bsd_enddate.Value.ToLocalTime();
                    if (DateTime.Now.Date < bsd_startdate.Value.Date && DateTime.Now.Date < bsd_enddate.Value.Date)
                    {
                        ItemColor = "#009a81";
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
