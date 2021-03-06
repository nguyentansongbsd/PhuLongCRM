using System;
using PhuLongCRM.Models;
using PhuLongCRM.Helper;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using PhuLongCRM.Settings;
using PhuLongCRM.Resources;

namespace PhuLongCRM.ViewModels
{
    public class LeadFormViewModel : BaseViewModel
    {
        public Guid LeadId { get; set; }

        private LeadFormModel _singleLead;
        public LeadFormModel singleLead { get => _singleLead; set { _singleLead = value; OnPropertyChanged(nameof(singleLead)); } }

        public ObservableCollection<OptionSet> list_currency_lookup { get; set; } = new ObservableCollection<OptionSet>();

        private OptionSet _selectedCurrency;
        public OptionSet SelectedCurrency { get => _selectedCurrency; set { _selectedCurrency = value; OnPropertyChanged(nameof(SelectedCurrency)); } }

        public ObservableCollection<OptionSet> list_industrycode_optionset { get; set; } = new ObservableCollection<OptionSet>();

        private OptionSet _industryCode;
        public OptionSet IndustryCode { get => _industryCode; set { _industryCode = value; OnPropertyChanged(nameof(IndustryCode)); } }

        public ObservableCollection<OptionSet> list_campaign_lookup { get; set; } = new ObservableCollection<OptionSet>();

        private OptionSet _campaign;
        public OptionSet Campaign { get => _campaign; set { _campaign = value; OnPropertyChanged(nameof(Campaign)); } }

        private OptionSet _gender;
        public OptionSet Gender { get => _gender; set { _gender = value; OnPropertyChanged(nameof(Gender)); } }

        private List<OptionSet> _genders;
        public List<OptionSet> Genders { get => _genders; set { _genders = value; OnPropertyChanged(nameof(Genders)); } }

        private OptionSet _leadSource;
        public OptionSet LeadSource { get => _leadSource; set { _leadSource = value;OnPropertyChanged(nameof(LeadSource)); } }

        private List<OptionSet> _leadSources;
        public List<OptionSet> LeadSources { get => _leadSources; set { _leadSources = value; OnPropertyChanged(nameof(LeadSources)); } }

        private OptionSet _rating;
        public OptionSet Rating { get => _rating; set { _rating = value; OnPropertyChanged(nameof(Rating)); } }

        private List<OptionSet> _ratings;
        public List<OptionSet> Ratings { get => _ratings; set { _ratings = value; OnPropertyChanged(nameof(Ratings)); } }

        private OptionSet _topic;
        public OptionSet Topic { get => _topic; set { _topic = value; OnPropertyChanged(nameof(Topic)); } }

        private List<OptionSet> _topics;
        public List<OptionSet> Topics { get => _topics; set { _topics = value; OnPropertyChanged(nameof(Topics)); } }

        private OptionSet _customerGroup;
        public OptionSet CustomerGroup { get => _customerGroup; set { _customerGroup = value; OnPropertyChanged(nameof(CustomerGroup)); } }

        private List<OptionSet> _customerGroups;
        public List<OptionSet> CustomerGroups { get => _customerGroups; set { _customerGroups = value; OnPropertyChanged(nameof(CustomerGroups)); } }

        private OptionSet _typeIdCard;
        public OptionSet TypeIdCard { get => _typeIdCard; set { _typeIdCard = value; OnPropertyChanged(nameof(TypeIdCard)); } }

        private List<OptionSet> _typeIdCardss;
        public List<OptionSet> TypeIdCards { get => _typeIdCardss; set { _typeIdCardss = value; OnPropertyChanged(nameof(TypeIdCards)); } }

        private OptionSet _area;
        public OptionSet Area { get => _area; set { _area = value; OnPropertyChanged(nameof(Area)); } }

        private List<OptionSet> _areas;
        public List<OptionSet> Areas { get => _areas; set { _areas = value; OnPropertyChanged(nameof(Areas)); } }

        private bool _isShowbtnClearAddress;
        public bool IsShowbtnClearAddress { get => _isShowbtnClearAddress; set { _isShowbtnClearAddress = value; OnPropertyChanged(nameof(IsShowbtnClearAddress)); } }

        //IsShowbtnClearAddress

        //Address1 // địa chỉ liên lạc
        private AddressModel _address1;
        public AddressModel Address1 { get => _address1; set { _address1 = value; OnPropertyChanged(nameof(Address1)); } }
        //Address2 // địa chỉ thường trú
        private AddressModel _address2;
        public AddressModel Address2 { get => _address2; set { _address2 = value; OnPropertyChanged(nameof(Address2)); } }

        // địa chỉ công ty
        private AddressModel _address3;
        public AddressModel Address3 { get => _address3; set { _address3 = value; OnPropertyChanged(nameof(Address3)); } }

        private AddressModel _addressCopy;
        public AddressModel AddressCopy { get => _addressCopy; set { _addressCopy = value; OnPropertyChanged(nameof(AddressCopy)); } }
        public LeadFormViewModel()
        {
            singleLead = new LeadFormModel();
            list_industrycode_optionset = new ObservableCollection<OptionSet>();
            this.Genders = new List<OptionSet>() { new OptionSet("1",Language.nam), new OptionSet("2", Language.nu), new OptionSet("100000000",Language.khac) };
            this.loadIndustrycode();
        }

        public async Task LoadOneLead()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='lead'>
                            <attribute name='lastname' />
                            <attribute name='companyname' />
                            <attribute name='statuscode' />
                            <attribute name='mobilephone' />
                            <attribute name='telephone1' />
                            <attribute name='jobtitle' />
                            <attribute name='websiteurl' />
                            <attribute name='address1_composite' />
                            <attribute name='address1_line1' />
                            <attribute name='address1_city' />
                            <attribute name='address1_stateorprovince' />
                            <attribute name='address1_postalcode' />
                            <attribute name='address1_country' />
                            <attribute name='description' />
                            <attribute name='industrycode' />
                            <attribute name='revenue' />
                            <attribute name='numberofemployees' />
                            <attribute name='sic' />
                            <attribute name='donotsendmm' />
                            <attribute name='emailaddress1' />
                            <attribute name='createdon' />
                            <attribute name='leadid' />
                            <attribute name='leadqualitycode' />
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
                            <attribute name='bsd_account_housenumberstreetwardvn' />
                            <attribute name='bsd_accountaddressvn' />
                            <attribute name='bsd_permanentaddress' />
                            <attribute name='bsd_permanentaddress1' />
                            <attribute name='bsd_housenumberstreet' />
                            <attribute name='bsd_contactaddress' />
                            <attribute name='fullname' />
                            <order attribute='createdon' descending='true' />
                            <filter type='and'>
                                <condition attribute='leadid' operator='eq' value='{LeadId}' />
                            </filter>
                            <link-entity name='transactioncurrency' from='transactioncurrencyid' to='transactioncurrencyid' visible='false' link-type='outer'>
                                <attribute name='currencyname'  alias='transactioncurrencyid_label'/>
                            </link-entity>
                            <link-entity name='bsd_topic' from='bsd_topicid' to='bsd_topic' visible='false' link-type='outer' alias='a_533be24fba81e911a83b000d3a07be23'>
                                <attribute name='bsd_name' alias='bsd_topic_label' />
                                <attribute name='bsd_topicid' alias='_bsd_topic_value'/>
                            </link-entity>
                          </entity>
                        </fetch>";
            //$@"<link-entity name='campaign' from='campaignid' to='campaignid' visible='false' link-type='outer'>
            //                    <attribute name='campaignid'  alias='_campaignid_value'/>
            //                    <attribute name='name'  alias='campaignid_label'/>
            //                </link-entity>" lỗi

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch);
            if (result == null || result.value.Count == 0)
            {
                return;
            }
            var tmp = result.value.FirstOrDefault();
            tmp.lastname = tmp.fullname;
            this.singleLead = tmp;
            if (singleLead.bsd_dategrant.HasValue)
                singleLead.bsd_dategrant = tmp.bsd_dategrant.Value.ToLocalTime();
            if (singleLead.new_birthday.HasValue)
                singleLead.new_birthday = tmp.new_birthday.Value.ToLocalTime();

            string fetch2 = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='lead'>
                            <attribute name='lastname' />
                            <attribute name='leadid' />
                            <order attribute='createdon' descending='true' />
                            <filter type='and'>
                                <condition attribute='leadid' operator='eq' value='{" + LeadId + @"}' />
                            </filter>
                             <link-entity name='bsd_country' from='bsd_countryid' to='bsd_accountcountry' link-type='outer' alias='aa'>
                                    <attribute name='bsd_countryid' alias='bsd_accountcountry_id' />
                                    <attribute name='bsd_name' alias='bsd_accountcountry_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_accountcountry_name_en'/>
                                </link-entity>
                                <link-entity name='new_district' from='new_districtid' to='bsd_accountdistrict' link-type='outer' alias='ab'>
                                    <attribute name='new_districtid' alias='bsd_accountdistrict_id' />
                                    <attribute name='new_name' alias='bsd_accountdistrict_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_accountdistrict_name_en'/>
                                </link-entity>
                                <link-entity name='new_province' from='new_provinceid' to='bsd_accountprovince' link-type='outer' alias='ac'>
                                    <attribute name='new_provinceid' alias='bsd_accountprovince_id' />
                                    <attribute name='new_name' alias='bsd_accountprovince_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_accountprovince_name_en'/>
                                </link-entity>
                                <link-entity name='bsd_country' from='bsd_countryid' to='bsd_country' link-type='outer' alias='ad'>
                                    <attribute name='bsd_countryid' alias='bsd_country_id' />
                                    <attribute name='bsd_name' alias='bsd_country_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_country_name_en'/>
                                </link-entity>
                                <link-entity name='new_district' from='new_districtid' to='bsd_district' link-type='outer' alias='ae'>
                                    <attribute name='new_districtid' alias='bsd_district_id' />
                                    <attribute name='new_name' alias='bsd_district_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_district_name_en'/>
                                </link-entity>
                                <link-entity name='new_province' from='new_provinceid' to='bsd_province' link-type='outer' alias='af'>
                                    <attribute name='new_provinceid' alias='bsd_province_id' />
                                    <attribute name='new_name' alias='bsd_province_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_province_name_en'/>
                                </link-entity>
                                <link-entity name='bsd_country' from='bsd_countryid' to='bsd_permanentcountry' link-type='outer' alias='ag'>
                                    <attribute name='bsd_countryid' alias='bsd_permanentcountry_id' />
                                    <attribute name='bsd_name' alias='bsd_permanentcountry_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_permanentcountry_name_en'/>
                                </link-entity>
                                <link-entity name='new_district' from='new_districtid' to='bsd_permanentdistrict' link-type='outer' alias='ah'>
                                    <attribute name='new_districtid' alias='bsd_permanentdistrict_id' />
                                    <attribute name='new_name' alias='bsd_permanentdistrict_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_permanentdistrict_name_en'/>
                                </link-entity>
                                <link-entity name='new_province' from='new_provinceid' to='bsd_permanentprovince' link-type='outer' alias='ai'>
                                    <attribute name='new_provinceid' alias='bsd_permanentprovince_id' />
                                    <attribute name='new_name' alias='bsd_permanentprovince_name' />
                                    <attribute name='bsd_nameen'  alias='bsd_permanentprovince_name_en'/>
                                </link-entity>
                          </entity>
                        </fetch>";

            var result2 = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadFormModel>>("leads", fetch2);
            if (result2 == null || result2.value.Count == 0)
            {
                return;
            }
            var tmp2 = result2.value.FirstOrDefault();

            if (tmp2.bsd_accountcountry_id != Guid.Empty)
            {
                singleLead.bsd_accountcountry_id = tmp2.bsd_accountcountry_id;
                singleLead.bsd_accountcountry_name = tmp2.bsd_accountcountry_name;
                singleLead.bsd_accountcountry_name_en = tmp2.bsd_accountcountry_name_en;
                singleLead.bsd_accountdistrict_id = tmp2.bsd_accountdistrict_id;
                singleLead.bsd_accountdistrict_name = tmp2.bsd_accountdistrict_name;
                singleLead.bsd_accountdistrict_name_en = tmp2.bsd_accountdistrict_name_en;
                singleLead.bsd_accountprovince_id = tmp2.bsd_accountprovince_id;
                singleLead.bsd_accountprovince_name = tmp2.bsd_accountprovince_name;
                singleLead.bsd_accountprovince_name_en = tmp2.bsd_accountprovince_name_en;

                Address3 = new AddressModel
                {
                    country_id = singleLead.bsd_accountcountry_id,
                    country_name = !string.IsNullOrWhiteSpace(singleLead.bsd_accountcountry_name_en) && UserLogged.Language == "en" ? singleLead.bsd_accountcountry_name_en : singleLead.bsd_accountcountry_name,
                    country_name_en = singleLead.bsd_accountcountry_name_en,
                    province_id = singleLead.bsd_accountprovince_id,
                    province_name = !string.IsNullOrWhiteSpace(singleLead.bsd_accountprovince_name_en) && UserLogged.Language == "en" ? singleLead.bsd_accountprovince_name_en : singleLead.bsd_accountprovince_name,
                    province_name_en = singleLead.bsd_accountprovince_name_en,
                    district_id = singleLead.bsd_accountdistrict_id,
                    district_name = !string.IsNullOrWhiteSpace(singleLead.bsd_accountdistrict_name_en) && UserLogged.Language == "en" ? singleLead.bsd_accountdistrict_name_en : singleLead.bsd_accountdistrict_name,
                    district_name_en = singleLead.bsd_accountdistrict_name_en,
                    lineaddress = singleLead.bsd_account_housenumberstreetwardvn,
                    address = singleLead.bsd_accountaddressvn
                };
            }
            if (tmp2.bsd_permanentcountry_id != Guid.Empty)
            {
                singleLead.bsd_permanentcountry_id = tmp2.bsd_permanentcountry_id;
                singleLead.bsd_permanentcountry_name = tmp2.bsd_permanentcountry_name;
                singleLead.bsd_permanentcountry_name_en = tmp2.bsd_permanentcountry_name_en;
                singleLead.bsd_permanentdistrict_id = tmp2.bsd_permanentdistrict_id;
                singleLead.bsd_permanentdistrict_name = tmp2.bsd_permanentdistrict_name;
                singleLead.bsd_permanentdistrict_name_en = tmp2.bsd_permanentdistrict_name_en;
                singleLead.bsd_permanentprovince_id = tmp2.bsd_permanentprovince_id;
                singleLead.bsd_permanentprovince_name = tmp2.bsd_permanentprovince_name;
                singleLead.bsd_permanentprovince_name_en = tmp2.bsd_permanentprovince_name_en;

                Address2 = new AddressModel
                {
                    country_id = singleLead.bsd_permanentcountry_id,
                    country_name = !string.IsNullOrWhiteSpace(singleLead.bsd_permanentcountry_name_en) && UserLogged.Language == "en" ? singleLead.bsd_permanentcountry_name_en : singleLead.bsd_permanentcountry_name,
                    country_name_en = singleLead.bsd_permanentcountry_name_en,
                    province_id = singleLead.bsd_permanentprovince_id,
                    province_name = !string.IsNullOrWhiteSpace(singleLead.bsd_permanentprovince_name_en) && UserLogged.Language == "en" ? singleLead.bsd_permanentprovince_name_en : singleLead.bsd_permanentprovince_name,
                    province_name_en = singleLead.bsd_permanentprovince_name_en,
                    district_id = singleLead.bsd_permanentdistrict_id,
                    district_name = !string.IsNullOrWhiteSpace(singleLead.bsd_permanentdistrict_name_en) && UserLogged.Language == "en" ? singleLead.bsd_permanentdistrict_name_en : singleLead.bsd_permanentdistrict_name,
                    district_name_en = singleLead.bsd_permanentdistrict_name_en,
                    lineaddress = singleLead.bsd_permanentaddress,
                    address = singleLead.bsd_permanentaddress1
                };
            }
            if (tmp2.bsd_permanentcountry_id != Guid.Empty)
            {
                singleLead.bsd_country_id = tmp2.bsd_country_id;
                singleLead.bsd_country_name = tmp2.bsd_country_name;
                singleLead.bsd_country_name_en = tmp2.bsd_country_name_en;
                singleLead.bsd_district_id = tmp2.bsd_district_id;
                singleLead.bsd_district_name = tmp2.bsd_district_name;
                singleLead.bsd_district_name_en = tmp2.bsd_district_name_en;
                singleLead.bsd_province_id = tmp2.bsd_province_id;
                singleLead.bsd_province_name = tmp2.bsd_province_name;
                singleLead.bsd_province_name_en = tmp2.bsd_province_name_en;

                Address1 = new AddressModel
                {
                    country_id = singleLead.bsd_country_id,
                    country_name = !string.IsNullOrWhiteSpace(singleLead.bsd_country_name_en) && UserLogged.Language == "en" ? singleLead.bsd_country_name_en : singleLead.bsd_country_name,
                    country_name_en = singleLead.bsd_country_name_en,
                    province_id = singleLead.bsd_province_id,
                    province_name = !string.IsNullOrWhiteSpace(singleLead.bsd_province_name_en) && UserLogged.Language == "en" ? singleLead.bsd_province_name_en : singleLead.bsd_province_name,
                    province_name_en = singleLead.bsd_province_name_en,
                    district_id = singleLead.bsd_district_id,
                    district_name = !string.IsNullOrWhiteSpace(singleLead.bsd_district_name_en) && UserLogged.Language == "en" ? singleLead.bsd_district_name_en : singleLead.bsd_district_name,
                    district_name_en = singleLead.bsd_district_name_en,
                    lineaddress = singleLead.bsd_housenumberstreet,
                    address = singleLead.bsd_contactaddress
                };
            }
        }

        public async Task<bool> updateLead()
        {
            string path = "/leads(" + LeadId + ")";
            singleLead.leadid = LeadId;
            var content = await this.getContent();
            CrmApiResponse result = await CrmHelper.PatchData(path, content);

            if (result.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<CrmApiResponse> createLead()
        {
            string path = "/leads";
            singleLead.leadid = Guid.NewGuid();
            var content = await this.getContent();
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            return result;
        }

        public async Task<Boolean> DeletLookup(string fieldName, Guid leadId)
        {
            var result = await CrmHelper.SetNullLookupField("leads", leadId, fieldName);
            return result.IsSuccess;
        }

        private async Task<object> getContent()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["leadid"] = singleLead.leadid;
            data["subject"] = this.Topic.Label;
            data["firstname"] = "";
            data["lastname"] = singleLead.lastname;
            data["jobtitle"] = singleLead.jobtitle;
            data["emailaddress1"] = singleLead.emailaddress1;
            data["companyname"] = singleLead.companyname;
            data["websiteurl"] = singleLead.websiteurl;
            data["address1_composite"] = singleLead.address1_composite;
            data["address1_line1"] = singleLead.address1_line1;
            data["address1_city"] = singleLead.address1_city;
            data["address1_stateorprovince"] = singleLead.address1_stateorprovince;
            data["address1_country"] = singleLead.address1_country;
            data["description"] = singleLead.description;
            data["industrycode"] = singleLead.industrycode;
            data["revenue"] = singleLead?.revenue;
            data["leadqualitycode"] = Rating.Val;
            data["new_gender"] = this.Gender?.Val;
            data["new_birthday"] = singleLead.new_birthday.Value.ToUniversalTime(); ;
            data["leadsourcecode"] = this.LeadSource?.Val;
            data["bsd_customergroup"] = this.CustomerGroup?.Val;
            data["bsd_typeofidcard"] = this.TypeIdCard?.Val;
            data["bsd_identitycardnumberid"] = singleLead.bsd_identitycardnumberid;
            data["bsd_area"] = this.Area?.Val;
            data["bsd_placeofissue"] = singleLead.bsd_placeofissue;
            data["bsd_dategrant"] = singleLead.bsd_dategrant.Value.ToUniversalTime(); ;
            data["bsd_registrationcode"] = singleLead.bsd_registrationcode;

            data["mobilephone"] = !string.IsNullOrWhiteSpace(singleLead.mobilephone) ? singleLead.mobilephone : null;
            data["telephone1"] = !string.IsNullOrWhiteSpace(singleLead.telephone1)? singleLead.telephone1 : "+84";

            if (!string.IsNullOrWhiteSpace(singleLead.numberofemployees))
            {
                data["numberofemployees"] = int.Parse(singleLead.numberofemployees);
            }
            else
            {
                data["numberofemployees"] = null;
            }

            data["sic"] = singleLead.sic;
            data["donotsendmm"] = singleLead.donotsendmm.ToString();
            data["lastusedincampaign"] = singleLead.lastusedincampaign.HasValue ? (DateTime.Parse(singleLead.lastusedincampaign.ToString()).ToLocalTime()).ToString("yyyy-MM-dd\"T\"HH:mm:ss\"Z\"") : null;

            if (this.Topic == null)
            {
                await DeletLookup("bsd_topic", singleLead.leadid);
            }
            else
            {
                data["bsd_topic@odata.bind"] = "/bsd_topics(" + this.Topic.Val + ")";
            }

            //if (singleLead._transactioncurrencyid_value == null)
            //{
            //    await DeletLookup("transactioncurrencyid", singleLead.leadid);
            //}
            //else
            //{
            //    data["transactioncurrencyid@odata.bind"] = "/transactioncurrencies(" + singleLead._transactioncurrencyid_value + ")"; /////Lookup Field
            //}

            if (singleLead._campaignid_value == null)
            {
                await DeletLookup("campaignid", singleLead.leadid);
            }
            else
            {
                data["campaignid@odata.bind"] = "/campaigns(" + singleLead._campaignid_value + ")"; /////Lookup Field
            }
            if (UserLogged.Id != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }

            // địa chỉ công ty
            //bsd_accountcountry 
            //bsd_accountprovince
            //bsd_accountdistrict

            //bsd_account_housenumberstreetwardvn
            //bsd_accountaddressvn
            if (Address3 == null || Address3.country_id == Guid.Empty)
            {
                await DeletLookup("bsd_accountcountry", singleLead.leadid);
            }
            else
            {
                data["bsd_accountcountry@odata.bind"] = "/bsd_countries(" + Address3.country_id + ")"; /////Lookup Field
            }

            if (Address3 == null || Address3.province_id == Guid.Empty)
            {
                await DeletLookup("bsd_accountprovince", singleLead.leadid);
            }
            else
            {
                data["bsd_accountprovince@odata.bind"] = "/new_provinces(" + Address3.province_id + ")"; /////Lookup Field
            }

            if (Address3 == null || Address3.district_id == Guid.Empty)
            {
                await DeletLookup("bsd_accountdistrict", singleLead.leadid);
            }
            else
            {
                data["bsd_accountdistrict@odata.bind"] = "/new_districts(" + Address3.district_id + ")"; /////Lookup Field
            }

            if (Address3 != null)
            {
                if (!string.IsNullOrWhiteSpace(Address3.lineaddress))
                {
                    data["bsd_account_housenumberstreetwardvn"] = Address3.lineaddress;
                    data["bsd_account_housenumberstreetwarden"] = Address3.lineaddress;
                }
                if (!string.IsNullOrWhiteSpace(Address3.address))
                {
                    data["bsd_accountaddressvn"] = Address3.address;
                    data["bsd_accountaddressen"] = Address3.address;
                }
            }

            ////bsd_permanentcountry
            ////bsd_permanentprovince
            ////bsd_permanentdistrict

            ////bsd_permanentaddress
            ////bsd_permanentaddress1
            if (Address2 == null || Address2.country_id == Guid.Empty)
            {
                await DeletLookup("bsd_PermanentCountry", singleLead.leadid);
            }
            else
            {
                data["bsd_PermanentCountry@odata.bind"] = "/bsd_countries(" + Address2.country_id + ")"; /////Lookup Field
            }

            if (Address2 == null || Address2.province_id == Guid.Empty)
            {
                await DeletLookup("bsd_PermanentProvince", singleLead.leadid);
            }
            else
            {
                data["bsd_PermanentProvince@odata.bind"] = "/new_provinces(" + Address2.province_id + ")"; /////Lookup Field
            }

            if (Address2 == null || Address2.district_id == Guid.Empty)
            {
                await DeletLookup("bsd_PermanentDistrict", singleLead.leadid);
            }
            else
            {
                data["bsd_PermanentDistrict@odata.bind"] = "/new_districts(" + Address2.district_id + ")"; /////Lookup Field
            }
            if (Address2 != null)
            {
                if (!string.IsNullOrWhiteSpace(Address2.lineaddress))
                {
                    data["bsd_permanentaddress"] = Address2.lineaddress;
                    data["bsd_permanenthousenumber"] = Address2.lineaddress;
                }
                if (!string.IsNullOrWhiteSpace(Address2.address))
                {
                    data["bsd_permanentaddress1"] = Address2.address;
                    data["bsd_diachithuongtru"] = Address2.address;
                }
            }
            else
            {
                data["bsd_permanentaddress"] = null;
                data["bsd_permanenthousenumber"] = null;
                data["bsd_permanentaddress1"] = null;
                data["bsd_diachithuongtru"] = null;
            }

            //bsd_country
            //bsd_province
            //bsd_district

            //bsd_housenumberstreet
            //bsd_contactaddress
            if (Address1 == null || Address1.country_id == Guid.Empty)
            {
                await DeletLookup("bsd_Country", singleLead.leadid);
            }
            else
            {
                data["bsd_Country@odata.bind"] = "/bsd_countries(" + Address1.country_id + ")"; /////Lookup Field
            }

            if (Address1 == null || Address1.province_id == Guid.Empty)
            {
                await DeletLookup("bsd_province", singleLead.leadid);
            }
            else
            {
                data["bsd_province@odata.bind"] = "/new_provinces(" + Address1.province_id + ")"; /////Lookup Field
            }

            if (Address1 == null || Address1.district_id == Guid.Empty)
            {
                await DeletLookup("bsd_District", singleLead.leadid);
            }
            else
            {
                data["bsd_District@odata.bind"] = "/new_districts(" + Address1.district_id + ")"; /////Lookup Field
            }
            if (Address1 != null )
            {
                if (!string.IsNullOrWhiteSpace(Address1.lineaddress))
                {
                    data["bsd_housenumberstreet"] = Address1.lineaddress;// singleLead.bsd_housenumberstreet;
                    data["bsd_housenumber"] = Address1.lineaddress;
                }
                if (!string.IsNullOrWhiteSpace(Address1.address))
                {
                    data["bsd_contactaddress"] = Address1.address;// singleLead.bsd_contactaddress;
                    data["bsd_diachi"] = Address1.address;
                }
            }
            else
            {
                data["bsd_housenumberstreet"] = null;
                data["bsd_housenumber"] = null;
                data["bsd_contactaddress"] = null;
                data["bsd_diachi"] = null;
            }    
            return data;
        }

        ///////////// CURRENCY LOOKUP AREA
        public async Task LoadCurrenciesForLookup()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='transactioncurrency'>
                                    <attribute name='transactioncurrencyid' alias='Val'/>
                                    <attribute name='currencyname' alias='Label'/>
                                    <order attribute='currencyname' descending='false' />
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("transactioncurrencies", fetch);
            if (result == null || result.value.Count == 0)
            {
                return;
            }

            foreach (var x in result.value)
            {
                list_currency_lookup.Add(x);
            }
        }

        //////// INDUSTRYCODE OPTIONSET AREA
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

        public async Task LoadTopics()
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_topic'>
                                    <attribute name='bsd_topicid' alias='Val'/>
                                    <attribute name='bsd_name' alias ='Label'/>
                                    <order attribute='bsd_name' descending='false' />
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_topics", fetchXml);
            if (result == null || result.value.Any() == false) return;

            this.Topics = result.value;
        }

        ////////// CAMPAIGN LOOKP AREA
        public async Task LoadCampainsForLookup()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='campaign'>
                                    <attribute name='name' alias='Label'/>
                                    <attribute name='campaignid' alias='Val'/>
                                    <order attribute='name' descending='true' />
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("campaigns", fetch);
            if (result == null || result.value.Count == 0) return;

            foreach (var x in result.value)
            {
                list_campaign_lookup.Add(x);
            }
        }
    }
}