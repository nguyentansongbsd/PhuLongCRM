using System;
using PhuLongCRM.ViewModels;

namespace PhuLongCRM.Models
{
    public class TotalReservationModel : BaseViewModel
    {
        private decimal _listedPrice;
        public decimal ListedPrice { get => _listedPrice; set { _listedPrice = value;OnPropertyChanged(nameof(ListedPrice)); } }
        private decimal _discount;
        public decimal Discount { get => _discount; set { _discount = value; OnPropertyChanged(nameof(Discount)); } }
        private decimal _handoverAmount;
        public decimal HandoverAmount { get => _handoverAmount; set { _handoverAmount = value; OnPropertyChanged(nameof(HandoverAmount)); } }
        private decimal _netSellingPrice;
        public decimal NetSellingPrice { get => _netSellingPrice; set { _netSellingPrice = value; OnPropertyChanged(nameof(NetSellingPrice)); } }
        private decimal _landValue;
        public decimal LandValue { get => _landValue; set { _landValue = value; OnPropertyChanged(nameof(LandValue)); } }
        private decimal _totalTax;
        public decimal TotalTax { get => _totalTax; set { _totalTax = value; OnPropertyChanged(nameof(TotalTax)); } }
        private decimal _maintenanceFee;
        public decimal MaintenanceFee { get => _maintenanceFee; set { _maintenanceFee = value; OnPropertyChanged(nameof(MaintenanceFee)); } }
        private decimal _netSellingPriceAfterVAT;
        public decimal NetSellingPriceAfterVAT { get => _netSellingPriceAfterVAT; set { _netSellingPriceAfterVAT = value; OnPropertyChanged(nameof(NetSellingPriceAfterVAT)); } }
        private decimal _totalAmount;
        public decimal TotalAmount { get => _totalAmount; set { _totalAmount = value; OnPropertyChanged(nameof(TotalAmount)); } }
    }
}
