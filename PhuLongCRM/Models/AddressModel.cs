using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class AddressModel
    {
        public Guid country_id { get; set; }
        public string country_name { get; set; }
        public Guid province_id { get; set; }
        public string province_name { get; set; }
        public Guid district_id { get; set; }
        public string district_name { get; set; }
        public string lineaddress { get; set; }
        public string address 
        { 
            get 
            {
                List<string> ad = new List<string>();
                ad.Add(lineaddress);
                ad.Add(district_name);
                ad.Add(province_name);
                ad.Add(country_name);
                return string.Join(", ", ad);
            } 
        }
    }
}
