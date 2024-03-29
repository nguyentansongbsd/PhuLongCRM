﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Firebase.Database;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        FirebaseClient firebase = new Firebase.Database.FirebaseClient("https://phulonguat-default-rtdb.firebaseio.com/");
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
        private List<DateTime> four_Month { get; set; }
        // tổng tiền hoa đồng format
        private string _totalCommission;
        public string TotalCommission { get => _totalCommission; set { _totalCommission = value; OnPropertyChanged(nameof(TotalCommission)); } }
        // tổng tiền thanh toán format
        private string _totalPaidCommission;
        public string TotalPaidCommission { get => _totalPaidCommission; set { _totalPaidCommission = value; OnPropertyChanged(nameof(TotalPaidCommission)); } }

        private List<NewsModel> _promotions;
        public List<NewsModel> Promotions { get => _promotions; set { _promotions = value; OnPropertyChanged(nameof(Promotions)); } }

        private List<NewsModel> _news;
        public List<NewsModel> News { get => _news; set { _news = value; OnPropertyChanged(nameof(News)); } }

        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            await RefreshDashboard();
            IsRefreshing = false;
        });

        private int _numTask;
        public int NumTask { get => _numTask; set { _numTask = value; OnPropertyChanged(nameof(NumTask)); } }

        private int _numPhoneCall;
        public int NumPhoneCall { get => _numPhoneCall; set { _numPhoneCall = value; OnPropertyChanged(nameof(NumPhoneCall)); } }

        private int _numMeet;
        public int NumMeet { get => _numMeet; set { _numMeet = value; OnPropertyChanged(nameof(NumMeet)); } }

        private int _numNotification;
        public int NumNotification { get => _numNotification; set { _numNotification = value; OnPropertyChanged(nameof(NumNotification)); } }

        private PromotionModel _promotionItem;
        public PromotionModel PromotionItem { get => _promotionItem; set { _promotionItem = value; OnPropertyChanged(nameof(PromotionItem)); } }

        public DashboardViewModel()
        {
            dateBefor = DateTime.Now;
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
            dateAfter = new DateTime(threeMonthsAgo.Year, threeMonthsAgo.Month, 1);
            first_Month = dateAfter;
            second_Month = dateAfter.AddMonths(1);
            third_Month = second_Month.AddMonths(1);
            fourth_Month = dateBefor;
            four_Month = new List<DateTime>();
            four_Month.Add(first_Month);
            four_Month.Add(second_Month);
            four_Month.Add(third_Month);
            four_Month.Add(fourth_Month);
        }

        public async Task LoadCommissionTransactions()
        {
            string attribute = UserLogged.IsLoginByUserCRM ? "bsd_salestaff" : "bsd_employee";

            //Phu long khong co file nay <attribute name='bsd_totalcommission' alias='CommissionTotal'/> // 100000003 :Accountant Confirmed
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                    <entity name='bsd_commissiontransaction'>
                                        <attribute name='createdon' groupby='true' alias='group' dategrouping='month'/>
                                        <attribute name='bsd_totalamountpaid' alias='totalamountpaid_sum' aggregate='sum'/>
                                        <attribute name='bsd_totalamount' alias='totalamount_sum' aggregate='sum'/>
                                        <attribute name='statuscode' groupby='true' alias='group_sts'/>
                                        <filter type='and'>
                                            <condition attribute='createdon' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                            <condition attribute='{attribute}' operator='eq' value='{UserLogged.Id}' />
                                        </filter>
                                        <link-entity name='bsd_commissioncalculation' from='bsd_commissioncalculationid' to='bsd_commissioncalculation' link-type='inner'>
                                            <filter type='and'>
                                                <condition attribute='statuscode' operator='eq' value='100000003' />
                                            </filter>
                                        </link-entity>
                                    </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartCommissionModel>>("bsd_commissiontransactions", fetchXml);
            if (result == null) return;

            List<CountChartCommissionModel> value1 = result.value.Where(x => x.group_sts != "100000009").ToList();
            List<CountChartCommissionModel> value2 = result.value.Where(x => x.group_sts == "100000009").ToList();

            List<CountChartCommissionModel> four_month = new List<CountChartCommissionModel>();
            four_month.Add(new CountChartCommissionModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = fourth_Month.Month.ToString() });

            four_month.AddRange(value1);
            var data = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, totalamount_sum: x.Sum(p => p.totalamount_sum))
               ).ToList();

            CommissionTransactionChart.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(data[0].totalamount_sum) });
            CommissionTransactionChart.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(data[1].totalamount_sum) });
            CommissionTransactionChart.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(data[2].totalamount_sum) });
            CommissionTransactionChart.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(data[3].totalamount_sum) });
            TotalCommissionAMonth = data[3].totalamount_sum;

            four_month.Clear();
            four_month.Add(new CountChartCommissionModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartCommissionModel { group = fourth_Month.Month.ToString() });

            four_month.AddRange(value2);
            var data2 = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, totalamountpaid_sum: x.Sum(p => p.totalamountpaid_sum))
               ).ToList();

            CommissionTransactionChart2.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(data2[0].totalamountpaid_sum) });
            CommissionTransactionChart2.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(data2[1].totalamountpaid_sum) });
            CommissionTransactionChart2.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(data2[2].totalamountpaid_sum) });
            CommissionTransactionChart2.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(data2[3].totalamountpaid_sum) });
            TotalPaidCommissionAMonth = data2[3].totalamountpaid_sum;

            //foreach (var item in result.value)
            //{
            //    // danh sách các hhgd có sts = Accountant Confirmed lấy từ entity Commission Calculator trong 4 tháng từ ngày hiện tại trở về trước
            //    if (item.statuscode_calculator == 100000003 && item.createdon.Month == first_Month.Month)
            //    {
            //        countTotalCommissionFr += item.bsd_totalamount;
            //        if (item.statuscode == 100000009)
            //            countTotalPaidFr += item.bsd_totalamountpaid;
            //    }
            //    if (item.statuscode_calculator == 100000003 && item.createdon.Month == second_Month.Month)
            //    {
            //        countTotalCommissionSe += item.bsd_totalamount;
            //        if (item.statuscode == 100000009)
            //            countTotalPaidSe += item.bsd_totalamountpaid;
            //    }
            //    if (item.statuscode_calculator == 100000003 && item.createdon.Month == third_Month.Month)
            //    {
            //        countTotalCommissionTh += item.bsd_totalamount;
            //        if (item.statuscode == 100000009)
            //            countTotalPaidTh += item.bsd_totalamountpaid;
            //    }
            //    if (item.statuscode_calculator == 100000003 && item.createdon.Month == fourth_Month.Month)
            //    {
            //        // tổng hhgd và hhgd paid được tính ở tháng hiện tại (sử dụng cho 2 giá trị thống kê)
            //        countTotalCommissionFo += item.bsd_totalamount;
            //        TotalCommissionAMonth += item.bsd_totalamount;
            //        if (item.statuscode == 100000009)
            //        {
            //            countTotalPaidFo += item.bsd_totalamountpaid;
            //            TotalPaidCommissionAMonth += item.bsd_totalamountpaid;
            //        }
            //    }
            //}

            //ChartModel chartFirstMonth = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionFr) };
            //ChartModel chartSecondMonth = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionSe) };
            //ChartModel chartThirdMonth = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionTh) };
            //ChartModel chartFourthMonth = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalCommissionFo) };

            //ChartModel chartFirstMonth2 = new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidFr) };
            //ChartModel chartSecondMonth2 = new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidSe) };
            //ChartModel chartThirdMonth2 = new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidTh) };
            //ChartModel chartFourthMonth2 = new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = TotalAMonth(countTotalPaidFo) };

            //this.CommissionTransactionChart.Add(chartFirstMonth);
            //this.CommissionTransactionChart.Add(chartSecondMonth);
            //this.CommissionTransactionChart.Add(chartThirdMonth);
            //this.CommissionTransactionChart.Add(chartFourthMonth);

            //this.CommissionTransactionChart2.Add(chartFirstMonth2);
            //this.CommissionTransactionChart2.Add(chartSecondMonth2);
            //this.CommissionTransactionChart2.Add(chartThirdMonth2);
            //this.CommissionTransactionChart2.Add(chartFourthMonth2);

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
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                    <entity name='opportunity'>
                                        <attribute name='bsd_bookingtime' groupby='true' alias='group' dategrouping='month'/>
                                        <attribute name='opportunityid' aggregate='count' alias='count' />
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("opportunities", fetchXml);
            if (result == null) return;

            List<CountChartModel> four_month = new List<CountChartModel>();
            four_month.Add(new CountChartModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = fourth_Month.Month.ToString() });
            four_month.AddRange(result.value);

            var data = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, count: x.Sum(p => p.count))
               ).ToList();

            DataMonthQueue.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = data[0].count });
            DataMonthQueue.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = data[1].count });
            DataMonthQueue.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = data[2].count });
            DataMonthQueue.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = data[3].count });
            numQueue = data[3].count;
        }
        public async Task LoadQuoteFourMonths()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='quote'>
                                    <attribute name='bsd_deposittime' groupby='true' alias='group' dategrouping='month' />
                                    <attribute name='quoteid' aggregate='count' alias='count'/>
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("quotes", fetchXml);
            if (result == null) return;

            List<CountChartModel> four_month = new List<CountChartModel>();
            four_month.Add(new CountChartModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = fourth_Month.Month.ToString() });
            four_month.AddRange(result.value);

            var data = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, count: x.Sum(p => p.count))
               ).ToList();

            DataMonthQuote.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = data[0].count });
            DataMonthQuote.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = data[1].count });
            DataMonthQuote.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = data[2].count });
            DataMonthQuote.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = data[3].count });
            numQuote = data[3].count;
        }
        public async Task LoadOptionEntryFourMonths()
        {
            // ngoại trừ các sts Terminated , 1st Installment, Option, Qualify, Signed D.A
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='salesorder'>
                                    <attribute name='salesorderid' aggregate='count' alias='count'/>
                                    <attribute name='createdon' groupby='true' alias='group' dategrouping='month'/>
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
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("salesorders", fetchXml);
            if (result == null) return;

            List<CountChartModel> four_month = new List<CountChartModel>();
            four_month.Add(new CountChartModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = fourth_Month.Month.ToString() });
            four_month.AddRange(result.value);

            var data = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, count: x.Sum(p => p.count))
               ).ToList();

            DataMonthOptionEntry.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = data[0].count });
            DataMonthOptionEntry.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = data[1].count });
            DataMonthOptionEntry.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = data[2].count });
            DataMonthOptionEntry.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = data[3].count });
            numOptionEntry = data[3].count;
        }
        public async Task LoadUnitFourMonths()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='salesorder'>
                                    <attribute name='salesorderid' aggregate='count' alias='count'/>
                                    <attribute name='createdon' groupby='true' alias='group' dategrouping='month'/>
                                    <filter type='and'>
                                      <condition attribute='bsd_unitstatus' operator='eq' value='100000002' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                      <condition attribute='bsd_signedcontractdate' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("salesorders", fetchXml);
            if (result == null) return;

            List<CountChartModel> four_month = new List<CountChartModel>();
            four_month.Add(new CountChartModel { group = first_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = second_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = third_Month.Month.ToString() });
            four_month.Add(new CountChartModel { group = fourth_Month.Month.ToString() });
            four_month.AddRange(result.value);

            var data = four_month.GroupBy(x => x.group)
               .Select(x => (group: x.Key, count: x.Sum(p => p.count))
               ).ToList();

            DataMonthUnit.Add(new ChartModel() { Category = first_Month.ToString("MM/yyyy"), Value = data[0].count });
            DataMonthUnit.Add(new ChartModel() { Category = second_Month.ToString("MM/yyyy"), Value = data[1].count });
            DataMonthUnit.Add(new ChartModel() { Category = third_Month.ToString("MM/yyyy"), Value = data[2].count });
            DataMonthUnit.Add(new ChartModel() { Category = fourth_Month.ToString("MM/yyyy"), Value = data[3].count });
            numUnit = data[3].count;
        }
        public async Task LoadLeads()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='lead'>
                                    <attribute name='leadid' aggregate='count' alias='count'/>
	                                <attribute name='statuscode' groupby='true' alias='group' />
                                    <filter type='and'>
                                      <condition attribute='createdon' operator='on-or-after' value='{dateAfter.ToString("yyyy-MM-dd")}' />
                                      <condition attribute='statuscode' operator='ne' value='2'/>
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("leads", fetchXml);
            if (result != null && result.value.Count > 0)
            {
                foreach (var item in result.value)
                {
                    if (item.group == "1")
                        numKHMoi += item.count;
                    else if (item.group == "3")
                        numKHDaChuyenDoi += item.count;
                    else
                        numKHKhongChuyenDoi += item.count;
                }
            }

            LeadsChart.Add(new ChartModel() { Category = Language.khach_hang_moi, Value = (numKHMoi == 0 && numKHDaChuyenDoi == 0 && numKHKhongChuyenDoi == 0) ? 1 : numKHMoi });
            LeadsChart.Add(new ChartModel() { Category = Language.da_chuyen_doi, Value = (numKHMoi == 0 && numKHDaChuyenDoi == 0 && numKHKhongChuyenDoi == 0) ? 1 : numKHDaChuyenDoi });
            LeadsChart.Add(new ChartModel() { Category = Language.khong_chuyen_doi, Value = (numKHMoi == 0 && numKHDaChuyenDoi == 0 && numKHKhongChuyenDoi == 0) ? 1 : numKHKhongChuyenDoi });

        }

        public async Task LoadTasks(int activityCount)
        {
            string fetchXml = $@"<fetch version='1.0' count='{activityCount}' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
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
            string fetchXml = $@"<fetch version='1.0' count='3' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
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

        public async Task LoadPhoneCalls(int activityCount)
        {
            string fetchXml = $@"<fetch version='1.0' count='{activityCount}' page='1' output-format='xml-platform' mapping='logical' distinct='false'>
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

        public async Task LoadActivityCount()
        {
            string fetchXml_phone = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='phonecall'>
                                    <attribute name='activityid' aggregate='count' alias='count'/>
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                  </entity>
                                </fetch>";
            var result_phone = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("phonecalls", fetchXml_phone);
            if (result_phone != null || result_phone.value.Count != 0)
                NumPhoneCall = result_phone.value.FirstOrDefault().count;

            string fetchXml_task = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='task'>
                                    <attribute name='activityid' aggregate='count' alias='count'/>
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='eq' value='0' />
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                  </entity>
                                </fetch>";
            var result_task = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("tasks", fetchXml_task);
            if (result_task != null || result_task.value.Count != 0)
                NumTask = result_task.value.FirstOrDefault().count;

            string fetchXml_meet = $@"<fetch version='1.0' count='5' page='1' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                  <entity name='appointment'>
                                    <attribute name='activityid' aggregate='count' alias='count'/>
                                    <filter type='and'>
                                      <condition attribute='statecode' operator='in'>
                                            <value>0</value>
                                            <value>3</value>
                                        </condition>
                                      <condition attribute='scheduledstart' operator='today' />
                                      <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                                    </filter>
                                  </entity>
                                </fetch>";
            var result_meet = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<CountChartModel>>("appointments", fetchXml_meet);
            if (result_meet != null || result_meet.value.Count != 0)
                NumMeet = result_meet.value.FirstOrDefault().count;
        }
        public async Task Load3Activity()
        {
            await LoadMettings();
            if (Activities.Count < 3)
            {
                await LoadPhoneCalls(3 - Activities.Count);
                if (Activities.Count < 3)
                {
                    await LoadTasks(3 - Activities.Count);
                }
            }

        }
        public async Task LoadNews()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_news'>
                                    <attribute name='bsd_newsid' />
                                    <attribute name='bsd_name' />
                                    <attribute name='createdon' />
                                    <attribute name='bsd_url' />
                                    <attribute name='bsd_priority' />
                                    <attribute name='bsd_type' />
                                    <attribute name='bsd_thumnail' />
                                    <attribute name='bsd_image' />
                                    <order attribute='bsd_name' descending='false' />
                                    <link-entity name='bsd_promotion' from='bsd_promotionid' to='bsd_promotion' link-type='outer' alias='ac'>
                                        <attribute name='bsd_name' alias='promotion_name'/>
                                        <attribute name='bsd_startdate' alias='promotion_startdate'/>
                                        <attribute name='bsd_enddate' alias='promotion_enddate'/>
                                        <attribute name='bsd_promotionid' alias='promotion_id'/>
                                        <attribute name='bsd_values' alias='promotion_values'/>                                 
                                        <attribute name='bsd_description' alias='promotion_description'/>
                                            <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='outer'>
                                                <attribute name='bsd_name' alias='promotion_project_name'/>
                                                <attribute name='bsd_projectid' alias='promotion_project_id'/>
                                            </link-entity>
                                            <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaselaunch' link-type='outer' alias='aa'>
                                                <attribute name='bsd_name' alias='promotion_phaseslaunch_name'/>
                                            </link-entity>
                                    </link-entity>
                                    <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' link-type='outer' alias='ab'>
                                        <attribute name='bsd_projectid' alias='project_id'/>
                                        <attribute name='bsd_name' alias='project_name'/>
                                    </link-entity>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<NewsModel>>("bsd_newses", fetchXml);
            if (result != null || result.value.Count > 0)
            {
                List<NewsModel> news = new List<NewsModel>();
                List<NewsModel> promotions = new List<NewsModel>();
                foreach (var item in result.value)
                {
                    if (item.bsd_type == "0")
                    {
                        news.Add(item);
                        await GetImages(item);
                    }
                    else if (item.bsd_type == "1")
                    {
                        promotions.Add(item);
                    }
                }
                news = news.OrderBy(x => x.bsd_priority).ToList();
                promotions = promotions.OrderBy(x => x.bsd_priority).ToList();

                News = news;
                Promotions = promotions;
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
            this.numKHMoi = 0;
            this.numKHDaChuyenDoi = 0;
            this.numKHKhongChuyenDoi = 0;
            this.NumNotification = 0;

            await Task.WhenAll(
                 this.Load3Activity(),
                 this.LoadQueueFourMonths(),
                 this.LoadQuoteFourMonths(),
                 this.LoadOptionEntryFourMonths(),
                 this.LoadUnitFourMonths(),
                 this.LoadLeads(),
                 this.LoadCommissionTransactions(),
                 LoadActivityCount(),
                 LoadNews()
                 //CountNumNotification()
                ); ;
        }
        public async Task CountNumNotification()
        {
            if (UserLogged.Notification == true)
            {
                var collection = firebase
                .Child("Notifications")
                .AsObservable<NotificaModel>()
                .Subscribe(async (dbevent) =>
                {
                    if (dbevent.EventType != Firebase.Database.Streaming.FirebaseEventType.Delete)
                    {
                        if (dbevent.Object.IsRead == false)
                        {
                            NumNotification++;
                        }
                    }
                });
            }
        }
        public async Task TimeOutLogin()
        {
            if (UserLogged.TimeOut > 0)
            {
                Thread t = new Thread(async () =>
            {
                int time = UserLogged.TimeOut * 60000;
                await Task.Delay(time);
                UserLogged.IsTimeOut = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.Navigation.PopToRootAsync();
                    await Shell.Current.GoToAsync("//LoginPage");
                });
            });
                t.Start();
            }
        }
        private async Task GetImages(NewsModel data)
        {
            var result = await LoadFiles<RetrieveMultipleApiResponse<GraphThumbnailsUrlModel>>($"{Config.OrgConfig.SP_ProjectID}/items/{data.bsd_image}/driveItem/thumbnails");
            if (result != null)
            {
                data.image = result.value.SingleOrDefault().large.url;
            }
        }
        private async static Task<T> LoadFiles<T>(string url) where T : class
        {
            var result = await CrmHelper.RetrieveImagesSharePoint<T>(url);
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
    public class CountChartModel
    {
        public string group { get; set; }
        public int count { get; set; }
    }
    public class CountChartCommissionModel : CountChartModel
    {
        public decimal totalamountpaid_sum { get; set; }
        public decimal totalamount_sum { get; set; }
        public string group_sts { get; set; }
    }
    public class DashboardChartModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal CommissionTotal { get; set; }
        public string CommissionStatus { get; set; }
        public string opportunity_count { get; set; }
        public string month { get; set; }
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
