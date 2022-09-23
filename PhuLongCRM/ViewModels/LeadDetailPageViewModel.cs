using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using PhuLongCRM.Resources;

namespace PhuLongCRM.ViewModels
{
    public class LeadDetailPageViewModel : BaseViewModel
    {
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();

        private LeadFormModel _singleLead;
        public LeadFormModel singleLead { get => _singleLead; set { _singleLead = value; OnPropertyChanged(nameof(singleLead)); } }

        private OptionSet _singleGender;
        public OptionSet singleGender { get => _singleGender; set { _singleGender = value; OnPropertyChanged(nameof(singleGender)); } }

        private OptionSet _leadSource;
        public OptionSet LeadSource { get => _leadSource; set { _leadSource = value; OnPropertyChanged(nameof(LeadSource)); } }
        private OptionSet _singleIndustrycode;

        public OptionSet singleIndustrycode { get => _singleIndustrycode; set { _singleIndustrycode = value; OnPropertyChanged(nameof(singleIndustrycode)); } }

        private OptionSet _customerGroup;
        public OptionSet CustomerGroup { get => _customerGroup; set { _customerGroup = value; OnPropertyChanged(nameof(CustomerGroup)); } }

        private OptionSet _typeIdCard;
        public OptionSet TypeIdCard { get => _typeIdCard; set { _typeIdCard = value; OnPropertyChanged(nameof(TypeIdCard)); } }

        private OptionSet _area;
        public OptionSet Area { get => _area; set { _area = value; OnPropertyChanged(nameof(Area)); } }

        private PhongThuyModel _PhongThuy;
        public PhongThuyModel PhongThuy { get => _PhongThuy; set { _PhongThuy = value; OnPropertyChanged(nameof(PhongThuy)); } }
        public ObservableCollection<HuongPhongThuy> list_HuongTot { set; get; }
        public ObservableCollection<HuongPhongThuy> list_HuongXau { set; get; }
        public bool IsSuccessContact { get; set; } = false;
        public bool IsSuccessAccount { get; set; } = false;

        public int LeadStatusCode { get; set; }
        public int LeadStateCode { get; set; }

        public string CodeLead = Controls.LookUpMultipleTabs.CodeLead;

        private ObservableCollection<HoatDongListModel> _list_customercare;
        public ObservableCollection<HoatDongListModel> list_customercare { get => _list_customercare; set { _list_customercare = value; OnPropertyChanged(nameof(list_customercare)); } }

        private bool _showMoreCase;
        public bool ShowMoreCase { get => _showMoreCase; set { _showMoreCase = value; OnPropertyChanged(nameof(ShowMoreCase)); } }
        public int PageCase { get; set; } = 1;

        public bool IsFromQRCode { get; set; }

        public LeadDetailPageViewModel()
        {
            list_HuongTot = new ObservableCollection<HuongPhongThuy>();
            list_HuongXau = new ObservableCollection<HuongPhongThuy>();
        }

        public async Task LoadOneLead(String leadid)
        {
            string filterEmployee = string.Empty;
            if (IsFromQRCode == false)
            {
                filterEmployee = $@"<filter type='and'>
                                          <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>";
            }
            singleLead = new LeadFormModel();
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='lead'>
                                    <attribute name='lastname' />
                                    <attribute name='fullname' />
                                    <attribute name='subject' alias='bsd_topic_label'/>
                                    <attribute name='statuscode' />
                                    <attribute name='statecode' />
                                    <attribute name='leadqualitycode' />
                                    <attribute name='mobilephone' />
                                    <attribute name='telephone1' />
                                    <attribute name='emailaddress1' />
                                    <attribute name='jobtitle' />  
                                    <attribute name='companyname' />
                                    <attribute name='websiteurl' />
                                    <attribute name='address1_composite' />
                                    <attribute name='description' />
                                    <attribute name='industrycode' />
                                    <attribute name='revenue' />
                                    <attribute name='numberofemployees' />
                                    <attribute name='sic' />
                                    <attribute name='donotsendmm' />
                                    <attribute name='lastusedincampaign' />
                                    <attribute name='createdon' />
                                    <attribute name='address1_line1' />
                                    <attribute name='address1_city' />
                                    <attribute name='address1_stateorprovince' />
                                    <attribute name='address1_postalcode' />
                                    <attribute name='address1_country' />
                                    <attribute name='new_gender' />
                                    <attribute name='new_birthday' />
                                    <attribute name='leadsourcecode' />
                                    <attribute name='bsd_customercode' />
                                    <attribute name='bsd_customergroup' />
                                    <attribute name='bsd_typeofidcard' />
                                    <attribute name='bsd_identitycardnumberid' />
                                    <attribute name='bsd_area' />
                                    <attribute name='bsd_placeofissue' />
                                    <attribute name='bsd_dategrant' />
                                    <attribute name='bsd_registrationcode' />
                                    <attribute name='bsd_accountaddressvn' />
                                    <attribute name='bsd_permanentaddress1' />
                                    <attribute name='bsd_contactaddress' />
                                    <attribute name='bsd_qrcode' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                        <condition attribute='leadid' operator='eq' value='{" + leadid + @"}' />
                                    </filter>
                                    <link-entity name='transactioncurrency' from='transactioncurrencyid' to='transactioncurrencyid' visible='false' link-type='outer'>
                                        <attribute name='currencyname'  alias='transactioncurrencyid_label'/>
                                    </link-entity>
                                    <link-entity name='account' from='originatingleadid' to='leadid' link-type='outer'>
                                        <attribute name='accountid' alias='account_id'/>
                                    </link-entity>
                                    <link-entity name='contact' from='originatingleadid' to='leadid' link-type='outer'>
                                        <attribute name='contactid' alias='contact_id'/>
                                    </link-entity>
                                    " + filterEmployee + @"
                                </entity>
                            </fetch>";
            //$@"<link-entity name='campaign' from='campaignid' to='campaignid' visible='false' link-type='outer'>
            //                            <attribute name='name'  alias='campaignid_label'/>
            //                        </link-entity>"
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch);
            if (result == null)
            {
                return;
            }
            var data = result.value.FirstOrDefault();
            singleLead = data;
            if (!string.IsNullOrWhiteSpace(singleLead.new_gender))
                singleGender = ContactGender.GetGenderById(singleLead.new_gender);
            if (!string.IsNullOrWhiteSpace(singleLead.leadsourcecode))
                LeadSource = LeadSourcesData.GetLeadSourceById(singleLead.leadsourcecode);
            if (!string.IsNullOrWhiteSpace(singleLead.industrycode))
                singleIndustrycode = LeadIndustryCode.GetIndustryCodeById(singleLead.industrycode);
            if (!string.IsNullOrWhiteSpace(singleLead.bsd_customergroup)) 
                CustomerGroup = CustomerGroupData.GetCustomerGroupById(singleLead.bsd_customergroup);
            if (!string.IsNullOrWhiteSpace(singleLead.bsd_area))
                Area = AreaData.GetAreaById(singleLead.bsd_area);
            if (!string.IsNullOrWhiteSpace(singleLead.bsd_typeofidcard))
                TypeIdCard = TypeIdCardData.GetTypeIdCardById(singleLead.bsd_typeofidcard);
            if (singleLead.bsd_dategrant.HasValue)
                singleLead.bsd_dategrant = data.bsd_dategrant.Value.ToLocalTime();
            if (singleLead.new_birthday.HasValue)
                singleLead.new_birthday = data.new_birthday.Value.ToLocalTime();
        }

        public async Task<CrmApiResponse> Qualify(Guid id)
        {
            string path = "/leads(" + id + ")//Microsoft.Dynamics.CRM.bsd_Action_Lead_QualifyLead";
            var content = new { };
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            return result;
        }

        public async Task<bool> UpdateStatusCodeLead()
        {
            string path = "/leads(" + this.singleLead.leadid + ")";
            var content = await GetContentUpdateStatusCode();
            CrmApiResponse apiResponse = await CrmHelper.PatchData(path, content);
            if (apiResponse.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<object> GetContentUpdateStatusCode()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["statuscode"] = this.LeadStatusCode.ToString();
            data["statecode"] = this.LeadStateCode.ToString();
            return data;
        }
        public void LoadPhongThuy()
        {
            PhongThuy = new PhongThuyModel();
            if (!string.IsNullOrWhiteSpace(singleLead.new_gender))
                singleGender = ContactGender.GetGenderById(singleLead.new_gender);
            if (list_HuongTot != null || list_HuongXau != null)
            {
                list_HuongTot.Clear();
                list_HuongXau.Clear();
                if (singleLead != null && singleLead.new_gender != null && singleGender != null && singleGender.Val != null)
                {
                    PhongThuy.gioi_tinh = Int32.Parse(singleLead.new_gender);
                    PhongThuy.nam_sinh = singleLead.new_birthday.HasValue ? singleLead.new_birthday.Value.Year : 0;
                    if (PhongThuy.huong_tot != null && PhongThuy.huong_tot != null)
                    {
                        string[] huongtot = PhongThuy.huong_tot.Split('\n');
                        string[] huongxau = PhongThuy.huong_xau.Split('\n');
                        int i = 1;
                        foreach (var x in huongtot)
                        {
                            string[] huong = x.Split(':');
                            string name_huong = i + ". " + huong[0];
                            string detail_huong = huong[1].Remove(0, 1);
                            list_HuongTot.Add(new HuongPhongThuy { Name = name_huong, Detail = detail_huong });
                            i++;
                        }
                        int j = 1;
                        foreach (var x in huongxau)
                        {
                            string[] huong = x.Split(':');
                            string name_huong = j + ". " + huong[0];
                            string detail_huong = huong[1].Remove(0, 1);
                            list_HuongXau.Add(new HuongPhongThuy { Name = name_huong, Detail = detail_huong });
                            j++;
                        }
                    }
                }
                else
                {
                    PhongThuy.gioi_tinh = 0;
                    PhongThuy.nam_sinh = 0;
                }
            }
        }
        private async Task<object> getContent()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = "1";
            return data;
        }
        // check id
        public async Task<bool> CheckID(string identitycardnumber)
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='fullname' alias='Label'/>
                                <filter type='and'>
                                  <condition attribute='bsd_identitycardnumber' operator='eq' value='{identitycardnumber}' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("contacts", fetch);
            if (result != null && result.value.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task LoadCaseForLead()
        {
            if (list_customercare == null)
                list_customercare = new ObservableCollection<HoatDongListModel>();

            if (list_customercare != null && singleLead != null && singleLead.leadid != Guid.Empty)
            {
                await Task.WhenAll(
                    LoadActiviy(singleLead.leadid, "task", "tasks"),
                    LoadActiviy(singleLead.leadid, "phonecall", "phonecalls"),
                    LoadActiviy(singleLead.leadid, "appointment", "appointments")
                );
            }
            ShowMoreCase = list_customercare.Count < (3 * PageCase) ? false : true;
        }
        public async Task LoadActiviy(Guid leadID, string entity, string entitys)
        {
            string forphonecall = null;
            if (entity == "phonecall")
            {
                forphonecall = @"<link-entity name='activityparty' from='activityid' to='activityid' link-type='outer' alias='aee'>
                                        <filter type='and'>
                                            <condition attribute='participationtypemask' operator='eq' value='2' />
                                        </filter>
                                        <link-entity name='contact' from='contactid' to='partyid' link-type='outer' alias='aff'>
                                            <attribute name='fullname' alias='callto_contact_name'/>
                                        </link-entity>
                                        <link-entity name='account' from='accountid' to='partyid' link-type='outer' alias='agg'>
                                            <attribute name='bsd_name' alias='callto_account_name'/>
                                        </link-entity>
                                        <link-entity name='lead' from='leadid' to='partyid' link-type='outer' alias='ahh'>
                                            <attribute name='fullname' alias='callto_lead_name'/>
                                        </link-entity>
                                    </link-entity>";
            }

            string fetch = $@"<fetch version='1.0' count='3' page='{PageCase}' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='{entity}'>
                                    <attribute name='subject' />
                                    <attribute name='statecode' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' /> 
                                    <attribute name='activitytypecode' />
                                    <order attribute='modifiedon' descending='true' />
                                    <filter type='and'>
                                        <condition attribute='regardingobjectid' operator='eq' value='{leadID}' />
                                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                    <link-entity name='activityparty' from='activityid' to='activityid' link-type='inner' alias='party'/>
                                    <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer' alias='ae'>
                                        <attribute name='bsd_name' alias='accounts_bsd_name'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer' alias='af'>
                                        <attribute name='fullname' alias='contact_bsd_fullname'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='ag'>
                                        <attribute name='fullname' alias='lead_fullname'/>
                                    </link-entity>
                                    {forphonecall}
                                </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<HoatDongListModel>>(entitys, fetch);
            if (result == null || result.value.Count == 0) return;
            var data = result.value;
            if (entity == "appointment")
            {
                foreach (var item in data)
                {
                    item.customer = await MeetCustomerHelper.MeetCustomer(item.activityid);
                    list_customercare.Add(item);
                }
            }
            else
            {
                foreach (var item in data)
                {
                    item.customer = item.regarding_name;
                    list_customercare.Add(item);
                }
            }
        }

        // Save qrcode
        public async Task<bool> SaveQRCode(string qrCode)
        {
            string path = "/leads(" + this.singleLead.leadid + ")";
            object content = new
            {
                bsd_qrcode = qrCode,
            };

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            if (result.IsSuccess)
                return true;
            else
                return false;
        }
        public async Task<LeadFormModel> LoadStatusLead()
        {
            if (singleLead == null || singleLead.leadid == Guid.Empty) return null;
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='lead'>
                                    <attribute name='statuscode' />
                                    <attribute name='statecode' />
                                    <attribute name='bsd_qrcode' />
                                    <attribute name='leadqualitycode' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                        <condition attribute='leadid' operator='eq' value='{singleLead.leadid}' />
                                    </filter>
                                    <link-entity name='account' from='originatingleadid' to='leadid' link-type='outer'>
                                        <attribute name='accountid' alias='account_id'/>
                                    </link-entity>
                                    <link-entity name='contact' from='originatingleadid' to='leadid' link-type='outer'>
                                        <attribute name='contactid' alias='contact_id'/>
                                    </link-entity>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch);
            if (result == null || result.value.Count == 0)
                return null;
            return result.value.FirstOrDefault();
        }
    }
}
