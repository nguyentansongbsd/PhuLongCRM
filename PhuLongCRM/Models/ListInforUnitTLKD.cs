using PhuLongCRM.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.Models
{
    public class ListInforUnitTLKD 
    {
        public string name { get; set; }
        public string bsd_projectname { get; set; }
        public string description { get; set; }
        public string bsd_unitscodesams { get; set; }
        public int statuscode { get; set; }
        public string statuscodevalue
        {
            get
            {
                switch (statuscode)
                {
                    case 1:
                        return "Preparing";
                    case 100000000:
                        return "Available";
                    case 100000004:
                        return "Queuing";
                    case 100000007:
                        return "Giữ chỗ";
                    case 100000006:
                        return "Reserve";
                    case 100000005:
                        return "Collected";
                    case 100000003:
                        return "Deposited";
                    case 100000001:
                        return "1st Installment";
                    case 100000008:
                        return "Đủ điều kiện";
                    case 100000009:
                        return "Thỏa thuận đặt cọc";
                    case 100000002:
                        return "Sold";
                    default:
                        return "";
                }
            }
        }
        public DateTime createdon { get; set; }
        public string createdonformat
        {
            get => StringHelper.DateFormat(createdon);
        }
    }
}

