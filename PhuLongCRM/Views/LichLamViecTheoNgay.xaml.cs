using System;
using System.Collections.Generic;
using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class LichLamViecTheoNgay : ContentPage
    {
        LichLamViecViewModel viewModel;
        public Action<bool> OnComplete;
        public static bool? NeedToRefresh = null;
        public LichLamViecTheoNgay()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LichLamViecViewModel();
            NeedToRefresh = false;
            this.loadData();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();
                await viewModel.loadAllActivities();
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
        }

        public async void loadData()
        {
            viewModel.EntityName = "tasks";
            viewModel.entity = "task";
            VisualStateManager.GoToState(radBorderTask, "Active");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "Active");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            viewModel.selectedDate = DateTime.Today;
            await viewModel.loadAllActivities();
            if (viewModel.lstEvents != null && viewModel.lstEvents.Count > 0)
                OnComplete?.Invoke(true);
            else
                OnComplete?.Invoke(false);
        }

        private async void Task_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "Active");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "Active");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            if (viewModel.entity != "task")
            {
                viewModel.EntityName = "tasks";
                viewModel.entity = "task";
                viewModel.lstEvents.Clear();
                await viewModel.loadAllActivities();
            }

            LoadingHelper.Hide();
        }

        private async void Meeting_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "InActive");
            VisualStateManager.GoToState(radBorderMeeting, "Active");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "InActive");
            VisualStateManager.GoToState(lblMeeting, "Active");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            if (viewModel.entity != "appointment")
            {
                viewModel.EntityName = "appointments";
                viewModel.entity = "appointment";
                viewModel.lstEvents.Clear();
                await viewModel.loadAllActivities();
            }

            LoadingHelper.Hide();
        }

        private async void PhoneCall_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "InActive");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "Active");
            VisualStateManager.GoToState(lblTask, "InActive");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "Active");

            if (viewModel.entity != "phonecall")
            {
                viewModel.EntityName = "phonecalls";
                viewModel.entity = "phonecall";
                viewModel.lstEvents.Clear();
                await viewModel.loadAllActivities();
            }

            LoadingHelper.Hide();
        }

        private async void Handle_AppointmentTapped(object sender, Telerik.XamarinForms.Input.AppointmentTappedEventArgs e)
        {
            LoadingHelper.Show();
            var val = e.Appointment as CalendarEvent;
            
            if (val.Activity.activitytypecode == "task")
            {
                await viewModel.loadTask(val.Activity.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Task.statecode.ToString());
                viewModel.ActivityType = "Task";
                if (viewModel.Task.activityid != Guid.Empty)
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
            else if (val.Activity.activitytypecode == "phonecall")
            {
                await viewModel.loadPhoneCall(val.Activity.activityid);
                await viewModel.loadFromTo(val.Activity.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.PhoneCall.statecode.ToString());
                viewModel.ActivityType = "Phone Call";
                if (viewModel.PhoneCall.activityid != Guid.Empty)
                {
                    ContentActivity.IsVisible = true;
                    ContentPhoneCall.IsVisible = true;
                    ContentTask.IsVisible = false;
                    ContentMeet.IsVisible = false;

                    viewModel.Task.activityid = Guid.Empty;
                    viewModel.Meet.activityid = Guid.Empty;
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            }
            else if (val.Activity.activitytypecode == "appointment")
            {
                await viewModel.loadMeet(val.Activity.activityid);
                await viewModel.loadFromToMeet(val.Activity.activityid);
                viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Meet.statecode.ToString());
                viewModel.ActivityType = "Collection Meeting";
                if (viewModel.Meet.activityid != Guid.Empty)
                {
                    ContentActivity.IsVisible = true;
                    ContentPhoneCall.IsVisible = false;
                    ContentTask.IsVisible = false;
                    ContentMeet.IsVisible = true;

                    viewModel.Task.activityid = Guid.Empty;
                    viewModel.PhoneCall.activityid = Guid.Empty;
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            }
        }

        private async void AddButton_Clicked(object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { Language.tao_cong_viec, Language.tao_cuoc_hop, Language.tao_cuoc_goi };
            string asw = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, options);
            if (asw == Language.tao_cong_viec)
            {
                await Navigation.PushAsync(new TaskForm());
            }
            else if (asw == Language.tao_cuoc_goi)
            {
                await Navigation.PushAsync(new PhoneCallForm());
            }
            else if (asw == Language.tao_cuoc_hop)
            {
                await Navigation.PushAsync(new MeetingForm());
            }
            LoadingHelper.Hide();
        }

        void Handle_TimeSlotTapped(object sender, Telerik.XamarinForms.Input.TimeSlotTapEventArgs e)
        {
            viewModel.selectedDate = e.StartTime;
        }

        void Handle_DisplayDateChanged(object sender, Telerik.XamarinForms.Common.ValueChangedEventArgs<object> e)
        {
            viewModel.selectedDate = (DateTime)e.NewValue;
        }

        private void CloseContentActivity_Tapped(object sender, EventArgs e)
        {
            ContentActivity.IsVisible = false;
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
            else if (viewModel.Task.activityid != Guid.Empty)
            {
                LoadingHelper.Show();
                TaskForm newPage = new TaskForm(viewModel.Task.activityid);
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
                else if (viewModel.Task.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusTask(viewModel.CodeCompleted))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Task.statecode.ToString());
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
                else if (viewModel.Task.activityid != Guid.Empty)
                {
                    LoadingHelper.Show();
                    if (await viewModel.UpdateStatusTask(viewModel.CodeCancel))
                    {
                        viewModel.ActivityStatusCode = StatusCodeActivity.GetStatusCodeById(viewModel.Task.statecode.ToString());
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

        private void PhoneCallTo_Tapped(object sender, EventArgs e)
        {
            if (viewModel.PhoneCall != null)
            {
                if (viewModel.PhoneCall.callto_lead_id != Guid.Empty)
                {
                    LeadDetailPage newPage = new LeadDetailPage(viewModel.PhoneCall.callto_lead_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin");
                        }
                    };

                }
                else if (viewModel.PhoneCall.callto_contact_id != Guid.Empty)
                {
                    ContactDetailPage newPage = new ContactDetailPage(viewModel.PhoneCall.callto_contact_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };

                }
                else if (viewModel.PhoneCall.callto_account_id != Guid.Empty)
                {
                    AccountDetailPage newPage = new AccountDetailPage(viewModel.PhoneCall.callto_account_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };
                }
            }
        }

        private void TaskCustomer_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Task != null)
            {
                if (viewModel.Task.lead_id != Guid.Empty)
                {
                    LeadDetailPage newPage = new LeadDetailPage(viewModel.Task.lead_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin");
                        }
                    };

                }
                else if (viewModel.Task.contact_id != Guid.Empty)
                {
                    ContactDetailPage newPage = new ContactDetailPage(viewModel.Task.contact_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };

                }
                else if (viewModel.Task.account_id != Guid.Empty)
                {
                    AccountDetailPage newPage = new AccountDetailPage(viewModel.Task.account_id);
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };
                }
            }
        }       
    }
}
