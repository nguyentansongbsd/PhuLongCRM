using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeetingForm : ContentPage
    {
        public Action<bool> OnCompleted;
        public MeetingViewModel viewModel;
        private Guid MeetId;
        private bool IsInit;
        public MeetingForm()
        {
            InitializeComponent();
            Init();
            Create();
        }

        public MeetingForm(Guid id)
        {
            InitializeComponent();
            Init();
            MeetId = id;
            Update();
        }     

        private void Init()
        {
            LoadingHelper.Show();
            BindingContext = viewModel = new MeetingViewModel();
            DatePickerStart.DefaultDisplay = DateTime.Now;
            DatePickerEnd.DefaultDisplay = DateTime.Now;
            // kiểm tra page trước là page nào
            var page_before = App.Current.MainPage.Navigation.NavigationStack.Last()?.GetType().Name;
            if(page_before == "ContactDetailPage" || page_before == "AccountDetailPage")
            {
                if (page_before == "ContactDetailPage" && ContactDetailPage.FromCustomer != null && !string.IsNullOrWhiteSpace(ContactDetailPage.FromCustomer.Val))
                {
                    viewModel.CustomerMapping = ContactDetailPage.FromCustomer;
                    Lookup_Required.IsVisible = false;
                    CustomerMapping.IsVisible = true;
                }
                else if (page_before == "AccountDetailPage" && AccountDetailPage.FromCustomer != null && !string.IsNullOrWhiteSpace(AccountDetailPage.FromCustomer.Val))
                {
                    viewModel.CustomerMapping = AccountDetailPage.FromCustomer;
                    Lookup_Required.IsVisible = false;
                    CustomerMapping.IsVisible = true;
                }
                else
                {
                    Lookup_Required.IsVisible = true;
                    CustomerMapping.IsVisible = false;
                }
            }
            else
            {
                Lookup_Required.IsVisible = true;
                CustomerMapping.IsVisible = false;
            }
        }

        private void Create()
        {
            this.Title = Language.tao_moi_cuoc_hop;
            BtnSave.Text = Language.tao_cuoc_hop;
            IsInit = true;
            BtnSave.Clicked += Create_Clicked;
        }

        private void Create_Clicked(object sender, EventArgs e)
        {
            SaveData(Guid.Empty);
        }

        private async void Update()
        {
            this.Title = Language.cap_nhat_cuoc_hop;
            BtnSave.Text = Language.cap_nhat;
            BtnSave.Clicked += Update_Clicked;
            await viewModel.loadDataMeet(this.MeetId);

            if (viewModel.MeetingModel.activityid != Guid.Empty)
            {
                OnCompleted?.Invoke(true);
                IsInit = true;
            }
            else
                OnCompleted?.Invoke(false);
        }

        private void Update_Clicked(object sender, EventArgs e)
        {
            SaveData(this.MeetId);
        }

        private async void SaveData(Guid id)
        {
            if (string.IsNullOrWhiteSpace(viewModel.MeetingModel.subject))
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_nhap_chu_de_cuoc_hop);
                return;
            }
            if (viewModel.CustomerMapping == null)
            {
                if (viewModel.Required == null || viewModel.Required.Count <= 0)
                {
                    ToastMessageHelper.ShortMessage(Language.vui_long_chon_nguoi_tham_du_bat_buoc);
                    return;
                }
            }
            if (viewModel.MeetingModel.scheduledstart == null || viewModel.MeetingModel.scheduledend == null)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_ket_thuc_va_thoi_gian_bat_dau);
                return;
            }
            if (viewModel.MeetingModel.scheduledstart != null && viewModel.MeetingModel.scheduledend != null)
            {
                if (this.compareDateTime(viewModel.MeetingModel.scheduledstart, viewModel.MeetingModel.scheduledend) != -1)
                {
                    ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_ket_thuc_lon_hon_thoi_gian_bat_dau);
                    return;
                }
            }
            if (viewModel.CustomerMapping == null)
            {
                if (viewModel.Optional != null && viewModel.Optional.Count > 0)
                {
                    if (!CheckCusomer(viewModel.Required, viewModel.Optional))
                    {
                        ToastMessageHelper.ShortMessage(Language.nguoi_tham_du_bat_buoc_phai_khac_nguoi_tham_du_khong_bat_buoc);
                        return;
                    }
                }
            }
            else
            {
                if (viewModel.Optional != null && viewModel.Optional.Count > 0)
                {
                    if (!CheckCusomer(null, viewModel.Optional, viewModel.CustomerMapping))
                    {
                        ToastMessageHelper.ShortMessage(Language.nguoi_tham_du_bat_buoc_phai_khac_nguoi_tham_du_khong_bat_buoc);
                        return;
                    }
                }
            }
            if (viewModel.CustomerMapping == null)
            {
                if (viewModel.Customer != null)
                {
                    if (!CheckCusomer(viewModel.Required, null, viewModel.Customer))
                    {
                        ToastMessageHelper.ShortMessage(Language.nguoi_tham_du_bat_buoc_phai_khac_nguoi_lien_quan);
                        return;
                    }
                }
            }
            else
            {
                if (viewModel.Customer != null)
                {
                    if (viewModel.Customer.Val == viewModel.CustomerMapping.Val)
                    {
                        ToastMessageHelper.ShortMessage(Language.nguoi_tham_du_bat_buoc_phai_khac_nguoi_lien_quan);
                        return;
                    }
                }
            }
            if (viewModel.Optional != null && viewModel.Optional.Count > 0 && viewModel.Customer != null)
            {
                if (!CheckCusomer(null, viewModel.Optional, viewModel.Customer))
                {
                    ToastMessageHelper.ShortMessage(Language.nguoi_tham_du_khong_bat_buoc_phai_khac_nguoi_lien_quan);
                    return;
                }
            }

            LoadingHelper.Show();

            if (id == Guid.Empty)
            {
                if (await viewModel.createMeeting())
                {
                    if (Dashboard.NeedToRefreshMeet.HasValue) Dashboard.NeedToRefreshMeet = true;
                    if (ActivityList.NeedToRefreshMeet.HasValue) ActivityList.NeedToRefreshMeet = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
                    ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
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
                if (await viewModel.UpdateMeeting(id))
                {
                    if (Dashboard.NeedToRefreshMeet.HasValue) Dashboard.NeedToRefreshMeet = true;
                    if (ActivityList.NeedToRefreshMeet.HasValue) ActivityList.NeedToRefreshMeet = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
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

        private int compareDateTime(DateTime? date, DateTime? date1)
        {
            if (date != null && date1 != null)
            {
                int result = DateTime.Compare(date.Value, date1.Value);
                if (result < 0)
                    return -1;
                else if (result == 0)
                    return 0;
                else
                    return 1;
            }
            if (date == null && date1 != null)
                return -1;
            if (date1 == null && date != null)
                return 1;
            return 0;
        }

        private void DatePickerStart_DateSelected(object sender, EventArgs e)
        {
            if (IsInit)
            {
                if (viewModel.MeetingModel.scheduledend != null)
                {
                    if (this.compareDateTime(viewModel.MeetingModel.scheduledstart, viewModel.MeetingModel.scheduledend) != -1)
                    {
                        ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_ket_thuc_lon_hon_thoi_gian_bat_dau);
                        viewModel.MeetingModel.scheduledstart = viewModel.MeetingModel.scheduledend;
                    }
                }
            }
        }

        private void DatePickerEnd_DateSelected(object sender, EventArgs e)
        {
            if (IsInit)
            {
                if (viewModel.MeetingModel.scheduledstart != null)
                {
                    if (this.compareDateTime(viewModel.MeetingModel.scheduledstart, viewModel.MeetingModel.scheduledend) != -1)
                    {
                        ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_ket_thuc_lon_hon_thoi_gian_bat_dau);
                        viewModel.MeetingModel.scheduledend = viewModel.MeetingModel.scheduledstart;
                    }
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_bat_dau);
                }
            }
        }

        private void AllDayEvent_changeChecked(object sender, EventArgs e)
        {
            if (viewModel.MeetingModel.scheduledstart != null)
            {
                if (viewModel.MeetingModel.isalldayevent)
                {
                    var timeStart = viewModel.MeetingModel.scheduledstart.Value;
                    viewModel.MeetingModel.timeStart = new TimeSpan(timeStart.Hour, timeStart.Minute, timeStart.Second);
                    if (viewModel.MeetingModel.scheduledend != null)
                    {
                        var actualdurationminutes = Math.Round((viewModel.MeetingModel.scheduledend.Value - viewModel.MeetingModel.scheduledstart.Value).TotalMinutes);
                        viewModel.MeetingModel.scheduleddurationminutes = int.Parse(actualdurationminutes.ToString());
                    }
                    else
                    {
                        viewModel.MeetingModel.scheduleddurationminutes = 0;
                    }    

                    viewModel.MeetingModel.scheduledstart = new DateTime(timeStart.Year, timeStart.Month, timeStart.Day, 0, 0, 0);
                    viewModel.MeetingModel.scheduledend = viewModel.MeetingModel.scheduledstart.Value.AddDays(1);
                }
                else
                {
                    var dateStart = viewModel.MeetingModel.scheduledstart.Value;
                    TimeSpan timeStart = viewModel.MeetingModel.timeStart;

                    viewModel.MeetingModel.scheduledstart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, timeStart.Hours, timeStart.Minutes, timeStart.Seconds);
                    viewModel.MeetingModel.scheduledend = viewModel.MeetingModel.scheduledstart.Value.AddMinutes(viewModel.MeetingModel.scheduleddurationminutes);
                }
            }
            else
            {
                viewModel.MeetingModel.isalldayevent = false;
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_thoi_gian_bat_dau);
            }    
        }
        private bool CheckCusomer(List<OptionSetFilter> required = null, List<OptionSetFilter> option = null, OptionSet customer = null)
        {
            // kiểm tra từ kh hàng- kh liên quan k check
            if (required != null && option != null)
            {
                if (required.Where(x => option.Any(s => s == x)).ToList().Count > 0)
                    return false;
                else
                    return true;
            }
            else if (required != null && customer != null)
            {
                if (required.Where(x => x.Val == customer.Val).ToList().Count > 0)
                    return false;
                else
                    return true;
            }
            else if (option != null && customer != null)
            {
                if (option.Where(x => x.Val == customer.Val).ToList().Count > 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
}