using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ReservationListViewModel : ListViewBaseViewModel2<ReservationListModel>
    {
        public string Keyword { get; set; }
        public ReservationListViewModel()
        {
            PreLoadData = new Command(() =>
            {
                EntityName = "quotes";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='quote'>
                        <attribute name='name' />
                        <attribute name='totalamount' />
                        <attribute name='bsd_unitno' alias='bsd_unitno_id' />
                        <attribute name='statuscode' />
                        <attribute name='bsd_projectid' alias='bsd_project_id' />
                        <attribute name='quoteid' />
                        <order attribute='createdon' descending='true' />
                        <filter type='and'>
                            <filter type='or'>
                              <condition attribute='customeridname' operator='like' value='%25{Keyword}%25' />
                              <condition attribute='bsd_projectidname' operator='like' value='%25{Keyword}%25' />
                              <condition attribute='bsd_quotationnumber' operator='like' value='%25{Keyword}%25' />
                              <condition attribute='name' operator='like' value='%25{Keyword}%25' />
                            </filter>
                            <condition attribute='bsd_employee' operator='eq' value='{UserLogged.Id}'/>
                            <filter type='or'>
                              <condition attribute='statuscode' operator='in'>
                                <value>100000007</value>
                              </condition>
                              <filter type='and'>
                                 <condition attribute='statuscode' operator='in'>
                                    <value>100000009</value>
                                    <value>6</value>
                                  </condition>
                                  <condition attribute='bsd_quotationsigneddate' operator='null' />
                              </filter>
                            </filter>
                        </filter>
                        <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectid' visible='false' link-type='outer' alias='a'>
                            <attribute name='bsd_name' alias='bsd_project_name' />
                        </link-entity>
                        <link-entity name='product' from='productid' to='bsd_unitno' visible='false' link-type='outer' alias='b'>
                          <attribute name='name' alias='bsd_unitno_name' />
                        </link-entity>
                        <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer' alias='c'>
                          <attribute name='bsd_name' alias='purchaser_accountname' />
                        </link-entity>
                        <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='d'>
                          <attribute name='bsd_fullname' alias='purchaser_contactname' />
                        </link-entity>
                      </entity>
                    </fetch>";
            });
        }
    }
}
