using System;
namespace PhuLongCRM.Models
{
    public class TotalReservationModel
    {
        public decimal ListedPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal HandoverAmount { get; set; }
        public decimal NetSellingPrice { get; set; }
        public decimal LandValue { get; set; }
        public decimal TotalTax { get; set; }
        public decimal MaintenanceFee { get; set; }
        public decimal NetSellingPriceAfterVAT { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
