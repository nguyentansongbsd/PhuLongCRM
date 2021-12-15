using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ContactFormViewModel : BaseViewModel
    {
        private ContactFormModel _singleContact;
        public ContactFormModel singleContact { get { return _singleContact; } set { _singleContact = value; OnPropertyChanged(nameof(singleContact)); } }

        private OptionSet _singleGender;
        public OptionSet singleGender { get => _singleGender; set { _singleGender = value; OnPropertyChanged(nameof(singleGender)); } }

        private OptionSet _singleLocalization;
        public OptionSet singleLocalization { get => _singleLocalization; set { _singleLocalization = value; OnPropertyChanged(nameof(singleLocalization)); } }

        public ObservableCollection<LookUp> list_account_lookup { get; set; }

        private LookUp _account;
        public LookUp Account { get => _account; set { _account = value; OnPropertyChanged(nameof(Account)); } }

        public ObservableCollection<LookUp> list_lookup { get; set; }

        private List<OptionSet> _contactTypes;
        public List<OptionSet> ContactTypes { get => _contactTypes; set { _contactTypes = value; OnPropertyChanged(nameof(ContactTypes)); } }

        public OptionSet _contactType;
        public OptionSet ContactType { get => _contactType; set { _contactType = value; OnPropertyChanged(nameof(ContactType)); } }

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

        private string _addressLine3Contac;
        public string AddressLine3Contac { get => _addressLine3Contac; set { _addressLine3Contac = value; OnPropertyChanged(nameof(AddressLine3Contac)); } }

        private string _addressLine2Contac;
        public string AddressLine2Contac { get => _addressLine2Contac; set { _addressLine2Contac = value; OnPropertyChanged(nameof(AddressLine2Contac)); } }

        private string _addressLine1Contac;
        public string AddressLine1Contact { get => _addressLine1Contac; set { _addressLine1Contac = value; OnPropertyChanged(nameof(AddressLine1Contact)); } }

        private string _addressCompositePermanent;
        public string AddressCompositePermanent { get => _addressCompositePermanent; set { _addressCompositePermanent = value; OnPropertyChanged(nameof(AddressCompositePermanent)); } }

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

        private string _addressLine3Permanent;
        public string AddressLine3Permanent { get => _addressLine3Permanent; set { _addressLine3Permanent = value; OnPropertyChanged(nameof(AddressLine3Permanent)); } }

        private string _addressLine2Permanent;
        public string AddressLine2Permanent { get => _addressLine2Permanent; set { _addressLine2Permanent = value; OnPropertyChanged(nameof(AddressLine2Permanent)); } }

        private string _addressLine1Permanent;
        public string AddressLine1Permanent { get => _addressLine1Permanent; set { _addressLine1Permanent = value; OnPropertyChanged(nameof(AddressLine1Permanent)); } }

        private List<OptionSet> _customerStatusReasons;
        public List<OptionSet> CustomerStatusReasons { get => _customerStatusReasons; set { _customerStatusReasons = value;OnPropertyChanged(nameof(CustomerStatusReasons)); } }

        private OptionSet _customerStatusReason;
        public OptionSet CustomerStatusReason { get => _customerStatusReason; set { _customerStatusReason = value;OnPropertyChanged(nameof(CustomerStatusReason)); } }

        public ObservableCollection<LookUp> list_country_lookup { get; set; }
        public ObservableCollection<LookUp> list_province_lookup { get; set; }
        public ObservableCollection<LookUp> list_district_lookup { get; set; }

        public ObservableCollection<OptionSet> GenderOptions { get; set; }
        public ObservableCollection<OptionSet> LocalizationOptions { get; set; }

        private string IMAGE_CMND_FOLDER = "Contact_CMND";
        private string frontImage_name;
        private string behindImage_name;

        private string checkCMND;

        private bool _isOfficial;
        public bool IsOfficial { get => _isOfficial; set { _isOfficial = value; OnPropertyChanged(nameof(IsOfficial)); } }

        public ContactFormViewModel()
        {
            singleContact = new ContactFormModel();
            
            list_lookup = new ObservableCollection<LookUp>();
            list_account_lookup = new ObservableCollection<LookUp>();
            list_country_lookup = new ObservableCollection<LookUp>();
            list_province_lookup = new ObservableCollection<LookUp>();
            list_district_lookup = new ObservableCollection<LookUp>();
            GenderOptions = new ObservableCollection<OptionSet>();
            LocalizationOptions = new ObservableCollection<OptionSet>();

            ContactTypes = ContactTypeData.ContactTypes();
        }

        public async Task LoadOneContact(String id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                <attribute name='fullname' />
                                <attribute name='statuscode' />
                                <attribute name='ownerid' />
                                <attribute name='mobilephone' />
                                <attribute name='jobtitle' />
                                <attribute name='bsd_identitycardnumber' />
                                <attribute name='gendercode' />
                                <attribute name='emailaddress1' />
                                <attribute name='createdon' />
                                <attribute name='birthdate' />
                                <attribute name='address1_composite' />
                                <attribute name='bsd_fullname' />
                                <attribute name='contactid' />
                                <attribute name='telephone1' />
                                <attribute name='parentcustomerid' />
                                <attribute name='bsd_province' />
                                <attribute name='bsd_placeofissuepassport' />
                                <attribute name='bsd_placeofissue' />
                                <attribute name='bsd_permanentprovince' />
                                <attribute name='bsd_permanentdistrict' />
                                <attribute name='bsd_permanentcountry' />
                                <attribute name='bsd_permanentaddress1' />
                                <attribute name='bsd_permanentaddress' />
                                <attribute name='bsd_passport' />
                                <attribute name='bsd_localization' />
                                <attribute name='bsd_jobtitlevn' />
                                <attribute name='bsd_issuedonpassport' />
                                <attribute name='bsd_housenumberstreet' />
                                <attribute name='bsd_district' />
                                <attribute name='bsd_dategrant' />
                                <attribute name='bsd_country' />
                                <attribute name='bsd_postalcode' />
                                <attribute name='bsd_contactaddress' />
                                <attribute name='bsd_customercode' />
                                    <link-entity name='account' from='accountid' to='parentcustomerid' visible='false' link-type='outer' alias='aa'>
                                          <attribute name='accountid' alias='_parentcustomerid_value' />
                                          <attribute name='bsd_name' alias='parentcustomerid_label' />
                                    </link-entity>
                                    <link-entity name='bsd_country' from='bsd_countryid' to='bsd_country' visible='false' link-type='outer'>
                                        <attribute name='bsd_countryname'  alias='bsd_country_label'/>                                        
                                    </link-entity>
                                    <link-entity name='new_province' from='new_provinceid' to='bsd_province' visible='false' link-type='outer'>
                                        <attribute name='bsd_provincename'  alias='bsd_province_label'/>
                                    </link-entity>
                                    <link-entity name='new_district' from='new_districtid' to='bsd_district' visible='false' link-type='outer'>
                                        <attribute name='new_name'  alias='bsd_district_label'/>                                      
                                    </link-entity>
                                    <link-entity name='bsd_country' from='bsd_countryid' to='bsd_permanentcountry' visible='false' link-type='outer'>
                                        <attribute name='bsd_countryname'  alias='bsd_permanentcountry_label'/>                                      
                                    </link-entity>
                                    <link-entity name='new_province' from='new_provinceid' to='bsd_permanentprovince' visible='false' link-type='outer'>
                                        <attribute name='bsd_provincename'  alias='bsd_permanentprovince_label'/>                                        
                                    </link-entity>
                                    <link-entity name='new_district' from='new_districtid' to='bsd_permanentdistrict' visible='false' link-type='outer'>
                                        <attribute name='new_name'  alias='bsd_permanentdistrict_label'/>
                                    </link-entity>
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                     <condition attribute='contactid' operator='eq' value='" + id + @"' />
                                    </filter>
                              </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetch);
            if (result == null || result.value == null)
            {
                return;
            }

            var tmp = result.value.FirstOrDefault();
            this.singleContact = tmp;

            checkCMND = tmp.bsd_identitycardnumber;

            if (singleContact.statuscode == "100000000")
                IsOfficial = false;
            else
                IsOfficial = true;
        }

        public async Task<Boolean> updateContact(ContactFormModel contact)
        {
            string path = "/contacts(" + contact.contactid + ")";
            var content = await this.getContent(contact);

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

        public async Task<Guid> createContact(ContactFormModel contact)
        {
            string path = "/contacts";
            contact.contactid = Guid.NewGuid();
            var content = await this.getContent(contact);

            CrmApiResponse result = await CrmHelper.PostData(path, content);

            if (result.IsSuccess)
            {
                //if (singleContact.bsd_mattruoccmnd_base64 != null)
                //{
                //    await UpLoadCMNDFront();
                //}
                //if (singleContact.bsd_matsaucmnd_base64 != null)
                //{
                //    await UpLoadCMNDBehind();
                //}
                return contact.contactid;
            }
            else
            {
                return new Guid();
            }

        }

        public async Task<Boolean> DeletLookup(string fieldName, Guid contactId)
        {
            var result = await CrmHelper.SetNullLookupField("contacts", contactId, fieldName);
            return result.IsSuccess;
        }

        private async Task<object> getContent(ContactFormModel contact)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["contactid"] = contact.contactid;
            data["lastname"] = contact.bsd_fullname;
            data["firstname"] = "";
            data["bsd_fullname"] = contact.bsd_fullname;
            data["emailaddress1"] = contact.emailaddress1;
            data["birthdate"] = contact.birthdate.HasValue ? (DateTime.Parse(contact.birthdate.ToString()).ToUniversalTime()).ToString("yyyy-MM-dd") : null;
            data["mobilephone"] = contact.mobilephone;
            data["gendercode"] = contact.gendercode;
            if (checkCMND != contact.bsd_identitycardnumber)
            {
                data["bsd_identitycardnumber"] = contact.bsd_identitycardnumber;
            }
            data["bsd_localization"] = contact.bsd_localization;
            data["bsd_dategrant"] = contact.bsd_dategrant.HasValue ? (DateTime.Parse(contact.bsd_dategrant.ToString()).ToLocalTime()).ToString("yyy-MM-dd") : null;
            data["bsd_placeofissue"] = contact.bsd_placeofissue;
            data["bsd_passport"] = contact.bsd_passport;
            data["bsd_issuedonpassport"] = contact.bsd_issuedonpassport.HasValue ? (DateTime.Parse(contact.bsd_issuedonpassport.ToString()).ToLocalTime()).ToString("yyyy-MM-dd") : null;
            data["bsd_placeofissuepassport"] = contact.bsd_placeofissuepassport;
            data["bsd_jobtitlevn"] = contact.bsd_jobtitlevn;
            data["telephone1"] = contact.telephone1;
            data["statuscode"] = this.CustomerStatusReason?.Val;
            data["bsd_housenumberstreet"] = contact.bsd_housenumberstreet;
            data["bsd_contactaddress"] = contact.bsd_contactaddress;
            data["bsd_diachi"] = contact.bsd_diachi;
          //  data["bsd_postalcode"] = contact.bsd_postalcode;
            data["bsd_housenumber"] = contact.bsd_housenumberstreet;

            data["bsd_permanentaddress1"] = contact.bsd_permanentaddress1;
            data["bsd_diachithuongtru"] = contact.bsd_diachithuongtru;
            data["bsd_permanenthousenumber"] = contact.bsd_permanentaddress;
            data["bsd_permanentaddress"] = contact.bsd_permanentaddress;

            data["bsd_type"] = this.ContactType.Val;

            if (contact._parentcustomerid_value == null)
            {
                await DeletLookup("parentcustomerid_account", contact.contactid);
            }
            else
            {
                data["parentcustomerid_account@odata.bind"] = "/accounts(" + contact._parentcustomerid_value + ")"; /////Lookup Field

            }

            if (contact._bsd_country_value == null)
            {
                await DeletLookup("bsd_country", contact.contactid);
            }
            else
            {
                data["bsd_country@odata.bind"] = "/bsd_countries(" + contact._bsd_country_value + ")"; /////Lookup Field
            }

            if (contact._bsd_province_value == null)
            {
                await DeletLookup("bsd_province", contact.contactid);
            }
            else
            {
                data["bsd_province@odata.bind"] = "/new_provinces(" + contact._bsd_province_value + ")"; /////Lookup Field
            }

            if (contact._bsd_district_value == null)
            {
                await DeletLookup("bsd_district", contact.contactid);
            }
            else
            {
                data["bsd_district@odata.bind"] = "/new_districts(" + contact._bsd_district_value + ")"; /////Lookup Field
            }

            if (contact._bsd_permanentcountry_value == null)
            {
                await DeletLookup("bsd_permanentcountry", contact.contactid);
            }
            else
            {
                data["bsd_permanentcountry@odata.bind"] = "/bsd_countries(" + contact._bsd_permanentcountry_value + ")"; /////Lookup Field
            }

            if (contact._bsd_permanentprovince_value == null)
            {
                await DeletLookup("bsd_permanentprovince", contact.contactid);
            }
            else
            {
                data["bsd_permanentprovince@odata.bind"] = "/new_provinces(" + contact._bsd_permanentprovince_value + ")"; /////Lookup Field
            }

            if (contact._bsd_permanentdistrict_value == null)
            {
                await DeletLookup("bsd_permanentdistrict", contact.contactid);
            }
            else
            {
                data["bsd_permanentdistrict@odata.bind"] = "/new_districts(" + contact._bsd_permanentdistrict_value + ")"; /////Lookup Field
            }
            if (UserLogged.Id != null)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }

            return data;
        }

        public async Task LoadAccountsLookup()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='account'>
                                        <attribute name='name' alias='Name'/>
                                        <attribute name='accountid' alias='Id'/>
                                    </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("accounts", fetch);
            if (result == null)
            {
                return;
            }
            foreach (var x in result.value)
            {
                x.Detail = "Account";
                list_account_lookup.Add(x);
            }
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


        //public async Task GetImageCMND()
        //{
        //    frontImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_front.jpg";
        //    behindImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_behind.jpg";

        //    string token = (await CrmHelper.getSharePointToken()).access_token;
        //    var client = BsdHttpClient.Instance();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    var front_request = new HttpRequestMessage(HttpMethod.Get, OrgConfig.SharePointResource
        //                    + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFileByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + frontImage_name + "')/$value");
        //    var front_result = await client.SendAsync(front_request);
        //    if (front_result.IsSuccessStatusCode)
        //    {
        //        singleContact.bsd_mattruoccmnd_base64 = Convert.ToBase64String(front_result.Content.ReadAsByteArrayAsync().Result);
        //    }

        //    var behind_request = new HttpRequestMessage(HttpMethod.Get, OrgConfig.SharePointResource
        //                    + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFileByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + behindImage_name + "')/$value");
        //    var behind_result = await client.SendAsync(behind_request);
        //    if (behind_result.IsSuccessStatusCode)
        //    {
        //        singleContact.bsd_matsaucmnd_base64 = Convert.ToBase64String(behind_result.Content.ReadAsByteArrayAsync().Result);
        //    }
        //}

        //public async Task<bool> UpLoadCMNDFront()
        //{
        //    frontImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_front.jpg";

        //    string token = (await CrmHelper.getSharePointToken()).access_token;

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        if (singleContact.bsd_mattruoccmnd_base64 != null)
        //        {
        //            byte[] arrByteFront = Convert.FromBase64String(singleContact.bsd_mattruoccmnd_base64);

        //            using (var response = client.PostAsync
        //            (new Uri(OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFolderByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "')/Files/add(url='" + frontImage_name + "',overwrite=true)")
        //            , new StreamContent(new MemoryStream(arrByteFront))).Result)
        //            {
        //                return response.IsSuccessStatusCode;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        //public async Task<bool> UpLoadCMNDBehind()
        //{
        //    behindImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_behind.jpg";

        //    string token = (await CrmHelper.getSharePointToken()).access_token;

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //        if (singleContact.bsd_matsaucmnd_base64 != null)
        //        {
        //            byte[] arrByteBehind = Convert.FromBase64String(singleContact.bsd_matsaucmnd_base64);

        //            using (var response = client.PostAsync(
        //            new Uri(OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFolderByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "')/Files/add(url='" + behindImage_name + "',overwrite=true)")
        //            , new StreamContent(new MemoryStream(arrByteBehind))).Result)
        //            {
        //                return response.IsSuccessStatusCode;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        public async Task<bool> CheckCMND(string identitycardnumber, string contactid)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='contact'>
                                    <attribute name='fullname' />
                                    <filter type='and'>
                                        <condition attribute='bsd_identitycardnumber' operator='eq' value='" + identitycardnumber + @"' />
                                        <condition attribute='contactid' operator='ne' value='" + contactid + @"' />
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

        public async Task<bool> CheckPassport(string bsd_passport, string contactid)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='contact'>
                                    <attribute name='fullname' />
                                    <filter type='and'>
                                        <condition attribute='bsd_passport' operator='eq' value='" + bsd_passport + @"' />
                                        <condition attribute='contactid' operator='ne' value='" + contactid + @"' />
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

        public async Task<bool> CheckCCCD(string idcard, string contactid)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='contact'>
                                    <attribute name='fullname' />
                                    <filter type='and'>
                                        <condition attribute='bsd_idcard' operator='eq' value='" + idcard + @"' />
                                        <condition attribute='contactid' operator='ne' value='" + contactid + @"' />
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

        public async Task<bool> CheckEmail(string email, string contactid)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='contact'>
                                    <attribute name='fullname' />
                                    <filter type='and'>
                                        <condition attribute='emailaddress1' operator='eq' value='" + email + @"' />
                                        <condition attribute='contactid' operator='ne' value='" + contactid + @"' />
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
    }
}