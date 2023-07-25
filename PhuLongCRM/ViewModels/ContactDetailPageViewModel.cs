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
using System.Net.Http;
using System.Net.Http.Headers;
using Stormlion.PhotoBrowser;
using System.Collections.Generic;
using PhuLongCRM.Config;
using System.Net;

namespace PhuLongCRM.ViewModels
{
    public class ContactDetailPageViewModel : BaseViewModel
    {
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

        private ObservableCollection<ActivityListModel> cares;
        public ObservableCollection<ActivityListModel> Cares { get => cares; set { cares = value; OnPropertyChanged(nameof(Cares)); } }

        private bool _showMoreCase;
        public bool ShowMoreCase { get => _showMoreCase; set { _showMoreCase = value; OnPropertyChanged(nameof(ShowMoreCase)); } }
        public int PageCase { get; set; } = 1;

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
        public List<Photo> Photos { get; set; }

        public bool IsCurrentRecordOfUser { get; set; }

        private List<OptionSet> _provinces;
        public List<OptionSet> Provinces { get => _provinces; set { _provinces = value; OnPropertyChanged(nameof(Provinces)); } }

        private List<OptionSet> _projects;
        public List<OptionSet> Projects { get => _projects; set { _projects = value; OnPropertyChanged(nameof(Projects)); } }
        public byte[] AvatarArr { get; set; }

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }

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
                                    <attribute name='bsd_new_birthday' />
                                    <attribute name='mobilephone' />
                                    <attribute name='createdon' />
                                    <attribute name='ownerid' alias='owner_id'/>
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
                                    <attribute name='bsd_quantam_nhapho' />
                                    <attribute name='bsd_quantam_khuthuongmai' />
                                    <attribute name='bsd_quantam_datnen' />
                                    <attribute name='bsd_quantam_canho' />
                                    <attribute name='bsd_quantam_bietthu' />
                                    <attribute name='bsd_tieuchi_vitri' />
                                    <attribute name='bsd_tieuchi_thietkenoithatcanho' />
                                    <attribute name='bsd_tieuchi_tangdepcanhodep' />
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
                                    <attribute name='bsd_haveprotector' />
                                    <attribute name='entityimage' />
                                    <attribute name='bsd_employee' alias='employee_id'/>
                                    <order attribute='createdon' descending='true' />
                                    <link-entity name='account' from='accountid' to='parentcustomerid' visible='false' link-type='outer' alias='aa'>
                                          <attribute name='accountid' alias='_parentcustomerid_value' />
                                          <attribute name='bsd_name' alias='parentcustomerid_label' />
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='originatingleadid' link-type='outer'>
                                        <attribute name='leadid' alias='leadid_originated'/>
                                        <attribute name='fullname' alias='leadname_originated'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='bsd_protecter' link-type='outer'>
                                        <attribute name='contactid' alias='protecter_id'/>
                                        <attribute name='bsd_fullname' alias='protecter_name'/>
                                    </link-entity>
                                    <link-entity name='bsd_bankingloan' from='bsd_purchaser' to='contactid' link-type='outer'>
                                        <attribute name='bsd_bankingloanid' alias='bankingloan_id'/>
                                        <filter type='and'>
                                            <condition attribute='statuscode' operator='eq' value='100000000'/>
                                        </filter>
                                    </link-entity>
                                    <filter type='and'>
                                        <condition attribute='contactid' operator='eq' value='" + id + @"' />
                                    </filter>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ContactFormModel>>("contacts", fetch);
            if (result == null || result.value.Any() == false) return;
            var tmp = result.value.FirstOrDefault();
            this.singleContact = tmp;
            this.IsCurrentRecordOfUser = (singleContact.owner_id == UserLogged.Id || singleContact.employee_id == UserLogged.Id) ? true : false;
            if (string.IsNullOrWhiteSpace(singleContact.entityimage))
            {
                singleContact.avatar = singleContact.bsd_fullname;
            }
            else
            {
                singleContact.avatar = singleContact.entityimage;
            }
            await GetImageCMND();
        }

        public async Task GetImageCMND()
        {
            if (this.singleContact.contactid != Guid.Empty)
            {
                GetTokenResponse getTokenResponse = await LoginHelper.getSharePointToken();
                var client = BsdHttpClient.Instance();
                string name_folder = StringFormatHelper.ReplaceNameContact(singleContact.bsd_fullname) + "_" + singleContact.contactid.ToString().Replace("-", "").ToUpper();
                string fileListUrl = $"https://graph.microsoft.com/v1.0/drives/{Config.OrgConfig.Graph_ContactID}/root:/{name_folder}:/children?$select=name,eTag";
                var request = new HttpRequestMessage(HttpMethod.Get, fileListUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getTokenResponse.access_token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<RetrieveMultipleApiResponse<SharePointGraphModel>>(body);

                    if (result == null || result.value.Any() == false)
                    {
                        ShowCMND = false;
                        return;
                    }
                    Photos = new List<Photo>();
                    List<SharePointGraphModel> list = result.value;
                    string url_front = null;
                    string url_behind = null;
                    foreach (var item in list)
                    {
                        if (item.name.Contains("_front.jpg"))
                        {
                            var urlVideo = await CrmHelper.RetrieveImagesSharePoint<RetrieveMultipleApiResponse<GraphThumbnailsUrlModel>>($"{Config.OrgConfig.SP_ContactID}/items/{item.id}/driveItem/thumbnails");
                            url_front = urlVideo.value.SingleOrDefault().large.url;
                            ShowCMND = true;
                        }
                        else if (item.name.Contains("_behind.jpg"))
                        {
                            var urlVideo = await CrmHelper.RetrieveImagesSharePoint<RetrieveMultipleApiResponse<GraphThumbnailsUrlModel>>($"{Config.OrgConfig.SP_ContactID}/items/{item.id}/driveItem/thumbnails");
                            url_behind = urlVideo.value.SingleOrDefault().large.url;
                            ShowCMND = true;
                        }
                    }
                    if (url_front != null)
                    {
                        Photos.Add(new Photo { URL = url_front });
                        singleContact.bsd_mattruoccmnd_source = url_front;
                    }
                    if (url_behind != null)
                    {
                        Photos.Add(new Photo { URL = url_behind });
                        singleContact.bsd_matsaucmnd_source = url_behind;
                    }
                }
            }
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
                                <attribute name='bsd_queuenumber' />
                                <attribute name='bsd_queueforproject' />
                                <attribute name='bsd_queuingfeepaid' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='parentcontactid' operator='eq' uitype='contact' value='{customerId}' />
                                  <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' visible='false' link-type='outer' alias='a_edc3f143ba81e911a83b000d3a07be23'>
                                    <attribute name='bsd_name' alias='bsd_project_name'/>
                                </link-entity>
                                <link-entity name='account' from='accountid' to='parentaccountid' visible='false' link-type='outer' alias='a_87ea9a00777ee911a83b000d3a07fbb4'>
                                    <attribute name='name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='parentcontactid' visible='false' link-type='outer' alias='a_8eea9a00777ee911a83b000d3a07fbb4'>
                                    <attribute name='bsd_fullname' alias='contact_name'/>
                                </link-entity>
                                <link-entity name='product' from='productid' to='bsd_units' visible='false' link-type='outer' alias='a_5025d361ba81e911a83b000d3a07be23'>
                                    <attribute name='name' alias='bsd_units_name'/>
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
                                           <condition attribute='bsd_quotationsigneddate' operator='null' />
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
        public async Task LoadCase()
        {
            if (Cares == null)
                Cares = new ObservableCollection<ActivityListModel>();
            if (singleContact == null || singleContact.contactid == Guid.Empty) return;

            string attribute = string.Empty;
            string value = string.Empty;
            if (singleContact.employee_id != Guid.Empty)
            {
                attribute = "bsd_employee";
                value = singleContact.employee_id.ToString();
            }
            else
            {
                attribute = "ownerid";
                value = singleContact.owner_id.ToString();
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
                                        <condition attribute='regardingobjectid' operator='eq' value='{singleContact.contactid}' />
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
                    PhongThuy.nam_sinh = singleContact.bsd_new_birthday.HasValue ? singleContact.bsd_new_birthday.Value.Year : 0;
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
        public async Task<CrmApiResponse> updateNhuCauDienTich()
        {
            string path = "/contacts(" + singleContact.contactid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_dientich_3060m2"] = singleContact.bsd_dientich_3060m2;
            content["bsd_dientich_6080m2"] = singleContact.bsd_dientich_6080m2;
            content["bsd_dientich_80100m2"] = singleContact.bsd_dientich_80100m2;
            content["bsd_dientich_100120m2"] = singleContact.bsd_dientich_100120m2;
            content["bsd_dientich_lonhon120m2"] = singleContact.bsd_dientich_lonhon120m2;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task<CrmApiResponse> updateTieuChiChonMua()
        {
            string path = "/contacts(" + singleContact.contactid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_tieuchi_vitri"] = singleContact.bsd_tieuchi_vitri;
            content["bsd_tieuchi_phuongthucthanhtoan"] = singleContact.bsd_tieuchi_phuongthucthanhtoan;
            content["bsd_tieuchi_giacanho"] = singleContact.bsd_tieuchi_giacanho;
            content["bsd_tieuchi_nhadautuuytin"] = singleContact.bsd_tieuchi_nhadautuuytin;
            content["bsd_tieuchi_moitruongsong"] = singleContact.bsd_tieuchi_moitruongsong;
            content["bsd_tieuchi_baidauxe"] = singleContact.bsd_tieuchi_baidauxe;
            content["bsd_tieuchi_hethonganninh"] = singleContact.bsd_tieuchi_hethonganninh;
            content["bsd_tieuchi_huongcanho"] = singleContact.bsd_tieuchi_huongcanho;
            content["bsd_tieuchi_hethongcuuhoa"] = singleContact.bsd_tieuchi_hethongcuuhoa;
            content["bsd_tieuchi_nhieutienich"] = singleContact.bsd_tieuchi_nhieutienich;
            content["bsd_tieuchi_ganchosieuthi"] = singleContact.bsd_tieuchi_ganchosieuthi;
            content["bsd_tieuchi_gantruonghoc"] = singleContact.bsd_tieuchi_gantruonghoc;
            content["bsd_tieuchi_ganbenhvien"] = singleContact.bsd_tieuchi_ganbenhvien;
            content["bsd_tieuchi_dientichcanho"] = singleContact.bsd_tieuchi_dientichcanho;
            content["bsd_tieuchi_thietkenoithatcanho"] = singleContact.bsd_tieuchi_thietkenoithatcanho;
            content["bsd_tieuchi_tangdepcanhodep"] = singleContact.bsd_tieuchi_tangdepcanhodep;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task<CrmApiResponse> updateLoaiBDSQuanTam()
        {
            string path = "/contacts(" + singleContact.contactid + ")";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["bsd_quantam_datnen"] = singleContact.bsd_quantam_datnen;
            content["bsd_quantam_canho"] = singleContact.bsd_quantam_canho;
            content["bsd_quantam_bietthu"] = singleContact.bsd_quantam_bietthu;
            content["bsd_quantam_khuthuongmai"] = singleContact.bsd_quantam_khuthuongmai;
            content["bsd_quantam_nhapho"] = singleContact.bsd_quantam_nhapho;

            CrmApiResponse result = await CrmHelper.PatchData(path, content);
            return result;
        }
        public async Task LoadProvince()
        {
            if (Provinces == null)
                Provinces = new List<OptionSet>();
            if (singleContact == null || singleContact.contactid == Guid.Empty) return;

            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_province'>
                                <attribute name='new_name' alias='Label' />   
                                <attribute name='new_provinceid' alias='Val' />   
                                <link-entity name='bsd_contact_new_province' from='new_provinceid' to='new_provinceid' intersect='true'>
                                  <filter>
                                    <condition attribute='contactid' operator='eq' value='{singleContact.contactid}' />
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
                }
                Provinces = list;
            }
        }
        //public async Task<bool> updateNhuCauDiaDiem()
        //{
        //    bool res = true;
        //    if (Provinces != null)
        //    {
        //        foreach (var item in Provinces)
        //        {
        //            string path = $"/contacts({singleContact.contactid})/bsd_contact_new_province/$ref";
        //            IDictionary<string, object> content = new Dictionary<string, object>();
        //            content["@odata.id"] = $"{OrgConfig.ApiUrl}/new_provinces(" + item.Val + ")";
        //            CrmApiResponse result = await CrmHelper.PostData(path, content);
        //            if (!result.IsSuccess)
        //                res = false;
        //            ProvincesForDetele.Remove(item);
        //        }
        //        if (ProvincesForDetele.Count > 0)
        //        {
        //            foreach (var item in ProvincesForDetele)
        //            {
        //                var res_delete = await Delete_NhuCau(item.Val, "bsd_contact_new_province");
        //                if (!res_delete)
        //                    res = false;
        //            }
        //        }
        //    }
        //    return res;
        //}

        public async Task<bool> themNhuCauDiaDiem(OptionSet item)
        {
            string path = $"/contacts({singleContact.contactid})/bsd_contact_new_province/$ref";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["@odata.id"] = $"{OrgConfig.ApiUrl}/new_provinces(" + item.Val + ")";
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            return result.IsSuccess;
            
        }
        public async Task<bool> xoaNhuCauDiaDiem(OptionSet item)
        {
            var result = await Delete_NhuCau(item.Val, "bsd_contact_new_province");
            return result;
        }

        public async Task<Boolean> Delete_NhuCau(string id, string entity)
        {
            string Token = UserLogged.AccessToken;
            var request = $"{OrgConfig.ApiUrl}/contacts({singleContact.contactid})/{entity}(" + id + ")/$ref";

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
        public async Task LoadProject()
        {
            if (Projects == null)
                Projects = new List<OptionSet>();
            if (singleContact == null || singleContact.contactid == Guid.Empty) return;

            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_project'>
                                    <attribute name='bsd_projectid' alias='Val'/>
                                    <attribute name='bsd_name' alias='Label'/>
                                    <link-entity name='bsd_contact_bsd_project' from='bsd_projectid' to='bsd_projectid' intersect='true'>
                                      <filter>
                                        <condition attribute='contactid' operator='eq' value='{singleContact.contactid}' />
                                      </filter>
                                    </link-entity>
                                    </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_projects", fetch);
            if (result != null && result.value.Count > 0)
            {
                List<OptionSet> list = new List<OptionSet>();
                foreach (var item in result.value)
                {
                    list.Add(item);
                }
                Projects = list;
            }
        }
        public async Task<bool> themNhuCauDuAn(OptionSet item)
        {
            string path = $"/contacts({singleContact.contactid})/bsd_contact_bsd_project/$ref";
            IDictionary<string, object> content = new Dictionary<string, object>();
            content["@odata.id"] = $"{OrgConfig.ApiUrl}/bsd_projects(" + item.Val + ")";
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            return result.IsSuccess;

        }
        public async Task<bool> xoaNhuCauDuAn(OptionSet item)
        {
            var result = await Delete_NhuCau(item.Val, "bsd_contact_bsd_project");
            return result;
        }
        public async Task<bool> ChangeAvatar()
        {
            string path = "/contacts(" + singleContact.contactid + ")";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["entityimage"] = AvatarArr;

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
