using System;
namespace PhuLongCRM.Models
{
    public class HandoverConditionModel :OptionSet
    {
        public Guid _bsd_unittype_value { get; set; }
        public bool bsd_byunittype { get; set; }
        public string bsd_method { get; set; }
        public decimal bsd_priceperm2 { get; set; }
        public decimal bsd_amount { get; set; }
        public decimal bsd_percent { get; set; }
    }
}
