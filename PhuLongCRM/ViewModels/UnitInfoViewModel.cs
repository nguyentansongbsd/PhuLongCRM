using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Stormlion.PhotoBrowser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class UnitInfoViewModel : BaseViewModel
    {
        private List<CollectionData> _collections;
        public List<CollectionData> Collections { get => _collections; set { _collections = value; OnPropertyChanged(nameof(Collections)); } }

        public List<Photo> Photos;
        public PhotoBrowser photoBrowser;

        private int _totalMedia;
        public int TotalMedia { get => _totalMedia; set { _totalMedia = value; OnPropertyChanged(nameof(TotalMedia)); } }

        private int _totalPhoto;
        public int TotalPhoto { get => _totalPhoto; set { _totalPhoto = value; OnPropertyChanged(nameof(TotalPhoto)); } }

        private bool _showCollections;
        public bool ShowCollections { get => _showCollections; set { _showCollections = value; OnPropertyChanged(nameof(ShowCollections)); } }
        public Guid UnitId { get; set; }

        private ObservableCollection<ReservationListModel> _bangTinhGiaList;
        public ObservableCollection<ReservationListModel> BangTinhGiaList { get => _bangTinhGiaList; set { _bangTinhGiaList = value; OnPropertyChanged(nameof(BangTinhGiaList)); } }

        public ObservableCollection<QueuesModel> _list_danhsachdatcho;
        public ObservableCollection<QueuesModel> list_danhsachdatcho { get => _list_danhsachdatcho; set { _list_danhsachdatcho = value; OnPropertyChanged(nameof(list_danhsachdatcho)); } }
        public ObservableCollection<ReservationListModel> list_danhsachdatcoc { get; set; } = new ObservableCollection<ReservationListModel>();
        public ObservableCollection<ContractModel> list_danhsachhopdong { get; set; } = new ObservableCollection<ContractModel>();

        private UnitInfoModel _unitInfo;
        public UnitInfoModel UnitInfo { get => _unitInfo; set { _unitInfo = value; OnPropertyChanged(nameof(UnitInfo)); } }

        private OptionSet _diretion;
        public OptionSet Direction { get => _diretion; set { _diretion = value; OnPropertyChanged(nameof(Direction)); } }

        private string _view;
        public string View { get => _view; set { _view = value; OnPropertyChanged(nameof(View)); } }

        private StatusCodeModel _statusCode;
        public StatusCodeModel StatusCode { get => _statusCode; set { _statusCode = value; OnPropertyChanged(nameof(StatusCode)); } }

        private bool _showMoreDanhSachDatCho;
        public bool ShowMoreDanhSachDatCho { get => _showMoreDanhSachDatCho; set { _showMoreDanhSachDatCho = value; OnPropertyChanged(nameof(ShowMoreDanhSachDatCho)); } }

        private bool _showMoreDanhSachDatCoc;
        public bool ShowMoreDanhSachDatCoc { get => _showMoreDanhSachDatCoc; set { _showMoreDanhSachDatCoc = value; OnPropertyChanged(nameof(ShowMoreDanhSachDatCoc)); } }

        private bool _showMoreDanhSachHopDong;
        public bool ShowMoreDanhSachHopDong { get => _showMoreDanhSachHopDong; set { _showMoreDanhSachHopDong = value; OnPropertyChanged(nameof(ShowMoreDanhSachHopDong)); } }

        private bool _isShowBtnBangTinhGia;
        public bool IsShowBtnBangTinhGia { get => _isShowBtnBangTinhGia; set { _isShowBtnBangTinhGia = value; OnPropertyChanged(nameof(IsShowBtnBangTinhGia)); } }

        private bool _showMoreBangTinhGia;
        public bool ShowMoreBangTinhGia { get => _showMoreBangTinhGia; set { _showMoreBangTinhGia = value; OnPropertyChanged(nameof(ShowMoreBangTinhGia)); } }

        public int PageDanhSachHopDong { get; set; } = 1;
        public int PageDanhSachDatCoc { get; set; } = 1;
        public int PageDanhSachDatCho { get; set; } = 1;
        public int PageBangTinhGia { get; set; } = 1;

        public bool IsLoaded { get; set; } = false;

        public bool IsVip { get; set; } = false;

        private EventModel _event;
        public EventModel Event { get => _event; set { _event = value; OnPropertyChanged(nameof(Event)); } }

        public UnitInfoViewModel()
        {
            list_danhsachdatcho = new ObservableCollection<QueuesModel>();
            Photos = new List<Photo>();
            Collections = new List<CollectionData>();
            photoBrowser = new PhotoBrowser
            {
                Photos = Photos,
            };
        }

        public async Task LoadUnit()
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='product'>
                <attribute name='productid' />
                <attribute name='bsd_units' />
                <attribute name='name' />
                <attribute name='productnumber' />
                <attribute name='bsd_queuingfee' />
                <attribute name='bsd_depositamount' />
                <attribute name='statuscode' />
                <attribute name='bsd_areavariance' />
                <attribute name='bsd_constructionarea' />
                <attribute name='bsd_netsaleablearea' />
                <attribute name='price' />
                <attribute name='bsd_landvalueofunit' />
                <attribute name='bsd_landvalue' />
                <attribute name='bsd_maintenancefeespercent' />
                <attribute name='bsd_maintenancefees' />
                <attribute name='bsd_taxpercent'/>
                <attribute name='bsd_vat' />
                <attribute name='bsd_estimatehandoverdate' />
                <attribute name='bsd_numberofmonthspaidmf' />
                <attribute name='bsd_managementamountmonth' />
                <attribute name='bsd_handovercondition' />
                <attribute name='bsd_direction' />
                <attribute name='bsd_vippriority' />
                <attribute name='bsd_viewphulong' />
                <attribute name='bsd_phaseslaunchid' alias='bsd_phaseslaunch_id' />
                <filter type='and'>
                  <condition attribute='productid' operator='eq' uitype='product' value='" + UnitId.ToString() + @"' />
                </filter>
                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectcode' visible='false' link-type='outer' alias='a_a77d98e66ce2e811a94e000d3a1bc2d1'>
                  <attribute name='bsd_name' alias='bsd_project_name' />
                </link-entity>
                <link-entity name='bsd_floor' from='bsd_floorid' to='bsd_floor' visible='false' link-type='outer' alias='a_4d73a1e06ce2e811a94e000d3a1bc2d1'>
                  <attribute name='bsd_name' alias='bsd_floor_name' />
                </link-entity>
                <link-entity name='bsd_block' from='bsd_blockid' to='bsd_blocknumber' visible='false' link-type='outer' alias='a_290ca3da6ce2e811a94e000d3a1bc2d1'>
                  <attribute name='bsd_name' alias='bsd_block_name' />
                </link-entity>
                <link-entity name='bsd_unittype' from='bsd_unittypeid' to='bsd_unittype' visible='false' link-type='outer' alias='a_493690ec6ce2e811a94e000d3a1bc2d1'>
                  <attribute name='bsd_name'  alias='bsd_unittype_name'/>
                </link-entity>
                <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='outer' alias='ac'>
                  <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='outer' alias='ad'>
                    <attribute name='bsd_eventid' alias='event_id' />
                    <filter type='and'>
                        <condition attribute='statuscode' operator='eq' value='100000000' />
                        <condition attribute='bsd_eventid' operator='not-null' />
                    </filter>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<UnitInfoModel>>("products", fetchXml);
            if (result == null || result.value.Count == 0) return;
            UnitInfo = result.value.FirstOrDefault();
            //await LoadAllCollection();
        }

        public async Task LoadQueues()
        {
            string fetch = $@"<fetch version='1.0' count='5' page='{PageDanhSachDatCho}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='opportunity'>
                        <attribute name='name' />
                        <attribute name='customerid' />
                        <attribute name='bsd_bookingtime' />
                        <attribute name='createdon' />
                        <attribute name='statuscode' />
                        <attribute name='bsd_queuingexpired' />
                        <attribute name='opportunityid' />
                        <order attribute='statecode' descending='false' />
                        <order attribute='statuscode' descending='true' />
                        <filter type='and'>
                          <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                          <condition attribute='bsd_units' operator='eq' value='{UnitInfo.productid}' />
                        </filter>
                        <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='fullname' alias='contact_name'/>
                        </link-entity>
                        <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='name'  alias='account_name'/>
                        </link-entity>
                        <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' visible='false' link-type='outer' alias='a_805e44d019dbeb11bacb002248168cad'>
                          <attribute name='bsd_name' alias='project_name'/>
                        </link-entity>
                      </entity>
                    </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueuesModel>>("opportunities", fetch);
            if (result == null || result.value.Count == 0) return;

            IsLoaded = true;
            var data = result.value;
            ShowMoreDanhSachDatCho = data.Count < 5 ? false : true;

            foreach (var item in data)
            {
                list_danhsachdatcho.Add(item);
            }
            List<QueuesModel> list_sort = new List<QueuesModel>();
            list_sort = list_danhsachdatcho.OrderByDescending(num => num, new QueuesModel()).ToList();
            list_danhsachdatcho.Clear();
            foreach (var item in list_sort)
            {
                list_danhsachdatcho.Add(item);
            }
        }

        public async Task LoadDanhSachBangTinhGia()
        {
            string fetchXml = $@"<fetch version='1.0' count='5' page='{PageBangTinhGia}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='quote'>
                        <attribute name='name' />
                        <attribute name='totalamount' />
                        <attribute name='bsd_unitno' alias='bsd_unitno_id' />
                        <attribute name='statuscode' />
                        <attribute name='bsd_projectid' alias='bsd_project_id' />
                        <attribute name='quoteid' />
                        <order attribute='createdon' descending='true' />
                        <filter type='and'>
                            <condition attribute='bsd_unitno' operator='eq' value='{UnitInfo.productid}'/>
                            <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                            <filter type='or'>
                              <condition attribute='statuscode' operator='in'>
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationListModel>>("quotes", fetchXml);
            if (result == null || result.value.Any() == false) return;

            this.ShowMoreBangTinhGia = result.value.Count > 4 ? true : false;

            foreach (var item in result.value)
            {
                this.BangTinhGiaList.Add(item);
            }
        }

        public async Task LoadDanhSachDatCoc()
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
                                    <condition attribute='bsd_unitno' operator='eq' value='{UnitInfo.productid}'/>
                                    <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    <filter type='or'>
                                       <condition attribute='statuscode' operator='in'>
                                            <value>100000000</value>
                                            <value>100000001</value>
                                            <value>100000006</value>
                                            <value>861450001</value>
                                            <value>861450002</value>
                                            <value>4</value>                
                                            <value>3</value>
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
            if (result == null) return;
            IsLoaded = true;
            var data = result.value;
            ShowMoreDanhSachDatCoc = data.Count < 5 ? false : true;

            foreach (var x in data)
            {
                list_danhsachdatcoc.Add(x);
            }
        }

        public async Task LoadOptoinEntry()
        {
            string fetch = $@"<fetch version='1.0' count='5' page='{PageDanhSachHopDong}' output-format='xml-platform' mapping='logical' distinct='false'>
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
                                        <condition attribute='bsd_unitnumber' operator='eq' value='{UnitInfo.productid}'/>                
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
            if (result == null) return;

            IsLoaded = true;
            var data = result.value;
            ShowMoreDanhSachHopDong = data.Count < 5 ? false : true;

            foreach (var x in data)
            {
                list_danhsachhopdong.Add(x);
            }
        }

        public async Task CheckShowBtnBangTinhGia()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='bsd_phaseslaunch'>
                                <attribute name='bsd_name' />
                                <attribute name='createdon' />
                                <attribute name='bsd_phaseslaunchid' />
                                <order attribute='createdon' descending='true' />
                                <link-entity name='product' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='al'>
                                  <filter type='and'>
                                    <condition attribute='productid' operator='eq' value='{UnitId}' />
                                  </filter>
                                </link-entity><link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='an' >
                                   <attribute name='bsd_startdate' alias='startdate_event' />
                                   <attribute name='bsd_enddate' alias='enddate_event'/>
                                   <attribute name='statuscode' alias='statuscode_event'/>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PhasesLanchModel>>("bsd_phaseslaunchs", fetchXml);
            if (result == null || result.value.Any() == false) return;

            var data = result.value;
            foreach (var item in data)
            {
                if (item.startdate_event < DateTime.Now && item.enddate_event > DateTime.Now && item.statuscode_event == "100000000")
                {
                    if (UnitInfo.statuscode == 100000000 || UnitInfo.statuscode == 100000004)
                    {
                        IsShowBtnBangTinhGia = true;
                    }
                    else
                    {
                        IsShowBtnBangTinhGia = false;
                    }
                    return;
                }
                else
                {
                    IsShowBtnBangTinhGia = false;
                }
            }
        }

        public async Task LoadAllCollection()
        {
            //if (UnitId != null && UnitInfo != null && !string.IsNullOrWhiteSpace(UnitInfo.name))
            //{
            //    var Folder = UnitInfo.name.Replace('.', '-') + "_" + UnitId.ToString().Replace("-", string.Empty).ToUpper();
            //    var Category = "Units";
            //    var category_value = "product";

            //    GetTokenResponse getTokenResponse = await CrmHelper.getSharePointToken();
            //    var client = BsdHttpClient.Instance();
            //    string fileListUrl = $"{OrgConfig.SharePointResource}/sites/" + OrgConfig.SharePointSiteName + "/_api/web/Lists/GetByTitle('" + Category + "')/RootFolder/Folders('" + Folder + "')/Files";
            //    var request = new HttpRequestMessage(HttpMethod.Get, fileListUrl);

            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getTokenResponse.access_token);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = await client.SendAsync(request);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        TotalMedia = 0;
            //        TotalPhoto = 0;
            //        var body = await response.Content.ReadAsStringAsync();
            //        SharePointFieldResult sharePointFieldResult = JsonConvert.DeserializeObject<SharePointFieldResult>(body);
            //        var list = sharePointFieldResult.value;
            //        foreach (var item in list)
            //        {
            //            var names = item.Name.ToLower().Split('.');
            //            string type_item = names[names.Count() - 1];
            //            if (type_item == "flv" || type_item == "mp4" || type_item == "m3u8" || type_item == "3gp" || type_item == "mov" || type_item == "avi" || type_item == "wmv")
            //            {
            //                var soucre = OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_layouts/15/download.aspx?SourceUrl=/sites/" + OrgConfig.SharePointSiteName + "/" + category_value + "/" + Folder + "/" + item.Name + "&access_token=" + getTokenResponse.access_token;
            //                if (Device.RuntimePlatform == Device.iOS)
            //                {
            //                    soucre = await DependencyService.Get<IUrlEnCodeSevice>().GetUrlEnCode(soucre);
            //                }
            //                var mediaItem = await CrossMediaManager.Current.Extractor.CreateMediaItem(soucre);
            //                var image = await CrossMediaManager.Current.Extractor.GetVideoFrame(mediaItem, TimeSpan.FromSeconds(5));
            //                //ImageSource imageSource = image.ToImageSource();

            //                //Collections.Add(new CollectionData { MediaSource = soucre, ImageSource = imageSource, Index = TotalMedia });
            //                //TotalMedia++;
            //            }
            //            else if (type_item == "jpg" || type_item == "jpeg" || type_item == "png")
            //            {
            //                var soucre = OrgConfig.SharePointResource + "/sites/" + OrgConfig.SharePointSiteName + "/_layouts/15/download.aspx?SourceUrl=/sites/" + OrgConfig.SharePointSiteName + "/" + category_value + "/" + Folder + "/" + item.Name + "&access_token=" + getTokenResponse.access_token;
            //                if (Device.RuntimePlatform == Device.iOS)
            //                {
            //                    soucre = await DependencyService.Get<IUrlEnCodeSevice>().GetUrlEnCode(soucre);
            //                }
            //                Photos.Add(new Photo { URL = soucre });
            //                Collections.Add(new CollectionData { MediaSourceId = null, ImageSource = soucre, Index = TotalPhoto });
            //                TotalPhoto++;
            //            }
            //        }
            //    }
            //    if (Collections != null && Collections.Count > 0)
            //    {
            //        ShowCollections = true;
            //    }
            //    else
            //    {
            //        ShowCollections = false;
            //    }
            //}
        }

        public async Task LoadDataEvent()
        {
            if (UnitInfo == null || UnitInfo.event_id == Guid.Empty) return;

            string FetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_event'>
                                    <attribute name='bsd_name' />
                                    <attribute name='bsd_startdate' />
                                    <attribute name='bsd_eventcode' />
                                    <attribute name='bsd_enddate' />
                                    <attribute name='bsd_eventid' />
                                    <order attribute='bsd_eventcode' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='statuscode' operator='eq' value='100000000' />
                                      <condition attribute='bsd_eventid' operator='eq' value='{UnitInfo.event_id}' />
                                    </filter>
                                    <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaselaunch' link-type='outer' alias='ab'>
                                      <attribute name='bsd_name' alias='bsd_phaselaunch_name'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EventModel>>("bsd_events", FetchXml);
            if (result == null || result.value.Any() == false) return;
            var data = result.value.FirstOrDefault();
            Event = data;
            if (data.bsd_startdate != null && data.bsd_enddate != null)
            {
                Event.bsd_startdate = data.bsd_startdate.Value.ToLocalTime();
                Event.bsd_enddate = data.bsd_enddate.Value.ToLocalTime();
            }
        }
    }
}
