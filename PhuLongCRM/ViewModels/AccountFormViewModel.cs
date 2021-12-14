using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;

namespace PhuLongCRM.ViewModels
{
    public class AccountFormViewModel : BaseViewModel
    {
        private AccountFormModel _singleAccount;
        public AccountFormModel singleAccount { get => _singleAccount; set { _singleAccount = value; OnPropertyChanged(nameof(singleAccount)); } }

        private List<OptionSet> _businessTypeOptionList;
        public List<OptionSet> BusinessTypeOptionList { get => _businessTypeOptionList; set { _businessTypeOptionList = value; OnPropertyChanged(nameof(BusinessTypeOptionList)); } }
        public ObservableCollection<OptionSet> LocalizationOptionList { get; set; }

        public OptionSet _localization;
        public OptionSet Localization { get => _localization; set { _localization = value; OnPropertyChanged(nameof(Localization)); } }

        public OptionSet _businessType;
        public OptionSet BusinessType { get => _businessType; set { _businessType = value; OnPropertyChanged(nameof(BusinessType)); } }

        private List<LookUp> _primaryContactOptionList;
        public List<LookUp> PrimaryContactOptionList { get=>_primaryContactOptionList; set { _primaryContactOptionList = value;OnPropertyChanged(nameof(PrimaryContactOptionList)); } }

        private LookUp _PrimaryContact;
        public LookUp PrimaryContact { get => _PrimaryContact; set { _PrimaryContact = value; OnPropertyChanged(nameof(PrimaryContact)); } }

        private string _addressCompositeContac;
        public string AddressCompositeContac { get => _addressCompositeContac; set { _addressCompositeContac = value; OnPropertyChanged(nameof(AddressCompositeContac)); } }

        private LookUp _addressCountryContac;
        public LookUp AddressCountryContac
        {
            get => _addressCountryContac;
            set
            {
                _addressCountryContac = value;
                OnPropertyChanged(nameof(AddressCountryContac));
                AddressStateProvinceContac = null;
                list_province_lookup.Clear();
            }
        }

        private LookUp _addressStateProvinceContac;
        public LookUp AddressStateProvinceContac
        {
            get => _addressStateProvinceContac;
            set
            {
                _addressStateProvinceContac = value;
                OnPropertyChanged(nameof(AddressStateProvinceContac));
                AddressCityContac = null;
                list_district_lookup.Clear();
            }
        }

        private LookUp _addressCityContac;
        public LookUp AddressCityContac { get => _addressCityContac; set { _addressCityContac = value; OnPropertyChanged(nameof(AddressCityContac)); } }

        private string _addressLine1Contac;
        public string AddressLine1Contac { get => _addressLine1Contac; set { _addressLine1Contac = value; OnPropertyChanged(nameof(AddressLine1Contac)); } }

        private LookUp _addressCountryPermanent;
        public LookUp AddressCountryPermanent
        {
            get => _addressCountryPermanent;
            set
            {
                _addressCountryPermanent = value;
                OnPropertyChanged(nameof(AddressCountryPermanent));
                AddressStateProvincePermanent = null;
                list_province_lookup.Clear();
            }
        }

        private LookUp _addressStateProvincePermanent;
        public LookUp AddressStateProvincePermanent
        {
            get => _addressStateProvincePermanent;
            set
            {
                _addressStateProvincePermanent = value;
                OnPropertyChanged(nameof(AddressStateProvincePermanent));
                AddressCityPermanent = null;
                list_district_lookup.Clear();
            }
        }

        private LookUp _addressCityPermanent;
        public LookUp AddressCityPermanent { get => _addressCityPermanent; set { _addressCityPermanent = value; OnPropertyChanged(nameof(AddressCityPermanent)); } }

        private string _addressLine1Permanent;
        public string AddressLine1Permanent { get => _addressLine1Permanent; set { _addressLine1Permanent = value; OnPropertyChanged(nameof(AddressLine1Permanent)); } }

        private List<OptionSet> _operationScopes;
        public List<OptionSet> OperationScopes { get => _operationScopes; set { _operationScopes = value;OnPropertyChanged(nameof(OperationScopes)); } }

        private OptionSet _operationScope;
        public OptionSet OperationScope { get => _operationScope; set { _operationScope = value; OnPropertyChanged(nameof(OperationScope)); } }

        private List<OptionSet> _customerStatusReasons;
        public List<OptionSet> CustomerStatusReasons { get => _customerStatusReasons; set { _customerStatusReasons = value; OnPropertyChanged(nameof(CustomerStatusReasons)); } }

        private OptionSet _customerStatusReason;
        public OptionSet CustomerStatusReason { get => _customerStatusReason; set { _customerStatusReason = value; OnPropertyChanged(nameof(CustomerStatusReason)); } }

        public ObservableCollection<LookUp> list_country_lookup { get; set; }
        public ObservableCollection<LookUp> list_province_lookup { get; set; }
        public ObservableCollection<LookUp> list_district_lookup { get; set; }
        
        public AccountFormViewModel()
        {
            singleAccount = new AccountFormModel();                   
            list_country_lookup = new ObservableCollection<LookUp>();
            list_province_lookup = new ObservableCollection<LookUp>();
            list_district_lookup = new ObservableCollection<LookUp>();

            BusinessTypeOptionList = BussinessTypeData.BussinessTypes();
            LocalizationOptionList = new ObservableCollection<OptionSet>();
            PrimaryContactOptionList = new List<LookUp>();          
        }

        public async Task LoadOneAccount(Guid accountid)
        {
           string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='account'>
                                <attribute name='primarycontactid'/>
                                <attribute name='telephone1' />
                                <attribute name='websiteurl' />
                                <attribute name='bsd_vatregistrationnumber' />
                                <attribute name='bsd_incorporatedate' />
                                <attribute name='bsd_hotlines' />
                                <attribute name='bsd_generalledgercompanynumber' />
                                <attribute name='fax' />
                                <attribute name='emailaddress1' />
                                <attribute name='bsd_email2' />
                                <attribute name='bsd_placeofissue' />
                                <attribute name='bsd_issuedon' />
                                <attribute name='bsd_permanentaddress1' />
                                <attribute name='bsd_groupgstregisttationnumber' />
                                <attribute name='statuscode' />
                                <attribute name='ownerid' />
                                <attribute name='createdon' />
                                <attribute name='bsd_address'/>
                                <attribute name='bsd_nation' alias='_bsd_country_value' />
                                <attribute name='bsd_province' alias='_bsd_province_value'/>
                                <attribute name='bsd_district' alias='_bsd_district_value'/>
                                <attribute name='bsd_postalcode' />
                                <attribute name='bsd_housenumberstreet' />
                                <attribute name='bsd_companycode' />
                                <attribute name='bsd_registrationcode' />
                                <attribute name='bsd_accountnameother' />
                                <attribute name='bsd_localization' />
                                <attribute name='bsd_name' />
                                <attribute name='name' />
                                <attribute name='accountid' />
                                <attribute name='bsd_businesstype' />
                                <attribute name='bsd_permanentprovince' alias='_bsd_permanentprovince_value'/>
                                <attribute name='bsd_permanentnation' alias='_bsd_permanentnation_value'/>
                                <attribute name='bsd_permanentdistrict' alias='_bsd_permanentdistrict_value'/>
                                <attribute name='bsd_permanenthousenumberstreetwardvn' />
                                <attribute name='bsd_operationscope' />
                                <attribute name='bsd_customercode' />
                                <order attribute='createdon' descending='true' />
                                    <link-entity name='contact' from='contactid' to='primarycontactid' visible='false' link-type='outer' alias='contacts'>
                                        <attribute name='bsd_fullname' alias='primarycontactname'/>
                                    </link-entity>                                
                                   <link-entity name='new_district' from='new_districtid' to='bsd_district' link-type='outer' alias='af' >
                                        <attribute name='new_name' alias='district_name' />                                       
                                    </link-entity>
                                     <link-entity name='new_province' from='new_provinceid' to='bsd_province' link-type='outer' alias='ag'>
                                        <attribute name='new_name' alias='province_name' />                                       
                                    </link-entity>
                                   <link-entity name='bsd_country' from='bsd_countryid' to='bsd_nation' link-type='outer' alias='as'>
                                        <attribute name='bsd_name' alias='country_name' />                                      
                                    </link-entity>
                                    <link-entity name='bsd_country' from='bsd_countryid' to='bsd_permanentnation' link-type='outer' alias='ae' >
                                        <attribute name='bsd_name' alias='permanentnation_name' /> 
                                    </link-entity>
                                    <link-entity name='new_province' from='new_provinceid' to='bsd_permanentprovince' link-type='outer' alias='afa' >
                                        <attribute name='new_name' alias='permanentprovince_name' />
                                    </link-entity>
                                    <link-entity name='new_district' from='new_districtid' to='bsd_permanentdistrict' link-type='outer' alias='agt' >
                                        <attribute name='new_name' alias='permanentdistrict_name' />  
                                    </link-entity>
                                    <filter type='and'>
                                        <condition attribute='accountid' operator='eq' value='{" + accountid + @"}' />
                                    </filter>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<AccountFormModel>>("accounts", fetch);
            if (result == null || result.value == null)
                return;
            var tmp = result.value.FirstOrDefault();
            this.singleAccount = tmp;
        }

        public void GetPrimaryContactByID()
        {
            PrimaryContact = new LookUp{ Name = singleAccount.primarycontactname, Id = singleAccount._primarycontactid_value, Detail = "Contact" };
        }    

        public async Task<bool> createAccount()
        {
            string path = "/accounts";
            singleAccount.accountid = Guid.NewGuid();
            var content = await this.getContent();
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            if (result.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Boolean> updateAccount( )
        {
            string path = "/accounts(" + singleAccount.accountid + ")";
            var content = await this.getContent();
            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            if (result.IsSuccess)
            {
                return true;
            }else
            {
                return false;
            }

        }

        public async Task<Boolean> DeletLookup(string fieldName, Guid AccountId)
        {
            var result = await CrmHelper.SetNullLookupField("accounts", AccountId, fieldName);
            return result.IsSuccess;
        }

        private async Task<object> getContent()
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["accountid"] = singleAccount.accountid;
            data["bsd_name"] = singleAccount.bsd_name;
            data["bsd_accountnameother"] = singleAccount.bsd_accountnameother;
            data["bsd_companycode"] = singleAccount.bsd_companycode;
            //if (singleAccount.bsd_businesstypesys != null)
            //{
            //    data["bsd_businesstypesys"] = singleAccount.bsd_businesstypesys.Replace(" ", "");
            //}
            data["bsd_businesstype"] = this.BusinessType.Val;
            data["bsd_operationscope"] = this.OperationScope?.Val;
            if (singleAccount.bsd_localization != null)
            {
                data["bsd_localization"] = int.Parse(singleAccount.bsd_localization);
            }
            data["emailaddress1"] = singleAccount.emailaddress1;
            data["bsd_email2"] = singleAccount.bsd_email2;
            data["websiteurl"] = singleAccount.websiteurl;
            data["fax"] = singleAccount.fax;
            data["telephone1"] = singleAccount.telephone1;
            data["bsd_registrationcode"] = singleAccount.bsd_registrationcode;
            data["bsd_issuedon"] = singleAccount.bsd_issuedon.HasValue ? (DateTime.Parse(singleAccount.bsd_issuedon.ToString()).ToLocalTime()).ToString("yyyy-MM-dd\"T\"HH:mm:ss\"Z\"") : null;
            data["bsd_placeofissue"] = singleAccount.bsd_placeofissue;
            data["statuscode"] = this.CustomerStatusReason?.Val;
            data["bsd_vatregistrationnumber"] = singleAccount.bsd_vatregistrationnumber;

            data["bsd_housenumberstreet"] = singleAccount.bsd_housenumberstreet;
            data["bsd_street"] = singleAccount.bsd_housenumberstreet;
            data["bsd_diachi"] = singleAccount.bsd_diachi;
            data["bsd_address"] = singleAccount.bsd_address;
            //data["bsd_postalcode"] = singleAccount.bsd_postalcode;

            data["bsd_permanenthousenumberstreetwardvn"] = singleAccount.bsd_permanenthousenumberstreetwardvn;
            data["bsd_permanenthousenumberstreetward"] = singleAccount.bsd_permanenthousenumberstreetwardvn;
            data["bsd_permanentaddress1"] = singleAccount.bsd_permanentaddress1;
            data["bsd_diachithuongtru"] = singleAccount.bsd_diachithuongtru;

            if (singleAccount._primarycontactid_value == null)
            {
                await DeletLookup("primarycontactid", singleAccount.accountid);
            }
            else
            {
                data["primarycontactid@odata.bind"] = "/contacts(" + singleAccount._primarycontactid_value + ")"; /////Lookup Field
            }

            if (singleAccount._bsd_country_value == null)
            {
                await DeletLookup("bsd_nation", singleAccount.accountid);
            }
            else
            {
                data["bsd_nation@odata.bind"] = "/bsd_countries(" + singleAccount._bsd_country_value + ")"; /////Lookup Field
            }
            if (singleAccount._bsd_province_value == null)
            {
                await DeletLookup("bsd_province", singleAccount.accountid);
            }
            else
            {
                data["bsd_province@odata.bind"] = "/new_provinces(" + singleAccount._bsd_province_value + ")"; /////Lookup Field
            }
            if (singleAccount._bsd_district_value == null)
            {
                await DeletLookup("bsd_district", singleAccount.accountid);
            }
            else
            {
                data["bsd_district@odata.bind"] = "/new_districts(" + singleAccount._bsd_district_value + ")"; /////Lookup Field
            }

            if (singleAccount._bsd_permanentnation_value == null)
            {
                await DeletLookup("bsd_PermanentNation", singleAccount.accountid);
            }
            else
            {
                data["bsd_PermanentNation@odata.bind"] = "/bsd_countries(" + singleAccount._bsd_permanentnation_value + ")"; /////Lookup Field
            }
            if (singleAccount._bsd_permanentprovince_value == null)
            {
                await DeletLookup("bsd_PermanentProvince", singleAccount.accountid);
            }
            else
            {
                data["bsd_PermanentProvince@odata.bind"] = "/new_provinces(" + singleAccount._bsd_permanentprovince_value + ")"; /////Lookup Field
            }
            if (singleAccount._bsd_permanentdistrict_value == null)
            {
                await DeletLookup("bsd_PermanentDistrict", singleAccount.accountid);
            }
            else
            {
                data["bsd_PermanentDistrict@odata.bind"] = "/new_districts(" + singleAccount._bsd_permanentdistrict_value + ")"; /////Lookup Field
            }

            if (UserLogged.Id != Guid.Empty)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }
            return data;
        }

        public async Task LoadContactForLookup() // bubg
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='contactid' alias='Id' />
                    <attribute name='fullname' alias='Name' />
                    <order attribute='fullname' descending='false' />
                    <filter type='and'>
                      <condition attribute='bsd_employee' operator='eq' uitype='bsd_employee' value='{UserLogged.Id}' />
                    </filter>
                  </entity>
                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("contacts", fetch);
            if (result == null)
                return;
            PrimaryContactOptionList = result.value;
        }
        public async Task LoadCountryForLookup()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_country'>
                                    <attribute name='bsd_countryname' alias='Name'/>
                                    <attribute name='bsd_countryid' alias='Id'/>
                                    <attribute name='bsd_nameen' alias='Detail'/>
                                    <order attribute='bsd_priority' descending='false' />
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("bsd_countries", fetch);
            if (result == null)
            {
                return;
            }
            foreach (var x in result.value)
            {
                list_country_lookup.Add(x);
            }
        }

        public async Task<LookUp> LoadCountryByName(string CountryName)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_country'>
                                    <attribute name='bsd_countryname' alias='Name'/>
                                    <attribute name='bsd_countryid' alias='Id'/>
                                    <attribute name='bsd_nameen' alias='Detail'/>
                                    <order attribute='bsd_countryname' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bsd_countryname' operator='eq' value='" + CountryName + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("bsd_countries", fetch);
            if (result != null && result.value.Count > 0)
            {
                LookUp country = new LookUp();
                country = result.value.FirstOrDefault();
                return country;
            }
            else
            {
                return null;
            }
        }

        public async Task LoadProvincesForLookup(LookUp Country)
        {
            if (Country == null) return;
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='new_province'>
                                    <attribute name='bsd_provincename' alias='Name'/>
                                    <attribute name='new_provinceid' alias='Id'/>
                                    <attribute name='bsd_nameen' alias='Detail'/>
                                    <order attribute='bsd_priority' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bsd_country' operator='eq' value='" + Country.Id + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_provinces", fetch);
            if (result == null)
            {
                return;
            }
            foreach (var x in result.value)
            {
                list_province_lookup.Add(x);
            }
        }

        public async Task<LookUp> LoadProvinceByName(string CountryId, string ProvinceName)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='new_province'>
                                    <attribute name='bsd_provincename' alias='Name'/>
                                    <attribute name='new_provinceid' alias='Id'/>
                                    <attribute name='bsd_nameen' alias='Detail'/>
                                    <order attribute='bsd_provincename' descending='false' />
                                    <filter type='and'>
                                        <condition attribute='bsd_country' operator='eq' value='" + CountryId + @"' />
                                        <condition attribute='bsd_provincename' operator='eq' value='" + ProvinceName + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_provinces", fetch);
            if (result != null && result.value.Count > 0)
            {
                LookUp Province = new LookUp();
                Province = result.value.FirstOrDefault();
                return Province;
            }
            else
            {
                return null;
            }
        }

        public async Task LoadDistrictForLookup(LookUp Province)
        {
            if (Province == null) return;
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_district'>
                                <attribute name='new_name' alias='Name'/>
                                <attribute name='new_districtid' alias='Id'/>
                                <attribute name='bsd_nameen' alias='Detail'/>
                                <order attribute='new_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='new_province' operator='eq' value='" + Province.Id + @"' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_districts", fetch);
            if (result == null)
            {
                return;
            }
            foreach (var x in result.value)
            {
                list_district_lookup.Add(x);
            }
        }

        public async Task<LookUp> LoadDistrictByName(string ProvinceId, string DistrictName)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_district'>
                                <attribute name='new_name' alias='Name'/>
                                <attribute name='new_districtid' alias='Id'/>
                                <attribute name='bsd_nameen' alias='Detail'/>
                                <order attribute='new_name' descending='false' />
                                <filter type='and'>
                                    <condition attribute='new_province' operator='eq' value='" + ProvinceId + @"' />
                                    <condition attribute='new_name' operator='eq' value='" + DistrictName + @"' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("new_districts", fetch);
            if (result != null && result.value.Count > 0)
            {
                LookUp District = new LookUp();
                District = result.value.FirstOrDefault();
                return District;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Check_form_keydata(string bsd_vatregistrationnumber, string bsd_registrationcode, string accountid)
        {
            var fetchxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='account'>
                                <attribute name='bsd_vatregistrationnumber' />
                                <attribute name='bsd_registrationcode' />
                                <attribute name='accountid' />
                                <order attribute='createdon' descending='true' />                              
                                <filter type='and'>
                                  <filter type='or'>
                                    <filter type='and'>
                                      <condition attribute='accountid' operator='ne' value='{" + accountid + @"}' />
                                      <condition attribute='bsd_vatregistrationnumber' operator='eq' value='" + bsd_vatregistrationnumber + @"' />
                                    </filter>
                                    <filter type='and'>
                                      <condition attribute='bsd_registrationcode' operator='eq' value='" + bsd_registrationcode + @"' />
                                      <condition attribute='accountid' operator='ne' value='{" + accountid + @"}' />
                                    </filter>
                                  </filter>
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<AccountForm_CheckdataModel>>("accounts", fetchxml);
            if (result != null && result.value.Count > 0)
            {
                return false;
            }
            return true;           
        }

        public async Task<bool> Check_GPKD(string bsd_vatregistrationnumber, string bsd_registrationcode, string accountid)
        {
            var fetchxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='account'>
                                <attribute name='bsd_vatregistrationnumber' />
                                <attribute name='bsd_registrationcode' />
                                <attribute name='accountid' />
                                <order attribute='createdon' descending='true' />                              
                                <filter type='and'>
                                  <filter type='or'>
                                    <filter type='and'>
                                      <condition attribute='accountid' operator='ne' value='{" + accountid + @"}' />
                                      <condition attribute='bsd_vatregistrationnumber' operator='eq' value='" + bsd_vatregistrationnumber + @"' />
                                    </filter>
                                    <filter type='and'>
                                      <condition attribute='bsd_registrationcode' operator='eq' value='" + bsd_registrationcode + @"' />
                                      <condition attribute='accountid' operator='ne' value='{" + accountid + @"}' />
                                    </filter>
                                  </filter>
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<AccountForm_CheckdataModel>>("accounts", fetchxml);
            if (result != null && result.value.Count > 0)
            {
                return false;
            }
            return true;
        }
    };
}
