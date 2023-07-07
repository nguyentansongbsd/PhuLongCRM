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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Windows.Input;

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

        private ObservableCollection<ActivityListModel> cares;
        public ObservableCollection<ActivityListModel> Cares { get => cares; set { cares = value; OnPropertyChanged(nameof(Cares)); } }

        private bool _showMoreCase;
        public bool ShowMoreCase { get => _showMoreCase; set { _showMoreCase = value; OnPropertyChanged(nameof(ShowMoreCase)); } }
        public int PageCase { get; set; } = 1;

        public bool IsFromQRCode { get; set; }
        public bool IsCurrentRecordOfUser { get; set; }
        public string Duplicate { get; set; }

        private List<OptionSet> _provinces;
        public List<OptionSet> Provinces { get => _provinces; set { _provinces = value; OnPropertyChanged(nameof(Provinces)); } }

        private List<OptionSet> ProvincesForDetele { get; set; } = new List<OptionSet>();

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }       

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
                                    <attribute name='bsd_hasguardian' />
                                    <attribute name='bsd_employee' alias='employee_id'/>
                                    <attribute name='ownerid' alias='owner_id'/>
                                    <attribute name='bsd_quantam_nhapho' />
                                    <attribute name='bsd_quantam_khuthuongmai' />
                                    <attribute name='bsd_quantam_datnen' />
                                    <attribute name='bsd_quantam_canho' />
                                    <attribute name='bsd_quantam_bietthu' />
                                    <attribute name='bsd_tieuchi_vitri' />
                                    <attribute name='bsd_tieuchi_thietkenoithatcanho' />
                                    <attribute name='bsd_tieuchi_tangcanhodep' />
                                    <attribute name='bsd_tieuchi_phuongthucthanhtoan' />
                                    <attribute name='bsd_tieuchi_nhieutienich' />
                                    <attribute name='bsd_tieuchi_nhadautuuytin' />
                                    <attribute name='bsd_tieuchi_moitruongsong' />
                                    <attribute name='bsd_tieuchi_huongcanho' />
                                    <attribute name='bsd_tieuchi_hethongcuuhoa' />
                                    <attribute name='bsd_tieuchi_hethonganninh' />
                                    <attribute name='bsd_tieuchi_giacanho' />
                                    <attribute name='bsd_tieuchi_gantruonghoc' />
                                    <attribute name='bsd_tieuchi_ganchosieuthi' />
                                    <attribute name='bsd_tieuchi_ganbenhvien' />
                                    <attribute name='bsd_tieuchi_dientichcanho' />
                                    <attribute name='bsd_tieuchi_baidauxe' />
                                    <attribute name='bsd_dientich_lonhon120m2' />
                                    <attribute name='bsd_dientich_80100m2' />
                                    <attribute name='bsd_dientich_6080m2' />
                                    <attribute name='bsd_dientich_3060m2' />
                                    <attribute name='bsd_dientich_100120m2' />
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
                                <link-entity name='bsd_country' from='bsd_countryid' to='bsd_country' link-type='outer' alias='ad'>
                                    <attribute name='bsd_countryid' alias='bsd_country_id' />
                                </link-entity>
                                <link-entity name='new_district' from='new_districtid' to='bsd_district' link-type='outer' alias='ae'>
                                    <attribute name='new_districtid' alias='bsd_district_id' />
                                </link-entity>
                                <link-entity name='new_province' from='new_provinceid' to='bsd_province' link-type='outer' alias='af'>
                                    <attribute name='new_provinceid' alias='bsd_province_id' />
                                </link-entity>
                                <link-entity name='bsd_country' from='bsd_countryid' to='bsd_permanentcountry' link-type='outer' alias='ag'>
                                    <attribute name='bsd_countryid' alias='bsd_permanentcountry_id' />
                                </link-entity>
                                <link-entity name='new_district' from='new_districtid' to='bsd_permanentdistrict' link-type='outer' alias='ah'>
                                    <attribute name='new_districtid' alias='bsd_permanentdistrict_id' />
                                </link-entity>
                                <link-entity name='new_province' from='new_provinceid' to='bsd_permanentprovince' link-type='outer' alias='ai'>
                                    <attribute name='new_provinceid' alias='bsd_permanentprovince_id' />
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='bsd_guardian' link-type='outer'>
                                    <attribute name='contactid' alias='guardian_id' />
                                    <attribute name='bsd_fullname' alias='guardian_name' />
                                    <attribute name='birthdate' alias='guardian_birthdate' />
                                </link-entity>
                                    " + filterEmployee + @"
                                </entity>
                            </fetch>";
            //$@"<link-entity name='campaign' from='campaignid' to='campaignid' visible='false' link-type='outer'>
            //                            <attribute name='name'  alias='campaignid_label'/>
            //                        </link-entity>"
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch);
            if (result == null || result.value.Any() == false) return;

            var data = result.value.FirstOrDefault();
            singleLead = data;
            if (!string.IsNullOrWhiteSpace(singleLead.new_gender))
                singleGender = ContactGender.GetGenderById(singleLead.new_gender);
            else
                singleGender = new OptionSet();

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
            this.IsCurrentRecordOfUser = (singleLead.owner_id == UserLogged.Id || singleLead.employee_id == UserLogged.Id) ? true : false;
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
                                    <filter type='or'>
                                        <condition attribute='bsd_identitycardnumber' operator='eq' value='{identitycardnumber}' />
                                        <condition attribute='bsd_identitycard' operator='eq' value='{identitycardnumber}' />
                                        <condition attribute='bsd_passport' operator='eq' value='{identitycardnumber}' />
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
        public async Task<bool> CheckGPKD(string bsd_registrationcode)
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='account'>
                                <attribute name='fullname' alias='Label'/>
                                    <filter type='and'>
                                        <condition attribute='bsd_registrationcode' operator='eq' value='{bsd_registrationcode}' />
                                    </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("accounts", fetch);
            if (result != null && result.value.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
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
        public async Task LoadCase()
        {
            if (Cares == null)
                Cares = new ObservableCollection<ActivityListModel>();
            if (singleLead == null || singleLead.leadid == Guid.Empty) return;

            string attribute = string.Empty;
            string value = string.Empty;
            if (singleLead.employee_id != Guid.Empty)
            {
                attribute = "bsd_employee";
                value = singleLead.employee_id.ToString();
            }
            else
            {
                attribute = "ownerid";
                value = singleLead.owner_id.ToString();
            }
            string fetch = $@"<fetch version='1.0' count='5' page='{PageCase}' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='activitypointer'>
                                    <attribute name='subject' />
                                    <attribute name='statecode' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' /> 
                                    <attribute name='activitytypecode' />
                                    <filter type='and'>
                                        <condition attribute='activitytypecode' operator='in'>
                                            <value>4212</value>
                                            <value>4210</value>
                                            <value>4201</value>
                                        </condition>
	                                    <filter type='or'>
                                            <condition entityname='meet' attribute='{attribute}' operator='eq' value='{value}' />
                                            <condition entityname='task' attribute='{attribute}' operator='eq' value='{value}' />
                                            <condition entityname='phonecall' attribute='{attribute}' operator='eq' value='{value}' />
                                        </filter>
                                        <condition attribute='regardingobjectid' operator='eq' value='{singleLead.leadid}' />
                                    </filter>
                                    <link-entity name='appointment' from='activityid' to='activityid' alias='meet' link-type='outer'>
                                        <attribute name='requiredattendees' />
                                    </link-entity>
                                    <link-entity name='task' from='activityid' to='activityid' alias='task' link-type='outer'>
                                        <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer'>
                                            <attribute name='bsd_name' alias='task_account_name'/>
                                        </link-entity>
                                        <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer'>
                                            <attribute name='fullname' alias='task_contact_name'/>
                                        </link-entity>
                                        <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='ag'>
                                            <attribute name='fullname' alias='task_lead_name'/>
                                        </link-entity>
                                    </link-entity>
                                    <link-entity name='phonecall' from='activityid' to='activityid' alias='phonecall' link-type='outer'>
                                        <link-entity name='activityparty' from='activityid' to='activityid' link-type='outer'>
                                            <filter type='and'>
                                                <condition attribute='participationtypemask' operator='eq' value='2' />
                                            </filter>
                                            <link-entity name='contact' from='contactid' to='partyid' link-type='outer'>
                                                <attribute name='fullname' alias='phonecall_contact_name'/>
                                            </link-entity>
                                            <link-entity name='account' from='accountid' to='partyid' link-type='outer'>
                                                <attribute name='bsd_name' alias='phonecall_account_name'/>
                                            </link-entity>
                                            <link-entity name='lead' from='leadid' to='partyid' link-type='outer'>
                                                <attribute name='fullname' alias='phonecall_lead_name'/>
                                            </link-entity>
                                        </link-entity>
                                    </link-entity>
                                </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ActivityListModel>>("activitypointers", fetch);
            if (result != null && result.value.Count > 0)
            {
                foreach (var item in result.value)
                {
                    if (item.activitytypecode == "appointment")
                        item.customer = await MeetCustomerHelper.MeetCustomer(item.activityid);
                    else if (item.activitytypecode == "task")
                        item.customer = item.task__customer;
                    else if (item.activitytypecode == "phonecall")
                        item.customer = item.phonecall_customer;
                    Cares.Add(item);
                }
            }
            ShowMoreCase = Cares.Count < (5 * PageCase) ? false : true;
        }
        public async Task LoadDuplicate()
        {
            if (singleLead != null && singleLead.statuscode != "3")
            {
                string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='lead'>
                            <attribute name='mobilephone'/>
                            <attribute name='emailaddress1'/>
                            <attribute name='bsd_identitycardnumberid'/>
                            <order attribute='createdon' descending='true' />
                            <filter type='or'>
                                <filter type='and'>
                                    <filter type='or'>
                                        <condition attribute='mobilephone' operator='eq' value='{singleLead.mobilephone}' />
                                        <condition attribute='mobilephone' operator='eq' value='{singleLead.mobilephone_format}' />
                                    </filter>
                                    <condition attribute='leadid' operator='ne' value='{singleLead.leadid}'/>
                                </filter>
                                <filter type='and'>
                                    <condition attribute='emailaddress1' operator='eq' value='{singleLead.emailaddress1}' />
                                    <condition attribute='leadid' operator='ne' value='{singleLead.leadid}'/>
                                </filter>
                                <filter type='and'>
                                    <condition attribute='bsd_identitycardnumberid' operator='eq' value='{singleLead.bsd_identitycardnumberid}' />
                                    <condition attribute='leadid' operator='ne' value='{singleLead.leadid}'/>
                                </filter>
                            </filter>
                          </entity>
                        </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch);
                if (result != null && result.value.Count > 0)
                {
                    List<string> duplicates = new List<string>();
                    var data = result.value.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(data.mobilephone) && data.mobilephone == singleLead.mobilephone)
                        duplicates.Add(Language.so_dien_thoai);
                    if (!string.IsNullOrWhiteSpace(data.emailaddress1) && data.emailaddress1 == singleLead.emailaddress1)
                        duplicates.Add(Language.email);
                    if (!string.IsNullOrWhiteSpace(data.bsd_identitycardnumberid) && data.bsd_identitycardnumberid == singleLead.bsd_identitycardnumberid)
                        duplicates.Add(Language.so_id);
                    Duplicate = string.Join(", ", duplicates);
                    if (UserLogged.Language == "en")
                        Duplicate += " already exists.";
                    else
                        Duplicate += " đã tồn tại.";
                }
                if (string.IsNullOrWhiteSpace(Duplicate))
                {
                    string fetchcontact = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                        <entity name='contact'>
                                            <attribute name='fullname' />
                                             <attribute name='emailaddress1' />
                                             <attribute name='mobilephone' />
                                             <attribute name='bsd_identitycard' />
                                             <attribute name='bsd_identitycardnumber' />
                                             <attribute name='bsd_passport' />
                                            <filter type='or'>
                                                <condition attribute='emailaddress1' operator='eq' value='{singleLead.emailaddress1}' />
                                                <condition attribute='mobilephone' operator='eq' value='{singleLead.mobilephone}' />
                                                <condition attribute='mobilephone' operator='eq' value='{singleLead.mobilephone_format}' />
                                                <condition attribute='bsd_identitycard' operator='eq' value='{singleLead.bsd_identitycardnumberid}' />
                                                <condition attribute='bsd_identitycardnumber' operator='eq' value='{singleLead.bsd_identitycardnumberid}' />
                                                <condition attribute='bsd_passport' operator='eq' value='{singleLead.bsd_identitycardnumberid}' />
                                            </filter>
                                        </entity>
                                    </fetch>";
                    var resultcontact = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetchcontact);
                    if (resultcontact != null && resultcontact.value.Count > 0)
                    {
                        List<string> duplicates = new List<string>();
                        var data = resultcontact.value.FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(data.mobilephone) && data.mobilephone == singleLead.mobilephone)
                            duplicates.Add(Language.so_dien_thoai);
                        if (!string.IsNullOrWhiteSpace(data.emailaddress1) && data.emailaddress1 == singleLead.emailaddress1)
                            duplicates.Add(Language.email);
                        if (!string.IsNullOrWhiteSpace(data.bsd_identitycard) && data.bsd_identitycard == singleLead.bsd_identitycardnumberid || 
                            !string.IsNullOrWhiteSpace(data.bsd_identitycardnumber) && data.bsd_identitycardnumber == singleLead.bsd_identitycardnumberid ||
                            !string.IsNullOrWhiteSpace(data.bsd_passport) && data.bsd_passport == singleLead.bsd_identitycardnumberid)
                            duplicates.Add(Language.so_id);
                        Duplicate = string.Join(", ", duplicates);
                        if (UserLogged.Language == "en")
                            Duplicate += " already exists.";
                        else
                            Duplicate += " đã tồn tại.";
                    }
                }
            }
        }
        public async Task<CrmApiResponse> updateNhuCauDienTich()
        {
            string path = "/leads(" + singleLead.leadid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_dientich_3060m2"] = singleLead.bsd_dientich_3060m2;
            content["bsd_dientich_6080m2"] = singleLead.bsd_dientich_6080m2;
            content["bsd_dientich_80100m2"] = singleLead.bsd_dientich_80100m2;
            content["bsd_dientich_100120m2"] = singleLead.bsd_dientich_100120m2;
            content["bsd_dientich_lonhon120m2"] = singleLead.bsd_dientich_lonhon120m2;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task<CrmApiResponse> updateTieuChiChonMua()
        {
            string path = "/leads(" + singleLead.leadid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_tieuchi_vitri"] = singleLead.bsd_tieuchi_vitri;
            content["bsd_tieuchi_phuongthucthanhtoan"] = singleLead.bsd_tieuchi_phuongthucthanhtoan;
            content["bsd_tieuchi_giacanho"] = singleLead.bsd_tieuchi_giacanho;
            content["bsd_tieuchi_nhadautuuytin"] = singleLead.bsd_tieuchi_nhadautuuytin;
            content["bsd_tieuchi_moitruongsong"] = singleLead.bsd_tieuchi_moitruongsong;
            content["bsd_tieuchi_baidauxe"] = singleLead.bsd_tieuchi_baidauxe;
            content["bsd_tieuchi_hethonganninh"] = singleLead.bsd_tieuchi_hethonganninh;
            content["bsd_tieuchi_huongcanho"] = singleLead.bsd_tieuchi_huongcanho;
            content["bsd_tieuchi_hethongcuuhoa"] = singleLead.bsd_tieuchi_hethongcuuhoa;
            content["bsd_tieuchi_nhieutienich"] = singleLead.bsd_tieuchi_nhieutienich;
            content["bsd_tieuchi_ganchosieuthi"] = singleLead.bsd_tieuchi_ganchosieuthi;
            content["bsd_tieuchi_gantruonghoc"] = singleLead.bsd_tieuchi_gantruonghoc;
            content["bsd_tieuchi_ganbenhvien"] = singleLead.bsd_tieuchi_ganbenhvien;
            content["bsd_tieuchi_dientichcanho"] = singleLead.bsd_tieuchi_dientichcanho;
            content["bsd_tieuchi_thietkenoithatcanho"] = singleLead.bsd_tieuchi_thietkenoithatcanho;
            content["bsd_tieuchi_tangcanhodep"] = singleLead.bsd_tieuchi_tangcanhodep;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task<CrmApiResponse> updateLoaiBDSQuanTam()
        {
            string path = "/leads(" + singleLead.leadid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_quantam_datnen"] = singleLead.bsd_quantam_datnen;
            content["bsd_quantam_canho"] = singleLead.bsd_quantam_canho;
            content["bsd_quantam_bietthu"] = singleLead.bsd_quantam_bietthu;
            content["bsd_quantam_khuthuongmai"] = singleLead.bsd_quantam_khuthuongmai;
            content["bsd_quantam_nhapho"] = singleLead.bsd_quantam_nhapho;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task LoadProvince()
        {
            if (Provinces == null)
                Provinces = new List<OptionSet>();
            if (singleLead == null || singleLead.leadid == Guid.Empty) return;

            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_province'>
                                <attribute name='new_name' alias='Label' />   
                                <attribute name='new_provinceid' alias='Val' />   
                                <link-entity name='bsd_lead_new_province' from='new_provinceid' to='new_provinceid' intersect='true'>
                                  <filter>
                                    <condition attribute='leadid' operator='eq' value='{singleLead.leadid}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("new_provinces", fetch);
            if (result != null && result.value.Count > 0)
            {
                List<OptionSet> list = new List<OptionSet>();
                foreach (var item in result.value)
                {
                    list.Add(item);
                    ProvincesForDetele.Add(item);
                }
                Provinces = list;
            }
        }
        public async Task<bool> updateNhuCauDiaDiem()
        {
            bool res = true;
            if (Provinces != null)
            {
                foreach (var item in Provinces)
                {
                    string path = $"/leads({singleLead.leadid})/bsd_lead_new_province/$ref";
                    IDictionary<string, object> content = new Dictionary<string, object>();
                    content["@odata.id"] = $"{OrgConfig.ApiUrl}/new_provinces(" + item.Val + ")";
                    CrmApiResponse result = await CrmHelper.PostData(path, content);
                    if (!result.IsSuccess)
                        res = false;
                    ProvincesForDetele.Remove(item);
                }
                if (ProvincesForDetele.Count > 0)
                {
                    foreach (var item in ProvincesForDetele)
                    {
                        var res_delete = await Delete_NhuCau(item.Val, "bsd_lead_new_province");
                        if (!res_delete)
                            res = false;
                    }
                }
            }
            return res;
        }
        public async Task<Boolean> Delete_NhuCau(string id, string entity)
        {
            string Token = UserLogged.AccessToken;
            var request = $"{OrgConfig.ApiUrl}/leads({singleLead.leadid})/{entity}(" + id + ")/$ref";

            using (HttpClientHandler ClientHandler = new HttpClientHandler())
            using (HttpClient Client = new HttpClient(ClientHandler))
            {
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                using (HttpRequestMessage RequestMessage = new HttpRequestMessage(new HttpMethod("DELETE"), request))
                {
                    using (HttpResponseMessage ResponseMessage = await Client.SendAsync(RequestMessage))
                    {
                        string result = await ResponseMessage.Content.ReadAsStringAsync();

                        if (ResponseMessage.StatusCode == HttpStatusCode.NoContent)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
        //public async Task LoadProject()
        //{
        //    if (Projects == null)
        //        Projects = new List<OptionSet>();
        //    if (singleLead == null || singleLead.contactid == Guid.Empty) return;

        //    string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
        //                            <entity name='bsd_project'>
        //                            <attribute name='bsd_projectid' alias='Val'/>
        //                            <attribute name='bsd_name' alias='Label'/>
        //                            <link-entity name='bsd_contact_bsd_project' from='bsd_projectid' to='bsd_projectid' intersect='true'>
        //                              <filter>
        //                                <condition attribute='contactid' operator='eq' value='{singleContact.contactid}' />
        //                              </filter>
        //                            </link-entity>
        //                            </entity>
        //                        </fetch>";

        //    var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_projects", fetch);
        //    if (result != null && result.value.Count > 0)
        //    {
        //        List<OptionSet> list = new List<OptionSet>();
        //        foreach (var item in result.value)
        //        {
        //            list.Add(item);
        //            ProjectsForDetele.Add(item);
        //        }
        //        Projects = list;
        //    }
        //}
        //public async Task<bool> updateNhuCauDuAn()
        //{
        //    bool res = true;
        //    if (Projects != null && Projects.Count > 0)
        //    {
        //        foreach (var item in Projects)
        //        {
        //            string path = $"/contacts({singleContact.contactid})/bsd_contact_bsd_project/$ref";
        //            IDictionary<string, object> content = new Dictionary<string, object>();
        //            content["@odata.id"] = $"{OrgConfig.ApiUrl}/bsd_projects(" + item.Val + ")";
        //            CrmApiResponse result = await CrmHelper.PostData(path, content);
        //            if (!result.IsSuccess)
        //                res = false;
        //            ProjectsForDetele.Remove(item);
        //        }
        //        if (ProjectsForDetele.Count > 0)
        //        {
        //            foreach (var item in ProjectsForDetele)
        //            {
        //                var res_delete = await Delete_NhuCau(item.Val, "bsd_contact_bsd_project");
        //                if (!res_delete)
        //                    res = false;
        //            }
        //        }
        //    }
        //    return res;
        //}
    }
}
