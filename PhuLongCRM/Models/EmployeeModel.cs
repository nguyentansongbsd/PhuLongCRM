using System;
namespace PhuLongCRM.Models
{
    public class EmployeeModel
    {
        public Guid bsd_employeeid { get; set; }
        public string bsd_name { get; set; }
        public string bsd_avatar { get; set; }
        public string bsd_password { get; set; }
        public string bsd_imeinumber { get; set; }
        public Guid contact_id { get; set; }
        public string contact_name { get; set; }
        public string contact_phone { get; set; }
        public Guid manager_id { get; set; }
        public string manager_name { get; set; }
        public DateTime createdon { get; set; }
        public decimal bsd_numberlogin { get; set; }
        public DateTime bsd_logindate { get; set; }
        public string contact_email { get; set; }
        public int bsd_loginlimit { get; set; }
        public int? bsd_timeoutminute { get; set; }
        public Guid agent_id { get; set; }
        public string agent_name { get; set; }
        public string statuscode { get; set; }
        public int bsd_apprating { get; set; }
    }
}
