﻿using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class QueuListViewModel : ListViewBaseViewModel2<QueuesModel>
    {
        public ObservableCollection<OptionSet> FiltersStatus { get; set; } = new ObservableCollection<OptionSet>();

        public List<string> _filterStatus;
        public List<string> FilterStatus { get => _filterStatus; set { _filterStatus = value; OnPropertyChanged(nameof(FilterStatus)); } }
        public ObservableCollection<OptionSet> FiltersProject { get; set; } = new ObservableCollection<OptionSet>();
        public ObservableCollection<OptionSet> FiltersQueueForProject { get; set; } = new ObservableCollection<OptionSet>();

        public OptionSet _filterProject;
        public OptionSet FilterProject { get => _filterProject; set { _filterProject = value; OnPropertyChanged(nameof(FilterProject)); } }

        public OptionSet _filterQueueForProject;
        public OptionSet FilterQueueForProject { get => _filterQueueForProject; set { _filterQueueForProject = value; OnPropertyChanged(nameof(FilterQueueForProject)); } }
        public string Keyword { get; set; }
        public ObservableCollection<OptionSet> RapCans { get; set; } = new ObservableCollection<OptionSet>();

        public OptionSet _rapCan;
        public OptionSet RapCan { get => _rapCan; set { _rapCan = value; OnPropertyChanged(nameof(RapCan)); } }

        public QueuListViewModel()
        {
            PreLoadData = new Command(() =>
            {
                string project = null;
                string status = null;
                string queueforproject = null;
                string rapcan = null;
                if (FilterStatus != null && FilterStatus.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(FilterStatus.Where(x => x == "-1").FirstOrDefault()))
                    {
                        string sts = string.Empty;
                        foreach (var item in FilterStatus)
                        {
                            sts += $@"<value>{item}</value>";
                        }
                        status = @"<condition attribute='statuscode' operator='in'>" + sts + "</condition>";
                    }
                    else
                    {
                        status = null;
                    }
                }
                else
                {
                    status = null;
                }
                if (FilterProject != null && FilterProject.Val != "-1")
                {
                    project = $@"<condition attribute='bsd_project' operator='eq' value='{FilterProject.Val}' />";
                }
                else
                {
                    project = null;
                }
                if (FilterQueueForProject != null && FilterQueueForProject.Val != "-1")
                {
                    queueforproject = $@"<condition attribute='bsd_queueforproject' operator='eq' value='{FilterQueueForProject.Val}' />";
                }
                else
                {
                    queueforproject = null;
                }
                if(RapCan != null && RapCan.Val != "-1")
                {
                    if (RapCan.Val != "1")
                        rapcan = @"<condition attribute='bsd_units' operator='null' />
                                    <condition attribute='statuscode' operator='in'>
                                        <value>100000000</value>
                                        <value>100000002</value>
                                    </condition>";
                    else if (RapCan.Val != "0")
                        rapcan = @"<condition attribute='bsd_units' operator='not-null' />        
                                    <condition attribute='statuscode' operator='in'>
                                        <value>100000000</value>
                                        <value>100000002</value>
                                    </condition>"; ;
                }   
                else
                {
                    rapcan = null;
                }    

                EntityName = "opportunities";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='opportunity'>
                        <attribute name='name'/>
                        <attribute name='statuscode' />
                        <attribute name='bsd_project' />
                        <attribute name='opportunityid' />
                        <attribute name='bsd_queuenumber' />
                        <attribute name='bsd_queuingexpired' />
                        <attribute name='createdon' />
                        <attribute name='bsd_bookingtime' />
                        <attribute name='bsd_queueforproject' />
                        <attribute name='bsd_queuingfeepaid' />
                        <order attribute='bsd_bookingtime' descending='true' />
                        <filter type='and'>                          
                            <filter type='or'>
                                <condition attribute='name' operator='like' value='%25{Keyword}%25' />
                                <condition attribute='customeridname' operator='like' value='%25{Keyword}%25' />
                                <condition attribute='bsd_queuenumber' operator='like' value='%25{Keyword}%25' />
                                <condition attribute='bsd_unitsname' operator='like' value='%25{Keyword}%25' /> 
                            </filter>
                        <condition attribute='statuscode' operator='in'>
                            <value>4</value>
                            <value>100000004</value>
                            <value>100000003</value>
                            <value>100000000</value>
                            <value>100000002</value>
                            <value>100000009</value>
                            <value>100000010</value>
                            <value>100000008</value>
                          </condition>
                          <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                          {status}
                          {project}
                          {queueforproject}
                          {rapcan}
                        </filter>
                        <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='fullname'  alias='contact_name'/>
                        </link-entity>
                        <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='name'  alias='account_name'/>
                        </link-entity>
                        <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' visible='false' link-type='outer' alias='a_edc3f143ba81e911a83b000d3a07be23'>
                            <attribute name='bsd_name' alias='project_name'/>
                        </link-entity>
                        <link-entity name='product' from='productid' to='bsd_units' visible='false' link-type='outer' alias='a_5025d361ba81e911a83b000d3a07be23'>
                            <attribute name='name' alias='bsd_units_name'/>
                        </link-entity>
                      </entity>
                    </fetch>";
            });
        }

        public void LoadStatus()
        {
            if (FiltersStatus != null && FiltersStatus.Count == 0)
            {
                FiltersStatus.Add(new OptionSet("-1", Language.tat_ca));
                var list = QueuesStatusCodeData.GetQueuesByIds("100000000,100000002,100000008,4,100000004,100000003");
                foreach (var item in list)
                {
                    FiltersStatus.Add(new OptionSet(item.Id, item.Name));
                }
            }
        }

        public async Task LoadProject()
        {
            if (FiltersProject != null)
            {
                string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='bsd_project'>
                                    <attribute name='bsd_projectid' alias='Val'/>
                                    <attribute name='bsd_projectcode'/>
                                    <attribute name='bsd_name' alias='Label'/>
                                    <attribute name='createdon' />
                                    <order attribute='bsd_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='statuscode' operator='eq' value='861450002' />
                                    </filter>
                                  </entity>
                            </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_projects", fetchXml);
                if (result == null || result.value.Any() == false) return;

                FiltersProject.Add(new OptionSet("-1", Language.tat_ca));
                var data = result.value;
                foreach (var item in data)
                {
                    FiltersProject.Add(item);
                }
            }
        }
        public void LoadQueueForProject()
        {
            if (FiltersQueueForProject != null && FiltersQueueForProject.Count == 0)
            {
                FiltersQueueForProject.Add(new OptionSet("-1", Language.tat_ca));
                FiltersQueueForProject.Add(new OptionSet("1", Language.co));
                FiltersQueueForProject.Add(new OptionSet("0", Language.khong));
            }
        }
        public void LoadRapCan()
        {
            if (RapCans != null && RapCans.Count == 0)
            {
                RapCans.Add(new OptionSet("-1", Language.tat_ca));
                RapCans.Add(new OptionSet("1", Language.co));
                RapCans.Add(new OptionSet("0", Language.khong));
            }
        }
    }
}
