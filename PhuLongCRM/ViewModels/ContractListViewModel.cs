using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ContractListViewModel : ListViewBaseViewModel2<ContractModel>
    {
        public string Keyword { get; set; }
        public ContractListViewModel()
        {
            PreLoadData = new Command(() =>
            {
                EntityName = "salesorders";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='salesorder'>
                                    <attribute name='name' />
                                    <attribute name='customerid' />
                                    <attribute name='statuscode' />
                                    <attribute name='totalamount' />
                                    <attribute name='bsd_unitnumber' alias='unit_id'/>
                                    <attribute name='bsd_project' alias='project_id'/>
                                    <attribute name='salesorderid' />
                                    <attribute name='ordernumber' />
                                    <attribute name='bsd_contractnumber' />
                                    <order attribute='bsd_project' descending='true' />
                                    <filter type='and'>
                                        <condition attribute = '{UserLogged.UserAttribute}' operator= 'eq' value = '{UserLogged.Id}' />  
                                        <filter type='or'>      
                                            <condition attribute='customeridname' operator='like' value ='%25{Keyword}%25' />          
                                            <condition attribute='bsd_projectname' operator='like' value ='%25{Keyword}%25' />              
                                            <condition attribute='bsd_unitnumbername' operator='like' value ='%25{Keyword}%25' />             
                                            <condition attribute='ordernumber' operator='like' value ='%25{Keyword}%25' />
                                            <condition attribute='bsd_contractnumber' operator='like' value ='%25{Keyword}%25' />
                                        </filter >                  
                                    </filter >
                                    <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='outer' alias='aa'>
                                        <attribute name='bsd_name' alias='project_name'/>
                                    </link-entity>
                                    <link-entity name='product' from='productid' to='bsd_unitnumber' link-type='outer' alias='ab'>
                                        <attribute name='name' alias='unit_name'/>
                                    </link-entity>
                                    <link-entity name='account' from='accountid' to='customerid' link-type='outer' alias='ac'>
                                        <attribute name='name' alias='account_name'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='customerid' link-type='outer' alias='ad'>
                                        <attribute name='bsd_fullname' alias='contact_name'/>
                                    </link-entity>
                                </entity>
                            </fetch>";
            }); 
        }
    }
}
