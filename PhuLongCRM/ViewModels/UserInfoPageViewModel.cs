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
    public class UserInfoPageViewModel : BaseViewModel
    {
        private ContactFormModel _contactModel;
        public ContactFormModel ContactModel { get => _contactModel; set { _contactModel = value;OnPropertyChanged(nameof(ContactModel)); } }

        public byte[] AvatarArr { get; set; }
        private string _avatar;
        public string Avatar { get => _avatar; set { _avatar = value; OnPropertyChanged(nameof(Avatar)); } }

        private string _userName;
        public string UserName { get => _userName; set { _userName = value;OnPropertyChanged(nameof(UserName)); } }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(nameof(Address)); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }

        private string _oldPassword;
        public string OldPassword { get => _oldPassword; set { _oldPassword = value; OnPropertyChanged(nameof(OldPassword)); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(nameof(NewPassword)); } }

        private string _confirmNewPassword;
        public string ConfirmNewPassword { get => _confirmNewPassword; set { _confirmNewPassword = value; OnPropertyChanged(nameof(ConfirmNewPassword)); } }

        private OptionSet _gender;
        public OptionSet Gender { get => _gender; set { _gender = value;OnPropertyChanged(nameof(Gender)); } }

        public ObservableCollection<LookUp> list_country_lookup { get; set; } = new ObservableCollection<LookUp>();
        public ObservableCollection<LookUp> list_province_lookup { get; set; } = new ObservableCollection<LookUp>();
        public ObservableCollection<LookUp> list_district_lookup { get; set; } = new ObservableCollection<LookUp>();

        private string _addressLine1Contact;
        public string AddressLine1Contact { get => _addressLine1Contact; set { _addressLine1Contact = value; OnPropertyChanged(nameof(AddressLine1Contact)); } }

        private string _addressPostalCodeContact;
        public string AddressPostalCodeContact { get => _addressPostalCodeContact; set { _addressPostalCodeContact = value; OnPropertyChanged(nameof(AddressPostalCodeContact)); } }

        private List<OptionSet> _genders;
        public List<OptionSet> Genders { get => _genders; set { _genders = value; OnPropertyChanged(nameof(Genders)); } }

        private LookUp _addressCountryContact;
        public LookUp AddressCountryContact
        {
            get => _addressCountryContact;
            set
            {
                _addressCountryContact = value;
                OnPropertyChanged(nameof(AddressCountryContact));
                AddressStateProvinceContact = null;
                list_province_lookup.Clear();
            }
        }

        private LookUp _addressStateProvinceContact;
        public LookUp AddressStateProvinceContact
        {
            get => _addressStateProvinceContact;
            set
            {
                _addressStateProvinceContact = value;
                OnPropertyChanged(nameof(AddressStateProvinceContact));
                AddressCityContact = null;
                list_district_lookup.Clear();
            }
        }

        private LookUp _addressCityContact;
        public LookUp AddressCityContact { get => _addressCityContact; set { _addressCityContact = value; OnPropertyChanged(nameof(AddressCityContact)); } }

        public UserInfoPageViewModel()
        {
            Password = UserLogged.Password;
            this.Genders = new List<OptionSet>() { new OptionSet("1", "Nam"), new OptionSet("2", "Nữ"), new OptionSet("100000000", "Khác") };
            this.Avatar = UserLogged.Avartar;
            this.UserName = UserLogged.User;
        }

        public async Task LoadContact()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='contact'>
                                    <attribute name='bsd_fullname' />
                                    <attribute name='mobilephone' />
                                    <attribute name='bsd_identitycardnumber' />
                                    <attribute name='gendercode' />
                                    <attribute name='emailaddress1' />
                                    <attribute name='createdon' />
                                    <attribute name='birthdate' />
                                    <attribute name='contactid' />
                                    <attribute name='bsd_postalcode' />
                                    <attribute name='bsd_housenumberstreet' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='contactid' operator='eq' value='{UserLogged.ContactId}'/>
                                    </filter>
                                    <link-entity name='bsd_country' from='bsd_countryid' to='bsd_country' visible='false' link-type='outer' alias='a_8b5241be19dbeb11bacb002248168cad'>
                                        <attribute name='bsd_countryid' alias='_bsd_country_value'/>
                                        <attribute name='bsd_countryname' alias='bsd_country_label'/>
                                    </link-entity>
                                    <link-entity name='new_province' from='new_provinceid' to='bsd_province' visible='false' link-type='outer' alias='a_8fd440dc19dbeb11bacb002248168cad'>
                                        <attribute name='new_provinceid' alias='_bsd_province_value'/>
                                        <attribute name='new_name' alias='bsd_province_label'/>
                                    </link-entity>
                                    <link-entity name='new_district' from='new_districtid' to='bsd_district' visible='false' link-type='outer' alias='a_50d440dc19dbeb11bacb002248168cad'>
                                        <attribute name='new_districtid' alias='_bsd_district_value'/>
                                        <attribute name='new_name' alias='bsd_district_label'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetchXml);
            if (result == null || result.value.Any() == false) return;

            this.ContactModel = result.value.SingleOrDefault();
            SetAddress();
            this.Gender = this.Genders.SingleOrDefault(x => x.Val == ContactModel.gendercode);
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
            if (result == null || result.value.Any() == false) return;
            foreach (var x in result.value)
            {
                list_country_lookup.Add(x);
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
            if (result == null || result.value.Any() ==false) return;
            foreach (var x in result.value)
            {
                list_province_lookup.Add(x);
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
            if (result == null || result.value.Any() ==false) return;
            foreach (var x in result.value)
            {
                list_district_lookup.Add(x);
            }
        }

        public void SetAddress()
        {
            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.ContactModel.bsd_housenumberstreet))
            {
                address.Add(this.ContactModel.bsd_housenumberstreet);
            }

            if (!string.IsNullOrWhiteSpace(this.ContactModel.bsd_district_label))
            {
                address.Add(this.ContactModel.bsd_district_label);
            }

            if (!string.IsNullOrWhiteSpace(this.ContactModel.bsd_province_label))
            {
                address.Add(this.ContactModel.bsd_province_label);
            }

            if (!string.IsNullOrWhiteSpace(this.ContactModel.bsd_postalcode))
            {
                address.Add(this.ContactModel.bsd_postalcode);
            }

            if (!string.IsNullOrWhiteSpace(this.ContactModel.bsd_country_label))
            {
                address.Add(this.ContactModel.bsd_country_label);
            }

            this.Address = string.Join(", ", address);
        }

        public async Task<bool> ChangePassword()
        {
            string path = $"/bsd_employees({UserLogged.Id})";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_password"] = this.ConfirmNewPassword;

            var content = data as object;
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

        public async Task<bool> ChangeAvatar()
        {
            string path = $"/bsd_employees({UserLogged.Id})";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_avatar"] = this.AvatarArr;
            
            var content = data as object;
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

        public async Task<bool> UpdateUserInfor()
        {
            string path = $"/contacts({UserLogged.ContactId})";
            var content = await GetContent();
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

        public async Task<Boolean> DeletLookup(string fieldName, Guid contactId)
        {
            var result = await CrmHelper.SetNullLookupField("contacts", contactId, fieldName);
            return result.IsSuccess;
        }

        private async Task<object> GetContent()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //data["lastname"] = this.ContactModel.bsd_fullname;
            //data["bsd_fullname"] = this.ContactModel.bsd_fullname;
            data["emailaddress1"] = this.ContactModel.emailaddress1;
            data["mobilephone"] = this.ContactModel.mobilephone;
            data["birthdate"] = (DateTime.Parse(this.ContactModel.birthdate.ToString()).ToLocalTime()).ToString("yyyy-MM-dd");
            data["gendercode"] = this.Gender?.Val;
            data["bsd_contactaddress"] = this.Address;
            data["bsd_housenumberstreet"] = this.ContactModel.bsd_housenumberstreet;
            data["bsd_postalcode"] = this.ContactModel.bsd_postalcode;

            if (this.ContactModel._bsd_country_value == null)
            {
                await DeletLookup("bsd_country", this.ContactModel.contactid);
            }
            else
            {
                data["bsd_country@odata.bind"] = "/bsd_countries(" + this.ContactModel._bsd_country_value + ")"; /////Lookup Field
            }

            if (this.ContactModel._bsd_province_value == null)
            {
                await DeletLookup("bsd_province", this.ContactModel.contactid);
            }
            else
            {
                data["bsd_province@odata.bind"] = "/new_provinces(" + this.ContactModel._bsd_province_value + ")"; /////Lookup Field
            }

            if (this.ContactModel._bsd_district_value == null)
            {
                await DeletLookup("bsd_district", this.ContactModel.contactid);
            }
            else
            {
                data["bsd_district@odata.bind"] = "/new_districts(" + this.ContactModel._bsd_district_value + ")"; /////Lookup Field
            }

            return data;
        }
    }
}
