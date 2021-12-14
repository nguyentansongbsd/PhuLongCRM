using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class PartyModel : BaseViewModel
    {
        public Guid partyID { get; set; }
        public int typemask { get; set; }
        public Guid contact_id { get; set; }
        public Guid account_id { get; set; }
        public Guid lead_id { get; set; }
        public string contact_name { get; set; }
        public string account_name { get; set; }
        public string lead_name { get; set; }
        public string user_name { get; set; }

        private CustomerLookUp _customer;
        public CustomerLookUp Customer
        {
            get => _customer;
            set
            {
                if (_customer != value)
                {
                    _customer = value;
                    OnPropertyChanged(nameof(Customer));
                }
            }
        }
    }
}
