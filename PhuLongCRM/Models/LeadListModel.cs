using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class LeadListModel
    {
        public Guid leadid { get; set; }
        public string lastname { get; set; }
        public string mobilephone { get; set; }
        public string subject { get; set; }
        public string statuscode { get; set; }
        public string telepphone1 { get; set; }
        public string emailaddress1 { get; set; }
        public string bsd_contactaddress { get; set; }
        public int leadqualitycode { get; set; }
        public DateTime createdon { get; set; }
        public string createdon_format
        {
            get
            {
                return this.createdon.ToString("dd/MM/yyyy");
            }
        }
    }
}
