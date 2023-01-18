using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class DirectSaleViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectListModel> Projects { get; set; } = new ObservableCollection<ProjectListModel>();
        public ObservableCollection<OptionSet> PhasesLaunchs { get; set; } = new ObservableCollection<OptionSet>();

        private List<OptionSetFilter> _viewOptions;
        public List<OptionSetFilter> ViewOptions { get => _viewOptions; set { _viewOptions = value;OnPropertyChanged(nameof(ViewOptions)); } }

        private List<Block> _blocks;
        public List<Block> Blocks { get => _blocks; set { _blocks = value; OnPropertyChanged(nameof(Blocks)); } }

        private List<OptionSetFilter> _directionOptions;
        public List<OptionSetFilter> DirectionOptions { get=>_directionOptions; set { _directionOptions = value;OnPropertyChanged(nameof(DirectionOptions)); } }

        private List<string> _selectedDirections;
        public List<string> SelectedDirections { get => _selectedDirections; set { _selectedDirections = value; OnPropertyChanged(nameof(SelectedDirections)); } }

        private List<string> _SelectedViews;
        public List<string> SelectedViews { get => _SelectedViews; set { _SelectedViews = value; OnPropertyChanged(nameof(SelectedViews)); } }

        private List<OptionSetFilter> _unitStatusOptions;
        public List<OptionSetFilter> UnitStatusOptions { get=>_unitStatusOptions; set { _unitStatusOptions = value;OnPropertyChanged(nameof(UnitStatusOptions)); } }

        private List<string> _selectedUnitStatus;
        public List<string> SelectedUnitStatus { get => _selectedUnitStatus; set { _selectedUnitStatus = value; OnPropertyChanged(nameof(SelectedUnitStatus)); } }

        private OptionSet _phasesLaunch;
        public OptionSet PhasesLaunch { get => _phasesLaunch; set { _phasesLaunch = value; OnPropertyChanged(nameof(PhasesLaunch)); } }

        private List<NetAreaDirectSaleModel> _netAreas;
        public List<NetAreaDirectSaleModel> NetAreas { get=>_netAreas; set { _netAreas = value; OnPropertyChanged(nameof(NetAreas)); } }

        private NetAreaDirectSaleModel _netArea;
        public NetAreaDirectSaleModel NetArea { get => _netArea; set { _netArea = value; OnPropertyChanged(nameof(NetArea)); } }

        private List<PriceDirectSaleModel> _prices;
        public List<PriceDirectSaleModel> Prices { get => _prices; set { _prices = value; OnPropertyChanged(nameof(Prices)); } }

        private PriceDirectSaleModel _price;
        public PriceDirectSaleModel Price { get => _price; set { _price = value; OnPropertyChanged(nameof(Price)); } }

        private string _unitCode;
        public string UnitCode { get => _unitCode; set { _unitCode = value; OnPropertyChanged(nameof(UnitCode)); } }

        private ProjectListModel _project;
        public ProjectListModel Project
        {
            get => _project;
            set
            {
                if (_project != value)
                {
                    PhasesLaunch = null;
                    PhasesLaunchs.Clear();
                    _project = value;
                    OnPropertyChanged(nameof(Project));
                    
                }
            }
        }

        private bool? _isEvent =false;
        public bool? IsEvent
        {
            get => _isEvent;
            set
            {
                if (_isEvent != value)
                {
                    this._isEvent = value;
                    OnPropertyChanged(nameof(IsEvent));
                }
            }
        }
        private bool _isOwner;
        public bool isOwner { get => _isOwner; set { _isOwner = value; OnPropertyChanged(nameof(isOwner)); } }
        

        public DirectSaleViewModel()
        {
        }

        public async Task LoadProject()
        {
            string fetch = "";
            var teamid = await LoadTeamID();
            if (teamid == null && teamid.Count > 0)
            {
                foreach(var item in teamid)
                {
                  fetch +=  $@"<condition entityname='share' attribute='principalid' operator='eq' value='{item.Name}'/>";
                }
                fetch += $@"<condition entityname='share' attribute='principalid' operator='eq' value='{UserLogged.Id}'/>";
            }

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='bsd_project'>
                                    <attribute name='bsd_projectid'/>
                                    <attribute name='bsd_projectcode'/>
                                    <attribute name='bsd_name'/>
                                    <attribute name='createdon' />
                                    <order attribute='bsd_name' descending='false' />
                                    <filter type='and'>
                                        <condition attribute='statuscode' operator='eq' value='861450002' />
                                            <filter type='or'>
                                                {fetch}
                                            </filter>
                                    </filter>
                                    <link-entity name='principalobjectaccess' to='bsd_projectid' from='objectid' link-type='inner' alias='share'/>
                                  </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ProjectListModel>>("bsd_projects", fetchXml);
            if (result == null || result.value.Any() == false) return;

             var data = result.value.Distinct().ToList();
             
            foreach (var item in data)
            {
                Projects.Add(item);
            }
        }

        public async Task<List<LookUp>> LoadTeamID()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='systemuser'>
                                    <attribute name='systemuserid' alias='Id'/>
                                    <filter type='and'>
                                        <condition attribute='systemuserid' operator='eq' value='{UserLogged.ManagerId}'/>
                                    </filter>
                                    <link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                        <link-entity name='team' from='teamid' to='teamid' alias='af'>
                                            <attribute name='teamid' alias='Name'/>
                                        </link-entity>
                                    </link-entity>
                                </entity>
                            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("systemusers", fetchXml);
            if (result == null || result.value.Any() == false) return null;

            return result.value;
        }

        public async Task LoadPhasesLanch()
        {
            if (Project == null) return;

            List<OptionSet> listTeam = new List<OptionSet>();
            List<OptionSet> listMember = new List<OptionSet>();

            string fetchCheckTeam = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                      <entity name='systemuser'>
                                        <filter type='and'>
                                            <condition attribute='systemuserid' operator='eq' value='{UserLogged.ManagerId}'/>
                                        </filter>
                                        <link-entity name='teammembership' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                            <link-entity name='team' from='teamid' to='teamid' alias='bh'>
                                                <filter type='or'>
                                                    <condition attribute='name' operator='like' value='Team_CLKD'/>
                                                    <condition attribute='name' operator='like' value='Team_DVKH'/>
                                                    <condition attribute='name' operator='like' value='Team_KT'/>
                                                    <condition attribute='name' operator='like' value='%Team[_]SALES%'/>
                                                </filter>
                                            </link-entity>
                                        </link-entity>
                                      </entity>
                                    </fetch>";
            var resultCheckTeam = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("systemusers", fetchCheckTeam);
            if (resultCheckTeam != null || resultCheckTeam.value.Any() != false)
            {
                string fetchXmlTeam = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                            <entity name='bsd_phaseslaunch'>
                                                <attribute name='bsd_name' alias='Label' />
                                                <attribute name='bsd_phaseslaunchid' alias='Val' />
                                                <order attribute='createdon' descending='true' />
                                                    <filter type='and'>
                                                        <condition attribute='statecode' operator='eq' value='0' />
                                                        <condition attribute='statuscode' operator='eq' value='100000000' />
                                                        <condition attribute='bsd_projectid' operator='eq' uitype='bsd_project' value='{Project.bsd_projectid}' />
                                                        <condition entityname='bteam' attribute='teamid' operator='null' />
                                                    </filter>
                                                <link-entity name='bsd_bsd_phaseslaunch_team' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' intersect='true' link-type='outer'>
                                                    <link-entity name='team' from='teamid' to='teamid' link-type='outer' alias='bteam' />
                                                </link-entity>
                                          </entity>
                                        </fetch>";
                var resultTeam = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_phaseslaunchs", fetchXmlTeam);
                if (resultTeam != null || resultTeam.value.Any() != false)
                    listTeam = resultTeam.value;
            }

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_phaseslaunch'>
                                    <attribute name='bsd_name' alias='Label' />
                                    <attribute name='bsd_phaseslaunchid' alias='Val' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                      <condition attribute='statuscode' operator='eq' value='100000000' />
                                      <condition attribute='bsd_projectid' operator='eq' uitype='bsd_project' value='{Project.bsd_projectid}' />
                                    </filter>
                                    <link-entity name='bsd_bsd_phaseslaunch_team' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' intersect='true'>
                                      <link-entity name='team' from='teamid' to='teamid' alias='team' intersect='true' >
                                        <link-entity name='teammembership' from='teamid' to='teamid'>
                                            <link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='user' intersect='true'>
                                                <filter type='and'>
                                                    <condition attribute='systemuserid' operator='eq' value='{UserLogged.ManagerId}'/>
                                                </filter>
                                            </link-entity>
                                        </link-entity>
                                      </link-entity>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_phaseslaunchs", fetchXml);
            if (result != null || result.value.Any() != false)
                listMember = result.value;

            var data = listMember.Union(listTeam).Distinct().ToList();
            foreach (var item in data)
            {
                PhasesLaunchs.Add(item);
            }
        }

        public async Task LoadBlocks()
        {
            string conditionPhaselaunch = string.Empty;
            conditionPhaselaunch = this.PhasesLaunch != null ? $@"<link-entity name='product' from='bsd_blocknumber' to='bsd_blockid' link-type='inner' alias='al'>
                                      <filter type='and'>
                                        <condition attribute='bsd_phaseslaunchid' operator='eq' uitype='bsd_phaseslaunch' value='{this.PhasesLaunch.Val}' />
                                      </filter>
                                    </link-entity>" : "";

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                  <entity name='bsd_block'>
                                    <attribute name='bsd_name' />
                                    <attribute name='bsd_blockid' />
                                    <order attribute='bsd_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bsd_project' operator='eq' uitype='bsd_project' value='{this.Project.bsd_projectid}' />
                                    </filter>
                                    {conditionPhaselaunch}
                                  </entity>
                                </fetch>";

            var block_result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<Block>>("bsd_blocks", fetchXml);
            if (block_result == null || block_result.value.Count == 0) return;

            this.Blocks = block_result.value;
        }
    }
}
