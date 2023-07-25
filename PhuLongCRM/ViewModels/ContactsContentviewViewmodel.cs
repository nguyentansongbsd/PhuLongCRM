using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ContactsContentviewViewmodel : ListViewBaseViewModel2<ContactListModel>
    {
        public string Keyword { get; set; }
        public string KeyFilter { get; set; }
        public string FillterStatus { get; set; } = @"<condition attribute='bsd_type' operator='contain-values'>
                                                        <value>100000000</value>
                                                      </condition>";
        public bool FillterBirtday { get; set; }
        public ContactsContentviewViewmodel()
        {
            PreLoadData = new Command(() =>
            {
             //   string birtday = string.Empty;
                string filter = string.Empty;
                if (!string.IsNullOrWhiteSpace(KeyFilter))
                {
                    if (KeyFilter == "-1")
                        filter = null;
                    else if (KeyFilter == "0")
                        filter = @"<filter type='or'>
                                  <condition attribute='statuscode' operator='in'>
                                    <value>1</value>
                                    <value>100000000</value>
                                  </condition>
                                </filter>";
                    else if (KeyFilter == "1")
                        filter = "<condition attribute='statuscode' operator='eq' value='100000000' />";
                    else if (KeyFilter == "2")
                        filter = "<condition attribute='statuscode' operator='eq' value='1' />";
                    else if (KeyFilter == "3")
                        filter = @"<filter type='or'>
                                  <condition attribute='statuscode' operator='in'>
                                    <value>2</value>
                                    <value>100000001</value>
                                    <value>100000002</value>
                                  </condition>
                                </filter>"; // Vo hieu luc
                    else
                        filter = string.Empty;
                }
                //if (FillterBirtday)
                //{
                //    birtday = $"<condition attribute='birthdate' operator='on' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}' />";
                //}
                //else
                //{
                //    birtday = string.Empty;
                //}
                EntityName = "contacts";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='bsd_fullname' />
                    <attribute name='mobilephone' />
                    <attribute name='bsd_new_birthday' />
                    <attribute name='emailaddress1' />
                    <attribute name='bsd_contactaddress' />
                    <attribute name='createdon' />
                    <attribute name='contactid' />
                    <attribute name='statuscode' />
                    <attribute name='bsd_customercode' />
                    <order attribute='createdon' descending='true' />
                    <filter type='or'>
                        <condition attribute='bsd_fullname' operator='like' value='%25{Keyword}%25' />
                        <condition attribute='bsd_identitycard' operator='like' value='%25{Keyword}%25' />
                        <condition attribute='mobilephone' operator='like' value='%25{Keyword}%25' />
                        <condition attribute='emailaddress1' operator='like' value='%25{Keyword}%25' />
                        <condition attribute='bsd_customercode' operator='like' value='%25{Keyword}%25' />
                    </filter>
                    <filter type='and'>
                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                      {filter}
                      {FillterStatus}
                    </filter>
                  </entity>
                </fetch>";
            });
        }
        public override async Task LoadOnRefreshCommandAsync()
        {
            await base.LoadOnRefreshCommandAsync();
            if (Data != null && Data.Count > 0 && FillterBirtday)
            {
                List<ContactListModel> list = new List<ContactListModel>();
                foreach (var item in Data)
                {
                    if (item.bsd_new_birthday?.Day == DateTime.Today.Day && item.bsd_new_birthday?.Month == DateTime.Today.Month)
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
