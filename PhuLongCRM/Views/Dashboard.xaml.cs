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
            if (item.activitytypecode == "phonecall")
            {
                await viewModel.loadPhoneCall(item.activityid);
                await viewModel.loadFromTo(item.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.PhoneCall.statecode.ToString());
                viewModel.ActivityType = "Phone Call";
                if (viewModel.PhoneCall.activityid != Guid.Empty)
                {
                    ContentActivity.IsVisible = true;
                    ContentPhoneCall.IsVisible = true;
                    ContentTask.IsVisible = false;
                    ContentMeet.IsVisible = false;

                    viewModel.TaskDetail.activityid = Guid.Empty;
                    viewModel.Meet.activityid = Guid.Empty;
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            }
            else if (item.activitytypecode == "task")
            {
                await viewModel.loadTask(item.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.TaskDetail.statecode.ToString());
                viewModel.ActivityType = "Task";
                if (viewModel.TaskDetail.activityid != Guid.Empty)
                {
                    ContentActivity.IsVisible = true;
                    ContentPhoneCall.IsVisible = false;
                    ContentTask.IsVisible = true;
                    ContentMeet.IsVisible = false;

                    viewModel.PhoneCall.activityid = Guid.Empty;
                    viewModel.Meet.activityid = Guid.Empty;
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            }
            else if (item.activitytypecode == "appointment")
            {
                await viewModel.loadMeet(item.activityid);
                await viewModel.loadFromToMeet(item.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Meet.statecode.ToString());
                viewModel.ActivityType = "Collection Meeting";
                if (viewModel.Meet.activityid != Guid.Empty)
                {
                    ContentActivity.IsVisible = true;
                    ContentPhoneCall.IsVisible = false;
                    ContentTask.IsVisible = false;
                    ContentMeet.IsVisible = true;

                    viewModel.TaskDetail.activityid = Guid.Empty;
                    viewModel.PhoneCall.activityid = Guid.Empty;
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            }
            LoadingHelper.Hide();
        }

        private async void Update_Clicked(object sender, EventArgs e)
        {
            if (viewModel.PhoneCall.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                PhoneCallForm newPage = new PhoneCallForm(viewModel.PhoneCall.activityid);
                newPage.OnCompleted = async (OnCompleted) =>
                {
                    if (OnCompleted == true)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại");
                    }
                };
            }
            else if (viewModel.TaskDetail.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                TaskForm newPage = new TaskForm(viewModel.TaskDetail.activityid);
                newPage.CheckTaskForm = async (OnCompleted) =>
                {
                    if (OnCompleted == true)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại");
                    }
                };
            }
            else if (viewModel.Meet.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                MeetingForm newPage = new MeetingForm(viewModel.Meet.activityid);
                newPage.OnCompleted = async (OnCompleted) =>
                {
                    if (OnCompleted == true)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại");
                    }
                };
            }
        }

        private async void Completed_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { "Hoàn Thành", "Hủy" };
            string asw = await DisplayActionSheet("Tuỳ chọn", "Đóng", null, options);
            if (asw == "Hoàn Thành")
            {
                if (viewModel.PhoneCall.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusPhoneCall(viewModel.CodeCompleted))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.PhoneCall.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Cuộc gọi đã hoàn thành");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hoàn thành cuộc gọi. Vui lòng thử lại");
                    }
                }
                else if (viewModel.TaskDetail.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusTask(viewModel.CodeCompleted))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.TaskDetail.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Công việc đã hoàn thành");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hoàn thành công việc. Vui lòng thử lại");
                    }
                }
                else if (viewModel.Meet.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusMeet(viewModel.CodeCompleted))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Meet.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Cuộc họp đã hoàn thành");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hoàn thành cuộc họp. Vui lòng thử lại");
                    }
                }
            }
            else if (asw == "Hủy")
            {
                if (viewModel.PhoneCall.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusPhoneCall(viewModel.CodeCancel))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.PhoneCall.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Cuộc gọi đã được hủy");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hủy cuộc gọi. Vui lòng thử lại");
                    }
                }
                else if (viewModel.TaskDetail.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusTask(viewModel.CodeCancel))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.TaskDetail.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Công việc đã được hủy");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hủy công việc. Vui lòng thử lại");
                    }
                }
                else if (viewModel.Meet.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusMeet(viewModel.CodeCancel))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Meet.statecode.ToString());
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Cuộc họp đã được hủy");
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Lỗi khi hủy cuộc họp. Vui lòng thử lại");
                    }
                }
            }
            LoadingHelper.Hide();
        }

        private void CloseContentActivity_Tapped(object sender, EventArgs e)
        {
            ContentActivity.IsVisible = false;
        }
    }
}