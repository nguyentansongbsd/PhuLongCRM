using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class DirectSaleDetailTestViewModel : BaseViewModel
    {
        private ObservableCollection<Block> _blocks;
        public ObservableCollection<Block> Blocks { get => _blocks; set { _blocks = value; OnPropertyChanged(nameof(Blocks)); } }

        private Block _block;
        public Block Block { get => _block; set { _block = value; OnPropertyChanged(nameof(Block)); } }

        private Unit _unit;
        public Unit Unit { get => _unit; set { _unit = value; OnPropertyChanged(nameof(Unit)); } }

        private bool _isShowBtnBangTinhGia;
        public bool IsShowBtnBangTinhGia { get => _isShowBtnBangTinhGia; set { _isShowBtnBangTinhGia = value; OnPropertyChanged(nameof(IsShowBtnBangTinhGia)); } }

        public DirectSaleSearchModel Filter { get; set; }

        private string FilterXml;

        public DirectSaleDetailTestViewModel()
        {
            Blocks = new ObservableCollection<Block>();
        }

        public void SetNumStatus(string UnitStatusNew, string UnitStatusOld,Guid floorId)
        {
            try
            {
                var floor = Block.Floors.SingleOrDefault(x => x.bsd_floorid == floorId);

                switch (UnitStatusOld)
                {
                    case "1":
                        this.Block.NumChuanBiInBlock--;
                        floor.NumChuanBiInFloor--;
                        break;
                    case "100000000":
                        this.Block.NumSanSangInBlock--;
                        floor.NumSanSangInFloor--;
                        break;
                    case "100000007":
                        this.Block.NumBookingInBlock--;
                        floor.NumBookingInFloor--;
                        break;
                    case "100000004":
                        this.Block.NumGiuChoInBlock--;
                        floor.NumGiuChoInFloor--;
                        break;
                    case "100000006":
                        this.Block.NumDatCocInBlock--;
                        floor.NumDatCocInFloor--;
                        break;
                    case "100000005":
                        this.Block.NumDongYChuyenCoInBlock--;
                        floor.NumDongYChuyenCoInFloor--;
                        break;
                    case "100000003":
                        this.Block.NumDaDuTienCocInBlock--;
                        floor.NumDaDuTienCocInFloor--;
                        break;
                    case "100000010":
                        this.Block.NumOptionInBlock--;
                        floor.NumOptionInFloor--;
                        break;
                    case "100000001":
                        this.Block.NumThanhToanDot1InBlock--;
                        floor.NumThanhToanDot1InFloor--;
                        break;
                    case "100000009":
                        this.Block.NumSignedDAInBlock--;
                        floor.NumSignedDAInFloor--;
                        break;
                    case "100000008":
                        this.Block.NumQualifiedInBlock--;
                        floor.NumQualifiedInFloor--;
                        break;
                    case "100000002":
                        this.Block.NumDaBanInBlock--;
                        floor.NumDaBanInFloor--;
                        break;
                    default:
                        break;
                }
                switch (UnitStatusNew)
                {
                    case "1":
                        this.Block.NumChuanBiInBlock++;
                        floor.NumChuanBiInFloor++;
                        break;
                    case "100000000":
                        this.Block.NumSanSangInBlock++;
                        floor.NumSanSangInFloor++;
                        break;
                    case "100000007":
                        this.Block.NumBookingInBlock++;
                        floor.NumBookingInFloor++;
                        break;
                    case "100000004":
                        this.Block.NumGiuChoInBlock++;
                        floor.NumGiuChoInFloor++;
                        break;
                    case "100000006":
                        this.Block.NumDatCocInBlock++;
                        floor.NumDatCocInFloor++;
                        break;
                    case "100000005":
                        this.Block.NumDongYChuyenCoInBlock++;
                        floor.NumDongYChuyenCoInFloor++;
                        break;
                    case "100000003":
                        this.Block.NumDaDuTienCocInBlock++;
                        floor.NumDaDuTienCocInFloor++;
                        break;
                    case "100000010":
                        this.Block.NumOptionInBlock++;
                        floor.NumOptionInFloor++;
                        break;
                    case "100000001":
                        this.Block.NumThanhToanDot1InBlock++;
                        floor.NumThanhToanDot1InFloor++;
                        break;
                    case "100000009":
                        this.Block.NumSignedDAInBlock++;
                        floor.NumSignedDAInFloor++;
                        break;
                    case "100000008":
                        this.Block.NumQualifiedInBlock++;
                        floor.NumQualifiedInFloor++;
                        break;
                    case "100000002":
                        this.Block.NumDaBanInBlock++;
                        floor.NumDaBanInFloor++;
                        break;
                    default:
                        break;
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        public void ChangeStatusUnitPopup(ResponseRealtime item)
        {
            if (this.Unit != null && this.Unit.productid == Guid.Parse(item.UnitId))
            {
                this.Unit.statuscode = int.Parse(item.StatusNew);
            }
        }

        public async Task LoadUnitByFloor(Guid floorId)
        {
            string linkentityOwner = Filter.isOwner ? $@"<link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='giucho' />
                                                        <link-entity name='salesorder' from='salesorderid' to='bsd_optionentry' link-type='outer' alias='hopdong' />
                                                        <link-entity name='quote' from='bsd_unitno' to='productid' link-type='outer' alias='btg' />" : "";

            string isEvent = (Filter.Event.HasValue && Filter.Event.Value) ? $@"<link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='as'>
                                                                                    <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='at'>
                                                                                        <filter type='and'>
                                                                                            <condition attribute='statuscode' operator='eq' value='100000000' />
                                                                                            <condition attribute='bsd_startdate' operator='on-or-before' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}'/>
                                                                                            <condition attribute='bsd_enddate' operator='on-or-after' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}' />
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
                                    <condition attribute='bsd_floor' operator='eq' uitype='bsd_floor' value='{floorId}' />
                                    {FilterXml}
                                </filter>
                                <link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='ag' >
                                    <attribute name='statuscode' alias='queses_statuscode'/>
                                    <attribute name='bsd_employee' alias='queue_employee_id'/>
                                </link-entity>
                            {linkentityOwner}
                            {isEvent}
                              </entity>
                            </fetch>";
            try
            {
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Unit>>("products", fetchXml);
                if (result == null || result.value.Any() == false) return;

                List<Unit> unitsResult = result.value.GroupBy(x => new
                {
                    productid = x.productid
                }).Select(y => y.First()).ToList();

                var units = Block.Floors.SingleOrDefault(x => x.bsd_floorid == floorId).Units;
                foreach (var item in unitsResult)
                {
                    // dem unit co nhung trang thai giu cho la: queuing, waiting
                    item.NumQueses = result.value.Where(x => x.productid == item.productid && (x.queses_statuscode == "100000000" || x.queses_statuscode == "100000002")).ToList().Count();
                    item.NumQueueEmployee = result.value.Where(x => x.productid == item.productid && (x.queses_statuscode == "100000000" || x.queses_statuscode == "100000002") && x.queue_employee_id == UserLogged.Id).ToList().Count();
                    units.Add(item);
                }
            }catch(Exception ex)
            {

            }
        }

        public void CreateFilterXml()
        {
            string Project = $"<condition attribute='bsd_projectcode' operator='eq' value='{Filter.Project}' />";
            string PhasesLaunch_Condition = (!string.IsNullOrWhiteSpace(Filter.Phase))
               ? $"<condition attribute='bsd_phaseslaunchid' operator='eq' value='{Filter.Phase}'/>"
               : null;

            string UnitCode_Condition = !string.IsNullOrEmpty(Filter.Unit)
                ? $"<condition attribute='name' operator='like' value='%25{Filter.Unit}%25'/>"
                : null;

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

            string isOwner = Filter.isOwner ? $@"<filter type='or'>
                                                    <filter type='and'>
                                                        <condition entityname='giucho' attribute='bsd_employee' operator='eq' value='{UserLogged.Id}'/>
                                                        <condition entityname='giucho' attribute='statuscode' operator='in'>
                                                            <value>100000000</value>
                                                            <value>100000002</value>
                                                        </condition>
                                                    </filter>

                                                    <filter type='and'>
                                                        <condition entityname='btg' attribute='statuscode' operator='in'>
                                                            <value>861450002</value>
                                                            <value>861450000</value>
                                                            <value>100000006</value>
                                                            <value>3</value>
                                                            <value>100000007</value>
                                                            <value>100000000</value>
                                                            <value>4</value>
                                                        </condition>
                                                        <condition entityname='btg' attribute='bsd_employee' operator='eq' value='{UserLogged.Id}'/>
                                                    </filter>

                                                    <filter type='and'>
                                                        <condition entityname='hopdong' attribute='bsd_employee' operator='eq' value='{UserLogged.Id}'/>
                                                        <condition entityname='hopdong' attribute='statuscode' operator='in'>
                                                            <value>100000001</value>
                                                            <value>100000009</value>
                                                            <value>100000003</value>
                                                            <value>100001</value>
                                                            <value>100000004</value>
                                                            <value>100000011</value>
                                                            <value>100000012</value>
                                                            <value>100000007</value>
                                                            <value>100000005</value>
                                                            <value>100000002</value>
                                                            <value>100000008</value>
                                                            <value>100000010</value>
                                                        </condition>
                                                    </filter>
	                                            </filter>" : "";

            FilterXml = $@"{Project}
                           {PhasesLaunch_Condition}
                           {UnitCode_Condition}
                           {UnitStatus_Condition}
                           {Direction_Condition}
                           {View_Condition}
                           {minNetArea_Condition}
                           {maxNetArea_Condition}
                           {minPrice_Condition}
                           {maxPrice_Condition}
                           {isOwner}";
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
                                    <attribute name='bsd_queuingfee' />
                                    <order attribute='bsd_constructionarea' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='productid' operator='eq' uitype='product' value='{unitId}' />
                                     </filter>
                                    <link-entity name='bsd_unittype' from='bsd_unittypeid' to='bsd_unittype' visible='false' link-type='outer' alias='a_493690ec6ce2e811a94e000d3a1bc2d1'>
                                      <attribute name='bsd_name'  alias='bsd_unittype_name'/>
                                    </link-entity>
                                    <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='outer' alias='ac'>
                                        <attribute name='bsd_name' alias='phaseslaunch_name' />
                                      <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='outer' alias='ad'>
                                        <attribute name='bsd_eventid' alias='event_id' />
                                        <filter type='and'>
                                            <condition attribute='statuscode' operator='eq' value='100000000' />
                                            <condition attribute='bsd_eventid' operator='not-null' />
                                        </filter>
                                      </link-entity>
                                    </link-entity>
                                    <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectcode' visible='false' link-type='outer' alias='a_a77d98e66ce2e811a94e000d3a1bc2d1'>
                                        <attribute name='bsd_name' alias='project_name' />
                                        <attribute name='bsd_queuesperunit' alias='project_queuesperunit' />
                                        <attribute name='bsd_unitspersalesman' alias='project_unitspersalesman' />
                                        <attribute name='bsd_queueunitdaysaleman' alias='project_queueunitdaysaleman'/>
                                        <attribute name='bsd_bookingfee' alias='project_bookingfee' />
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

        public async Task UpdateTotalDirectSale(Floor floor)
        {
            try
            {
                CreateFilterXml();
                string linkentityOwner = Filter.isOwner ? $@"<link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='giucho' />
                                                        <link-entity name='salesorder' from='salesorderid' to='bsd_optionentry' link-type='outer' alias='hopdong' />
                                                        <link-entity name='quote' from='bsd_unitno' to='productid' link-type='outer' alias='btg' />" : "";

                string groupbyOwner = Filter.isOwner ? $@"<attribute name='name' groupby='true' alias='group_unit_id'/>" : "";

                string isEvent = (Filter.Event.HasValue && Filter.Event.Value) ? $@"<link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='as'>
                                                                                    <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='at'>
                                                                                        <filter type='and'>
                                                                                            <condition attribute='statuscode' operator='eq' value='100000000' />
                                                                                            <condition attribute='bsd_startdate' operator='on-or-before' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}'/>
                                                                                            <condition attribute='bsd_enddate' operator='on-or-after' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}' />
                                                                                        </filter>
                                                                                    </link-entity>
                                                                                </link-entity>" : "";

                string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                    <entity name='product'>
                                        <attribute name='statuscode' groupby='true' alias='group_sts'/>
                                        <attribute name='productid' aggregate='count' alias='count'/>
                                        <attribute name='bsd_blocknumber' groupby='true' alias='group_block_id'/>
                                        <attribute name='bsd_floor' groupby='true' alias='group_floor_id'/>
                                        {groupbyOwner}
                                        <filter type='and'>
                                            {FilterXml}
                                        </filter>
	                                    <link-entity name='bsd_block' from='bsd_blockid' to='bsd_blocknumber' link-type='inner' alias='aa'>
                                            <attribute name='bsd_name' groupby='true' alias='group_block_name'/>
                                        </link-entity>
                                        <link-entity name='bsd_floor' from='bsd_floorid' to='bsd_floor' link-type='inner' alias='ab'>
                                            <attribute name='bsd_floor' groupby='true' alias='group_floor_name'/>
                                        </link-entity>
                                        {linkentityOwner}
                                        {isEvent}
                                    </entity>
                                </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DirectSaleFetchModel>>("products", fetchXml);
                if (result == null || result.value.Any() == false) return;

                var data = result.value;

                if (Filter.isOwner)
                {
                    foreach (var item in data)
                    {
                        item.count = 1;
                    }
                }

                var blocks = from item in data group item by item.group_block_name into g orderby g.Key select g;
                if (blocks != null && blocks.ToList().Count > 0)
                {
                    var block = blocks.SingleOrDefault(x => x.Key == Block.bsd_name.Replace("Block ", ""));

                    if (block != null)
                    {
                        Block.NumChuanBiInBlock = (from item in block group item by item.group_sts into g where g.Key == "1" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumSanSangInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000000" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumBookingInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000007" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumGiuChoInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000004" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumDatCocInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000006" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumDongYChuyenCoInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000005" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumDaDuTienCocInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000003" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumOptionInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000010" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumThanhToanDot1InBlock = (from item in block group item by item.group_sts into g where g.Key == "100000001" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumSignedDAInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000009" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumQualifiedInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000008" select g.Sum(u => u.count)).FirstOrDefault();
                        Block.NumDaBanInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000002" select g.Sum(u => u.count)).FirstOrDefault();

                        var floors = from item in block group item by item.group_floor_name into g orderby g.Key select g;
                        var f = floors.SingleOrDefault(x => x.Key == floor.bsd_name);
                        if (f != null)
                        {
                            floor.NumChuanBiInFloor = (from item in f group item by item.group_sts into g where g.Key == "1" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumSanSangInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000000" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumBookingInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000007" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumGiuChoInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000004" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumDatCocInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000006" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumDongYChuyenCoInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000005" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumDaDuTienCocInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000003" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumOptionInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000010" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumThanhToanDot1InFloor = (from item in f group item by item.group_sts into g where g.Key == "100000001" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumSignedDAInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000009" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumQualifiedInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000008" select g.Sum(u => u.count)).FirstOrDefault();
                            floor.NumDaBanInFloor = (from item in f group item by item.group_sts into g where g.Key == "100000002" select g.Sum(u => u.count)).FirstOrDefault();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public async Task LoadTotalDirectSale2()
        {
            try
            {
                CreateFilterXml();
                string linkentityOwner = Filter.isOwner ? $@"<link-entity name='opportunity' from='bsd_units' to='productid' link-type='outer' alias='giucho' />
                                                        <link-entity name='salesorder' from='salesorderid' to='bsd_optionentry' link-type='outer' alias='hopdong' />
                                                        <link-entity name='quote' from='bsd_unitno' to='productid' link-type='outer' alias='btg' />" : "";

                string groupbyOwner = Filter.isOwner ? $@"<attribute name='name' groupby='true' alias='group_unit_id'/>" : "";

                string isEvent = (Filter.Event.HasValue && Filter.Event.Value) ? $@"<link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='as'>
                                                                                    <link-entity name='bsd_event' from='bsd_phaselaunch' to='bsd_phaseslaunchid' link-type='inner' alias='at'>
                                                                                        <filter type='and'>
                                                                                            <condition attribute='statuscode' operator='eq' value='100000000' />
                                                                                            <condition attribute='bsd_startdate' operator='on-or-before' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}'/>
                                                                                            <condition attribute='bsd_enddate' operator='on-or-after' value='{string.Format("{0:yyyy-MM-dd}", DateTime.Now)}' />
                                                                                        </filter>
                                                                                    </link-entity>
                                                                                </link-entity>" : "";

                string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                    <entity name='product'>
                                        <attribute name='statuscode' groupby='true' alias='group_sts'/>
                                        <attribute name='productid' aggregate='count' alias='count'/>
                                        <attribute name='bsd_blocknumber' groupby='true' alias='group_block_id'/>
                                        <attribute name='bsd_floor' groupby='true' alias='group_floor_id'/>
                                        {groupbyOwner}
                                        <filter type='and'>
                                            {FilterXml}
                                        </filter>
	                                    <link-entity name='bsd_block' from='bsd_blockid' to='bsd_blocknumber' link-type='inner' alias='aa'>
                                            <attribute name='bsd_name' groupby='true' alias='group_block_name'/>
                                        </link-entity>
                                        <link-entity name='bsd_floor' from='bsd_floorid' to='bsd_floor' link-type='inner' alias='ab'>
                                            <attribute name='bsd_floor' groupby='true' alias='group_floor_name'/>
                                        </link-entity>
                                        {linkentityOwner}
                                        {isEvent}
                                    </entity>
                                </fetch>";
                var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DirectSaleFetchModel>>("products", fetchXml);
                if (result == null || result.value.Any() == false) return;

                var data = result.value;

                if (Filter.isOwner)
                {
                    foreach (var item in data)
                    {
                        item.count = 1;
                    }
                }

                var blocks = from item in data group item by item.group_block_name into g orderby g.Key select g;
                foreach (var block in blocks)
                {
                    Block b = new Block();
                    b.bsd_blockid = Guid.Parse(block.FirstOrDefault().group_block_id);
                    b.bsd_name = "Block " + block.Key;
                    b.TotalUnitInBlock = block.Sum(u => u.count);

                    b.NumChuanBiInBlock = (from item in block group item by item.group_sts into g where g.Key == "1" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumSanSangInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000000" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumBookingInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000007" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumGiuChoInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000004" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumDatCocInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000006" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumDongYChuyenCoInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000005" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumDaDuTienCocInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000003" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumOptionInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000010" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumThanhToanDot1InBlock = (from item in block group item by item.group_sts into g where g.Key == "100000001" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumSignedDAInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000009" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumQualifiedInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000008" select g.Sum(u => u.count)).FirstOrDefault();
                    b.NumDaBanInBlock = (from item in block group item by item.group_sts into g where g.Key == "100000002" select g.Sum(u => u.count)).FirstOrDefault();

                    var floors = from item in block group item by item.group_floor_name into g orderby g.Key select g;
                    foreach (var floor in floors)
                    {
                        Floor f = new Floor();
                        f.bsd_floorid = Guid.Parse(floor.FirstOrDefault().group_floor_id);
                        f.bsd_name = floor.Key;
                        f.TotalUnitInFloor = floor.Sum(u => u.count);

                        f.NumChuanBiInFloor = (from item in floor group item by item.group_sts into g where g.Key == "1" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumSanSangInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000000" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumBookingInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000007" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumGiuChoInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000004" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumDatCocInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000006" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumDongYChuyenCoInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000005" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumDaDuTienCocInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000003" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumOptionInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000010" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumThanhToanDot1InFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000001" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumSignedDAInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000009" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumQualifiedInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000008" select g.Sum(u => u.count)).FirstOrDefault();
                        f.NumDaBanInFloor = (from item in floor group item by item.group_sts into g where g.Key == "100000002" select g.Sum(u => u.count)).FirstOrDefault();
                        b.Floors.Add(f);
                    }
                    Blocks.Add(b);
                }
            }catch(Exception ex)
            {

            }
        }
    }
    public class DirectSaleFetchModel
    {
        public string group_sts { get; set; }
        public string group_block_id { get; set; }
        public string group_block_name { get; set; }
        public string group_floor_id { get; set; }
        public string group_floor_name { get; set; }
        public int count { get; set; }
        public string group_unit_id { get; set; }
    }

    public class ResponseRealtime
    {
        public string ProjectId { get; set; }
        public string BlockId { get; set; }
        public string FloorId { get; set; }
        public string UnitId { get; set; }
        public string StatusNew { get; set; }
        public string StatusOld { get; set; }

    }
}
