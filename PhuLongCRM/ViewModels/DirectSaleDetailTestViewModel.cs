using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class DirectSaleDetailTestViewModel : BaseViewModel
    {
        public int Page = -1;
        private string FilterXml;
        private int Size = 5;

        private Unit _unit;
        public Unit Unit { get => _unit; set { _unit = value; OnPropertyChanged(nameof(Unit)); } }

        private List<DirectSaleModel> _directSaleResult;
        public List<DirectSaleModel> DirectSaleResult { get => _directSaleResult; set { _directSaleResult = value; OnPropertyChanged(nameof(DirectSaleResult)); } }

        private DirectSaleSearchModel _filter;
        public DirectSaleSearchModel Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        private DirectSaleModel _block;
        public DirectSaleModel Block { get => _block; set { _block = value; OnPropertyChanged(nameof(Block)); } }

        private Block _numUnitblock;
        public Block NumUniBlock { get => _numUnitblock; set { _numUnitblock = value; OnPropertyChanged(nameof(NumUniBlock)); } }

        private ObservableCollection<Floor> _floors;
        public ObservableCollection<Floor> Floors { get => _floors; set { _floors = value; OnPropertyChanged(nameof(Floors)); } }

        private bool _isShowBtnBangTinhGia;
        public bool IsShowBtnBangTinhGia { get => _isShowBtnBangTinhGia; set { _isShowBtnBangTinhGia = value; OnPropertyChanged(nameof(IsShowBtnBangTinhGia)); } }

        public DirectSaleDetailTestViewModel()
        {
            Floors = new ObservableCollection<Floor>();
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
        public async Task LoadFloor()
        {
            if (Block != null && Block.listFloor != null)
            {
                Page += 1;
                var list = Block.listFloor.Skip(Page * Size).Take(Size);
                if (Floors.Count == Block.listFloor.Count)
                    return;
                foreach (var item in list)
                {
                    Floor floor = new Floor();
                    floor.bsd_floorid = Guid.Parse(item.ID);
                    floor.bsd_name = item.name;
                    var arrStatusInFloor = item.stringQty.Split(',');
                    floor.NumChuanBiInFloor = int.Parse(arrStatusInFloor[0]);
                    floor.NumSanSangInFloor = int.Parse(arrStatusInFloor[1]);
                    floor.NumBookingInFloor = int.Parse(arrStatusInFloor[2]);
                    floor.NumGiuChoInFloor = int.Parse(arrStatusInFloor[3]);
                    floor.NumDatCocInFloor = int.Parse(arrStatusInFloor[4]);
                    floor.NumDongYChuyenCoInFloor = int.Parse(arrStatusInFloor[5]);
                    floor.NumDaDuTienCocInFloor = int.Parse(arrStatusInFloor[6]);
                    floor.NumOptionInFloor = int.Parse(arrStatusInFloor[7]);
                    floor.NumThanhToanDot1InFloor = int.Parse(arrStatusInFloor[8]);
                    floor.NumSignedDAInFloor = int.Parse(arrStatusInFloor[9]);
                    floor.NumQualifiedInFloor = int.Parse(arrStatusInFloor[10]);
                    floor.NumDaBanInFloor = int.Parse(arrStatusInFloor[11]);
                    floor.TotalUnitInFloor = int.Parse(item.sumQty);
                    Floors.Add(floor);
                }
            }
        }
        public async Task LoadUnitByFloor(Guid floorId)
        {
            string now_date = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            string isEvent = (Filter.Event.HasValue && Filter.Event.Value) ? @"<link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='as'>
                                          <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='at'>
                                            <filter type='and'>
                                              <condition attribute='bsd_eventid' operator='not-null' />
                                            </filter>
                                          </link-entity>
                                        </link-entity>" : "";

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
                                    {FilterXml}
                                </filter>
                                <link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='ag' >
                                    <attribute name='statuscode' alias='queses_statuscode'/>
                                </link-entity>
                                        <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='outer' alias='asmn'>
                                          <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='outer' alias='atmn'>
                                            <attribute name='bsd_eventid' alias='event_id'/>
                                            <filter type='and'>
                                              <condition attribute='statuscode' operator='eq' value='100000000' />
                                              <condition attribute='bsd_enddate' operator='on-or-after' value='{now_date}' />
                                            </filter>
                                          </link-entity>
                                        </link-entity>
                                {isEvent}
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

            Floors.SingleOrDefault(x => x.bsd_floorid == floorId).Units.AddRange(units);
        }
        public void CreateFilterXml()
        {
            //string StatusReason_Condition = StatusReason == null ? "" : "<condition attribute='statuscode' operator='eq' value='" + StatusReason.Val + @"' />";
            string PhasesLaunch_Condition = (!string.IsNullOrWhiteSpace(Filter.Phase))
                ? @"<condition attribute='bsd_phaseslaunchid' operator='eq' uitype='bsd_phaseslaunch' value='" + Filter.Phase + @"' />"
                : "";
            
            string UnitCode_Condition = !string.IsNullOrEmpty(Filter.Unit) ? $"<condition attribute='name' operator='like' value='%25" + Filter.Unit + "%25'/>" : null;

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
                    View_Condition = @"<condition attribute='bsd_viewphulong' operator='contain-values'>" + tmp + "</condition>";
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
                else if (area.From != null && area.To != null)
                {
                    minNetArea_Condition = $"<condition attribute='bsd_netsaleablearea' operator='ge' value='{area.From}' />";
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

            FilterXml = $@"{PhasesLaunch_Condition}
                                    {UnitCode_Condition}
                                    {UnitStatus_Condition}
                                    {Direction_Condition}
                                    {View_Condition}
                                    {minNetArea_Condition}
                                    {maxNetArea_Condition}
                                    {minPrice_Condition}
                                    {maxPrice_Condition}";
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
                                    <attribute name='bsd_viewphulong' />
                                    <attribute name='bsd_direction' />
                                    <attribute name='bsd_constructionarea' />
                                    <attribute name='bsd_netsaleablearea' />
                                    <attribute name='bsd_floor' alias='floorid'/>
                                    <attribute name='bsd_blocknumber' alias='blockid'/>
                                    <attribute name='bsd_phaseslaunchid' alias='_bsd_phaseslaunchid_value' />
                                    <attribute name='bsd_vippriority' />
                                    <order attribute='bsd_constructionarea' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='productid' operator='eq' uitype='product' value='{unitId}' />
                                    </filter>
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Unit>>("products", fetchXml);
            if (result == null || result.value.Any() == false) return;

            Unit = result.value.FirstOrDefault();
            await CheckShowBtnBangTinhGia(unitId);
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
                    if (Unit?.statuscode == 100000000 || Unit?.statuscode == 100000004)
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
    }
}
