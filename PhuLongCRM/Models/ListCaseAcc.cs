using System;

using Xamarin.Forms;

namespace PhuLongCRM.Models
{
    public class ListCaseAcc : ContentView
    {
        public string customerid { get; set; }
        public string title { get; set; }
        public string ticketnumber { get; set; }
        public int prioritycode { get; set; }
        public string prioritycodevalue
        {
            get
            {
                switch (prioritycode)
                {
                    case 1:
                        return "High";
                    case 2:
                        return "Normal";
                    case 3:
                        return "Low";
                    default:
                        return "";
                }
            }
        }
        public int caseorigincode { get; set; }
        public string caseorigincodevalue
        {
            get
            {
                switch (caseorigincode)
                {
                    case 1:
                        return "Phone";
                    case 2:
                        return "Email";
                    case 3:
                        return "Web";
                    case 2483:
                        return "Facebook";
                    case 3986:
                        return "Twitter";
                    default:
                        return "";
                }
            }
        }
        public int statuscode { get; set; }
        public string statuscodevalue
        {
            get
            {
                switch (statuscode)
                {
                    case 1:
                        return "In Progress";
                    case 2:
                        return "On Hold";
                    case 3:
                        return "Waiting for Details";
                    case 4:
                        return "Researching";
                    case 5:
                        return "Problem Solved";
                    case 1000:
                        return "Information Provided";
                    case 6:
                        return "Canceled";
                    case 2000:
                        return "Merged";
                    default:
                        return "";
                }
            }
        }

        public string case_nameaccount { get; set; }
        public string case_nameaccontact { get; set; }
    }
}

