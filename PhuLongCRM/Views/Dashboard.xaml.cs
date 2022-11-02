using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Xamarin.Essentials;
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
                );

            MessagingCenter.Subscribe<ScanQRPage, string>(this, "CallBack", async (sender, e) =>
            {
                try
                {
                    string[] data = e.Trim().Split(',');
                    if (data[1] == "lead")
                    {
                        LeadDetailPage leadDetail = new LeadDetailPage(Guid.Parse(data[2]), true);
                        leadDetail.OnCompleted = async (onCompleted) =>
                        {
                            if (onCompleted == 1) // thanh cong
                            {
                                await Navigation.PushAsync(leadDetail);
                                LoadingHelper.Hide();
                            }
                            else if(onCompleted == 2) // lead khoong thuoc employee dang dang nhap
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else // e = 3 : khong tim thay thoong tin
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                    else if (data[1] == "account")
                    {
                        AccountDetailPage accountDetail = new AccountDetailPage(Guid.Parse(data[2]));
                        accountDetail.OnCompleted = async (isSuccess) =>
                        {
                            if (isSuccess == 1) // thanh cong
                            {
                                await Navigation.PushAsync(accountDetail);
                                LoadingHelper.Hide();
                            }
                            else if(isSuccess == 2) // KH khong thuoc employee dang dang nhap
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else // khong tim thay thong tin
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                    else if (data[1] == "contact")
                    {
                        ContactDetailPage contactDetail = new ContactDetailPage(Guid.Parse(data[2]));
                        contactDetail.OnCompleted = async (isSuccess) =>
                        {
                            if (isSuccess == 1)// thanh cong
                            {
                                await Navigation.PushAsync(contactDetail);
                                LoadingHelper.Hide();
                            }
                            else if(isSuccess == 2)
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.ma_qr_khong_dung);
                    }
                }
                catch (Exception ex)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.LongMessage(ex.Message);
                }

            });
            AddToolTip();
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

        private async void ShowMore_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Shell.Current.GoToAsync("//HoatDong");
            LoadingHelper.Hide();
        }

        private void ActivitiItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (ActivitiModel)((sender as ExtendedFrame).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item != null && item.activityid != Guid.Empty)
            {
                ActivityPopup.ShowActivityPopup(item.activityid, item.activitytypecode);
            }
            LoadingHelper.Hide();
        }

        private async void ScanQRCode_Clicked(object sender, EventArgs e)
        {
            PermissionStatus camerastatus = await PermissionHelper.RequestCameraPermission();
            if (camerastatus == PermissionStatus.Granted)
            {
                LoadingHelper.Show();
                ScanQRPage scanQR = new ScanQRPage();
                await Navigation.PushAsync(scanQR);
                LoadingHelper.Hide();
            }
        }

        private void CloseToolTips_Tapped(object sender, EventArgs e)
        {
            foreach (var c in gridGiaoDich.Children)
            {
                if (TooltipEffect.GetHasTooltip(c))
                {
                    TooltipEffect.SetHasTooltip(c, false);
                    TooltipEffect.SetHasTooltip(c, true);
                }
            }
            foreach (var c in gridGiaoDichChart.Children)
            {
                if (TooltipEffect.GetHasTooltip(c))
                {
                    TooltipEffect.SetHasTooltip(c, false);
                    TooltipEffect.SetHasTooltip(c, true);
                }
            }
            foreach (var c in gridHoaHong.Children)
            {
                if (TooltipEffect.GetHasTooltip(c))
                {
                    TooltipEffect.SetHasTooltip(c, false);
                    TooltipEffect.SetHasTooltip(c, true);
                }
            }
            foreach (var c in gridHoaHongChart.Children)
            {
                if (TooltipEffect.GetHasTooltip(c))
                {
                    TooltipEffect.SetHasTooltip(c, false);
                    TooltipEffect.SetHasTooltip(c, true);
                }
            }
        }
        public void AddToolTip()
        {
            foreach (var c in gridGiaoDich.Children)
            {
                ListToolTip.ToolTips.Add(c);
            }
            foreach (var c in gridGiaoDichChart.Children)
            {
                ListToolTip.ToolTips.Add(c);
            }
            foreach (var c in gridHoaHong.Children)
            {
                ListToolTip.ToolTips.Add(c);
            }
            foreach (var c in gridHoaHongChart.Children)
            {
                ListToolTip.ToolTips.Add(c);
            }
        }
    }
}