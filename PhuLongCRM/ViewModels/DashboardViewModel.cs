using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public ObservableCollection<ActivitiModel> Activities { get; set; } = new ObservableCollection<ActivitiModel>();

        public ObservableCollection<ChartModel> DataMonthQueue { get; set; } = new ObservableCollection<ChartModel>();
        public ObservableCollection<ChartModel> DataMonthQuote { get; set; } = new ObservableCollection<ChartModel>();
        public ObservableCollection<ChartModel> DataMonthOptionEntry { get; set; } = new ObservableCollection<ChartModel>();
        public ObservableCollection<ChartModel> DataMonthUnit { get; set; } = new ObservableCollection<ChartModel>();

        public ObservableCollection<ChartModel> CommissionTransactionChart { get; set; } = new ObservableCollection<ChartModel>();
        public ObservableCollection<ChartModel> CommissionTransactionChart2 { get; set; } = new ObservableCollection<ChartModel>();
        public ObservableCollection<ChartModel> LeadsChart { get; set; } = new ObservableCollection<ChartModel>();

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }

        private decimal _totalCommissionAMonth;
        public decimal TotalCommissionAMonth { get => _totalCommissionAMonth; set { _totalCommissionAMonth = value; OnPropertyChanged(nameof(TotalCommissionAMonth)); } }

        private decimal _totalPaidCommissionAMonth;
        public decimal TotalPaidCommissionAMonth { get => _totalPaidCommissionAMonth; set { _totalPaidCommissionAMonth = value; OnPropertyChanged(nameof(TotalPaidCommissionAMonth)); } }

        private int _numQueue;
        public int numQueue { get => _numQueue; set { _numQueue = value; OnPropertyChanged(nameof(numQueue)); } }
        private int _numQuote;
        public int numQuote { get => _numQuote; set { _numQuote = value; OnPropertyChanged(nameof(numQuote)); } }
        private int _numOptionEntry;
        public int numOptionEntry { get => _numOptionEntry; set { _numOptionEntry = value; OnPropertyChanged(nameof(numOptionEntry)); } }
        private int _numUnit;
        public int numUnit { get => _numUnit; set { _numUnit = value; OnPropertyChanged(nameof(numUnit)); } }

        private int _numKHMoi;
        public int numKHMoi { get => _numKHMoi; set { _numKHMoi = value; OnPropertyChanged(nameof(numKHMoi)); } }
        private int _numKHDaChuyenDoi;
        public int numKHDaChuyenDoi { get => _numKHDaChuyenDoi; set { _numKHDaChuyenDoi = value; OnPropertyChanged(nameof(numKHDaChuyenDoi)); } }
        private int _numKHKhongChuyenDoi;
        public int numKHKhongChuyenDoi { get => _numKHKhongChuyenDoi; set { _numKHKhongChuyenDoi = value; OnPropertyChanged(nameof(numKHKhongChuyenDoi)); } }

        private PhoneCellModel _phoneCall;
        public PhoneCellModel PhoneCall { get => _phoneCall; set { _phoneCall = value; OnPropertyChanged(nameof(PhoneCall)); } }
        private TaskFormModel _taskDetail;
        public TaskFormModel TaskDetail { get => _taskDetail; set { _taskDetail = value; OnPropertyChanged(nameof(TaskDetail)); } }
        private MeetingModel _meet;
        public MeetingModel Meet { get => _meet; set { _meet = value; OnPropertyChanged(nameof(Meet)); } }

        #region Hoat dong
        private StatusCodeModel _activityStatusCode;
        public StatusCodeModel ActivityStatusCode { get => _activityStatusCode; set { _activityStatusCode = value; OnPropertyChanged(nameof(ActivityStatusCode)); } }
        private string _activityType;
        public string ActivityType { get => _activityType; set { _activityType = value; OnPropertyChanged(nameof(ActivityType)); } }
        public bool _showGridButton;
        public bool ShowGridButton { get => _showGridButton; set { _showGridButton = value; OnPropertyChanged(nameof(ShowGridButton)); } }
        private DateTime? _scheduledStartTask;
        public DateTime? ScheduledStartTask { get => _scheduledStartTask; set { _scheduledStartTask = value; OnPropertyChanged(nameof(ScheduledStartTask)); } }
        private DateTime? _scheduledEndTask;
        public DateTime? ScheduledEndTask { get => _scheduledEndTask; set { _scheduledEndTask = value; OnPropertyChanged(nameof(ScheduledEndTask)); } }
        public string CodeCompleted = "completed";
        public string CodeCancel = "cancel";
        #endregion

        private DateTime _dateBefor;
        public DateTime dateBefor { get => _dateBefor; set { _dateBefor = value; OnPropertyChanged(nameof(dateBefor)); } }
        public DateTime dateAfter { get; set; }

        public DateTime first_Month { get; set; }
        public DateTime second_Month { get; set; }
        public DateTime third_Month { get; set; }
        public DateTime fourth_Month { get; set; }
        // tổng tiền hoa đồng format
        private string _totalCommission;
        public string TotalCommission { get => _totalCommission; set { _totalCommission = value; OnPropertyChanged(nameof(TotalCommission)); } }
        // tổng tiền thanh toán format
        private string _totalPaidCommission;
        public string TotalPaidCommission { get => _totalPaidCommission; set { _totalPaidCommission = value; OnPropertyChanged(nameof(TotalPaidCommission)); } }

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            await RefreshDashboard();
            IsRefreshing = false;
        });

        public DashboardViewModel()
        {
            dateBefor = DateTime.Now;
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
            dateAfter = new DateTime(threeMonthsAgo.Year, threeMonthsAgo.Month, 1);
            first_Month = dateAfter;
            second_Month = dateAfter.AddMonths(1);
            third_Month = second_Month.AddMonths(1);
            fourth_Month = dateBefor;

            PhoneCall = new PhoneCellModel();
            TaskDetail = new TaskFormModel();
            Meet = new MeetingModel();
        }

        public async Task LoadCommissionTransactions()
        {
            string attribute = UserLogged.IsLoginByUserCRM ? "bsd_salestaff" : "bsd_employee";

            //Phu long khong co file nay <attribute name='bsd_totalcommission' alias='CommissionTotal'/> // 100000003 :Accountant Confirmed
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_commissiontransaction'>
                                        <attribute name='bsd_commissiontransactionid' />
                                        <attribute name='bsd_name' />
                                        <attribute name='createdon' />
                                        <attribute name='bsd_totalamountpaid' />
                                        <attribute name='bsd_totalamount' />
                                        <attribute name='statuscode' />
                                        <attribute name='statecode' />
                                        <order attribute='bsd_name' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='createdon' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                            <condition attribute='{attribute}' operator='eq' value='{UserLogged.Id}' />
                                        </filter>
                                        <link-entity name='bsd_commissioncalculation' from='bsd_commissioncalculationid' to='bsd_commissioncalculation' link-type='inner'>
                                            <attribute name='statuscode' alias='statuscode_calculator'/>
                                            <filter type='and'>
                                                <condition attribute='statuscode' operator='eq' value='100000003' />
                                            </filter>
                                        </link-entity>
                                    </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CommissionTransaction>>("bsd_commissiontransactions", fetchXml);
            if (result == null) return;

            decimal countTotalCommissionFr = 0;
            decimal countTotalCommissionSe = 0;
            decimal countTotalCommissionTh = 0;
            decimal countTotalCommissionFo = 0;

            decimal countTotalPaidFr = 0;
            decimal countTotalPaidSe = 0;
            decimal countTotalPaidTh = 0;
            decimal countTotalPaidFo = 0;

            foreach (var item in result.value)
            {
                // danh sách các hhgd có sts = Accountant Confirmed lấy từ entity Commission Calculator trong 4 tháng từ ngày hiện tại trở về trước
                if (item.statuscode_calculator == 100000003 && item.createdon.Month == first_Month.Month)
                {
                    countTotalCommissionFr += item.bsd_totalamount;
                    if (item.statuscode == 100000009)
                        countTotalPaidFr += item.bsd_totalamountpaid;
                }
                if (item.statuscode_calculator == 100000003 && item.createdon.Month == second_Month.Month)
                {
                    countTotalCommissionSe += item.bsd_totalamount;
                    if (item.statuscode == 100000009)
                        countTotalPaidSe += item.bsd_totalamountpaid;
                }
                if (item.statuscode_calculator == 100000003 && item.createdon.Month == third_Month.Month)
                {
                    countTotalCommissionTh += item.bsd_totalamount;
                    if (item.statuscode == 100000009)
                        countTotalPaidTh += item.bsd_totalamountpaid;
                }
                if (item.statuscode_calculator == 100000003 && item.createdon.Month == fourth_Month.Month)
                {
                    // tổng hhgd và hhgd paid được tính ở tháng hiện tại (sử dụng cho 2 giá trị thống kê)
                    countTotalCommissionFo += item.bsd_totalamount;
                    TotalCommissionAMonth += item.bsd_totalamount;
                    if (item.statuscode == 100000009)
                    {
                        countTotalPaidFo += item.bsd_totalamountpaid;
                        TotalPaidCommissionAMonth += item.bsd_totalamountpaid;
                    }
                }
            }

            ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionFr) };
            ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionSe) };
            ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionTh) };
            ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionFo) };

            ChartModel chartFirstMonth2 = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidFr) };
            ChartModel chartSecondMonth2 = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidSe) };
            ChartModel chartThirdMonth2 = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidTh) };
            ChartModel chartFourthMonth2 = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidFo) };

            this.CommissionTransactionChart.Add(chartFirstMonth);
            this.CommissionTransactionChart.Add(chartSecondMonth);
            this.CommissionTransactionChart.Add(chartThirdMonth);
            this.CommissionTransactionChart.Add(chartFourthMonth);

            this.CommissionTransactionChart2.Add(chartFirstMonth2);
            this.CommissionTransactionChart2.Add(chartSecondMonth2);
            this.CommissionTransactionChart2.Add(chartThirdMonth2);
            this.CommissionTransactionChart2.Add(chartFourthMonth2);

            //format sau khi tính tổng
            TotalCommission = StringFormatHelper.FormatCurrency(TotalCommissionAMonth);
            TotalPaidCommission = StringFormatHelper.FormatCurrency(TotalPaidCommissionAMonth);
        }
        private double TotalAMonth(decimal total)
        {
            if (total > 0 && total.ToString().Length > 6)
            {
                var _currency = decimal.ToDouble(total).ToString().Substring(0, decimal.ToDouble(total).ToString().Length - 6);
                return double.Parse(_currency);
            }
            else
            {
                return 0;
            }
        }
        public async Task LoadQueueFourMonths()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='opportunity'>
                                        <attribute name='bsd_bookingtime' alias='Date'/>
                                        <attribute name='opportunityid' alias='Id' />
                                            <filter type='and'>
                                                <condition attribute='bsd_bookingtime' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                                <condition attribute='statuscode' operator='in'>
                                                    <value>100000002</value>
                                                    <value>100000000</value>
                                                </condition>
                                                <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                            </filter>
                                    </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DashboardChartModel>>("opportunities", fetchXml);
            if (result == null) return;

            var countQueueFr = result.value.Where(x => x.Date.ToLocalTime().Month == first_Month.Month).Count();
            ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = countQueueFr };

            var countQueueSe = result.value.Where(x => x.Date.ToLocalTime().Month == second_Month.Month).Count();
            ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = countQueueSe };

            var countQueueTh = result.value.Where(x => x.Date.ToLocalTime().Month == third_Month.Month).Count();
            ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = countQueueTh };

            var countQueueFo = this.numQueue = result.value.Where(x => x.Date.ToLocalTime().Month == fourth_Month.Month).Count();
            ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = countQueueFo };

            this.DataMonthQueue.Add(chartFirstMonth);
            this.DataMonthQueue.Add(chartSecondMonth);
            this.DataMonthQueue.Add(chartThirdMonth);
            this.DataMonthQueue.Add(chartFourthMonth);
        }
        public async Task LoadQuoteFourMonths()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='quote'>
                                    <attribute name='bsd_deposittime' alias='Date' />
                                    <attribute name='quoteid' alias='Id' />
                                    <filter type='and'>
                                      <condition attribute='statuscode' operator='in'>
                                        <value>3</value>
                                        <value>861450000</value>
                                        <value>4</value>
                                      </condition>
                                      <condition attribute='bsd_deposittime' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DashboardChartModel>>("quotes", fetchXml);
            if (result == null) return;

            var countQuoteFr = result.value.Where(x => x.Date.ToLocalTime().Month == first_Month.Month).Count();
            ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = countQuoteFr };

            var countQuoteSe = result.value.Where(x => x.Date.ToLocalTime().Month == second_Month.Month).Count();
            ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = countQuoteSe };

            var countQuoteTh = result.value.Where(x => x.Date.ToLocalTime().Month == third_Month.Month).Count();
            ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = countQuoteTh };

            var countQuoteFo = this.numQuote = result.value.Where(x => x.Date.ToLocalTime().Month == fourth_Month.Month).Count();
            ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = countQuoteFo };

            this.DataMonthQuote.Add(chartFirstMonth);
            this.DataMonthQuote.Add(chartSecondMonth);
            this.DataMonthQuote.Add(chartThirdMonth);
            this.DataMonthQuote.Add(chartFourthMonth);
        }
        public async Task LoadOptionEntryFourMonths()
        {
            // ngoại trừ các sts Terminated , 1st Installment, Option, Qualify, Signed D.A
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='salesorder'>
                                    <attribute name='createdon' alias='Date'/>
                                    <attribute name='quoteid' alias='Id'/>
                                    <filter type='and'>
                                      <condition attribute='statuscode' operator='not-in'>
                                        <value>100000006</value>
                                        <value>100000001</value>
                                        <value>100000000</value>
                                        <value>100000007</value>
                                        <value>100000008</value>
                                      </condition>
                                      <condition attribute='createdon' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DashboardChartModel>>("salesorders", fetchXml);
            if (result == null) return;

            var countOptionEntryFr = result.value.Where(x => x.Date.ToLocalTime().Month == first_Month.Month).Count();
            ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = countOptionEntryFr };

            var countOptionEntrySe = result.value.Where(x => x.Date.ToLocalTime().Month == second_Month.Month).Count();
            ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = countOptionEntrySe };

            var countOptionEntryTh = result.value.Where(x => x.Date.ToLocalTime().Month == third_Month.Month).Count();
            ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = countOptionEntryTh };

            var countOptionEntryFo = this.numOptionEntry = result.value.Where(x => x.Date.ToLocalTime().Month == fourth_Month.Month).Count();
            ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = countOptionEntryFo };

            this.DataMonthOptionEntry.Add(chartFirstMonth);
            this.DataMonthOptionEntry.Add(chartSecondMonth);
            this.DataMonthOptionEntry.Add(chartThirdMonth);
            this.DataMonthOptionEntry.Add(chartFourthMonth);
        }
        public async Task LoadUnitFourMonths()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='salesorder'>
                                    <attribute name='salesorderid' alias='Id'/>
                                    <attribute name='bsd_signedcontractdate' alias='Date' />
                                    <order attribute='createdon' descending='true' />
                                    <filter type='and'>
                                      <condition attribute='bsd_unitstatus' operator='eq' value='100000002' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                      <condition attribute='bsd_signedcontractdate' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<DashboardChartModel>>("salesorders", fetchXml);
            if (result == null) return;

            var countUnitFr = result.value.Where(x => x.Date.ToLocalTime().Month == first_Month.Month).Count();
            ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = countUnitFr };

            var countUnitSe = result.value.Where(x => x.Date.ToLocalTime().Month == second_Month.Month).Count();
            ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = countUnitSe };

            var countUnitTh = result.value.Where(x => x.Date.ToLocalTime().Month == third_Month.Month).Count();
            ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = countUnitTh };

            var countUnitFo = this.numUnit = result.value.Where(x => x.Date.ToLocalTime().Month == fourth_Month.Month).Count();
            ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = countUnitFo };

            this.DataMonthUnit.Add(chartFirstMonth);
            this.DataMonthUnit.Add(chartSecondMonth);
            this.DataMonthUnit.Add(chartThirdMonth);
            this.DataMonthUnit.Add(chartFourthMonth);
        }
        public async Task LoadLeads()
        {
            ChartModel chartKHMoi = new ChartModel();
            ChartModel chartKHDaChuyenDoi = new ChartModel();
            ChartModel chartKHKhongChuyenDoi = new ChartModel();

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='lead'>
                                    <attribute name='statuscode' alias='Label'/>
                                    <attribute name='leadid' alias='Val' />
                                    <filter type='and'>
                                      <condition attribute='createdon' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                      <condition attribute='statuscode' operator='ne' value='2'/>
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("leads", fetchXml);
            if (result != null && result.value.Count > 0)
            {
                numKHMoi = result.value.Where(x => x.Label == "1").Count();
                numKHDaChuyenDoi = result.value.Where(x => x.Label == "3").Count();
                numKHKhongChuyenDoi = result.value.Where(x => x.Label == "4" || x.Label == "5" || x.Label == "6" || x.Label == "7").Count();

                chartKHMoi = new ChartModel() { Category = Language.khach_hang_moi, Value = numKHMoi };
                chartKHDaChuyenDoi = new ChartModel() { Category = Language.da_chuyen_doi, Value = numKHDaChuyenDoi };
                chartKHKhongChuyenDoi = new ChartModel() { Category = Language.khong_chuyen_doi, Value = numKHKhongChuyenDoi };
            }
            else
            {
                chartKHMoi = new ChartModel() { Category = Language.khach_hang_moi, Value = 1 };
                chartKHDaChuyenDoi = new ChartModel() { Category = Language.da_chuyen_doi, Value = 1 };
                chartKHKhongChuyenDoi = new ChartModel() { Category = Language.khong_chuyen_doi, Value = 1 };
            }

            this.LeadsChart.Add(chartKHMoi);
            this.LeadsChart.Add(chartKHDaChuyenDoi);
            this.LeadsChart.Add(chartKHKhongChuyenDoi);
        }

        public async Task LoadTasks()
        {
            string fetchXml = $@"<fetch version='1.0' count='5' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='task'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />
                                    <attribute name='createdon' />
                                    <order attribute='scheduledstart' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='a_48f82b1a8ad844bd90d915e7b3c4f263'>
                                          <attribute name='fullname' alias='contact_name'/>
                                          <attribute name='contactid' alias='contact_id'/>
                                    </link-entity>
                                    <link-entity name='account' from='accountid' to='regardingobjectid' visible='false' link-type='outer' alias='a_9cdbdceab5ee4a8db875050d455757bd'>
                                          <attribute name='accountid' alias='account_id'/>
                                          <attribute name='name' alias='account_name'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' visible='false' link-type='outer' alias='a_0a67d7c87cd1eb11bacc000d3a80021e'>
                                          <attribute name='leadid' alias='lead_id'/>
                                          <attribute name='lastname' alias='lead_name'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ActivitiModel>>("tasks", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                item.scheduledstart = item.scheduledstart.ToLocalTime();
                item.scheduledend = item.scheduledend.ToLocalTime();
                if (!string.IsNullOrWhiteSpace(item.contact_name))
                {
                    item.customer = item.contact_name;
                }
                if (!string.IsNullOrWhiteSpace(item.account_name))
                {
                    item.customer = item.account_name;
                }
                if (!string.IsNullOrWhiteSpace(item.lead_name))
                {
                    item.customer = item.lead_name;
                }

                this.Activities.Add(item);
            }
        }

        public async Task LoadMettings()
        {
            string fetchXml = $@"<fetch version='1.0' count='5' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='appointment'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />   
                                    <attribute name='createdon' />
                                    <order attribute='scheduledstart' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='in'>
                                            <value>0</value>
                                            <value>3</value>
                                        </condition>
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='a_48f82b1a8ad844bd90d915e7b3c4f263'>
                                        <attribute name='fullname' alias='contact_name'/>
                                        <attribute name='contactid' alias='contact_id'/>
                                    </link-entity>
                                    <link-entity name='account' from='accountid' to='regardingobjectid' visible='false' link-type='outer' alias='a_9cdbdceab5ee4a8db875050d455757bd'>
                                          <attribute name='accountid' alias='account_id'/>
                                          <attribute name='name' alias='account_name'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' visible='false' link-type='outer' alias='a_0a67d7c87cd1eb11bacc000d3a80021e'>
                                          <attribute name='leadid' alias='lead_id'/>
                                          <attribute name='lastname' alias='lead_name'/>
                                    </link-entity>
                                    <link-entity name='activityparty' from='activityid' to='activityid' link-type='outer' alias='aee'>
                                        <filter type='and'>
                                            <condition attribute='participationtypemask' operator='eq' value='5' />
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ActivitiModel>>("appointments", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                var meet = Activities.FirstOrDefault(x => x.activityid == item.activityid);
                if (meet != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                    {
                        string new_customer = ", " + item.callto_contact_name;
                        meet.customer += new_customer;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_accounts_name))
                    {
                        string new_customer = ", " + item.callto_accounts_name;
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
                    item.scheduledstart = item.scheduledstart.ToLocalTime();
                    item.scheduledend = item.scheduledend.ToLocalTime();
                    if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                    {
                        item.customer = item.callto_contact_name;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_accounts_name))
                    {
                        item.customer = item.callto_accounts_name;
                    }
                    if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                    {
                        item.customer = item.callto_lead_name;
                    }
                    this.Activities.Add(item);
                }
            }
        }

        public async Task LoadPhoneCalls()
        {
            string fetchXml = $@"<fetch version='1.0' count='5' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='phonecall'>
                                    <attribute name='subject' />
                                    <attribute name='activityid' />
                                    <attribute name='scheduledstart' />
                                    <attribute name='scheduledend' />
                                    <attribute name='activitytypecode' />
                                    <attribute name='createdon' />
                                    <order attribute='scheduledstart' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                    <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='a_48f82b1a8ad844bd90d915e7b3c4f263'>
                                        <attribute name='fullname' alias='contact_name'/>
                                        <attribute name='contactid' alias='contact_id'/>
                                    </link-entity>
                                    <link-entity name='account' from='accountid' to='regardingobjectid' visible='false' link-type='outer' alias='a_9cdbdceab5ee4a8db875050d455757bd'>
                                          <attribute name='accountid' alias='account_id'/>
                                          <attribute name='name' alias='account_name'/>
                                    </link-entity>
                                    <link-entity name='lead' from='leadid' to='regardingobjectid' visible='false' link-type='outer' alias='a_0a67d7c87cd1eb11bacc000d3a80021e'>
                                          <attribute name='leadid' alias='lead_id'/>
                                          <attribute name='lastname' alias='lead_name'/>
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ActivitiModel>>("phonecalls", fetchXml);
            if (result == null || result.value.Count == 0) return;

            foreach (var item in result.value)
            {
                item.scheduledstart = item.scheduledstart.ToLocalTime();
                item.scheduledend = item.scheduledend.ToLocalTime();
                if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                {
                    item.customer = item.callto_contact_name;
                }
                if (!string.IsNullOrWhiteSpace(item.callto_accounts_name))
                {
                    item.customer = item.callto_accounts_name;
                }
                if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                {
                    item.customer = item.callto_lead_name;
                }

                this.Activities.Add(item);
            }
        }

        public async Task RefreshDashboard()
        {
            this.Activities.Clear();
            this.DataMonthQueue.Clear();
            this.DataMonthQuote.Clear();
            this.DataMonthOptionEntry.Clear();
            this.DataMonthUnit.Clear();
            this.CommissionTransactionChart.Clear();
            this.LeadsChart.Clear();

            this.TotalCommissionAMonth = 0;
            this.TotalPaidCommissionAMonth = 0;

            await Task.WhenAll(
                 this.LoadTasks(),
                 this.LoadMettings(),
                 this.LoadPhoneCalls(),
                 this.LoadQueueFourMonths(),
                 this.LoadQuoteFourMonths(),
                 this.LoadOptionEntryFourMonths(),
                 this.LoadUnitFourMonths(),
                 this.LoadLeads(),
                 this.LoadCommissionTransactions()
                );
        }

        public async Task loadPhoneCall(Guid id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name='phonecall'>
                                <attribute name='subject' />
                                <attribute name='statecode' />
                                <attribute name='prioritycode' />
                                <attribute name='scheduledend' alias='scheduledend' />
                                <attribute name='createdby' />
                                <attribute name='regardingobjectid' />
                                <attribute name='activityid' />
                                <attribute name='statuscode' />
                                <attribute name='scheduledstart' alias='scheduledstart' />
                                <attribute name='actualdurationminutes' />
                                <attribute name='description' />
                                <attribute name='activitytypecode' />
                                <attribute name='phonenumber' />
                                <order attribute='subject' descending='false' />
                                <filter type='and'>
                                    <condition attribute='activityid' operator='eq' uitype='phonecall' value='" + id + @"' />
                                </filter>
                                <link-entity name='account' from='accountid' to='regardingobjectid' visible='false' link-type='outer' alias='a_9b4f4019bdc24dd79b1858c2d087a27d'>
                                    <attribute name='accountid' alias='account_id' />                  
                                    <attribute name='bsd_name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='a_66b6d0af970a40c9a0f42838936ea5ce'>
                                    <attribute name='contactid' alias='contact_id' />                  
                                    <attribute name='fullname' alias='contact_name'/>
                                </link-entity>
                                <link-entity name='lead' from='leadid' to='regardingobjectid' visible='false' link-type='outer' alias='a_fb87dbfd8304e911a98b000d3aa2e890'>
                                    <attribute name='leadid' alias='lead_id'/>                  
                                    <attribute name='fullname' alias='lead_name'/>
                                </link-entity>
                                <link-entity name='bsd_employee' from='bsd_employeeid' to='bsd_employee' link-type='outer' alias='aa'>
                                    <attribute name='bsd_name' alias='user_name'/>
                                    <attribute name='bsd_employeeid' alias='user_id'/>
                                </link-entity>
                            </entity>
                          </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PhoneCellModel>>("phonecalls", fetch);
            if (result == null || result.value == null)
                return;
            var data = result.value.FirstOrDefault();
            PhoneCall = data;

            if (data.scheduledend != null && data.scheduledstart != null)
            {
                PhoneCall.scheduledend = data.scheduledend.Value.ToLocalTime();
                PhoneCall.scheduledstart = data.scheduledstart.Value.ToLocalTime();
            }

            if (PhoneCall.contact_id != Guid.Empty)
            {
                PhoneCall.Customer = new CustomerLookUp
                {
                    Name = PhoneCall.contact_name
                };
            }
            else if (PhoneCall.account_id != Guid.Empty)
            {
                PhoneCall.Customer = new CustomerLookUp
                {
                    Name = PhoneCall.account_name
                };
            }
            else if (PhoneCall.lead_id != Guid.Empty)
            {
                PhoneCall.Customer = new CustomerLookUp
                {
                    Name = PhoneCall.lead_name
                };
            }

            if (PhoneCall.statecode == 0)
                ShowGridButton = true;
            else
                ShowGridButton = false;
        }

        public async Task loadFromTo(Guid id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' >
                                    <entity name='phonecall' >
                                        <attribute name='subject' />
                                        <attribute name='activityid' />
                                        <order attribute='subject' descending='false' />
                                        <filter type='and' >
                                            <condition attribute='activityid' operator='eq' value='" + id + @"' />
                                        </filter>
                                        <link-entity name='activityparty' from='activityid' to='activityid' link-type='inner' alias='ab' >
                                            <attribute name='partyid' alias='partyID'/>
                                            <attribute name='participationtypemask' alias='typemask' />
                                            <link-entity name='account' from='accountid' to='partyid' link-type='outer' alias='partyaccount' >
                                                <attribute name='bsd_name' alias='account_name'/>
                                            </link-entity>
                                            <link-entity name='contact' from='contactid' to='partyid' link-type='outer' alias='partycontact' >
                                                <attribute name='fullname' alias='contact_name'/>
                                            </link-entity>
                                            <link-entity name='lead' from='leadid' to='partyid' link-type='outer' alias='partylead' >
                                                <attribute name='fullname' alias='lead_name'/>
                                            </link-entity>
                                            <link-entity name='systemuser' from='systemuserid' to='partyid' link-type='outer' alias='partyuser' >
                                                <attribute name='fullname' alias='user_name'/>
                                            </link-entity>
                                        </link-entity>
                                    </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PartyModel>>("phonecalls", fetch);
            if (result == null || result.value == null)
                return;
            var data = result.value;
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (item.typemask == 1) // from call
                    {
                        PhoneCall.call_from = item.user_name;
                    }
                    else if (item.typemask == 2) // to call
                    {
                        if (item.contact_name != null && item.contact_name != string.Empty)
                        {
                            PhoneCall.call_to = item.contact_name;
                        }
                        else if (item.account_name != null && item.account_name != string.Empty)
                        {
                            PhoneCall.call_to = item.account_name;
                        }
                        else if (item.lead_name != null && item.lead_name != string.Empty)
                        {
                            PhoneCall.call_to = item.lead_name;
                        }
                    }
                }
            }
        }

        public async Task loadTask(Guid id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='task'>
                                <attribute name='subject' />
                                <attribute name='statecode' />
                                <attribute name='scheduledend' />
                                <attribute name='activityid' />
                                <attribute name='statuscode' />
                                <attribute name='scheduledstart' />
                                <attribute name='description' />
                                <order attribute='subject' descending='false' />
                                <filter type='and' >
                                    <condition attribute='activityid' operator='eq' value='" + id + @"' />
                                </filter>
                                <link-entity name='account' from='accountid' to='regardingobjectid' link-type='outer' alias='ah'>
    	                            <attribute name='accountid' alias='account_id' />                  
    	                            <attribute name='bsd_name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='contact' from='contactid' to='regardingobjectid' link-type='outer' alias='ai'>
	                            <attribute name='contactid' alias='contact_id' />                  
                                    <attribute name='fullname' alias='contact_name'/>
                                </link-entity>
                                <link-entity name='lead' from='leadid' to='regardingobjectid' link-type='outer' alias='aj'>
	                            <attribute name='leadid' alias='lead_id'/>                  
                                    <attribute name='fullname' alias='lead_name'/>
                                </link-entity>
                              </entity>
                            </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<TaskFormModel>>("tasks", fetch);
            if (result == null || result.value == null) return;
            TaskDetail = result.value.FirstOrDefault();

            this.ScheduledStartTask = TaskDetail.scheduledstart.Value.ToLocalTime();
            this.ScheduledEndTask = TaskDetail.scheduledend.Value.ToLocalTime();

            if (TaskDetail.contact_id != Guid.Empty)
            {
                TaskDetail.Customer = new CustomerLookUp
                {
                    Name = TaskDetail.contact_name
                };
            }
            else if (TaskDetail.account_id != Guid.Empty)
            {
                TaskDetail.Customer = new CustomerLookUp
                {
                    Name = TaskDetail.account_name
                };
            }
            else if (TaskDetail.lead_id != Guid.Empty)
            {
                TaskDetail.Customer = new CustomerLookUp
                {
                    Name = TaskDetail.lead_name
                };
            }

            if (TaskDetail.statecode == 0)
                ShowGridButton = true;
            else
                ShowGridButton = false;
        }

        public async Task loadMeet(Guid id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                            <entity name='appointment'>
                                <attribute name='subject' />
                                <attribute name='statecode' />
                                <attribute name='createdby' />
                                <attribute name='statuscode' />
                                <attribute name='requiredattendees' />
                                <attribute name='prioritycode' />
                                <attribute name='scheduledstart' />
                                <attribute name='scheduledend' />
                                <attribute name='scheduleddurationminutes' />
                                <attribute name='bsd_mmeetingformuploaded' />
                                <attribute name='optionalattendees' />
                                <attribute name='isalldayevent' />
                                <attribute name='location' />
                                <attribute name='activityid' />
                                <attribute name='description' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                    <condition attribute='activityid' operator='eq' uitype='appointment' value='" + id + @"' />
                                </filter>               
                                <link-entity name='contact' from='contactid' to='regardingobjectid' visible='false' link-type='outer' alias='contacts'>
                                  <attribute name='contactid' alias='contact_id' />                  
                                  <attribute name='fullname' alias='contact_name'/>
                                </link-entity>
                                <link-entity name='account' from='accountid' to='regardingobjectid' visible='false' link-type='outer' alias='accounts'>
                                    <attribute name='accountid' alias='account_id' />                  
                                    <attribute name='bsd_name' alias='account_name'/>
                                </link-entity>
                                <link-entity name='lead' from='leadid' to='regardingobjectid' visible='false' link-type='outer' alias='leads'>
                                    <attribute name='leadid' alias='lead_id'/>                  
                                    <attribute name='fullname' alias='lead_name'/>
                                </link-entity>
                            </entity>
                          </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<MeetingModel>>("appointments", fetch);
            if (result == null || result.value == null)
                return;
            var data = result.value.FirstOrDefault();
            Meet = data;

            if (data.scheduledend != null && data.scheduledstart != null)
            {
                Meet.scheduledend = data.scheduledend.Value.ToLocalTime();
                Meet.scheduledstart = data.scheduledstart.Value.ToLocalTime();
            }

            if (Meet.contact_id != Guid.Empty)
            {
                Meet.Customer = new CustomerLookUp
                {
                    Name = Meet.contact_name
                };
            }
            else if (Meet.account_id != Guid.Empty)
            {
                Meet.Customer = new CustomerLookUp
                {
                    Name = Meet.account_name
                };
            }
            else if (Meet.lead_id != Guid.Empty)
            {
                Meet.Customer = new CustomerLookUp
                {
                    Name = Meet.lead_name
                };
            }

            if (Meet.statecode == 0)
                ShowGridButton = true;
            else
                ShowGridButton = false;
        }

        public async Task loadFromToMeet(Guid id)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='appointment'>
                                    <attribute name='subject' />
                                    <attribute name='createdon' />
                                    <attribute name='activityid' />
                                    <order attribute='createdon' descending='false' />
                                    <filter type='and'>
                                        <condition attribute='activityid' operator='eq' value='" + id + @"' />
                                    </filter>
                                    <link-entity name='activityparty' from='activityid' to='activityid' link-type='inner' alias='ab'>
                                        <attribute name='partyid' alias='partyID'/>
                                        <attribute name='participationtypemask' alias='typemask'/>
                                      <link-entity name='account' from='accountid' to='partyid' link-type='outer' alias='partyaccount'>
                                        <attribute name='bsd_name' alias='account_name'/>
                                      </link-entity>
                                      <link-entity name='contact' from='contactid' to='partyid' link-type='outer' alias='partycontact'>
                                        <attribute name='fullname' alias='contact_name'/>
                                      </link-entity>
                                      <link-entity name='lead' from='leadid' to='partyid' link-type='outer' alias='partylead'>
                                        <attribute name='fullname' alias='lead_name'/>
                                      </link-entity>
                                      <link-entity name='systemuser' from='systemuserid' to='partyid' link-type='outer' alias='partyuser'>
                                        <attribute name='fullname' alias='user_name'/>
                                      </link-entity>
                                    </link-entity>
                                </entity>
                              </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<PartyModel>>("appointments", fetch);
            if (result == null || result.value == null)
                return;
            var data = result.value;
            if (data.Any())
            {
                List<string> required = new List<string>();
                List<string> optional = new List<string>();
                foreach (var item in data)
                {
                    if (item.typemask == 5) // from call
                    {
                        if (item.contact_name != null && item.contact_name != string.Empty)
                        {
                            required.Add(item.contact_name);
                        }
                        else if (item.account_name != null && item.account_name != string.Empty)
                        {
                            required.Add(item.account_name);
                        }
                        else if (item.lead_name != null && item.lead_name != string.Empty)
                        {
                            required.Add(item.lead_name);
                        }
                    }
                    else if (item.typemask == 6) // to call
                    {
                        if (item.contact_name != null && item.contact_name != string.Empty)
                        {
                            optional.Add(item.contact_name);
                        }
                        else if (item.account_name != null && item.account_name != string.Empty)
                        {
                            optional.Add(item.account_name);
                        }
                        else if (item.lead_name != null && item.lead_name != string.Empty)
                        {
                            optional.Add(item.lead_name);
                        }
                    }
                }
                Meet.required = string.Join(", ", required);
                Meet.optional = string.Join(", ", optional);
            }
        }

        public async Task<bool> UpdateStatusPhoneCall(string update)
        {
            if (update == CodeCompleted)
            {
                PhoneCall.statecode = 1;
                PhoneCall.statuscode = 2;
            }
            else if (update == CodeCancel)
            {
                PhoneCall.statecode = 2;
                PhoneCall.statuscode = 3;
            }

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = PhoneCall.statecode;
            data["statuscode"] = PhoneCall.statuscode;

            string path = "/phonecalls(" + PhoneCall.activityid + ")";
            CrmApiResponse result = await CrmHelper.PatchData(path, data);
            if (result.IsSuccess)
            {
                ShowGridButton = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusTask(string update)
        {
            if (update == CodeCompleted)
            {
                TaskDetail.statecode = 1;
            }
            else if (update == CodeCancel)
            {
                TaskDetail.statecode = 2;
            }

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = TaskDetail.statecode;

            string path = "/tasks(" + TaskDetail.activityid + ")";
            CrmApiResponse result = await CrmHelper.PatchData(path, data);
            if (result.IsSuccess)
            {
                ShowGridButton = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusMeet(string update)
        {
            if (update == CodeCompleted)
            {
                Meet.statecode = 1;
            }
            else if (update == CodeCancel)
            {
                Meet.statecode = 2;
            }

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = Meet.statecode;

            string path = "/appointments(" + Meet.activityid + ")";
            CrmApiResponse result = await CrmHelper.PatchData(path, data);
            if (result.IsSuccess)
            {
                ShowGridButton = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class DashboardChartModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal CommissionTotal { get; set; }
        public string CommissionStatus { get; set; }
    }
    public class CommissionTransaction
    {
        public Guid bsd_commissiontransactionid { get; set; }
        public string bsd_name { get; set; }
        public DateTime createdon { get; set; }
        public decimal bsd_totalamountpaid { get; set; }
        public decimal bsd_totalamount { get; set; }
        public int statecode { get; set; }
        public int statuscode { get; set; }
        public int statuscode_calculator { get; set; }
    }
}
