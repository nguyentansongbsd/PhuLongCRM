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
        private bool isAll;

        public Dashboard()
        {
            InitializeComponent();
            LoadingHelper.Show();
            NeedToRefreshPhoneCall = false;
            NeedToRefreshMeet = false;
            NeedToRefreshTask = false;
            NeedToRefreshQueue = false;
            NeedToRefreshLeads = false;
            PropertyChanged += Dashboard_PropertyChanged;
            Init();
        }

        private void Dashboard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangLanguege();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new DashboardViewModel();
            isAll = true;
            await Task.WhenAll(
                 viewModel.Load3Activity(),
                 viewModel.LoadQueueFourMonths(),
                 viewModel.LoadQuoteFourMonths(),
                 viewModel.LoadOptionEntryFourMonths(),
                 viewModel.LoadUnitFourMonths(),
                 viewModel.LoadLeads(),
                 viewModel.LoadCommissionTransactions(),
                 viewModel.LoadActivityCount(),
                 viewModel.LoadPromotion(),
                 viewModel.LoadNews()
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
                await viewModel.Load3Activity();
                await viewModel.LoadActivityCount();
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
                    if (viewModel.News != null && viewModel.News.Count > 0)
                    {
                        news.IsVisible = true;
                        promotion.IsVisible = false;
                        news.ScrollTo(0, position: ScrollToPosition.Start);
                        isAll = true;
                    }
                    else if (viewModel.Promotions != null && viewModel.Promotions.Count > 0)
                    {
                        promotion.IsVisible = true;
                        news.IsVisible = false;
                    }
                }
                else if ((int)e.Item == 1)
                {
                    promotion.IsVisible = false;
                    news.IsVisible = true;
                    news.ScrollTo(0, position: ScrollToPosition.Start);
                    isAll = false;
                }
                else if ((int)e.Item == 2)
                {
                    promotion.IsVisible = true;
                    news.IsVisible = false;
                    isAll = false;
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
            if(e != null && viewModel.News != null && e.LastVisibleItemIndex == viewModel.News.Count-1 && isAll)
            {
                promotion.IsVisible = true;
                news.IsVisible = false;
            }
        }
    }
}