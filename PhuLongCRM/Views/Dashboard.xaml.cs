using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
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
        public static bool? NeedToRefreshNoti = null;
        public DashboardViewModel viewModel;
        private int currentSlide { get; set; } = 0;

        public Dashboard()
        {
            InitializeComponent();
            LoadingHelper.Show();
            NeedToRefreshPhoneCall = false;
            NeedToRefreshMeet = false;
            NeedToRefreshTask = false;
            NeedToRefreshQueue = false;
            NeedToRefreshLeads = false;
            NeedToRefreshNoti = false;
            PropertyChanged += Dashboard_PropertyChanged;
            Init();
            hot.HeightRequest = Application.Current.MainPage.Width * 4/6;
        }

        private void Dashboard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangLanguege();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new DashboardViewModel();
            await Task.WhenAll(
                 viewModel.Load3Activity(),
                 viewModel.LoadQueueFourMonths(),
                 viewModel.LoadQuoteFourMonths(),
                 viewModel.LoadOptionEntryFourMonths(),
                 viewModel.LoadUnitFourMonths(),
                 viewModel.LoadLeads(),
                 viewModel.LoadCommissionTransactions(),
                 viewModel.LoadActivityCount(),
                 AutoPlaySide(),
                 LoadNotification()
                 //viewModel.CountNumNotification()
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
                                ToastMessageHelper.Message(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else // e = 3 : khong tim thay thoong tin
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
                                ToastMessageHelper.Message(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else // khong tim thay thong tin
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
                                ToastMessageHelper.Message(Language.khach_hang_khong_thuoc_so_huu_cua_ban_va_nhom_cua_ban_vui_long_kiem_lai);
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.ma_qr_khong_dung);
                    }
                }
                catch (Exception ex)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(ex.Message);
                }

            });
            AddToolTip();
            await viewModel.TimeOutLogin();
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
                viewModel.numKHMoi = 0;
                viewModel.numKHDaChuyenDoi = 0;
                viewModel.numKHKhongChuyenDoi = 0;
                viewModel.LeadsChart.Clear();
                await viewModel.LoadLeads();
                NeedToRefreshLeads = false;
                LoadingHelper.Hide();
            }

            if (NeedToRefreshTask == true || NeedToRefreshPhoneCall == true || NeedToRefreshMeet == true)
            {
                LoadingHelper.Show();
                viewModel.Activities.Clear();
                await viewModel.Load3Activity();
                await viewModel.LoadActivityCount();
                NeedToRefreshPhoneCall = false;
                NeedToRefreshTask = false;
                NeedToRefreshMeet = false;
                LoadingHelper.Hide();
            }
            if (NeedToRefreshNoti == true)
            {
                LoadingHelper.Show();
                viewModel.NumNotification = 0;
                await viewModel.CountNumNotification();
                NeedToRefreshNoti = false;
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

        private async void GotoCustomer_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new CustomerPage());
            LoadingHelper.Hide();
        }
        private async void GotoProject_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new ProjectList());
            LoadingHelper.Hide();
        }
        private async void GotoDirectSale_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new DirectSale());
            LoadingHelper.Hide();
        }

        private void TabNewsControl_IndexTab(object sender, LookUpChangeEvent e)
        {
            if (e.Item != null)
            {
               if ((int)e.Item == 0)
                {
                    promotion.IsVisible = false;
                    news.IsVisible = true;
                    news.ScrollTo(0, position: ScrollToPosition.Start);
                }
                else if ((int)e.Item == 1)
                {
                    promotion.IsVisible = true;
                    news.IsVisible = false;
                }
            }
        }
        private void ChangLanguege()
        {
            this.Title = Language.trang_chu_title;
            lb_khachhang.Text = Language.khach_hang_title;
            lb_duan.Text = Language.du_an_title;
            lb_giohang.Text = Language.gio_hang;
            lb_khachhangtiemnang.Text = Language.khach_hang_tiem_nang;
            lb_khachhangmoi.Text = Language.khach_hang_moi;
            lb_dachuyendoi.Text = Language.da_chuyen_doi;
            lb_khongchuyendoi.Text = Language.khong_chuyen_doi;
            lb_giaodich.Text = Language.giao_dich;
            lb_giucho.Text = Language.giu_cho;
            lb_dat_coc_dashboard.Text = Language.dat_coc_dashboard;
            lb_hop_dong.Text = Language.hop_dong;
            lb_da_ban.Text = Language.da_ban;
            lb_hoa_hong.Text = Language.hoa_hong;
            lb_duoc_nhan.Text = Language.duoc_nhan;
            lb_da_nhan.Text = Language.da_nhan;
            var format = new FormattedString();
            format.Spans.Add(new Span { Text = "\uf111 ", FontFamily = "FontAwesomeSolid", FontSize = 15, TextColor = Color.FromHex("#D42A16") });
            format.Spans.Add(new Span { Text = Language.duoc_nhan, FontSize = 14, TextColor = Color.FromHex("#444444") });
            lb_duocnhan.FormattedText = format;
            var format2 = new FormattedString();
            format2.Spans.Add(new Span { Text = "\uf111 ", FontFamily = "FontAwesomeSolid", FontSize = 15, TextColor = Color.FromHex("#2196F3") });
            format2.Spans.Add(new Span { Text = Language.da_nhan, FontSize = 14, TextColor = Color.FromHex("#444444") });
            lb_danhan.FormattedText = format2;
            lb_hoat_dong_hom_nay.Text = Language.hoat_dong_hom_nay;
            lb_cong_viec.Text = Language.cong_viec;
            lb_cuoc_hop.Text = Language.cuoc_hop;
            lb_cuoc_goi.Text = Language.cuoc_goi;
        }

        private void news_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if(e != null && viewModel.News != null && e.LastVisibleItemIndex == viewModel.News.Count-1)
            {
                promotion.IsVisible = true;
                news.IsVisible = false;
            }
        }

        private async void ScanQRCode_Tapped(object sender, EventArgs e)
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
        private async void GoToNotificationPage_Tapped(object sender, EventArgs e)
        {
             await Navigation.PushAsync(new NotificationPage());
        }

        private void News_Tapped(object sender, EventArgs e)
        {
            var tap = sender as CachedImage;
            var item = (NewsModel)(tap.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item != null && item.project_id != Guid.Empty)
            {
                LoadingHelper.Show();
                ProjectInfo project = new ProjectInfo(item.project_id);
                project.OnCompleted = async (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(project);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
        }

        private void Promotion_Tapped(object sender, EventArgs e)
        {
            try
            {
                var tap = sender as Grid;
                var item = (NewsModel)(tap.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                if (item != null && item.promotion_id != Guid.Empty)
                {
                    LoadingHelper.Show();
                    PromotionModel promotion = new PromotionModel
                    {   
                        bsd_name = item.promotion_name,
                        bsd_values = item.promotion_values,
                        bsd_startdate = item.promotion_startdate.ToLocalTime(),
                        bsd_enddate = item.promotion_enddate.ToLocalTime(),
                        bsd_description = item.promotion_description,
                        project_name = item.promotion_project_name,
                        phaseslaunch_name = item.promotion_phaseslaunch_name
                    };
                    viewModel.PromotionItem = promotion;
                    KhuyenMai_CenterPopup.ShowCenterPopup();
                    LoadingHelper.Hide();
                }
            }
            catch(Exception ex)
            {

            }
        }
        private async Task AutoPlaySide()
        {
            await viewModel.LoadNews();
            if(viewModel.News != null && viewModel.News.Count > 0)
            {
                Device.StartTimer(TimeSpan.FromSeconds(4), () =>
                {
                    currentSlide++;
                    if (currentSlide == viewModel.News.Count)
                    {
                        currentSlide = 0;
                    }
                    news.Position = currentSlide;
                    return true;
                });
            }    
        }
        private async Task LoadNotification()
        {
            // await viewModel.CountNumNotification();
            if (viewModel.NumNotification > 0)
            {
              //  menu_notification.IconImageSource = new FontImageSource() { Glyph = "\uf0e0", FontFamily = "FontAwesomeSolid", Size = 18, Color = Color.Red };
            }
            else
            {
               // menu_notification.IconImageSource = new FontImageSource() { Glyph = "\uf2b6", FontFamily = "FontAwesomeRegular", Size = 18, Color = Color.White };
            }
        }
    }
}