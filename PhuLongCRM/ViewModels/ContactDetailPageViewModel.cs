using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using PhuLongCRM.Controls;

namespace PhuLongCRM.ViewModels
{
    public class ContactDetailPageViewModel : BaseViewModel
    {
        private ImageSource _myImage;
        public ImageSource MyImage { get => _myImage; set { _myImage = value; OnPropertyChanged(nameof(MyImage)); } }

        private ContactFormModel _singleContact;
        public ContactFormModel singleContact { get { return _singleContact; } set { _singleContact = value; OnPropertyChanged(nameof(singleContact)); } }

        private OptionSet _singleGender;
        public OptionSet singleGender { get => _singleGender; set { _singleGender = value; OnPropertyChanged(nameof(singleGender)); } }

        private OptionSet _singleLocalization;
        public OptionSet SingleLocalization { get => _singleLocalization; set { _singleLocalization = value; OnPropertyChanged(nameof(SingleLocalization)); } }
        private ObservableCollection<QueueFormModel> _list_danhsachdatcho;
        public ObservableCollection<QueueFormModel> list_danhsachdatcho { get => _list_danhsachdatcho; set { _list_danhsachdatcho = value; OnPropertyChanged(nameof(list_danhsachdatcho)); } }

        private bool _showMoreDanhSachDatCho;
        public bool ShowMoreDanhSachDatCho { get => _showMoreDanhSachDatCho; set { _showMoreDanhSachDatCho = value; OnPropertyChanged(nameof(ShowMoreDanhSachDatCho)); } }
        public int PageDanhSachDatCho { get; set; } = 1;

        private ObservableCollection<ReservationListModel> _list_danhsachdatcoc;
        public ObservableCollection<ReservationListModel> list_danhsachdatcoc { get => _list_danhsachdatcoc; set { _list_danhsachdatcoc = value; OnPropertyChanged(nameof(list_danhsachdatcoc)); } }
        private bool _showMoreDanhSachDatCoc;
        public bool ShowMoreDanhSachDatCoc { get => _showMoreDanhSachDatCoc; set { _showMoreDanhSachDatCoc = value; OnPropertyChanged(nameof(ShowMoreDanhSachDatCoc)); } }
        public int PageDanhSachDatCoc { get; set; } = 1;

        private ObservableCollection<ContractModel> _list_danhsachhopdong;
        public ObservableCollection<ContractModel> list_danhsachhopdong { get => _list_danhsachhopdong; set { _list_danhsachhopdong = value; OnPropertyChanged(nameof(list_danhsachhopdong)); } }
        private bool _showMoreDanhSachHopDong;
        public bool ShowMoreDanhSachHopDong { get => _showMoreDanhSachHopDong; set { _showMoreDanhSachHopDong = value; OnPropertyChanged(nameof(ShowMoreDanhSachHopDong)); } }
        public int PageDanhSachHopDong { get; set; } = 1;

        private ObservableCollection<HoatDongListModel> _list_chamsockhachhang;
        public ObservableCollection<HoatDongListModel> list_chamsockhachhang { get => _list_chamsockhachhang; set { _list_chamsockhachhang = value; OnPropertyChanged(nameof(list_chamsockhachhang)); } }
        private bool _showMoreChamSocKhachHang;
        public bool ShowMoreChamSocKhachHang { get => _showMoreChamSocKhachHang; set { _showMoreChamSocKhachHang = value; OnPropertyChanged(nameof(ShowMoreChamSocKhachHang)); } }
        public int PageChamSocKhachHang { get; set; } = 1;

        private PhongThuyModel _PhongThuy;
        public PhongThuyModel PhongThuy { get => _PhongThuy; set { _PhongThuy = value; OnPropertyChanged(nameof(PhongThuy)); } }
        public ObservableCollection<HuongPhongThuy> list_HuongTot { set; get; }
        public ObservableCollection<HuongPhongThuy> list_HuongXau { set; get; }

        private string IMAGE_CMND_FOLDER = "Contact_CMND";

        private string _frontImage;
        public string frontImage { get => _frontImage; set { _frontImage = value; OnPropertyChanged(nameof(frontImage)); } }

        private string _behindImage;
        public string behindImage { get => _behindImage; set { _behindImage = value; OnPropertyChanged(nameof(behindImage)); } }        

        private bool _showCMND;
        public bool ShowCMND { get => _showCMND; set { _showCMND = value; OnPropertyChanged(nameof(ShowCMND)); } }
        public ObservableCollection<PhotoCMND> CollectionCMNDs { get; set; } = new ObservableCollection<PhotoCMND>();
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();

        public string CodeContac = LookUpMultipleTabs.CodeContac;

        public ContactDetailPageViewModel()
        {
            singleContact = new ContactFormModel();
            list_HuongTot = new ObservableCollection<HuongPhongThuy>();
            list_HuongXau = new ObservableCollection<HuongPhongThuy>();
        }

        // load one contat
        public async Task loadOneContact(String id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='contact'>
                                    <attribute name='contactid' />
                                    <attribute name='fullname' />
                                    <attribute name='bsd_fullname' />
                                    <attribute name='bsd_firstname' />
                                    <attribute name='firstname' />
                                    <attribute name='bsd_lastname' />
                                    <attribute name='lastname' />
                                    <attribute name='emailaddress1' />
                                    <attribute name='jobtitle' />
                                    <attribute name='birthdate' />
                                    <attribute name='mobilephone' />
                                    <attribute name='createdon' />
                                    <attribute name='ownerid' />
                                    <attribute name='gendercode' />
                                    <attribute name='bsd_identitycardnumber' />
                                    <attribute name='statuscode' />
                                    <attribute name='bsd_contactaddress' />
                                    <attribute name='statecode' />
                                    <attribute name='bsd_type' />
                                    <attribute name='bsd_localization' />
                                    <attribute name='bsd_haveprotector' />
                                    <attribute name='bsd_dategrant' />
                                    <attribute name='bsd_placeofissue' />
                                    <attribute name='bsd_passport' />
                                    <attribute name='bsd_issuedonpassport' />
                                    <attribute name='bsd_placeofissuepassport' />
                                    <attribute name='bsd_jobtitlevn' />
                                    <attribute name='bsd_taxcode' />
                                    <attribute name='bsd_email2' />
                                    <attribute name='telephone1' />
                                    <attribute name='fax' />
                                    <attribute name='bsd_totaltransaction' />
                                    <attribute name='bsd_customergroup' />
                                    <attribute name='bsd_diachi' />
                                    <attribute name='bsd_housenumberstreet' />
                                    <attribute name='bsd_housenumber' />
                                    <attribute name='bsd_country' />
                                    <attribute name='bsd_province' />
                                    <attribute name='bsd_district' />
                                    <attribute name='bsd_permanentaddress1' />
                                    <attribute name='bsd_diachithuongtru' />
                                    <attribute name='bsd_permanentcountry' />
                                    <attribute name='bsd_permanentprovince' />
                                    <attribute name='bsd_permanentdistrict' />
                                    <attribute name='bsd_permanentaddress' />
                                    <attribute name='bsd_permanenthousenumber' />
                                    <attribute name='bsd_identitycard' />
                                    <attribute name='bsd_identitycarddategrant' />
                                    <attribute name='bsd_placeofissueidentitycard' />
                                    <attribute name='bsd_birthdate' />
                                    <attribute name='bsd_birthmonth' />
                                    <attribute name='bsd_birthyear' />
                                    <attribute name='bsd_postalcode' />
                                    <attribute name='bsd_qrcode' />
                                    <attribute name='bsd_customercode' />
                                    <order attribute='createdon' descending='true' />
                                    <link-entity name='account' from='accountid' to='parentcustomerid' visible='false' link-type='outer' alias='aa'>
                                          <attribute name='accountid' alias='_parentcustomerid_value' />
                                          <attribute name='bsd_name' alias='parentcustomerid_label' />
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='originatingleadid' link-type='outer'>
                                        <attribute name='leadid' alias='leadid_originated'/>
                                    </link-entity>
                                    <filter type='and'>
                                        <condition attribute='contactid' operator='eq' value='" + id + @"' />
                                    </filter>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetch);
            if (result == null)
            {
                return;
            }
            var tmp = result.value.FirstOrDefault();
            this.singleContact = tmp;
        }

        public async Task GetImageCMND()
        {
            ShowCMND = true;

            //var result = await CrmHelper.RetrieveImagesSharePoint<RetrieveMultipleApiResponse<Thumbnail>>();

            //var a = result.value;
            //MyImage = a[0].medium.url;

            //if (this.singleContact.contactid != Guid.Empty)
            //{
            //    var frontImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_front.jpg";
            //    var behindImage_name = this.singleContact.contactid.ToString().Replace("-", String.Empty).ToUpper() + "_behind.jpg";

            //    string token = (await CrmHelper.getSharePointToken()).access_token;
            //    var client = BsdHttpClient.Instance();
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    var front_request = new HttpRequestMessage(HttpMethod.Get, OrgConfig.SharePointResource
            //                    + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFileByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + frontImage_name + "')/$value");
            //    var front_result = await client.SendAsync(front_request);
            //    if (front_result.IsSuccessStatusCode)
            //    {
            //        ShowCMND = true;
            //        singleContact.bsd_mattruoccmnd_base64 = Convert.ToBase64String(front_result.Content.ReadAsByteArrayAsync().Result);
            //        CollectionCMNDs.Add(new PhotoCMND { ImageSoure = singleContact.bsd_mattruoccmnd_source });
            //        frontImage = OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_layouts/15/download.aspx?SourceUrl=/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + frontImage_name + "&access_token=" + token;
            //    }

            //    var behind_request = new HttpRequestMessage(HttpMethod.Get, OrgConfig.SharePointResource
            //                    + "/sites/" + OrgConfig.SharePointSiteName + "/_api/web/GetFileByServerRelativeUrl('/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + behindImage_name + "')/$value");
            //    var behind_result = await client.SendAsync(behind_request);
            //    if (behind_result.IsSuccessStatusCode)
            //    {
            //        ShowCMND = true;
            //        var abc = behind_result.Content.ReadAsByteArrayAsync().Result;
            //        singleContact.bsd_matsaucmnd_base64 = Convert.ToBase64String(behind_result.Content.ReadAsByteArrayAsync().Result);
            //        CollectionCMNDs.Add(new PhotoCMND { ImageSoure = singleContact.bsd_matsaucmnd_source });
            //        // behindImage = OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_layouts/15/download.aspx?SourceUrl=/sites/" + OrgConfig.SharePointSiteName + "/" + IMAGE_CMND_FOLDER + "/" + behindImage_name + "&access_token=" + token;
            //    }
            //}
        }

        // giao dich

        //DANH SACH DAT CHO
        public async Task LoadQueuesForContactForm(string customerId)
        {
            string fetchXml = $@"<fetch version='1.0' count='3' page='{PageDanhSachDatCho}' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='opportunity'>
                                <attribute name='name' />
                                <attribute name='customerid' />
                                <attribute name='createdon' />
                                <attribute name='bsd_queuingexpired' />
                                <attribute name='statuscode' />
                                <attribute name='opportunityid' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='parentcontactid' operator='eq' uitype='contact' value='{customerId}' />
                                  <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                </filter> 
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='inner' alias='ab'>
                                    <attribute name='bsd_name' alias='bsd_project_name'/>
                                </link-entity>
                                <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer' alias='a_434f5ec290d1eb11bacc000d3a80021e'>
                                  <attribute name='name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_884f5ec290d1eb11bacc000d3a80021e'>
                                  <attribute name='bsd_fullname' alias='contact_name'/>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueueFormModel>>("opportunities", fetchXml);
            if (result == null || result.value.Count == 0)
            {
                ShowMoreDanhSachDatCho = false;
                return;
            }
            var data = result.value;
            ShowMoreDanhSachDatCho = data.Count < 3 ? false : true;
            foreach (var item in data)
            {
                QueueFormModel queue = new QueueFormModel();
                queue = item;
                if (!string.IsNullOrWhiteSpace(item.contact_name))
                {
                    queue.customer_name = item.contact_name;
                }
                else if (!string.IsNullOrWhiteSpace(item.account_name))
                {
                    queue.customer_name = item.account_name;
                }
                list_danhsachdatcho.Add(queue);
            }
        }
        // DANH SACH DAT COC
        public async Task LoadReservationForContactForm(string customerId)
        {
            string fetch = $@"<fetch version='1.0' count='5' page='{PageDanhSachDatCoc}' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='quote'>
                                <attribute name='name' />
                                <attribute name='totalamount' />
                                <attribute name='bsd_unitno' alias='bsd_unitno_id' />
                                <attribute name='statuscode' />
                                <attribute name='bsd_projectid' alias='bsd_project_id' />
                                <attribute name='quoteid' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='customerid' operator='eq' value='{customerId}' />
                                  <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    <filter type='or'>
                                       <condition attribute='statuscode' operator='in'>
                                           <value>100000000</value>
                                            <value>100000001</value>
                                            <value>100000006</value>
                                            <value>861450001</value>
                                            <value>861450002</value>
                                            <value>4</value>                
                                            <value>3</value>
                                            <value>100000007</value>
                                       </condition>
                                       <filter type='and'>
                                           <condition attribute='statuscode' operator='in'>
                                               <value>100000009</value>
                                               <value>6</value>
                                           </condition>
                                           <condition attribute='bsd_quotationsigneddate' operator='not-null' />
                                       </filter>
                                     </filter>
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectid' visible='false' link-type='outer' alias='a'>
                                    <attribute name='bsd_name' alias='bsd_project_name' />
                                </link-entity>
                                <link-entity name='product' from='productid' to='bsd_unitno' visible='false' link-type='outer' alias='b'>
                                  <attribute name='name' alias='bsd_unitno_name' />
                                </link-entity>
                                <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer' alias='c'>
                                  <attribute name='bsd_name' alias='purchaser_accountname' />
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='d'>
                                  <attribute name='bsd_fullname' alias='purchaser_contactname' />
                                </link-entity>
                              </entity>
                        </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationListModel>>("quotes", fetch);
            if (result == null || result.value.Count == 0)
            {
                ShowMoreDanhSachDatCoc = false;
                return;
            }
            var data = result.value;

            if (data.Count < 5)
            {
                ShowMoreDanhSachDatCoc = false;
            }
            else
            {
                ShowMoreDanhSachDatCoc = true;
            }

            foreach (var x in data)
            {
                list_danhsachdatcoc.Add(x);
            }
        }

        // DANH SACH HOP DONG
        public async Task LoadOptoinEntryForContactForm(string customerId)
        {
            string fetch = $@"<fetch version='1.0' count='3' page='{PageDanhSachHopDong}' output-format='xml-platform' mapping='logical' distinct='false'>
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
                                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                        <condition attribute='customerid' operator='eq' value='{customerId}' />               
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContractModel>>("salesorders", fetch);
            if (result == null || result.value.Count == 0)
            {
                ShowMoreDanhSachHopDong = false;
                return;
            }
            var data = result.value;
            ShowMoreDanhSachHopDong = data.Count < 3 ? false : true;

            foreach (var x in data)
            {
                list_danhsachhopdong.Add(x);
            }
        }

        // CHAM SOC KHACH HANG
        public async Task LoadCaseForContactForm()
        {
            if(list_chamsockhachhang != null && singleContact.contactid != Guid.Empty)
            {
                await Task.WhenAll(
                LoadActiviy("task", "tasks"),
                LoadActiviy("phonecall", "phonecalls"),
                LoadActiviy("appointment", "appointments")
                //LoadTasks(singleContact.contactid),
                //LoadMettings(singleContact.contactid),
                //LoadPhoneCalls(singleContact.contactid)
                );
            }
            ShowMoreChamSocKhachHang = list_chamsockhachhang.Count < (3* PageChamSocKhachHang) ? false : true;
        }

        public async Task LoadActiviy(string entity, string entitys)
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
            string fetch = $@"<fetch version='1.0' count='3' page='{PageChamSocKhachHang}' output-format='xml-platform' mapping='logical' distinct='true'>
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
                                            <condition entityname='party' attribute='partyid' operator='eq' value='{singleContact.contactid}'/>
                                            <condition attribute='regardingobjectid' operator='eq' value='{singleContact.contactid}' />
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
                    list_chamsockhachhang.Add(item);
                }
            }
            else
            {
                foreach (var item in data)
                {
                    item.customer = item.regarding_name;
                    list_chamsockhachhang.Add(item);
                }
            }
        }

        public async Task LoadTasks(Guid contactID)
        {
            string fetchXml = $@"<fetch version='1.0' count='3' page='{PageChamSocKhachHang}' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='task'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />
                                    <attribute name='createdon' />
                                    <order attribute='modifiedon' descending='false' />
                                    <filter type='and'>
                                        <filter type='or'>
                                            <condition entityname='party' attribute='partyid' operator='eq' value='{contactID}'/>
                                            <condition attribute='regardingobjectid' operator='eq' value='{contactID}' />
                                        </filter>
                                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                     <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer' alias='ae'>
                                        <attribute name='bsd_name' alias='accounts_bsd_name'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer' alias='af'>
                                        <attribute name='fullname' alias='contact_bsd_fullname'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='ag'>
                                        <attribute name='fullname' alias='lead_fullname'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<HoatDongListModel>>("tasks", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                if (!string.IsNullOrWhiteSpace(item.contact_bsd_fullname))
                {
                    item.customer = item.contact_bsd_fullname;
                }
                if (!string.IsNullOrWhiteSpace(item.accounts_bsd_name))
                {
                    item.customer = item.accounts_bsd_name;
                }
                if (!string.IsNullOrWhiteSpace(item.lead_fullname))
                {
                    item.customer = item.lead_fullname;
                }

                this.list_chamsockhachhang.Add(item);
            }
        }

        public async Task LoadMettings(Guid contactID)
        {
            string fetchXml = $@"<fetch version='1.0' count='3' page='{PageChamSocKhachHang}' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='appointment'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />   
                                    <attribute name='createdon' />
                                    <order attribute='modifiedon' descending='false' />
                                   <filter type='and'>
                                        <filter type='or'>
                                            <condition entityname='party' attribute='partyid' operator='eq' value='{contactID}'/>
                                            <condition attribute='regardingobjectid' operator='eq' value='{contactID}' />
                                        </filter>
                                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                   <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer' alias='ae'>
                                        <attribute name='bsd_name' alias='accounts_bsd_name'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer' alias='af'>
                                        <attribute name='fullname' alias='contact_bsd_fullname'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='ag'>
                                        <attribute name='fullname' alias='lead_fullname'/>
                                    </link-entity>
                                    <link-entity name='activityparty' from='activityid' to='activityid' link-type='outer' alias='aee'>
                                        <filter type='and'>
                                            <condition attribute='participationtypemask' operator='eq' value='5' />
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
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<HoatDongListModel>>("appointments", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                var meet = list_chamsockhachhang.FirstOrDefault(x => x.activityid == item.activityid);
                if (meet != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                    {
                        string new_customer = ", " + item.callto_contact_name;
                        meet.customer += new_customer;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_account_name))
                    {
                        string new_customer = ", " + item.callto_account_name;
                        meet.customer += new_customer;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                    {
                        string new_customer = ", " + item.callto_lead_name;
                        meet.customer += new_customer;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                    {
                        item.customer = item.callto_contact_name;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_account_name))
                    {
                        item.customer = item.callto_account_name;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                    {
                        item.customer = item.callto_lead_name;
                    }
                    this.list_chamsockhachhang.Add(item);
                }
            }
        }

        public async Task LoadPhoneCalls(Guid contactID)
        {
            string fetchXml = $@"<fetch version='1.0' count='3' page='{PageChamSocKhachHang}' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='phonecall'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />
                                    <attribute name='createdon' />
                                    <order attribute='modifiedon' descending='false' />
                                    <filter type='and'>
                                        <filter type='or'>
                                            <condition entityname='party' attribute='partyid' operator='eq' value='{contactID}'/>
                                            <condition attribute='regardingobjectid' operator='eq' value='{contactID}' />
                                        </filter>
                                        <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                     <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer' alias='ae'>
                                        <attribute name='bsd_name' alias='accounts_bsd_name'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer' alias='af'>
                                        <attribute name='fullname' alias='contact_bsd_fullname'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='ag'>
                                        <attribute name='fullname' alias='lead_fullname'/>
                                    </link-entity>
                                    <link-entity name='activityparty' from='activityid' to='activityid' link-type='outer' alias='aee'>
                                        <filter type='and'>
                                            <condition attribute='participationtypemask' operator='eq' value='2' />
                                        </filter>
                                        <link-entity name='contact' from='contactid' to='partyid' link-type='outer' alias='aff'>
                                            <attribute name='fullname' alias='callto_contact_name'/>
                                        </link-entity>
                                        <link-entity name='account' from='accountid' to='partyid' link-type='outer' alias='agg'>
                                            <attribute name='bsd_name' alias='callto_accounts_name'/>
                                        </link-entity>
                                        <link-entity name='lead' from='leadid' to='partyid' link-type='outer' alias='ahh'>
                                            <attribute name='fullname' alias='callto_lead_name'/>
                                        </link-entity>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<HoatDongListModel>>("phonecalls", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                {
                    item.customer = item.callto_contact_name;
                }
                if (!string.IsNullOrWhiteSpace(item.callto_account_name))
                {
                    item.customer = item.callto_account_name;
                }
                if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                {
                    item.customer = item.callto_lead_name;
                }

                this.list_chamsockhachhang.Add(item);
            }
        }

        // phon thuy
        public void LoadPhongThuy()
        {
            if (singleContact.gendercode != null)
            {
                singleGender = ContactGender.GetGenderById(singleContact.gendercode);
            }
            if (list_HuongTot != null || list_HuongXau != null)
            {
                list_HuongTot.Clear();
                list_HuongXau.Clear();
                if (singleContact != null && singleContact.gendercode != null && singleGender != null)
                {
                    PhongThuy.gioi_tinh = Int32.Parse(singleContact.gendercode);
                    PhongThuy.nam_sinh = singleContact.birthdate.HasValue ? singleContact.birthdate.Value.Year : 0;
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

        // Save qrcode
        public async Task<bool> SaveQRCode(string qrCode)
        {
            string path = "/contacts(" + this.singleContact.contactid + ")";
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
    }

    public class User
    {
        public string email { get; set; }
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class CreatedBy
    {
        public User user { get; set; }
    }

    public class LastModifiedBy
    {
        public User user { get; set; }
    }

    public class ParentReference
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string path { get; set; }
    }

    public class Hashes
    {
        public string quickXorHash { get; set; }
    }

    public class File
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }

    public class FileSystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Root
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }

        [JsonProperty("@microsoft.graph.downloadUrl")]
        public string MicrosoftGraphDownloadUrl { get; set; }
        public DateTime createdDateTime { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string cTag { get; set; }
        public int size { get; set; }
        public CreatedBy createdBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public ParentReference parentReference { get; set; }
        public File file { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
        public Image image { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Large
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Medium
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Small
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Thumbnail
    {
        public string id { get; set; }
        public Large large { get; set; }
        public Medium medium { get; set; }
        public Small small { get; set; }
    }



}
