using PhuLongCRM.ViewModels;
using System;
namespace PhuLongCRM.Models
{
    public class QueueUnitModel : BaseViewModel
    {
        public Guid unit_id { get; set; }
        public string unit_name { get; set; }
        public Guid project_id { get; set; }
        public string project_name { get; set; }
        public Guid phaseslaunch_id { get; set; }
        public string phaseslaunch_name { get; set; }
        public int bsd_queuesperunit { get; set; }
        public int bsd_unitspersalesman { get; set; }
        public int bsd_queueunitdaysaleman { get; set; }
        public decimal bsd_queuingfee { get; set; }
        public decimal bsd_bookingfee { get; set; }

        private string _bsd_queuingfee_format;
        public string bsd_queuingfee_format { get => _bsd_queuingfee_format; set { _bsd_queuingfee_format = value; OnPropertyChanged(nameof(bsd_queuingfee_format)); } }
    }
}
