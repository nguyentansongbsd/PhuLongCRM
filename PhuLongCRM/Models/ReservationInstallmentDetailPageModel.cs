using PhuLongCRM.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ReservationInstallmentDetailPageModel
    {
        public Guid bsd_paymentschemedetailid { get; set; }
        public string bsd_name { get; set; }
        public DateTime bsd_duedate { get; set; } // ngày đến hạn
        public int statuscode { get; set; } // tình trạng.
        public string statuscode_format { get => InstallmentsStatusCodeData.GetInstallmentsStatusCodeById(statuscode.ToString()).Name; }
        public string statuscode_color { get => InstallmentsStatusCodeData.GetInstallmentsStatusCodeById(statuscode.ToString()).Background; }
        public decimal bsd_amountofthisphase { get; set; } // số tiền đợi thnah toán.
        public decimal bsd_amountwaspaid { get; set; } // số tiền đã thanh toán
        public decimal bsd_depositamount { get; set; } // số tiền đặt cọc
        public string bsd_depositamount_format
        {
            get
            {
                if (bsd_depositamount == 0)
                    return null;
                else
                    return bsd_depositamount.ToString();
            }
        }

        public int bsd_ordernumber { get; set; } // đợt
        public decimal bsd_amountpercent { get; set; }  // phần trăm thah toán

        public string bsd_name_format
        {
            get
            {
                string name = "";
                //if (!string.IsNullOrWhiteSpace(bsd_name))
                //    name += bsd_name;
                if (bsd_ordernumber != 0)
                    name += $" {Language.dot_thanh_toan} {bsd_ordernumber}";
                if (bsd_amountpercent != 0)
                    name += string.Format(" - {0:#,0}%", bsd_amountpercent);
                return name;
            }
        }
        public decimal bsd_maintenanceamount { get; set; } // phí bảo trì
        public string bsd_maintenanceamount_format
        {
            get
            {
                if (bsd_maintenanceamount == 0)
                    return null;
                else
                    return bsd_maintenanceamount.ToString();
            }
        }
        public decimal bsd_managementamount { get;set;} // phí quản lý
        public string bsd_managementamount_format
        {
            get
            {
                if (bsd_managementamount == 0)
                    return null;
                else
                    return bsd_managementamount.ToString();
            }
        }
    }
}
