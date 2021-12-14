using System;
using PhuLongCRM.Helper;
using Xamarin.Forms;

namespace PhuLongCRM.Models
{
    public class ListQuotationAcc : ContentView
    {
        public string customerid { get; set; }
        public int statuscode { get; set; }
        public decimal? totalamount { get; set; }
        public string totalamountformat => totalamount.HasValue ? string.Format("{0:#,0.#}", totalamount.Value) + " đ" : null;
        public string bsd_quotationnumber { get; set; }
        public string statuscodevalue
        {
            get
            {
                switch (statuscode)
                {
                    case 1:
                        return "In Progress";
                    case 100000007:
                        return "Quotation";
                    case 100000000:
                        return "Reservation";
                    case 100000006:
                        return "Collected";
                    case 2:
                        return "In Progress";
                    case 3:
                        return "Deposited";
                    case 100000002:
                        return "Pending Cancel Deposit";
                    case 100000004:
                        return "Signed RF";
                    case 100000009:
                        return "Expired";
                    case 4:
                        return "Won";
                    case 100000001:
                        return "Terminated";
                    case 100000003:
                        return "Reject";
                    case 5:
                        return "Lost";
                    case 6:
                        return "Canceled";
                    case 7:
                        return "Revised";
                    case 100000005:
                        return "Expired of signing RF";
                    case 100000008:
                        return "Expired Quotation";
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
        public string quo_nameproject { get; set; }
        public string quo_nameaccount { get; set; }
        public string quo_namecontact { get; set; }
        public string quo_nameproduct { get; set; }
    }
}

