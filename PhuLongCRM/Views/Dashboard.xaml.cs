using System;
using System.Threading.Tasks;
using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        public static bool? NeedToRefreshPhoneCall = null;
        public static bool? NeedToRefreshMeet = null;
        public static bool? NeedToRefreshTask = null;
        public static bool? NeedToRefreshQueue = null;
        public static bool? NeedToRefreshLeads = null;
        public DashboardViewModel viewModel;

        public Dashboard()
        {
            InitializeComponent();
            LoadingHelper.Show();
            NeedToRefreshPhoneCall = false;
            NeedToRefreshMeet = false;
            NeedToRefreshTask = false;
            NeedToRefreshQueue = false;
            NeedToRefreshLeads = false;
            Init();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new DashboardViewModel();
           
            await Task.WhenAll(
                 viewModel.LoadTasks(),
                 viewModel.LoadMettings(),
                 viewModel.LoadPhoneCalls(),
                 viewModel.LoadQueueFourMonths(),
                 viewModel.LoadQuoteFourMonths(),
                 viewModel.LoadOptionEntryFourMonths(),
                 viewModel.LoadUnitFourMonths(),
                 viewModel.LoadLeads(),
                 viewModel.LoadCommissionTransactions()
                ) ;
            LoadingHelper.Hide();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefreshQueue == true)
            {
                LoadingHelper.Show();
                viewModel.DataMonthQueue.Clear();
                await viewModel.LoadQueueFourMonths();
                NeedToRefreshQueue = false;
                LoadingHelper.Hide();
            }

            if (NeedToRefreshLeads == true)
            {
                LoadingHelper.Show();
                viewModel.LeadsChart.Clear();
                await viewModel.LoadLeads();
                NeedToRefreshLeads = false;
                LoadingHelper.Hide();
            }

            if (NeedToRefreshMeet == true && viewModel.Meet.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                await Task.WhenAll(
                    viewModel.loadFromToMeet(viewModel.Meet.activityid),
                    viewModel.loadMeet(viewModel.Meet.activityid)
                    );
                LoadingHelper.Hide();
            }

            if (NeedToRefreshPhoneCall == true && viewModel.PhoneCall.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                await Task.WhenAll(
                    viewModel.loadPhoneCall(viewModel.PhoneCall.activityid),
                    viewModel.loadFromTo(viewModel.PhoneCall.activityid)
                    );
                LoadingHelper.Hide();
            }

            if (NeedToRefreshTask == true && viewModel.TaskDetail.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                await viewModel.loadTask(viewModel.TaskDetail.activityid);
                LoadingHelper.Hide();
            }

            if (NeedToRefreshTask == true || NeedToRefreshPhoneCall == true || NeedToRefreshMeet == true)
            {
                LoadingHelper.Show();
                viewModel.Activities.Clear();
                await Task.WhenAll(
                    viewModel.LoadMettings(),
                    viewModel.LoadTasks(),
                    viewModel.LoadPhoneCalls()
                    );

                NeedToRefreshPhoneCall = false;
                NeedToRefreshTask = false;
                NeedToRefreshMeet = false;
                LoadingHelper.Hide();
            }
        }

        private async void ShowMore_Tapped(object sender,EventArgs e)
        {
            LoadingHelper.Show();
            await Shell.Current.GoToAsync("//HoatDong");
            LoadingHelper.Hide();
        }

        private async void ActivitiItem_Tapped(object sender,EventArgs e)
        {
            LoadingHelper.Show();
            var item = (ActivitiModel)((sender as ExtendedFrame).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item != null && item.activityid != Guid.Empty)
            {
                ActivityPopup.ShowActivityPopup(item.activityid, item.activitytypecode);
            }
            LoadingHelper.Hide();
        }
    }
}