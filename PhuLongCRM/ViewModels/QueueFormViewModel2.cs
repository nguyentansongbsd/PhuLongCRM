using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class QueueFormViewModel2 : BaseViewModel
    {
        private QueueFormModel _queue;
        public QueueFormModel Queue { get => _queue; set { _queue = value; OnPropertyChanged(nameof(Queue)); } }
        //public QueueUnitModel QueueUnit { get; set; }

        private QueueUnitModel _queueUnit;
        public QueueUnitModel QueueUnit { get => _queueUnit; set { _queueUnit = value; OnPropertyChanged(nameof(QueueUnit)); } }

        private OptionSetFilter _customer;
        public OptionSetFilter Customer { get => _customer; set { _customer = value; OnPropertyChanged(nameof(Customer));} }
        
        private List<LookUp> _daiLyOptions;
        public List<LookUp> DaiLyOptions { get => _daiLyOptions; set { _daiLyOptions = value; OnPropertyChanged(nameof(DaiLyOptions)); } }

        private LookUp _daiLyOption;
        public LookUp DailyOption { get => _daiLyOption; set { _daiLyOption = value; OnPropertyChanged(nameof(DailyOption)); } }

        private List<LookUp> _listCollaborator;
        public List<LookUp> ListCollaborator { get => _listCollaborator; set { _listCollaborator = value; OnPropertyChanged(nameof(ListCollaborator)); } }

        private LookUp _collaborator;
        public LookUp Collaborator { get => _collaborator; set { _collaborator = value; OnPropertyChanged(nameof(Collaborator)); } }

        private List<LookUp> _listCustomerReferral;
        public List<LookUp> ListCustomerReferral { get => _listCustomerReferral; set { _listCustomerReferral = value; OnPropertyChanged(nameof(ListCustomerReferral)); } }

        private LookUp _customerReferral;
        public LookUp CustomerReferral { get => _customerReferral; set { _customerReferral = value; OnPropertyChanged(nameof(CustomerReferral)); } }
        public string Error_message { get; set; }
        public bool queueProject { get; set; } = false;

        public QueueFormViewModel2()
        {
            Queue = new QueueFormModel();
        }
        /// <summary>
        /// kiểm tra giới hạn giữ chỗ
        /// return 0 có thể giữ chỗ,
        /// return 1 quá giới hạn unit,
        /// return 2 quá giới hạn giữ chỗ trên unit,
        /// return 3 quá giới hạn giữ chỗ trên 1 unit,
        /// return 4 không thấy project
        /// </summary>
        public async Task<int> CheckLimit()
        {
            if (QueueUnit != null && QueueUnit.project_id != Guid.Empty)
            {
                string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                                    <entity name='opportunity'>
                                        <attribute name='bsd_units' groupby='true' alias='group'/>
                                        <attribute name='opportunityid' aggregate='count' alias='count'/>
                                        <filter type='and'>
                                            <condition attribute='createdon' operator='today' />
                                            <condition attribute='bsd_units' operator='not-null' />
                                            <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                            <condition attribute='bsd_project' operator='eq' value='{QueueUnit.project_id}' />
                                        </filter>
                                      </entity>
                                    </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("opportunities", fetch);
                if (result == null || result.value.Count == 0)
                {
                    var queueperunit = await CheckLimitOnUnit();
                    return queueperunit;
                }
                // check sau aswait
                if (queueProject)
                    return 0;

                var data = result.value;
                var unit = data.Where(x => x.group == QueueUnit.unit_id.ToString()).FirstOrDefault();
                if (data.Count >= QueueUnit.bsd_unitspersalesman)
                {     // quá giới hạn số unit có thể giữ chỗ 
                    if(unit == null)
                        return 1;
                }
               // var unit = data.Where(x => x.group == QueueUnit.unit_id.ToString()).FirstOrDefault();
                if (unit != null)
                {
                    if (unit.count >= QueueUnit.bsd_queueunitdaysaleman)
                        // quá giới hạn số iuwx chỗ trên unit trên nhân viên trên ngày
                        return 2;
                    else
                    {
                        var queueperunit = await CheckLimitOnUnit();
                        return queueperunit;
                    }
                }
                else
                {
                    var queueperunit = await CheckLimitOnUnit();
                    return queueperunit;
                }
            }
            else
                return 4;
        }
        public async Task<int> CheckLimitOnUnit()
        {
            if (queueProject)
                return 0;
            if (QueueUnit != null && QueueUnit.unit_id != Guid.Empty)
            {
                string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                                    <entity name='opportunity'>
                                        <attribute name='opportunityid' aggregate='count' alias='count'/>
                                        <filter type='and'>
                                            <condition attribute='bsd_units' operator='eq' value='{QueueUnit.unit_id}' />
                                            <condition attribute='bsd_queueforproject' operator='eq' value='0' />
                                            <condition attribute='statuscode' operator='in'>
                                                <value>100000000</value>
                                                <value>100000002</value>
                                            </condition>
                                        </filter>
                                      </entity>
                                    </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("opportunities", fetch);
                if (result == null || result.value.Count == 0)
                    return 0;
                var data = result.value.FirstOrDefault();
                if (data.count >= QueueUnit.bsd_queuesperunit)
                    // quá giới hạn số unit có thể giữ chỗ 
                    return 3;
                else
                    return 0;
            }
            else
                return 4;
        }
        public async Task LoadSalesAgentCompany()
        {
            string fetchphaseslaunch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='bsd_phaseslaunch'>
                                <attribute name='bsd_name' />
                                <attribute name='bsd_locked' />
                                <attribute name='bsd_salesagentcompany' />
                                <attribute name='bsd_phaseslaunchid' />
                                <order attribute='bsd_name' descending='true' />
                                <filter type='and'>
                                    <condition attribute='bsd_phaseslaunchid' operator='eq' value='{Queue.bsd_phaseslaunch_id}' />
                                </filter>
                                <link-entity name='bsd_bsd_phaseslaunch_account' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='outer' alias='aw'>
                                    <link-entity name='account' from='accountid' to='accountid' link-type='inner' alias='ak'>
                                        <attribute name='name' alias='salesagentcompany_name' />
                                    </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result_phasesLaunch = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PhasesLaunch>>("bsd_phaseslaunchs", fetchphaseslaunch);

            string develop = $@"<link-entity name='bsd_project' from='bsd_investor' to='accountid' link-type='inner' alias='aj'>
                                                <filter type='and'>
                                                    <condition attribute='bsd_projectid' operator='eq' value='{Queue.bsd_project_id}' />
                                                </filter>
                                            </link-entity>";
            string all = $@"<link-entity name='bsd_projectshare' from='bsd_salesagent' to='accountid' link-type='inner' alias='az'>
                                                <filter type='and'>
                                                    <condition attribute='statuscode' operator='eq' value='1' />
                                                    <condition attribute='bsd_project' operator='eq' value='{Queue.bsd_project_id}' />
                                                </filter>
                                            </link-entity>";
            string sale_phasesLaunch = $@"<link-entity name='bsd_bsd_phaseslaunch_account' from='accountid' to='accountid' link-type='inner' alias='ak'>
                                                        <filter type='and'>
                                                            <condition attribute='bsd_phaseslaunchid' operator='eq' value='{Queue.bsd_phaseslaunch_id}' />
                                                         </filter>
                                                    </link-entity>";
            string isproject = $@"<filter type='and'>
                                       <condition attribute='bsd_businesstypesys' operator='contain-values'>
                                         <value>100000002</value>
                                       </condition>                                
                                    </filter>";

            if (result_phasesLaunch != null && result_phasesLaunch.value.Count > 0)
            {
                var phasesLaunch = result_phasesLaunch.value.FirstOrDefault();
                if (phasesLaunch.bsd_locked == false)
                {
                    if (string.IsNullOrWhiteSpace(phasesLaunch.salesagentcompany_name))
                    {
                        if (DaiLyOptions != null)
                        {
                            DaiLyOptions = await LoadAccuntSales(all);
                            //var list2 = await LoadAccuntSales(develop);
                            //DaiLyOptions = list1.Union(list2).Distinct().ToList();
                        }
                    }
                    else
                    {
                        if (DaiLyOptions != null)
                        {
                            DaiLyOptions = await LoadAccuntSales(sale_phasesLaunch);
                            //var list2 = await LoadAccuntSales(develop);
                            //DaiLyOptions = list1.Union(list2).Distinct().ToList();
                        }
                    }
                }
                else if (phasesLaunch.bsd_locked == true)
                {
                    if (!string.IsNullOrWhiteSpace(phasesLaunch.salesagentcompany_name))
                    {
                        if (DaiLyOptions != null)
                        {
                            DaiLyOptions.AddRange(await LoadAccuntSales(sale_phasesLaunch));
                        }
                    }
                    else
                    {
                        //if (DaiLyOptions != null)
                        //{
                        //    DaiLyOptions.AddRange(await LoadAccuntSales(develop));
                        //}
                    }
                }

            }
            else
            {
                if (DaiLyOptions != null)
                {
                    var list1 = await LoadAccuntSales(all);
                    var list2 = await LoadAccuntSales(develop);
                    DaiLyOptions = list1.Union(list2).Distinct().ToList();
                }
            }
        }
        public async Task<List<LookUp>> LoadAccuntSales(string filter)
        {
            List<LookUp> list = new List<LookUp>();
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='account'>
                                    <attribute name='name' alias='Name' />
                                    <attribute name='accountid' alias='Id' />
                                    <order attribute='createdon' descending='true' />
                                    " + filter + @"
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("accounts", fetch);
            if (result != null && result.value.Count != 0)
            {
                var data = result.value;
                foreach (var item in data)
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public async Task LoadCollaboratorLookUp()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='contactid' alias='Id' />
                    <attribute name='fullname' alias='Name' />
                    <order attribute='createdon' descending='true' />                   
                    <filter type='and'>
                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='" + UserLogged.Id + @"' />
                        <condition attribute='bsd_type' operator='eq' value='100000001' />
                    </filter>
                  </entity>
                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("contacts", fetch);
            if (result == null || result.value.Count == 0)
                return;
            var data = result.value;
            foreach (var item in data)
            {
                ListCollaborator.Add(item);
            }
        }
        public async Task LoadCustomerReferralLookUp()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='contactid' alias='Id' />
                    <attribute name='fullname' alias='Name' />
                    <order attribute='createdon' descending='true' />                   
                    <filter type='and'>
                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='" + UserLogged.Id + @"' />
                        <condition attribute='bsd_type' operator='eq' value='100000000' />
                    </filter>
                  </entity>
                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("contacts", fetch);
            if (result == null || result.value.Count == 0)
                return;
            var data = result.value;
            foreach (var item in data)
            {
                ListCustomerReferral.Add(item);
            }
        }
        public async Task<string> createQueueDraft(bool isQueueProject, Guid id)
        {
            if (isQueueProject)
            {
                var data = new
                {
                    Command = "ProjectQue"
                };
                var res = await CrmHelper.PostData($"/bsd_projects({id})//Microsoft.Dynamics.CRM.bsd_Action_Project_QueuesForProject", data);

                if (res.IsSuccess)
                {
                    string str = res.Content.ToString();
                    string[] arrListStr = str.Split(',');
                    foreach (var item in arrListStr)
                    {
                        if (item.Contains("content") == true)
                        {
                            var itemformat = item.Replace("content", "").Replace(":", "").Replace("'", "").Replace("}", "").Replace('"', ' ').Trim();
                            if (Guid.Parse(itemformat) != Guid.Empty)
                            {
                                Queue.opportunityid = Guid.Parse(itemformat);
                                var res_update = await UpdateQueue();
                                if (res_update)
                                    return null;
                                else
                                    return Error_message;
                            }
                            else
                            {
                                return res.ErrorResponse?.error.message;
                            }
                        }
                    }
                }
                else
                {
                    return res.ErrorResponse?.error.message;
                }
                return res.ErrorResponse?.error.message;
            }
            else
            {
                var data = new
                {
                    Command = "Book"
                };

                var res = await CrmHelper.PostData($"/products({id})//Microsoft.Dynamics.CRM.bsd_Action_DirectSale", data);

                if (res.IsSuccess)
                {
                    string str = res.Content.ToString();
                    string[] arrListStr = str.Split(',');
                    foreach (var item in arrListStr)
                    {
                        if (item.Contains("content") == true)
                        {
                            var itemformat = item.Replace("content", "").Replace(":", "").Replace("'", "").Replace("}", "").Replace('"', ' ').Trim();
                            if (Guid.Parse(itemformat) != Guid.Empty)
                            {
                                Queue.opportunityid = Guid.Parse(itemformat);
                                var res_update = await UpdateQueue();
                                if (res_update)
                                    return null;
                                else
                                    return Error_message;
                            }
                            else
                            {
                                return res.ErrorResponse?.error.message;
                            }
                        }
                    }
                }
                else
                {
                    return res.ErrorResponse?.error.message;
                }
                return res.ErrorResponse?.error.message;
            }
        }
        public async Task<bool> UpdateQueue()
        {
            if (Queue.opportunityid != Guid.Empty)
            {
                string path = "/opportunities(" + Queue.opportunityid + ")";
                var content = await this.getContent();
                CrmApiResponse result = await CrmHelper.PatchData(path, content);
                if (result.IsSuccess)
                {
                    return true;
                }
                else
                {
                    Error_message = result.ErrorResponse?.error.message;
                    return false;
                }
            }
            else
                return false;
        }
        private async Task<object> getContent()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_queuingfee"] = Queue.bsd_queuingfee;
            data["name"] = Queue.name;

            if (Customer != null || !string.IsNullOrWhiteSpace(Customer.Val))
            {
                if (Customer.Title == Controls.LookUpMultipleTabs.CodeAccount)
                {
                    data["customerid_account@odata.bind"] = $"/accounts({Customer.Val})";
                    await DeletLookup("customerid_contact", Queue.opportunityid);
                }
                else
                {
                    data["customerid_contact@odata.bind"] = $"/contacts({Customer.Val})";
                    await DeletLookup("customerid_account", Queue.opportunityid);
                }
            }

            data["budgetamount"] = Queue.budgetamount;
            data["estimatedvalue"] = 0;
            data["description"] = Queue.description;

            if (DailyOption == null || DailyOption.Id == Guid.Empty)
            {
                await DeletLookup("bsd_salesagentcompany", Queue.opportunityid);
            }
            else
            {
                data["bsd_salesagentcompany@odata.bind"] = $"/accounts({DailyOption.Id})";
            }

            if (Collaborator == null || Collaborator.Id == Guid.Empty)
            {
                await DeletLookup("bsd_collaborator", Queue.opportunityid);
            }
            else
            {
                data["bsd_collaborator@odata.bind"] = $"/contacts({Collaborator.Id})";
            }

            if (CustomerReferral == null || CustomerReferral.Id == Guid.Empty)
            {
                await DeletLookup("bsd_customerreferral_contact", Queue.opportunityid);
            }
            else
            {
                data["bsd_customerreferral_contact@odata.bind"] = $"/contacts({CustomerReferral.Id})";
            }

            data["bsd_nameofstaffagent"] = Queue.bsd_nameofstaffagent;

            if (UserLogged.IsLoginByUserCRM == false && UserLogged.Id != null)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.IsLoginByUserCRM == false && UserLogged.ManagerId != Guid.Empty)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }
            data["transactioncurrencyid@odata.bind"] = $"/transactioncurrencies(2366fb85-b881-e911-a83b-000d3a07be23)";
            return data;
        }
        public async Task<Boolean> DeletLookup(string fieldName, Guid Id)
        {
            var result = await CrmHelper.SetNullLookupField("opportunities", Id, fieldName);
            return result.IsSuccess;
        }
        public async Task<bool> CheckCustomer()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='opportunity'>
                                    <attribute name='customerid' />
                                    <attribute name='opportunityid' />
                                    <filter type='and'>
                                        <condition attribute='customerid' operator='eq' value='{Customer.Val}' />
                                        <condition attribute='bsd_queueforproject' operator='eq' value='0' />
                                        <condition attribute='bsd_units' operator='eq' uitype='product' value='{QueueUnit.unit_id}' />
                                        <condition attribute='statuscode' operator='in'>
                                            <value>100000000</value>
                                            <value>100000002</value>
                                        </condition>
                                    </filter>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueueFormModel>>("opportunities", fetch);
            if (result == null || result.value == null || result.value.Count == 0)
                return true;
            else
                return false;
        }
    }
}
