using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class QueuesModel : BaseViewModel
    {
        public Guid opportunityid { get; set; }
        public string name { get; set; }
        public Guid customer_id { get; set; }
        public Guid project_id { get; set; }
        public string project_name { get; set; }
        public string contact_name { get; set; }
        public string account_name { get; set; }

        private DateTime _createdon;
        public DateTime createdon { get => _createdon.AddHours(7); set { _createdon = value; OnPropertyChanged(nameof(createdon)); } }

        private DateTime _bsd_queuingexpired;
        public DateTime bsd_queuingexpired { get => _bsd_queuingexpired.AddHours(7); set { _bsd_queuingexpired = value; OnPropertyChanged(nameof(bsd_queuingexpired)); } }       
        public int statuscode { get; set; }
        public string statuscode_format { get { return QueuesStatusCodeData.GetQueuesById(statuscode.ToString()).Name; } }
        public string statuscode_color { get { return QueuesStatusCodeData.GetQueuesById(statuscode.ToString()).BackGroundColor; } }

        public string bsd_queuenumber { get; set; }
        public string customername
        {
            get { return contact_name ?? account_name ?? ""; }
        }
        public string telephone { get; set; }
    }
}
