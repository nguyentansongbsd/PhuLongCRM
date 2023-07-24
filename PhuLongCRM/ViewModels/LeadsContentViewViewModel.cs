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
        public string KeySort { get; set; }
        public bool Create_on_sort { get; set; } = true;
        public bool Rating_sort { get; set; } = true;
        public bool Allocation_sort { get; set; } = false;
        public string FillterStatus { get; set; }
        public bool FillterBirtday { get; set; }
        public LeadsContentViewViewModel()
        {                   
            PreLoadData = new Command(() =>
            {
                string filter_sts = string.Empty;
                string filter_name = string.Empty;
                string filter_phone = string.Empty;
                string filter_subject = string.Empty;
                string filter_customercode = string.Empty;
                string sort = string.Empty;
                string filter_allocation = string.Empty;
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    filter_name = $@"<condition attribute='lastname' operator='like' value='%25{Keyword}%25' />";
                    filter_phone = $@"<condition attribute='mobilephone' operator='like' value='%25{Keyword}%25' />";
                    filter_subject = $@"<condition entityname='Topic' attribute='bsd_name' operator='like' value='%25{Keyword}%25' />";
                    filter_customercode = $@"<condition attribute='bsd_customercode' operator='like' value='%25{Keyword}%25' />";
                }
                //else
                //{
                //    filter_sts = $@"<condition attribute='statuscode' operator='in'>
                //                    <value>3</value>
                //                    <value>4</value>
                //                    <value>5</value>
                //                    <value>7</value>
                //                    <value>6</value>
                //                  </condition>";
                //}
                if(!string.IsNullOrWhiteSpace(FillterStatus))
                {
                    filter_sts = FillterStatus;
                }    
                if (!string.IsNullOrWhiteSpace(KeySort))
                {
                    if (KeySort == "1")
                    {
                        if (Create_on_sort)
                            sort = $"<order attribute='createdon' descending='true' />";
                        else
                            sort = $"<order attribute='createdon' descending='false' />";
                    }
                    else if (KeySort == "2")
                    {
                        if (Rating_sort)
                            sort = $"<order attribute='leadqualitycode' descending='true' />";
                        else
                            sort = $"<order attribute='leadqualitycode' descending='false' />";
                    }
                    else if (KeySort == "3")
                    {
                        if (Allocation_sort)
                            filter_allocation = $"<condition attribute='bsd_allocation' operator='eq' value='1' />";
                        else
                            filter_allocation = $"<condition attribute='bsd_allocation' operator='eq' value='0' />";
                    }
                    else
                        sort = "<order attribute='createdon' descending='true' />";
                }
                else
                    sort = "<order attribute='createdon' descending='true' />";               

                EntityName = "leads";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='lead'>
                        <attribute name='lastname' />
                        <attribute name='fullname' />
                        <attribute name='subject' />
                        <attribute name='statuscode' />
                        <attribute name='mobilephone'/>
                        <attribute name='emailaddress1' />
                        <attribute name='createdon' />
                        <attribute name='new_birthday' />
                        <attribute name='leadid' />
                        <attribute name='leadqualitycode' />
                        <attribute name='bsd_customercode' />
                        {sort}
                        <filter type='and'>
                            {filter_sts}
                            {filter_allocation}
                             <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='" + UserLogged.Id + @"' />
                             <filter type='or'>
                                 '" + filter_name + @"'
                                 '" + filter_phone + @"'
                                 '" + filter_subject + @"'
                                 '" + filter_customercode + @"'
                             </filter>
                        </filter>
                        <link-entity name='bsd_topic' from='bsd_topicid' to='bsd_topic' link-type='inner' alias='Topic'>
                          <attribute name='bsd_name' />
                        </link-entity>
                      </entity>
                    </fetch>";
            });
        }
        public override async Task LoadOnRefreshCommandAsync()
        {
            await base.LoadOnRefreshCommandAsync();
            if (Data != null && Data.Count > 0 && FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var item in Data)
                {
                    var birthday = item.new_birthday.ToLocalTime();
                    if (birthday.Day == DateTime.Today.Day && birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }
                }
                Data.Clear();
                Data.AddRange(list);
            }
          //  return null;
        }
    }
}
