using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
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
            Lookup_NguoiLienQuan.IsVisible = true;
            ContactMapping.IsVisible = false;
        }

        public TaskForm(Guid taskId)
        {
            InitializeComponent();
            Init();
            viewModel.TaskId = taskId;
            InitUpdate();
            Lookup_NguoiLienQuan.IsVisible = true;
            ContactMapping.IsVisible = false;
        }

        public TaskForm(DateTime dateTimeNew)
        {
            InitializeComponent();

        }

        public TaskForm(Guid idCustomer, string nameCustomer, string codeCustomer)
        {
            InitializeComponent();
            Init();
            InitAdd();
            viewModel.Customer = new OptionSet { Val= idCustomer.ToString(), Label = nameCustomer, Title = codeCustomer};
            Lookup_NguoiLienQuan.IsVisible = false;
            ContactMapping.IsVisible = true;
        }

        public void Init()
        {
            this.BindingContext = viewModel = new TaskFormViewModel();
        }

        public void InitAdd()
        {
            viewModel.Title = "Tạo Công Việc";
            viewModel.TaskFormModel = new TaskFormModel();
            dateTimeTGBatDau.DefaultDisplay = DateTime.Now;
            dateTimeTGKetThuc.DefaultDisplay = DateTime.Now;
        }

        public async void InitUpdate()
        {
            await viewModel.LoadTask();
            if (viewModel.TaskFormModel != null)
            {
                viewModel.Title = "Cập Nhật Công Việc";
                btnSave.Text = "Cập nhật công việc";
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
                    ToastMessageHelper.ShortMessage("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                }
            }
        }

        private void DateEnd_Selected(object sender, EventArgs e)
        {
            if (viewModel.TaskFormModel.scheduledstart.HasValue && viewModel.TaskFormModel.scheduledend.HasValue)
            {
                if (viewModel.TaskFormModel.scheduledstart > viewModel.TaskFormModel.scheduledend || viewModel.TaskFormModel.scheduledstart == viewModel.TaskFormModel.scheduledend)
                {
                    ToastMessageHelper.ShortMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");
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
                ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian bắt đầu");
                viewModel.IsEventAllDay = false;
                return;
            }
            if (viewModel.IsEventAllDay == true)
            {
                viewModel.TaskFormModel.scheduledstart = new DateTime(viewModel.TaskFormModel.scheduledstart.Value.Year, viewModel.TaskFormModel.scheduledstart.Value.Month, viewModel.TaskFormModel.scheduledstart.Value.Day, 8, 0, 0); ;
                viewModel.TaskFormModel.scheduledend = viewModel.TaskFormModel.scheduledstart.Value.AddDays(1);
            }
        }

        private async void SaveTask_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.TaskFormModel.subject))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập chủ đề");
                return;
            }

            if (!viewModel.TaskFormModel.scheduledstart.HasValue)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian bắt đầu");
                return;
            }

            if (!viewModel.TaskFormModel.scheduledend.HasValue)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian kết thúc");
                return;
            }

            if ((viewModel.TaskFormModel.scheduledstart.HasValue && viewModel.TaskFormModel.scheduledend.HasValue) && (viewModel.TaskFormModel.scheduledstart > viewModel.TaskFormModel.scheduledend))
            {
                ToastMessageHelper.ShortMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu");
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
                    ToastMessageHelper.ShortMessage("Tạo công việc thành công");
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Tạo công việc thất bại");
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
                    ToastMessageHelper.ShortMessage("Cập nhật công việc thành công");
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Cập nhật công việc thất bại");
                }
            }
        }
    }
}