using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class EventModel
    {
        public Guid bsd_eventid { get; set; }
        public DateTime bsd_startdate { get; set; }
        public DateTime bsd_enddate { get; set; }
        public int statuscode { get; set; }
        public DateTime createdon { get; set; }
    }
}
