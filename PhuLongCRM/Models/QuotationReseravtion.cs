using System;
using PhuLongCRM.ViewModels;

namespace PhuLongCRM.Models
{
    public class QuotationReseravtion : BaseViewModel
    {
        private string _quoteid;
        public string quoteid { get => _quoteid; set { _quoteid = value; OnPropertyChanged(nameof(quoteid)); } }

        private string __bsd_projectid_value;
        public string _bsd_projectid_value { get => __bsd_projectid_value; set { __bsd_projectid_value = value; OnPropertyChanged(nameof(_bsd_projectid_value)); } }

        private string _bsd_projectid_label;
        public string bsd_projectid_label { get => _bsd_projectid_label; set { _bsd_projectid_label = value; OnPropertyChanged(nameof(bsd_projectid_label)); } }

        private string __bsd_unitno_value;
        public string _bsd_unitno_value { get => __bsd_unitno_value; set { __bsd_unitno_value = value; OnPropertyChanged(nameof(_bsd_unitno_value)); } }

        private string _bsd_unitno_label;
        public string bsd_unitno_label { get => _bsd_unitno_label; set { _bsd_unitno_label = value; OnPropertyChanged(nameof(bsd_unitno_label)); } }

        private string _bsd_reservationno;
        public string bsd_reservationno { get => _bsd_reservationno; set { _bsd_reservationno = value; OnPropertyChanged(nameof(bsd_reservationno)); } }

        private string __customerid_value;
        public string _customerid_value { get => __customerid_value; set { __customerid_value = value; OnPropertyChanged(nameof(_customerid_value)); } }

        private string _customerid_lable_account;
        public string customerid_label_account { get => _customerid_lable_account; set { _customerid_lable_account = value; if (value != null) { customerid_label_contact = null; customerid_label = value; }; OnPropertyChanged(nameof(customerid_label_account)); } }

        private string _customerid_label_contact;
        public string customerid_label_contact { get => _customerid_label_contact; set { _customerid_label_contact = value; if (value != null) { customerid_label_account = null; customerid_label = value; }; OnPropertyChanged(nameof(customerid_label_contact)); } }

        private string _customerid_label;
        public string customerid_label { get => _customerid_label; set { _customerid_label = value; OnPropertyChanged(nameof(customerid_label)); } }

        private string _statuscode;
        public string statuscode { get => _statuscode; set { _statuscode = value; OnPropertyChanged(nameof(statuscode)); } }
        public string statuscode_label { get
            {
                switch (statuscode)
                {
                    case "1":return "In Progress - Draf";
                    case "2": return "In Progress - Active";
                    case "3": return "Deposited";
                    case "4": return "Won";
                    case "5": return "Lost";
                    case "6": return "Canceled";
                    case "7": return "Revised";
                    case "100000007":return "Quotation";
                    case "100000000":return "Reservation";
                    case "100000006":return "Collected";
                    case "100000010": return "Đã ký phiếu cọc";
                    case "100000011": return "Draft";
                    case "100000002": return "Pending Cancel Deposit";
                    case "100000004": return "Signed RF";
                    case "100000009": return "Expired";
                    case "100000001": return "Terminated";
                    case "100000003": return "Reject";
                    case "100000005": return "Expired of signing RF";
                    case "100000008": return "Expired Quotation";
                    default: return null;
                }
            } }

        private string _transaction_currency;
        public string transaction_currency { get => _transaction_currency; set { _transaction_currency = value; OnPropertyChanged(nameof(transaction_currency)); } }

        private decimal? _totalamount;
        public decimal? totalamount { get => _totalamount; set { _totalamount = value; OnPropertyChanged(nameof(totalamount)); } }

        public string totalamount_format { get { return totalamount.HasValue ? string.Format("{0:#,0.#}", totalamount.Value) + transaction_currency : null; } }


        private DateTime? _createdon;
        public DateTime? createdon { get => _createdon; set { _createdon = value; OnPropertyChanged(nameof(createdon)); } }
        public string createdon_format { get => createdon.HasValue ? createdon.Value.ToString("dd/MM/yyyy") : null; }

        public QuotationReseravtion()
        {
            this.quoteid = null;
            this.bsd_reservationno = " ";
            this.totalamount = null;
        }
    }
}

