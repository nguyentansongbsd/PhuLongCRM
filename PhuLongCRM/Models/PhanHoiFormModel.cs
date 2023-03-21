using PhuLongCRM.ViewModels;
using System;

namespace PhuLongCRM.Models
{
    public class PhanHoiFormModel:BaseViewModel
    { 
        public Guid incidentid { get; set; }

        private string _title;
        public string title { get => _title; set { _title = value; OnPropertyChanged(nameof(title)); } }
        public string caseorigincode { get; set; }
        public string casetypecode { get; set; }

        private string _description;
        public string description { get => _description; set { _description = value; OnPropertyChanged(nameof(description)); } }

        public string subjectId { get; set; }
        public string subjectTitle { get; set; }

        public string parentCaseId { get; set; }
        public string parentCaseTitle { get; set; }

        public string accountId { get; set; }
        public string accountName { get; set; }

        public string contactId { get; set; }
        public string contactName { get; set; }

        public string unitId { get; set; }
        public string unitName { get; set; }

        public DateTime createdon { get; set; }
        public int statuscode { get; set; }
        public int statecode { get; set; }
    }
}

