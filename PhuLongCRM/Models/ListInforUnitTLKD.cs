using PhuLongCRM.Helper;
using PhuLongCRM.Resources;
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
                        return Language.chuan_bi;
                    case 100000000:
                        return Language.san_sang;//"Available";
                    case 100000004:
                        return Language.giu_cho;//"Queuing";
                    case 100000007:
                        return Language.dat_cho;//"Giữ chỗ";
                    case 100000006:
                        return Language.dat_coc;//"Reserve";
                    case 100000005:
                        return Language.dong_y_chuyen_coc;//"Collected";
                    case 100000003:
                        return Language.da_du_tien_coc;//"Deposited";
                    case 100000001:
                        return Language.thanh_toan_dot_1;//"1st Installment";
                    case 100000008:
                        return Language.du_dieu_dien;//"Đủ điều kiện";
                    case 100000009:
                        return Language.da_ky_ttdc_hddc;//"Thỏa thuận đặt cọc";
                    case 100000002:
                        return Language.da_ban;//"Sold";
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

