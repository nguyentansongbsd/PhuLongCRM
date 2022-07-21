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
        public ObservableCollection<OptionSet> list_gender_optionset { get; set; }
        public ObservableCollection<OptionSet> list_industrycode_optionset { get; set; }
        public ObservableCollection<HuongPhongThuy> list_HuongTot { set; get; }

        public ObservableCollection<HuongPhongThuy> list_HuongXau { set; get; }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(nameof(Address)); } }

        public bool IsSuccessContact { get; set; } = false;
        public bool IsSuccessAccount { get; set; } = false;

        private LookUp Country = new LookUp();
        private LookUp Province = new LookUp();
        private LookUp District = new LookUp();

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
            singleGender = new OptionSet();
            singleIndustrycode = new OptionSet();

            list_gender_optionset = new ObservableCollection<OptionSet>();
            list_industrycode_optionset = new ObservableCollection<OptionSet>();

            list_HuongTot = new ObservableCollection<HuongPhongThuy>();
            list_HuongXau = new ObservableCollection<HuongPhongThuy>();

            this.loadGender();
            this.loadIndustrycode();
        }

        public async Task LoadOneLead(String leadid)
        {
            string filterEmployee = string.Empty;
            if (IsFromQRCode == false)
            {
                filterEmployee = $@"<filter type='and'>
                                          <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{ UserLogged.Id}' />
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
            this.singleLead = data;
            this.singleGender = list_gender_optionset.SingleOrDefault(x => x.Val == this.singleLead.new_gender);
            this.LeadSource = LeadSourcesData.GetLeadSourceById(this.singleLead.leadsourcecode);
            if (singleLead.bsd_dategrant.HasValue)
                singleLead.bsd_dategrant = data.bsd_dategrant.Value.ToLocalTime();
            if (singleLead.new_birthday.HasValue)
                singleLead.new_birthday = data.new_birthday.Value.ToLocalTime();
            LoadAddress();
            await LoadCountryByName();
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

        public async Task CreateContact()
        {
            string path = "/contacts";
            singleLead.contactid = Guid.NewGuid();
            var content = await this.getContentContact();
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            if (result.IsSuccess)
            {
                IsSuccessContact = true;
                await CreateAccount();
            }
            else
            {
                IsSuccessContact = false;
            }
        }
        public async Task CreateAccount()
        {
            if (!string.IsNullOrWhiteSpace(singleLead.companyname))
            {
                string path = "/accounts";
                var content = await getContentAccount();
                CrmApiResponse result = await CrmHelper.PostData(path, content);
                if (result.IsSuccess)
                {
                    IsSuccessAccount = true;
                }
                else
                {
                    IsSuccessAccount = false;
                }
            }
        }

        public void loadGender()
        {
            list_gender_optionset.Add(new OptionSet() { Val = ("1"), Label = Language.nam, });
            list_gender_optionset.Add(new OptionSet() { Val = ("2"), Label = Language.nu, });
            list_gender_optionset.Add(new OptionSet() { Val = ("100000000"), Label = Language.khac, });
        }

        public async Task<OptionSet> loadOneGender(string id)
        {
            this.singleGender = list_gender_optionset.FirstOrDefault(x => x.Val == id); ;
            return singleGender;
        }

        //////// INDUSTRYCODE OPTIONSET AREA
        /// ////
        public void loadIndustrycode()
        {
            list_industrycode_optionset.Add(new OptionSet() { Val = ("1"), Label = Language.lead_1_industry, });
            //Accounting
            list_industrycode_optionset.Add(new OptionSet() { Val = ("2"), Label = Language.lead_2_industry, });
            //Agriculture and Non-petrol natural resource extraction
            list_industrycode_optionset.Add(new OptionSet() { Val = ("3"), Label = Language.lead_3_industry, });
            //Broadcasting printing and Publishing
            list_industrycode_optionset.Add(new OptionSet() { Val = ("4"), Label = Language.lead_4_industry, });
            //Brokers
            list_industrycode_optionset.Add(new OptionSet() { Val = ("5"), Label = Language.lead_5_industry, });
            //Building supply retail
            list_industrycode_optionset.Add(new OptionSet() { Val = ("6"), Label = Language.lead_6_industry, });
            //Business services
            list_industrycode_optionset.Add(new OptionSet() { Val = ("7"), Label = Language.lead_7_industry, });
            //Consulting
            list_industrycode_optionset.Add(new OptionSet() { Val = ("8"), Label = Language.lead_8_industry, });
            //Consumer services
            list_industrycode_optionset.Add(new OptionSet() { Val = ("9"), Label = Language.lead_9_industry, });
            //Design, direction and creative management
            list_industrycode_optionset.Add(new OptionSet() { Val = ("10"), Label = Language.lead_10_industry, });
            //Distributors, dispatchers and processors
            list_industrycode_optionset.Add(new OptionSet() { Val = ("11"), Label = Language.lead_11_industry, });
            //Doctor's offices and clinics
            list_industrycode_optionset.Add(new OptionSet() { Val = ("12"), Label = Language.lead_12_industry, });
            //Durable manufacturing
            list_industrycode_optionset.Add(new OptionSet() { Val = ("13"), Label = Language.lead_13_industry, });
            //Eating and drinking places
            list_industrycode_optionset.Add(new OptionSet() { Val = ("14"), Label = Language.lead_14_industry, });
            //Entertainment retail
            list_industrycode_optionset.Add(new OptionSet() { Val = ("15"), Label = Language.lead_15_industry, });
            //Equipment rental and leasing
            list_industrycode_optionset.Add(new OptionSet() { Val = ("16"), Label = Language.lead_16_industry, });
            //Financial
            list_industrycode_optionset.Add(new OptionSet() { Val = ("17"), Label = Language.lead_17_industry, });
            //Food and tobacco processing
            list_industrycode_optionset.Add(new OptionSet() { Val = ("18"), Label = Language.lead_18_industry, });
            //Inbound capital intensive processing
            list_industrycode_optionset.Add(new OptionSet() { Val = ("19"), Label = Language.lead_19_industry, });
            //Inbound repair and services
            list_industrycode_optionset.Add(new OptionSet() { Val = ("20"), Label = Language.lead_20_industry, });
            //Insurance
            list_industrycode_optionset.Add(new OptionSet() { Val = ("21"), Label = Language.lead_21_industry, });
            //Legal services
            list_industrycode_optionset.Add(new OptionSet() { Val = ("22"), Label = Language.lead_22_industry, });
            //Non-Durable merchandise retail
            list_industrycode_optionset.Add(new OptionSet() { Val = ("23"), Label = Language.lead_23_industry, });
            //Outbound consumer service
            list_industrycode_optionset.Add(new OptionSet() { Val = ("24"), Label = Language.lead_24_industry, });
            //Petrochemical extraction and distribution
            list_industrycode_optionset.Add(new OptionSet() { Val = ("25"), Label = Language.lead_25_industry, });
            //Service retail
            list_industrycode_optionset.Add(new OptionSet() { Val = ("26"), Label = Language.lead_26_industry, });
            //SIG affiliations
            list_industrycode_optionset.Add(new OptionSet() { Val = ("27"), Label = Language.lead_27_industry, });
            //Social services
            list_industrycode_optionset.Add(new OptionSet() { Val = ("28"), Label = Language.lead_28_industry, });
            //Special outbound trade contractors
            list_industrycode_optionset.Add(new OptionSet() { Val = ("29"), Label = Language.lead_29_industry, });
            //Specialty realty
            list_industrycode_optionset.Add(new OptionSet() { Val = ("30"), Label = Language.lead_30_industry, });
            //Transportation
            list_industrycode_optionset.Add(new OptionSet() { Val = ("31"), Label = Language.lead_31_industry, });
            //Utility creation and distribution
            list_industrycode_optionset.Add(new OptionSet() { Val = ("32"), Label = Language.lead_32_industry, });
            //Vehicle retail
            list_industrycode_optionset.Add(new OptionSet() { Val = ("33"), Label = Language.lead_33_industry, });
            //Wholesale
        }

        public async Task<OptionSet> loadOneIndustrycode(string id)
        {
            this.singleIndustrycode = list_industrycode_optionset.FirstOrDefault(x => x.Val == id); ;
            return singleIndustrycode;
        }

        public void LoadPhongThuy()
        {
            PhongThuy = new PhongThuyModel();
            _ = loadOneGender(singleLead.new_gender);
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
        private void LoadAddress()
        {
            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(singleLead.address1_line1))
            {
                address.Add(singleLead.address1_line1);
            }
            if (!string.IsNullOrWhiteSpace(singleLead.address1_city))
            {
                address.Add(singleLead.address1_city);
            }
            if (!string.IsNullOrWhiteSpace(singleLead.address1_stateorprovince))
            {
                address.Add(singleLead.address1_stateorprovince);
            }
            if (!string.IsNullOrWhiteSpace(singleLead.address1_postalcode))
            {
                address.Add(singleLead.address1_postalcode);
            }
            if (!string.IsNullOrWhiteSpace(singleLead.address1_country))
            {
                address.Add(singleLead.address1_country);
            }

            Address = string.Join(", ", address);
        }

        private async Task<object> getContentContact()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["contactid"] = singleLead.contactid;
            data["lastname"] = singleLead.lastname;
            data["bsd_fullname"] = singleLead.lastname;
            data["emailaddress1"] = singleLead.emailaddress1;
            data["mobilephone"] = singleLead.mobilephone;
            data["bsd_jobtitlevn"] = singleLead.jobtitle;
            data["telephone1"] = singleLead.telephone1;
            data["birthdate"] = singleLead.new_birthday.HasValue ? (DateTime.Parse(singleLead.new_birthday.ToString()).ToUniversalTime()).ToString("yyyy-MM-dd") : null;
            data["gendercode"] = singleLead.new_gender;

            if (UserLogged.Id != null && !UserLogged.IsLoginByUserCRM)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }

            return data;
        }

        private async Task<object> getContentAccount()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            Guid accountid = Guid.NewGuid();
            data["accountid"] = accountid;
            data["bsd_name"] = singleLead.companyname;
            data["websiteurl"] = singleLead.websiteurl;
            if (!string.IsNullOrWhiteSpace(singleLead.numberofemployees))
            {
                data["numberofemployees"] = int.Parse(singleLead.numberofemployees);
            }
            else
            {
                data["numberofemployees"] = null;
            }
            if (singleLead.contactid == Guid.Empty)
            {
                await DeletLookup("accounts", "primarycontactid", accountid);
            }
            else
            {
                data["primarycontactid@odata.bind"] = "/contacts(" + singleLead.contactid + ")"; /////Lookup Field
            }
            data["lastusedincampaign"] = singleLead.lastusedincampaign.HasValue ? (DateTime.Parse(singleLead.lastusedincampaign.ToString()).ToLocalTime()).ToString("yyyy-MM-dd\"T\"HH:mm:ss\"Z\"") : null;
            data["industrycode"] = singleLead.industrycode;
            data["revenue"] = singleLead?.revenue;
            data["donotsendmm"] = singleLead.donotsendmm.ToString();
            data["sic"] = singleLead.sic;
            data["description"] = singleLead.description;
            data["address1_composite"] = singleLead.address1_composite;

            data["bsd_housenumberstreet"] = singleLead.address1_line1;

            data["bsd_postalcode"] = singleLead.address1_postalcode;
            if (singleLead._transactioncurrencyid_value == null)
            {
                await DeletLookup("accounts", "transactioncurrencyid", accountid);
            }
            else
            {
                data["transactioncurrencyid@odata.bind"] = "/transactioncurrencies(" + singleLead._transactioncurrencyid_value + ")"; /////Lookup Field
            }

            if (singleLead.address1_country == null || Country.Id == Guid.Empty)
            {
                await DeletLookup("accounts", "bsd_nation", accountid);
            }
            else
            {
                data["bsd_nation@odata.bind"] = "/bsd_countries(" + Country.Id + ")"; /////Lookup Field
            }
            if (singleLead.address1_stateorprovince == null || Province.Id == Guid.Empty)
            {
                await DeletLookup("accounts", "bsd_province", accountid);
            }
            else
            {
                data["bsd_province@odata.bind"] = "/new_provinces(" + Province.Id + ")"; /////Lookup Field
            }
            if (singleLead.address1_city == null || District.Id == Guid.Empty)
            {
                await DeletLookup("accounts", "bsd_district", accountid);
            }
            else
            {
                data["bsd_district@odata.bind"] = "/new_districts(" + District.Id + ")"; /////Lookup Field
            }
            if (UserLogged.Id != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }
            return data;
        }

        public async Task<Boolean> DeletLookup(string Entity, string fieldName, Guid Id)
        {
            var result = await CrmHelper.SetNullLookupField(Entity, Id, fieldName);
            return result.IsSuccess;
        }

        private async Task<object> getContent()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = "1";
            return data;
        }

        public async Task LoadCountryByName()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_country'>
                                    <attribute name='bsd_countryname' alias='Name'/>
                                    <attribute name='bsd_countryid' alias='Id'/>
                                    <order attribute='bsd_countryname' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bsd_countryname' operator='eq' value='" + singleLead.address1_country + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("bsd_countries", fetch);
            if (result != null && result.value.Count > 0)
            {
                Country = result.value.FirstOrDefault();
                await LoadProvinceByName();
            }
        }
        public async Task LoadProvinceByName()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='new_province'>
                                    <attribute name='bsd_provincename' alias='Name'/>
                                    <attribute name='new_provinceid' alias='Id'/>
                                    <order attribute='bsd_provincename' descending='false' />
                                    <filter type='and'>
                                        <condition attribute='bsd_country' operator='eq' value='" + Country.Id + @"' />
                                        <condition attribute='bsd_provincename' operator='eq' value='" + singleLead.address1_stateorprovince + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_provinces", fetch);
            if (result != null && result.value.Count > 0)
            {
                this.Province = result.value.FirstOrDefault();
                await LoadDistrictByName();
            }
        }
        public async Task LoadDistrictByName()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_district'>
                                <attribute name='new_name' alias='Name'/>
                                <attribute name='new_districtid' alias='Id'/>
                                <attribute name='bsd_nameen' alias='Detail'/>
                                <order attribute='new_name' descending='false' />
                                <filter type='and'>
                                    <condition attribute='new_province' operator='eq' value='" + Province.Id + @"' />
                                    <condition attribute='new_name' operator='eq' value='" + singleLead.address1_city + @"' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_districts", fetch);
            if (result != null && result.value.Count > 0)
            {
                this.District = result.value.FirstOrDefault();
            }
        }
        // check id
        public async Task<bool> CheckID(string identitycardnumber)
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='fullname' />
                                <filter type='and'>
                                  <condition attribute='bsd_identitycardnumber' operator='eq' value='{identitycardnumber}' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetch);
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
                                        <filter type='or'>
                                            <condition entityname='party' attribute='partyid' operator='eq' value='{leadID}'/>
                                            <condition attribute='regardingobjectid' operator='eq' value='{leadID}' />
                                        </filter>
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
