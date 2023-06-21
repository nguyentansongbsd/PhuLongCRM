using PhuLongCRM.Helper;
using System;
namespace PhuLongCRM.Models
{
    public class PaymentSchemeModel
    {
        public Guid bsd_paymentschemeid { get; set; }
        public string bsd_name { get; set; }
        public DateTime bsd_startdate { get; set; }
        public DateTime bsd_enddate { get; set; }
        public bool bsd_applyforbankloan { get; set; }
        public decimal bsd_optionforfeiture { get; set; }
        public string bsd_optionforfeiture_format { get => StringFormatHelper.FormatPercent(bsd_optionforfeiture); }
        public decimal bsd_daforfeiture { get; set; }
        public string bsd_daforfeiture_format { get => StringFormatHelper.FormatPercent(bsd_daforfeiture); }
        public decimal bsd_spforfeiture { get; set; }
        public string bsd_spforfeiture_format { get => StringFormatHelper.FormatPercent(bsd_spforfeiture); }
        public string bsd_paymentschemecodenew { get; set; }
        public string bsd_paymentschemecode { get; set; }
        public string bsd_type { get; set; }
        public string bsd_type_format { get => PaymentSchemeTypeData.GetPaymentSchemeTypeById(bsd_type)?.Label; }
    }
}
