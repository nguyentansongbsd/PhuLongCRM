using PhuLongCRM.ViewModels;
using System;
namespace PhuLongCRM.Models
{
    public class ActivitiModel : BaseViewModel
    {
        public Guid activityid { get; set; }
        public string subject { get; set; }
        public string activitytypecode { get; set; }

        public string _customer;
        public string customer { get => _customer; set { _customer = value; OnPropertyChanged(nameof(customer)); } }
        public Guid contact_id { get; set; }
        public string contact_name { get; set; }

        public Guid account_id { get; set; }
        public string account_name { get; set; }

        public Guid lead_id { get; set; }
        public string lead_name { get; set; }

        public DateTime scheduledstart { get; set; }
        public DateTime scheduledend { get; set; }
        public DateTime createdon { get; set; }
        public string callto_contact_name { get; set; }
        public string callto_accounts_name { get; set; }
        public string callto_lead_name { get; set; }
    }
}
