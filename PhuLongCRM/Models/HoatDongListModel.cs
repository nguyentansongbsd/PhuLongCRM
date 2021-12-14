using System;
using System.Collections.Generic;
using System.Text;
using PhuLongCRM.Helper;
using PhuLongCRM.ViewModels;

namespace PhuLongCRM.Models
{
    public class HoatDongListModel : BaseViewModel
    {
        public Guid activityid { get; set; }
        public string subject { get; set; }
        public string accounts_bsd_name { get; set; }
        public string contact_bsd_fullname { get; set; }
        public string lead_fullname { get; set; }
        public string systemsetup_bsd_name { get; set; }
        public string regarding_name
        {
            get
            {
                if (activitytypecode == "appointment")
                {
                    return null;
                }

                    if (activitytypecode == "phonecall")
                {
                    if (this.callto_contact_name != null)
                    {
                        return this.callto_contact_name;
                    }
                    else if (this.callto_account_name != null)
                    {
                        return this.callto_account_name;
                    }
                    else if (this.callto_lead_name != null)
                    {
                        return this.callto_lead_name;
                    }
                    else
                    {
                        return " ";
                    }
                }
                else
                {
                    if (this.accounts_bsd_name != null)
                    {
                        return this.accounts_bsd_name;
                    }
                    else if (this.contact_bsd_fullname != null)
                    {
                        return this.contact_bsd_fullname;
                    }
                    else if (this.lead_fullname != null)
                    {
                        return this.lead_fullname;
                    }
                    else if (this.systemsetup_bsd_name != null)
                    {
                        return this.systemsetup_bsd_name;
                    }
                    else
                    {
                        return " ";
                    }
                }
            }
        }
        public string activitytypecode { get; set; }
        public string activitytypecode_format
        {
            get
            {
                switch (activitytypecode)
                {
                    case "task":
                        return "Công Việc";
                    case "phonecall":
                        return "Cuộc Gọi";
                    case "appointment":
                        return "Cuộc Họp";
                    default:
                        return " ";
                }
            }
        }
        public int statecode { get; set; }
        public string statecode_format
        {
            get
            {
                switch (this.statecode)
                {
                    case 0:
                        return "Đang Mở";
                    case 1:
                        return "Hoàn Thành";
                    case 2:
                        return "Đã hủy";
                    case 3:
                        return "Scheduled";
                    default:
                        return " ";
                }
            }
        }
        public string owners_fullname { get; set; }
        public int prioritycode { get; set; }
        public string prioritycode_format
        {
            get
            {
                switch (prioritycode)
                {
                    case 0:
                        return "Low";
                    case 1:
                        return "Normal";
                    case 2:
                        return "High";
                    default:
                        return " ";
                }
            }
        }
        public DateTime scheduledstart { get; set; }
        public string scheduledstart_format
        {
            get => StringHelper.DateFormat(this.scheduledstart.ToLocalTime());
        }
        public DateTime scheduledend { get; set; }
        public string scheduledend_format
        {
            get => StringHelper.DateFormat(this.scheduledend.ToLocalTime());
        }
        public DateTime createdon { get; set; }
        public string createdon_format
        {
            get => StringHelper.DateFormat(this.createdon);
        }

        public string statecode_color
        {
            get
            {
                switch (this.statecode)
                {
                    case 0:
                        return "#06CF79"; // open
                    case 1:
                        return "#03ACF5"; //com
                    case 2:
                        return "#333333"; //can
                    case 3:
                        return "#04A388"; //sha
                    default:
                        return "#333333";
                }
            }
        }

        // sử dụng cho phonecall

        public string callto_contact_name { get; set; }
        public string callto_account_name { get; set; }
        public string callto_lead_name { get; set; }

        public string _customer;
        public string customer { get => _customer; set { _customer = value; OnPropertyChanged(nameof(customer)); } }
    }
}
