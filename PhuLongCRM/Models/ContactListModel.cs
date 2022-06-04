using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ContactListModel
    {
        public string bsd_fullname { get; set; }
        public string mobilephone { get; set; }
        public DateTime? birthdate { get; set; }
        public string birthdate_format
        {
            get
            {
                if (birthdate.HasValue)
                    return this.birthdate.Value.ToString("dd/MM/yyyy");
                return "";
            }
        }

        public string emailaddress1 { get; set; }
        public string bsd_diachithuongtru { get; set; }
        public string bsd_contactaddress { get; set; }
        public Guid contactid { get; set; }

        public DateTime? createdon { get; set; }
        public string createdon_format
        {
            get
            {
                if (createdon.HasValue)
                    return this.createdon.Value.ToString("dd/MM/yyyy");
                return "";
            }
        }
        public string statuscode { get; set; }
        public string statuscode_format { get { return statuscode != null ? CustomerStatusCodeData.GetCustomerStatusCodeById(statuscode.ToString())?.Name : null; } }
        public string statuscode_color { get { return statuscode != null ? CustomerStatusCodeData.GetCustomerStatusCodeById(statuscode.ToString())?.Background : "#808080"; } }
        public string bsd_customercode { get; set; } // mã khách hàng
    }
}
