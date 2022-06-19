using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskForm : ContentPage
    {
        public Action<bool> CheckTaskForm;
        public TaskFormViewModel viewModel;

        public TaskForm()
        {
            InitializeComponent();
            Init();
            InitAdd();
        }
        public TaskForm(Guid taskId)
        {
            InitializeComponent();
            Init();
            viewModel.TaskId = taskId;
            InitUpdate();
        }
        public TaskForm(DateTime dateTimeNew)
        {
            InitializeComponent();
        }
        public void Init()
        {
            this.BindingContext = viewModel = new TaskFormViewModel();
            // kiểm tra page trước là page nào
            var page_before = App.Current.MainPage.Navigation.NavigationStack.Last()?.GetType().Name;
            if (page_before == "ContactDetailPage" || page_before == "AccountDetailPage" 
                || page_before == "LeadDetailPage" || page_before == "QueuesDetialPage")
            {
                if (page_before == "ContactDetailPage" && ContactDetailPage.FromCustomer != null && !string.IsNullOrWhiteSpace(ContactDetailPage.FromCustomer.Val))
                {
                    viewModel.Customer = ContactDetailPage.FromCustomer;
                    Lookup_NguoiLienQuan.IsVisible = false;
                    ContactMapping.IsVisible = true;
                }
                else if (page_before == "AccountDetailPage" && AccountDetailPage.FromCustomer != null && !string.IsNullOrWhiteSpace(AccountDetailPage.FromCustomer.Val))
                {
                    viewModel.Customer = AccountDetailPage.FromCustomer;
                    Lookup_NguoiLienQuan.IsVisible = false;
                    ContactMapping.IsVisible = true;
                }
                else if (page_before == "LeadDetailPage" && LeadDetailPage.FromCustomer != null && !string.IsNullOrWhiteSpace(LeadDetailPage.FromCustomer.Val))
                {
                    viewModel.Customer = LeadDetailPage.FromCustomer;
                    Lookup_NguoiLienQuan.IsVisible = false;
                    ContactMapping.IsVisible = true;
                }
                else if (page_before == "QueuesDetialPage" && QueuesDetialPage.FromQueue != null && !string.IsNullOrWhiteSpace(QueuesDetialPage.FromQueue.Val))
                {
                    viewModel.Customer = QueuesDetialPage.FromQueue;
                    Lookup_NguoiLienQuan.IsVisible = false;
                    ContactMapping.IsVisible = true;
                }
                else
                {
                    Lookup_NguoiLienQuan.IsVisible = true;
                    ContactMapping.IsVisible = false;
                }
            }
            else
            {
                Lookup_NguoiLienQuan.IsVisible = true;
                ContactMapping.IsVisible = false;
            }
        }
        public void InitAdd()
        {
            viewModel.Title = Language.tao_cong_viec;
            viewModel.TaskFormModel = new TaskFormModel();
        }
        public async void InitUpdate()
        {
            await viewModel.LoadTask();
            if (viewModel.TaskFormModel != null)
            {
                btnSave.Text = viewModel.Title = Language.cap_nhat;
                CheckTaskForm?.Invoke(true);
            }
            else
            {
                CheckTaskForm?.Invoke(false);
            }
        }
        private void DateStart_Selected(object sender, EventArgs e)
        {
            if (viewModel.TaskFormModel.scheduledstart.HasValue && viewModel.TaskFormModel.scheduledend.HasValue)
            {
                if (viewModel.TaskFormModel.scheduledstart > viewModel.TaskFormModel.scheduledend || viewModel.TaskFormModel.scheduledstart == viewModel.TaskFormModel.scheduledend)
                {
                    ToastMessageHelper.ShortMessage(Language.thoi_gian_bat_dau_phai_nho_hon_thoi_gian_ket_thuc);
                }
            }
        }
        private void DateEnd_Selected(object sender, EventArgs e)
        {
            if (viewModel.TaskFormModel.scheduledstart.HasValue && viewModel.TaskFormModel.scheduledend.HasValue)
            {
                if (viewModel.TaskFormModel.scheduledstart > viewModel.TaskFormModel.scheduledend || viewModel.TaskFormModel.scheduledstart == viewModel.TaskFormModel.scheduledend)
                {
                    ToastMessageHelper.ShortMessage(Language.thoi_gian_ket_thuc_phai_lon_hon_thoi_gian_bat_dau);
                }
            }
        }
        private void EventAllDay_Tapped(object sender, EventArgs e)
        {
            viewModel.IsEventAllDay = !viewModel.IsEventAllDay;
        }
        private void CheckedBoxEventAllDay_Change(object sender, EventArgs e)
        {
            if (!viewModel.TaskFormModel.scheduledstart.HasValue)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_bat_dau);
                viewModel.IsEventAllDay = false;
                return;
            }
            if (viewModel.IsEventAllDay == true)
            {
                viewModel.TaskFormModel.scheduledstart = new DateTime(viewModel.TaskFormModel.scheduledstart.Value.Year, viewModel.TaskFormModel.scheduledstart.Value.Month, viewModel.TaskFormModel.scheduledstart.Value.Day, 7, 0, 0); ;
                viewModel.TaskFormModel.scheduledend = viewModel.TaskFormModel.scheduledstart.Value.AddDays(1);
            }
        }
        private async void SaveTask_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.TaskFormModel.subject))
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_nhap_chu_de);
                return;
            }

            if (!viewModel.TaskFormModel.scheduledstart.HasValue || dateStart.IsTimeNull)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_bat_dau);
                return;
            }

            if (!viewModel.TaskFormModel.scheduledend.HasValue || dateEnd.IsTimeNull)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_ket_thuc);
                return;
            }

            if ((viewModel.TaskFormModel.scheduledstart.HasValue && viewModel.TaskFormModel.scheduledend.HasValue) && (viewModel.TaskFormModel.scheduledstart > viewModel.TaskFormModel.scheduledend))
            {
                ToastMessageHelper.ShortMessage(Language.thoi_gian_ket_thuc_phai_lon_hon_thoi_gian_bat_dau);
                return;
            }

            LoadingHelper.Show();
            if (viewModel.TaskFormModel.activityid == Guid.Empty)
            {
                bool isSuccess = await viewModel.CreateTask();
                if (isSuccess)
                {
                    if (Dashboard.NeedToRefreshTask.HasValue) Dashboard.NeedToRefreshTask = true;
                    if (ActivityList.NeedToRefreshTask.HasValue) ActivityList.NeedToRefreshTask = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
                    if (LeadDetailPage.NeedToRefreshActivity.HasValue) LeadDetailPage.NeedToRefreshActivity = true;
                    if (QueuesDetialPage.NeedToRefreshActivity.HasValue) QueuesDetialPage.NeedToRefreshActivity = true;
                    ToastMessageHelper.ShortMessage(Language.tao_cong_viec_thanh_cong);
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
                }
            }
            else
            {
                bool isSuccess = await viewModel.UpdateTask();
                if (isSuccess)
                {
                    if (Dashboard.NeedToRefreshTask.HasValue) Dashboard.NeedToRefreshTask = true;
                    if (ActivityList.NeedToRefreshTask.HasValue) ActivityList.NeedToRefreshTask = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
                    if (LeadDetailPage.NeedToRefreshActivity.HasValue) LeadDetailPage.NeedToRefreshActivity = true;
                    if (QueuesDetialPage.NeedToRefreshActivity.HasValue) QueuesDetialPage.NeedToRefreshActivity = true;
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_thanh_cong);
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_that_bai);
                }
            }
        }
    }
}