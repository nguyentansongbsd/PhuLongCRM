using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{   
    public class LeadsContentViewViewModel : ListViewBaseViewModel2<LeadListModel>
    {
        public string Keyword { get; set; }

        public LeadsContentViewViewModel()
        {                   
            PreLoadData = new Command(() =>
            {
                string filter_name = string.Empty;
                string filter_phone = string.Empty;
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    filter_name = $@"<condition attribute='lastname' operator='like' value='%25{Keyword}%25' />";
                    filter_phone = $@"<condition attribute='mobilephone' operator='like' value='%25{Keyword}%25' />";
                }
                EntityName = "leads";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='lead'>
                        <attribute name='lastname' />
                        <attribute name='subject' />
                        <attribute name='statuscode' />
                        <attribute name='mobilephone'/>
                        <attribute name='emailaddress1' />
                        <attribute name='createdon' />
                        <attribute name='leadid' />
                        <attribute name='leadqualitycode' />
                        <order attribute='createdon' descending='true' />
                        <filter type='and'>
                             <condition attribute='bsd_employee' operator='eq' uitype='bsd_employee' value='" + UserLogged.Id + @"' />
                             <filter type='or'>
                                 '" + filter_name + @"'
                                 '" + filter_phone + @"'   
                             </filter>
                        </filter>
                      </entity>
                    </fetch>";
            });
        }
    }
}
