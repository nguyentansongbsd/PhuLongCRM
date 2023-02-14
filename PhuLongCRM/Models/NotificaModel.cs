using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class NotificaModel : BaseViewModel
    {
        public string Key { get; set; }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid ProjectId { get; set; }
        public Guid QueueId { get; set; }
        public Guid QuoteId { get; set; }
        public Guid ReservationId { get; set; }
        public Guid ContractId { get; set; }

        private string _backgroundColor;
        public string BackgroundColor { get => _backgroundColor; set { _backgroundColor = value; OnPropertyChanged(nameof(BackgroundColor)); } }
    }
}
