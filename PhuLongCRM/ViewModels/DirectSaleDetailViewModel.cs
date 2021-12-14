using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class DirectSaleDetailViewModel : BaseViewModel
    {
        public string Keyword { get; set; }

        private List<Block> _blocks;
        public List<Block> Blocks { get => _blocks; set { _blocks = value; OnPropertyChanged(nameof(Blocks)); } }

        private Block _block;
        public Block Block { get => _block; set { _block = value; OnPropertyChanged(nameof(Block)); } }

        private Unit _unit;
        public Unit Unit { get => _unit; set { _unit = value; OnPropertyChanged(nameof(Unit)); } }

        //public ObservableCollection<Floor> Floors { get; set; } = new ObservableCollection<Floor>();

        private ObservableCollection<QueueListModel> _queueList;
        public ObservableCollection<QueueListModel> QueueList { get => _queueList; set { _queueList = value; OnPropertyChanged(nameof(QueueList)); } }

        private DirectSaleSearchModel _filter;
        public DirectSaleSearchModel Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        private List<DirectSaleModel> _directSaleResult;
        public List<DirectSaleModel> DirectSaleResult { get => _directSaleResult; set { _directSaleResult = value; OnPropertyChanged(nameof(DirectSaleResult)); } }

        private OptionSet _statusReason;
        public OptionSet StatusReason { get => _statusReason; set { _statusReason = value; OnPropertyChanged(nameof(StatusReason)); } }

        private List<OptionSet> _statusReasons;
        public List<OptionSet> StatusReasons { get => _statusReasons; set { _statusReasons = value; OnPropertyChanged(nameof(StatusReasons)); } }

        private StatusCodeModel _unitStatusCode;
        public StatusCodeModel UnitStatusCode { get => _unitStatusCode; set { _unitStatusCode = value; OnPropertyChanged(nameof(UnitStatusCode)); } }

        private OptionSet _unitDirection;
        public OptionSet UnitDirection { get => _unitDirection; set { _unitDirection = value; OnPropertyChanged(nameof(UnitDirection)); } }

        private OptionSet _unitView;
        public OptionSet UnitView { get => _unitView; set { _unitView = value; OnPropertyChanged(nameof(UnitView)); } }

        private string _numChuanBiInBlock;
        public string NumChuanBiInBlock { get => _numChuanBiInBlock; set { _numChuanBiInBlock = value; OnPropertyChanged(nameof(NumChuanBiInBlock)); } }

        private string _numSanSangInBlock;
        public string NumSanSangInBlock { get => _numSanSangInBlock; set { _numSanSangInBlock = value; OnPropertyChanged(nameof(NumSanSangInBlock)); } }

        private string _numGiuChoInBlock;
        public string NumGiuChoInBlock { get => _numGiuChoInBlock; set { _numGiuChoInBlock = value; OnPropertyChanged(nameof(NumGiuChoInBlock)); } }

        private string _numDatCocInBlock;
        public string NumDatCocInBlock { get => _numDatCocInBlock; set { _numDatCocInBlock = value; OnPropertyChanged(nameof(NumDatCocInBlock)); } }

        private string _numDongYChuyenCoInBlock;
        public string NumDongYChuyenCoInBlock { get => _numDongYChuyenCoInBlock; set { _numDongYChuyenCoInBlock = value; OnPropertyChanged(nameof(NumDongYChuyenCoInBlock)); } }

        private string _numDaDuTienCocInBlock;
        public string NumDaDuTienCocInBlock { get => _numDaDuTienCocInBlock; set { _numDaDuTienCocInBlock = value; OnPropertyChanged(nameof(NumDaDuTienCocInBlock)); } }

        private string _numThanhToanDot1InBlock;
        public string NumThanhToanDot1InBlock { get => _numThanhToanDot1InBlock; set { _numThanhToanDot1InBlock = value; OnPropertyChanged(nameof(NumThanhToanDot1InBlock)); } }

        private string _numDaBanInBlock;
        public string NumDaBanInBlock { get => _numDaBanInBlock; set { _numDaBanInBlock = value; OnPropertyChanged(nameof(NumDaBanInBlock)); } }

        private bool _showMoreDanhSachDatCho;
        public bool ShowMoreDanhSachDatCho { get => _showMoreDanhSachDatCho; set { _showMoreDanhSachDatCho = value; OnPropertyChanged(nameof(ShowMoreDanhSachDatCho)); } }

        private bool _isShowBtnBangTinhGia;
        public bool IsShowBtnBangTinhGia { get => _isShowBtnBangTinhGia; set { _isShowBtnBangTinhGia = value; OnPropertyChanged(nameof(IsShowBtnBangTinhGia)); } }

        public Guid blockId;
        public int PageDanhSachDatCho = 1;

        public DirectSaleDetailViewModel(DirectSaleSearchModel filter)
        {
            Filter = filter;
            QueueList = new ObservableCollection<QueueListModel>();
        }

        public async Task LoadTotalDirectSale()
        {
            string json = JsonConvert.SerializeObject(Filter);
            var input = new
            {
                input = json
            };
            string body = JsonConvert.SerializeObject(input);
            CrmApiResponse result = await CrmHelper.PostData("/bsd_Action_DirectSale_GetTotalQty", body);
            if (result.IsSuccess == false && result.Content == null) return;

            string content = result.Content;
            ResponseAction responseActions = JsonConvert.DeserializeObject<ResponseAction>(content);
            DirectSaleResult = JsonConvert.DeserializeObject<List<DirectSaleModel>>(responseActions.output);
        }

        public async Task LoadUnitByFloor(Guid floorId)
        {
            string StatusReason_Condition = StatusReason == null ? "" : "<condition attribute='statuscode' operator='eq' value='" + StatusReason.Val + @"' />";
            string PhasesLaunch_Condition = (!string.IsNullOrWhiteSpace(Filter.Phase))
                ? @"<condition attribute='bsd_phaseslaunchid' operator='eq' uitype='bsd_phaseslaunch' value='" + Filter.Phase + @"' />"
                : "";
            string isEvent = (Filter.Event.HasValue && Filter.Event.Value) ? @"<link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='as'>
                                          <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='at'>
                                            <filter type='and'>
                                              <condition attribute='bsd_eventid' operator='not-null' />
                                            </filter>
                                          </link-entity>
                                        </link-entity>" : "";

            string UnitCode_Condition = !string.IsNullOrEmpty(Filter.Unit) ? "<condition attribute='name' operator='like' value='%25" + Filter.Unit + "%25' />" : "";

            string Direction_Condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(Filter.Direction))
            {
                var Directions = Filter.Direction.Split(',').ToList();
                if (Directions != null && Directions.Count != 0)
                {
                    string tmp = string.Empty;
                    foreach (var i in Directions)
                    {
                        tmp += "<value>" + i + "</value>";
                    }
                    Direction_Condition = @"<condition attribute='bsd_direction' operator='in'>" + tmp + "</condition>";
                }
            }

            string View_Condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(Filter.view))
            {
                var Views = Filter.view.Split(',').ToList();
                if (Views != null && Views.Count != 0)
                {
                    string tmp = string.Empty;
                    foreach (var i in Views)
                    {
                        tmp += "<value>" + i + "</value>";
                    }
                    View_Condition = @"<condition attribute='bsd_viewphulong' operator='in'>" + tmp + "</condition>";
                }
            }

            string UnitStatus_Condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(Filter.stsUnit))
            {
                var UnitStatuses = Filter.stsUnit.Split(',').ToList();
                if (UnitStatuses != null && UnitStatuses.Count != 0)
                {
                    string tmp = string.Empty;
                    foreach (var i in UnitStatuses)
                    {
                        tmp += "<value>" + i + "</value>";
                    }
                    UnitStatus_Condition = @"<condition attribute='statuscode' operator='in'>" + tmp + "</condition>";
                }
            }
            string maxNetArea_Condition = string.Empty;
            string minNetArea_Condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(Filter.Area))
            {
                var area = NetAreaDirectSaleData.GetNetAreaById(Filter.Area);
                if (area.From != null && area.To == null)
                {
                    maxNetArea_Condition = $"<condition attribute='bsd_netsaleablearea' operator='le' value='{area.From}' />";
                }
                else if (area.From == null && area.To != null)
                {
                    minNetArea_Condition = $"<condition attribute='bsd_netsaleablearea' operator='ge' value='{area.To}' />";
                }
                else if(area.From != null && area.To != null)
                {
                    minNetArea_Condition = $"<condition attribute='bsd_netsaleablearea' operator='ge' value='{area.From}' />" ;
                    maxNetArea_Condition = $"<condition attribute='bsd_netsaleablearea' operator='le' value='{area.To}' />";
                }
                else
                {
                    minNetArea_Condition = null;
                    maxNetArea_Condition = null;
                }
            }
            
            string minPrice_Condition = string.Empty;
            string maxPrice_Condition = string.Empty;
            if (!string.IsNullOrWhiteSpace(Filter.Price))
            {
                var price = PriceDirectSaleData.GetPriceById(Filter.Price);
                if (price.From != null && price.To == null)
                {
                    maxPrice_Condition = $"<condition attribute='price' operator='le' value='{price.From}' />";
                }
                else if (price.From == null && price.To != null)
                {
                    minPrice_Condition = $"<condition attribute='price' operator='ge' value='{price.To}' />";
                }
                else if (price.From != null && price.To != null)
                {
                    minPrice_Condition = $"<condition attribute='price' operator='ge' value='{price.From}' />";
                    maxPrice_Condition = $"<condition attribute='price' operator='le' value='{price.To}' />";
                }
                else
                {
                    minPrice_Condition = null;
                    maxPrice_Condition = null;
                }
            }

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='product'>
                                <attribute name='productid' />
                                <attribute name='name' />
                                <attribute name='statuscode' />
                                <attribute name='price' />
                                <attribute name='bsd_phaseslaunchid' />
                                <order attribute='name' descending='false' />
                                <filter type='and'>
                                    <condition attribute='statuscode' operator='ne' value='0' />
                                    <condition attribute='bsd_projectcode' operator='eq' uitype='bsd_project' value='{Filter.Project}'/>
                                    <condition attribute='bsd_floor' operator='eq' uitype='bsd_floor' value='{floorId}' />
                                    '{PhasesLaunch_Condition}'
                                    '{UnitCode_Condition}'
                                    '{StatusReason_Condition}'
                                    '{UnitStatus_Condition}'
                                    '{Direction_Condition}'
                                    '{View_Condition}'
                                    '{minNetArea_Condition}'
                                    '{maxNetArea_Condition}'
                                    '{minPrice_Condition}'
                                    '{maxPrice_Condition}'
                                </filter>
                                <link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='ag' >
                                    <attribute name='statuscode' alias='queses_statuscode'/>
                                </link-entity>
                                '{isEvent}'
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Unit>>("products", fetchXml);
            if (result == null || result.value.Any() == false) return;

            List<Unit> unitsResult = result.value.GroupBy(x => new
            {
                productid = x.productid
            }).Select(y => y.First()).ToList();

            List<Unit> units = new List<Unit>();
            foreach (var item in unitsResult)
            {
                // dem unit co nhung trang thai giu cho la: queuing, waiting
                item.NumQueses = result.value.Where(x => x.productid == item.productid && (x.queses_statuscode == "100000000" || x.queses_statuscode == "100000002")).ToList().Count();
                units.Add(item);
            }

            this.Block.Floors.SingleOrDefault(x => x.bsd_floorid == floorId).Units.AddRange(units);
        }

        public async Task LoadUnitById(Guid unitId)
        {
             string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='product'>
                                    <attribute name='name' />
                                    <attribute name='statuscode' />
                                    <attribute name='bsd_projectcode' alias='_bsd_projectcode_value'/>
                                    <attribute name='price' />
                                    <attribute name='productid' />
                                    <attribute name='bsd_view' />
                                    <attribute name='bsd_direction' />
                                    <attribute name='bsd_constructionarea' />
                                    <attribute name='bsd_floor' alias='floorid'/>
                                    <attribute name='bsd_blocknumber' alias='blockid'/>
                                    <attribute name='bsd_phaseslaunchid' />
                                    <attribute name='bsd_vippriority' />
                                    <order attribute='bsd_constructionarea' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='productid' operator='eq' uitype='product' value='{unitId}' />
                                    </filter>
                                    <link-entity name='bsd_unittype' from='bsd_unittypeid' to='bsd_unittype' visible='false' link-type='outer' alias='a_493690ec6ce2e811a94e000d3a1bc2d1'>
                                      <attribute name='bsd_name'  alias='bsd_unittype_name'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Unit>>("products", fetchXml);
            if (result == null || result.value.Any() == false) return;

            Unit = result.value.FirstOrDefault();
        }

        public async Task LoadQueues(Guid unitId)
        {
            string fetch = $@"<fetch version='1.0' count='5' page='{PageDanhSachDatCho}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='opportunity'>
                        <attribute name='name'/>
                        <attribute name='statuscode' />
                        <attribute name='bsd_project' />
                        <attribute name='opportunityid' />
                        <attribute name='bsd_queuingexpired' />
                        <order attribute='statuscode' descending='false' />
                        <filter type='and'>
                            <condition attribute='bsd_units' operator='eq' value='{unitId}'/>
                            <condition attribute='bsd_employee' operator='eq' value='{UserLogged.Id}'/>
                            <condition attribute='statuscode' operator='in'>
                                <value>100000002</value>
                                <value>100000000</value>
                            </condition>
                        </filter>
                        <link-entity name='contact' from='contactid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='fullname'  alias='contact_name'/>
                        </link-entity>
                        <link-entity name='account' from='accountid' to='customerid' visible='false' link-type='outer'>
                           <attribute name='name'  alias='account_name'/>
                        </link-entity>
                      </entity>
                    </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<QueueListModel>>("opportunities", fetch);
            if (result == null)
            {
                return;
            }

            var data = result.value;
            ShowMoreDanhSachDatCho = data.Count < 5 ? false : true;

            foreach (var x in data)
            {
                x.statuscode_label = QueuesStatusCodeData.GetQueuesById(x.statuscode.ToString()).Name;
                QueueList.Add(x);
            }
            //if (QueueList.Any(x=>x.statuscode == 100000000))  // chỗ này đang bị lỗi khi có 2 giữ chỗ queue
            //{
            //    var item = QueueList.SingleOrDefault(x => x.statuscode == 100000000);
            //    QueueList.Remove(item);
            //    QueueList.Insert(0, item);
            //}
        }

        public async Task CheckShowBtnBangTinhGia(Guid unitId)
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='bsd_phaseslaunch'>
                                <attribute name='bsd_phaseslaunchid' />
                                <order attribute='createdon' descending='true' />
                                <link-entity name='product' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='al'>
                                  <filter type='and'>
                                    <condition attribute='productid' operator='eq' value='{unitId}' />
                                  </filter>
                                </link-entity>
                                <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='an' >
                                   <attribute name='bsd_startdate' alias='startdate_event' />
                                   <attribute name='bsd_enddate' alias='enddate_event'/>
                                </link-entity>
                              </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PhasesLanchModel>>("bsd_phaseslaunchs", fetchXml);
            if (result == null || result.value.Any() == false) return;

            var data = result.value;
            foreach (var item in data)
            {
                if (item.startdate_event < DateTime.Now && item.enddate_event > DateTime.Now)
                {
                    IsShowBtnBangTinhGia = true;
                    return;
                }
                else
                {
                    IsShowBtnBangTinhGia = false;
                }
            }
        }

        public void ResetDirectSale(DirectSaleModel directSaleModel)
        {
            this.Block = new Block();
            this.Block.bsd_blockid = blockId = Guid.Parse(directSaleModel.ID);
            var arrStatus = directSaleModel.stringQty.Split(',');
            this.Block.NumChuanBiInBlock = arrStatus[0];
            this.Block.NumSanSangInBlock = arrStatus[1];
            this.Block.NumGiuChoInBlock = arrStatus[2];
            this.Block.NumDatCocInBlock = arrStatus[3];
            this.Block.NumDongYChuyenCoInBlock = arrStatus[4];
            this.Block.NumDaDuTienCocInBlock = arrStatus[5];
            this.Block.NumThanhToanDot1InBlock = arrStatus[6];
            this.Block.NumDaBanInBlock = arrStatus[7];

            foreach (var item in directSaleModel.listFloor)
            {
                Floor floor = new Floor();
                floor.bsd_floorid = Guid.Parse(item.ID);
                floor.bsd_name = item.name;
                var arrStatusInFloor = item.stringQty.Split(',');
                floor.NumChuanBiInFloor = arrStatusInFloor[0];
                floor.NumSanSangInFloor = arrStatusInFloor[1];
                floor.NumGiuChoInFloor = arrStatusInFloor[2];
                floor.NumDatCocInFloor = arrStatusInFloor[3];
                floor.NumDongYChuyenCoInFloor = arrStatusInFloor[4];
                floor.NumDaDuTienCocInFloor = arrStatusInFloor[5];
                floor.NumThanhToanDot1InFloor = arrStatusInFloor[6];
                floor.NumDaBanInFloor = arrStatusInFloor[7];
                this.Block.Floors.Add(floor);
            };
        }

    }
    public class ResponseAction
    {
        public string output { get; set; }
    }
}
