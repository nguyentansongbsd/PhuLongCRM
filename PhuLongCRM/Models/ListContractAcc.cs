using System;
using PhuLongCRM.Helper;
using Xamarin.Forms;

namespace PhuLongCRM.Models
{
    public class ListContractAcc : ContentView
    {
        public string customerid { get; set; }
        public string bsd_optionno { get; set; }
        //public decimal? bsd_optionno { get; set; }
        //public string bsd_optionnovalueformat => bsd_optionno.HasValue ? string.Format("{0:#,0.#}", bsd_optionno.Value) + " đ" : null;

        public decimal? totalamount { get; set; }
        public string totalamountformat => totalamount.HasValue ? string.Format("{0:#,0.#}", totalamount.Value) + " đ" : null;
        public int statuscode { get; set; }
        public string statuscodevalue
        {
            get
            {
                switch (statuscode)
                {
                    case 1:
                        return "Open";
                    case 2:
                        return "Pending";
                    case 100000000:
                        return "Option";
                    case 100000001:
                        return "1st Installment";
                    case 100000002:
                        return "Signed Contract";
                    case 100000003:
                        return "Being Payment";
                    case 100000004:
                        return "Complete Payment";
                    case 100000005:
                        return "Handover";
                    case 100000006:
                        return "Terminated";
                    default:
                        return "";
                }
            }
        }
        public DateTime bsd_signingexpired { get; set; }
        public string bsd_signingexpiredformat
        {
            get => StringHelper.DateFormat(bsd_signingexpired);
        }

        public DateTime createdon { get; set; }
        public string createdonformat
        {
            get => StringHelper.DateFormat(createdon);
        }
        public string contract_nameproject { get; set; }
        public string contract_nameaccount { get; set; }
        public string contract_namecontact { get; set; }
        public string contract_nameproduct { get; set; }
    }
}

