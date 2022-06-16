using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Stormlion.PhotoBrowser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace PhuLongCRM.ViewModels
{
    public class ProjectInfoViewModel : BaseViewModel
    {
        public ObservableCollection<CollectionData> Collections { get; set; } = new ObservableCollection<CollectionData>();

        public List<Photo> Photos { get; set; }
        private bool _showCollections = false;
        public bool ShowCollections { get => _showCollections; set { _showCollections = value; OnPropertyChanged(nameof(ShowCollections)); } }

        private int _totalMedia;
        public int TotalMedia { get => _totalMedia; set { _totalMedia = value; OnPropertyChanged(nameof(TotalMedia)); } }

        private int _totalPhoto;
        public int TotalPhoto { get => _totalPhoto; set { _totalPhoto = value; OnPropertyChanged(nameof(TotalPhoto)); } }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ChartModel> unitChartModels { get; set; }
        public ObservableCollection<ChartModel> UnitChart { get; set; } = new ObservableCollection<ChartModel>();

        private ObservableCollection<QueuesModel> _listGiuCho;
        public ObservableCollection<QueuesModel> ListGiuCho { get => _listGiuCho; set { _listGiuCho = value; OnPropertyChanged(nameof(ListGiuCho)); } }

        private EventModel _event;
        public EventModel Event { get => _event; set { _event = value; OnPropertyChanged(nameof(Event)); } }

        private ProjectInfoModel _project;
        public ProjectInfoModel Project
        {
            get => _project;
            set
            {
                if (_project != value)
                { _project = value; OnPropertyChanged(nameof(Project)); }
            }
        }

        private OptionSet _projectType;
        public OptionSet ProjectType { get => _projectType; set { _projectType = value; OnPropertyChanged(nameof(ProjectType)); } }

        private OptionSet _propertyUsageType;
        public OptionSet PropertyUsageType { get => _propertyUsageType; set { _propertyUsageType = value; OnPropertyChanged(nameof(PropertyUsageType)); } }

        private OptionSet _handoverCoditionMinimum;
        public OptionSet HandoverCoditionMinimum { get => _handoverCoditionMinimum; set { _handoverCoditionMinimum = value; OnPropertyChanged(nameof(HandoverCoditionMinimum)); } }

        private bool _isShowBtnGiuCho;
        public bool IsShowBtnGiuCho { get => _isShowBtnGiuCho; set { _isShowBtnGiuCho = value; OnPropertyChanged(nameof(IsShowBtnGiuCho)); } }

        private int _numUnit = 0;
        public int NumUnit { get => _numUnit; set { _numUnit = value; OnPropertyChanged(nameof(NumUnit)); } }

        private int _soGiuCho = 0;
        public int SoGiuCho { get => _soGiuCho; set { _soGiuCho = value; OnPropertyChanged(nameof(SoGiuCho)); } }

        private int _soDatCoc = 0;
        public int SoDatCoc { get => _soDatCoc; set { _soDatCoc = value; OnPropertyChanged(nameof(SoDatCoc)); } }

        private int _soHopDong = 0;
        public int SoHopDong { get => _soHopDong; set { _soHopDong = value; OnPropertyChanged(nameof(SoHopDong)); } }

        private int _soBangTinhGia = 0;
        public int SoBangTinhGia { get => _soBangTinhGia; set { _soBangTinhGia = value; OnPropertyChanged(nameof(SoBangTinhGia)); } }

        private bool _showMoreBtnGiuCho;
        public bool ShowMoreBtnGiuCho { get => _showMoreBtnGiuCho; set { _showMoreBtnGiuCho = value; OnPropertyChanged(nameof(ShowMoreBtnGiuCho)); } }

        private bool _isHasEvent;
        public bool IsHasEvent { get => _isHasEvent; set { _isHasEvent = value; OnPropertyChanged(nameof(IsHasEvent)); } }

        public int ChuanBi { get; set; } = 0;
        public int SanSang { get; set; } = 0;
        public int GiuCho { get; set; } = 0;
        public int DatCoc { get; set; } = 0;
        public int DongYChuyenCoc { get; set; } = 0;
        public int DaDuTienCoc { get; set; } = 0;
        public int ThanhToanDot1 { get; set; } = 0;
        public int DaBan { get; set; } = 0;
        public int Booking { get; set; } = 0;
        public int Option { get; set; } = 0;
        public int SignedDA { get; set; } = 0;
        public int Qualified { get; set; } = 0;

        public bool IsLoadedGiuCho { get; set; }

        public int PageListGiuCho = 1;

        private ImageSource _ImageSource;
        public ImageSource ImageSource { get => _ImageSource; set { _ImageSource = value; OnPropertyChanged(nameof(ImageSource)); } }

        private StatusCodeModel _statusCode;
        public StatusCodeModel StatusCode { get => _statusCode; set { _statusCode = value; OnPropertyChanged(nameof(StatusCode)); } }

        public bool IsHasPhasesLaunch { get; set; }

        public ProjectInfoViewModel()
        {
            ListGiuCho = new ObservableCollection<QueuesModel>();
        }

        public async Task LoadData()
        {
            string FetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='bsd_project'>
                                <attribute name='bsd_projectid' />
                                <attribute name='bsd_projectcode' />
                                <attribute name='bsd_name' />
                                <attribute name='createdon' />
                                <attribute name='bsd_address' />
                                <attribute name='bsd_projecttype' />
                                <attribute name='bsd_propertyusagetype' />
                                <attribute name='bsd_depositpercentda' />
                                <attribute name='bsd_estimatehandoverdate' />
                                <attribute name='bsd_landvalueofproject' />
                                <attribute name='bsd_maintenancefeespercent' />
                                <attribute name='bsd_numberofmonthspaidmf' />
                                <attribute name='bsd_managementamount' />
                                <attribute name='bsd_bookingfee' />
                                <attribute name='bsd_depositamount' />
                                <attribute name='statuscode' />
                                <attribute name='bsd_queuesperunit' />
                                <attribute name='bsd_unitspersalesman' />
                                <attribute name='bsd_queueunitdaysaleman' />
                                <attribute name='bsd_longqueuingtime' />
                                <attribute name='bsd_shortqueingtime' />
                                <attribute name='bsd_projectslogo'/>
                                <attribute name='bsd_queueproject'/>
                                <attribute name='bsd_printagreement'/>
                                <order attribute='bsd_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bsd_projectid' operator='eq' uitype='bsd_project' value='" + ProjectId.ToString() + @"' />
                                </filter>
                                <link-entity name='account' from='accountid' to='bsd_investor' visible='false' link-type='outer' alias='a_8924f6d5b214e911a97f000d3aa04914'>
                                  <attribute name='bsd_name' alias='bsd_investor_name' />
                                  <attribute name='accountid' alias='bsd_investor_id' />
                                </link-entity>
                              </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ProjectInfoModel>>("bsd_projects", FetchXml);
            if (result == null || result.value.Any() == false) return;
            Project = result.value.FirstOrDefault();
            this.StatusCode = ProjectStatusCodeData.GetProjectStatusCodeById(Project.statuscode);
            //await LoadAllCollection();
        }

        public async Task CheckEvent()
        {
            // ham check su kien hide/show cua du an (show khi du an dang trong thoi gian dien ra su kien, va trang thai la "Approved")
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='bsd_event'>
                                <attribute name='createdon' />
                                <attribute name='statuscode' />
                                <attribute name='bsd_startdate' />
                                <attribute name='bsd_enddate' />
                                <attribute name='bsd_eventid' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='statuscode' operator='eq' value='100000000' />
                                  <condition attribute='bsd_project' operator='eq' uitype='bsd_project' value='{ProjectId}' />
                                </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EventModel>>("bsd_events", fetchXml);
            if (result == null || result.value.Any() == false) return;

            var data = result.value;
            foreach (var item in data)
            {
                if (item.bsd_startdate < DateTime.Now && item.bsd_enddate > DateTime.Now)
                {
                    IsHasEvent = true;
                    return;
                }
            }
        }

        public async Task LoadThongKe()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='product'>
                                <attribute name='name' />
                                <attribute name='productnumber' />
                                <attribute name='statecode' />
                                <attribute name='productstructure' />
                                <attribute name='statuscode' />
                                <attribute name='bsd_projectcode' />
                                <attribute name='createdon' />
                                <attribute name='bsd_unitscodesams' />
                                <attribute name='productid' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                    <condition attribute='statuscode' operator='not-in'>
                                        <value>0</value>
                                    </condition>
                                    <condition attribute='bsd_projectcode' operator='eq' uitype='bsd_project' value='" + this.ProjectId + @"'/>
                                  </filter>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Unit>>("products", fetchXml);
            if (result == null || result.value.Any() == false)
            {
                ChuanBi++;
                SanSang++;
                GiuCho++;
                DatCoc++;
                DongYChuyenCoc++;
                DaDuTienCoc++;
                ThanhToanDot1++;
                DaBan++;
                Booking++;
                Option++;
                SignedDA++;
                Qualified++;
                IsShowBtnGiuCho = true;
            }
            else
            {
                IsShowBtnGiuCho = false;
                var data = result.value;
                NumUnit = data.Count;
                ChuanBi = data.Where(x => x.statuscode == 1).Count();
                SanSang = data.Where(x => x.statuscode == 100000000).Count();
                GiuCho = data.Where(x => x.statuscode == 100000004).Count();
                DatCoc = data.Where(x => x.statuscode == 100000006).Count();
                DongYChuyenCoc = data.Where(x => x.statuscode == 100000005).Count();
                DaDuTienCoc = data.Where(x => x.statuscode == 100000003).Count();
                ThanhToanDot1 = data.Where(x => x.statuscode == 100000001).Count();
                DaBan = data.Where(x => x.statuscode == 100000002).Count();
                Booking = data.Where(x => x.statuscode == 100000007).Count();
                Option = data.Where(x => x.statuscode == 100000010).Count();
                SignedDA = data.Where(x => x.statuscode == 100000009).Count();
                Qualified = data.Where(x => x.statuscode == 100000008).Count();
            }

            unitChartModels = new List<ChartModel>()
            {
                new ChartModel {Category ="Giữ chỗ",Value=GiuCho},
                new ChartModel { Category = "Đặt cọc", Value = DatCoc },
                new ChartModel {Category ="Đồng ý chuyển cọc",Value=DongYChuyenCoc },
                new ChartModel { Category = "Đã đủ tiền cọc", Value = DaDuTienCoc },
                new ChartModel { Category = "Option", Value = Option },
                new ChartModel {Category ="Thanh toán đợt 1",Value=ThanhToanDot1},
                new ChartModel { Category = "Signed D.A", Value = SignedDA },
                new ChartModel { Category = "Qualified", Value = Qualified },
                new ChartModel { Category = "Đã bán", Value =  DaBan},
                new ChartModel {Category ="Chuẩn bị", Value=ChuanBi},
                new ChartModel { Category = "Sẵn sàng", Value = SanSang },
                new ChartModel { Category = "Booking", Value = Booking },
            };
            foreach (var item in unitChartModels)
            {
                UnitChart.Add(item);
            }
        }

        public async Task LoadThongKeGiuCho()
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='opportunity'>
                                    <attribute name='name' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='bsd_project' operator='eq' uitype='bsd_project' value='{" + ProjectId + @"}' />
                                      <condition attribute='statuscode' operator='in'>
                                        <value>100000000</value>
                                        <value>100000002</value>
                                      </condition>
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueueFormModel>>("opportunities", fetchXml);
            if (result == null || result.value.Any() == false) return;

            SoGiuCho = result.value.Count();
        }
        public async Task LoadThongKeHopDong()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='salesorder'>
                                <attribute name='name' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                    <condition attribute='statuscode' operator='ne' value='100000006' />
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='inner' alias='ad'>
                                  <filter type='and'>
                                    <condition attribute='bsd_projectid' operator='eq' value ='{ProjectId}'/>
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionEntry>>("salesorders", fetchXml);
            if (result == null || result.value.Any() == false) return;

            SoHopDong = result.value.Count();
        }
        public async Task LoadThongKeBangTinhGia()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='quote'>
                                <attribute name='name' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='statuscode' operator='eq' value='100000007' />
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectid' link-type='inner' alias='ae'>
                                  <filter type='and'>
                                    <condition attribute='bsd_projectid' operator='eq' value='{ProjectId}'/>
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QuoteModel>>("quotes", fetchXml);
            if (result == null || result.value.Any() == false) return;
            SoBangTinhGia = result.value.Count();
        }
        public async Task LoadThongKeDatCoc()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='quote'>
                                <attribute name='name' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                  <condition attribute='statuscode' operator='in'>
                                    <value>100000000</value>
                                    <value>861450001</value>
                                    <value>861450002</value>
                                    <value>100000006</value>
                                    <value>3</value>
                                    <value>861450000</value>
                                  </condition>
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectid' link-type='inner' alias='ae'>
                                  <filter type='and'>
                                    <condition attribute='bsd_projectid' operator='eq' value='{ProjectId}'/>
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QuoteModel>>("quotes", fetchXml);
            if (result == null || result.value.Any() == false) return;
            SoDatCoc = result.value.Count();
        }
        public async Task LoadGiuCho()
        {
            IsLoadedGiuCho = true;
            string fetchXml = $@"<fetch version='1.0' count='5' page='{PageListGiuCho}' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='opportunity'>
                                <attribute name='name' />
                                <attribute name='customerid' alias='customer_id'/>
                                <attribute name='bsd_bookingtime' />
                                <attribute name='statuscode' />
                                <attribute name='bsd_queuingexpired' />
                                <attribute name='opportunityid' />
                                <order attribute='bsd_bookingtime' descending='false' />
                                <filter type='and'>
                                    <condition attribute='bsd_project' operator='eq' value='{ProjectId}' />
                                    <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                  <condition attribute='statuscode' operator='in'>
                                    <value>100000002</value>
                                    <value>100000000</value>
                                  </condition>
                                </filter>
                                <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='inner' alias='ab'>
                                    <attribute name='bsd_name' alias='project_name'/>
                                </link-entity>
                                <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer' alias='a_434f5ec290d1eb11bacc000d3a80021e'>
                                  <attribute name='name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer' alias='a_884f5ec290d1eb11bacc000d3a80021e'>
                                  <attribute name='bsd_fullname' alias='contact_name'/>
                                </link-entity>
                              </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueuesModel>>("opportunities", fetchXml);
            if (result == null || result.value.Any() == false) return;

            List<QueuesModel> data = result.value;
            ShowMoreBtnGiuCho = data.Count < 10 ? false : true;
            foreach (var item in data)
            {
                ListGiuCho.Add(item);
            }
        }
        public async Task LoadAllCollection()
        {
            // khoa lai vi phu long chua co hinh anh va video

            //if (ProjectId != null)
            //{
            //    string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            //                      <entity name='sharepointdocument'>
            //                        <attribute name='documentid' />
            //                        <attribute name='absoluteurl' />
            //                        <attribute name='fullname' />
            //                        <attribute name='filetype' />
            //                        <attribute name='relativelocation' />
            //                        <order attribute='relativelocation' descending='false' />
            //                        <link-entity name='bsd_project' from='bsd_projectid' to='regardingobjectid' link-type='inner' alias='ad'>
            //                          <filter type='and'>
            //                            <condition attribute='bsd_projectid' operator='eq' value='{ProjectId}' />
            //                          </filter>
            //                        </link-entity>
            //                      </entity>
            //                    </fetch>";
            //    var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<SharePonitModel>>("sharepointdocuments", fetchXml);

            //    if (result == null || result.value.Any() == false)
            //    {
            //        ShowCollections = false;
            //        return;
            //    }

            //    Photos = new List<Photo>();
            //    List<SharePonitModel> list = result.value;
            //    var videos = list.Where(x => x.filetype == "mp4" || x.filetype == "flv" || x.filetype == "m3u8" || x.filetype == "3gp" || x.filetype == "mov" || x.filetype == "avi" || x.filetype == "wmv").ToList();
            //    var images = list.Where(x => x.filetype == "jpg" || x.filetype == "jpeg" || x.filetype == "png").ToList();
            //    this.TotalMedia = videos.Count;
            //    this.TotalPhoto = images.Count;

            //    foreach (var item in videos)
            //    {
            //        var urlVideo = await CrmHelper.RetrieveImagesSharePoint<RetrieveMultipleApiResponse<GraphThumbnailsUrlModel>>($"{Config.OrgConfig.SharePointProjectId}/items/{item.documentid}/driveItem/thumbnails");
            //        string url = urlVideo.value.SingleOrDefault().large.url;// retri se lay duoc thumbnails gom 3 kich thuoc : large,medium,small
            //        Collections.Add(new CollectionData {Id = item.documentid, MediaSourceId = item.documentid.ToString(), ImageSource = url, SharePointType = SharePointType.Video, Index = TotalMedia });
            //    }

            //    foreach (var item in images)
            //    {
            //        var urlVideo = await CrmHelper.RetrieveImagesSharePoint<RetrieveMultipleApiResponse<GraphThumbnailsUrlModel>>($"{Config.OrgConfig.SharePointProjectId}/items/{item.documentid}/driveItem/thumbnails");
            //        string url = urlVideo.value.SingleOrDefault().large.url;// retri se lay duoc thumbnails gom 3 kich thuoc : large,medium,small
            //        this.Photos.Add(new Photo { URL = url });
            //        Collections.Add(new CollectionData { Id = item.documentid, MediaSourceId = null, ImageSource = url, SharePointType = SharePointType.Image, Index = TotalMedia });
            //    }
            //}
        }
        public async Task LoadDataEvent()
        {
            if (ProjectId == Guid.Empty) return;

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
                                      <condition attribute='bsd_project' operator='eq' value='{ProjectId}' />
                                    </filter>
                                    <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaselaunch' link-type='outer' alias='ab'>
                                      <attribute name='bsd_name' alias='bsd_phaselaunch_name'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EventModel>>("bsd_events", FetchXml);
            if (result == null || result.value.Any() == false) return;
            Event = result.value.FirstOrDefault();
            if (Event.bsd_startdate.HasValue && Event.bsd_enddate.HasValue)
            {
                Event.bsd_startdate = Event.bsd_startdate.Value.ToLocalTime();
                Event.bsd_enddate = Event.bsd_enddate.Value.ToLocalTime();
            }
        }
        public async Task CheckPhasesLaunch()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_phaseslaunch'>
                                    <attribute name='bsd_phaseslaunchid' alias='Val'/>
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='bsd_projectid' operator='eq' value='{this.ProjectId}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_phaseslaunchs", fetchXml);
            if (result != null && result.value.Count > 0)
                this.IsHasPhasesLaunch = true;
            else
                this.IsHasPhasesLaunch = false;
        }
    }
}
