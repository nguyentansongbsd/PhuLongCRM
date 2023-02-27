using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountsContentView : ContentView
    {
        public Action<bool> OnCompleted;
        public AccountContentViewViewModel viewModel;
        public AccountsContentView()
        {
            InitializeComponent();
            BindingContext = viewModel = new AccountContentViewViewModel();
            PropertyChanged += AccountsContentView_PropertyChanged;
            Init();
        }

        private void AccountsContentView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshLanguege();
        }

        public async void Init()
        {
            viewModel.KeyFilter = "0";
            await viewModel.LoadData();
            rb_khachHang.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
            lb_khachHang.TextColor = Color.White;
            rb_chuDauTu.BackgroundColor = Color.White;
            lb_chuDauTu.TextColor = Color.FromHex("#444444");
            rb_sanDaiLy.BackgroundColor = Color.White;
            lb_sanDaiLy.TextColor = Color.FromHex("#444444");
            if (viewModel.Data.Count > 0)
            {
                OnCompleted?.Invoke(true);
            }
            else
            {
                OnCompleted?.Invoke(false);
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            var item = e.Item as AccountListModel;
            AccountDetailPage newPage = new AccountDetailPage(item.accountid);
            newPage.OnCompleted = async (OnCompleted) =>
            {
                if (OnCompleted == 1)
                {
                    await Navigation.PushAsync(newPage);
                    LoadingHelper.Hide();
                }
                else if(OnCompleted == 3 || OnCompleted == 2)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
                
            };
        }

        private async void SearchBar_SearchButtonPressed(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }

        private void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Keyword))
            {
                SearchBar_SearchButtonPressed(null, EventArgs.Empty);
            }
        }

        private void Filter_Tapped(object sender, EventArgs e)
        {
            FilterView.IsVisible = !FilterView.IsVisible;
        }

        private async void SelectFilter_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (TapGestureRecognizer)((Label)sender).GestureRecognizers[0];
            viewModel.KeyFilter = item.CommandParameter as string;
            // thay đổi icon
            if (viewModel.KeyFilter == "1")
            {
                label_official.FontAttributes = FontAttributes.Bold;
                label_inactive.FontAttributes = FontAttributes.None;
                label_potential.FontAttributes = FontAttributes.None;
                label_All.FontAttributes = FontAttributes.None;

                label_official.TextColor = Color.FromHex("1399D5");
                label_potential.TextColor = Color.FromHex("444444");
                label_All.TextColor = Color.FromHex("444444");
                label_inactive.TextColor = Color.FromHex("444444");
            }
            else if (viewModel.KeyFilter == "2")
            {
                label_potential.FontAttributes = FontAttributes.Bold;
                label_inactive.FontAttributes = FontAttributes.None;
                label_official.FontAttributes = FontAttributes.None;
                label_All.FontAttributes = FontAttributes.None;

                label_potential.TextColor = Color.FromHex("1399D5");
                label_official.TextColor = Color.FromHex("444444");
                label_All.TextColor = Color.FromHex("444444");
                label_inactive.TextColor = Color.FromHex("444444");
            }
            else if (viewModel.KeyFilter == "0")
            {
                label_All.FontAttributes = FontAttributes.Bold;
                label_inactive.FontAttributes = FontAttributes.None;
                label_potential.FontAttributes = FontAttributes.None;
                label_official.FontAttributes = FontAttributes.None;

                label_potential.TextColor = Color.FromHex("444444");
                label_official.TextColor = Color.FromHex("444444");
                label_inactive.TextColor = Color.FromHex("444444");
                label_All.TextColor = Color.FromHex("1399D5");
            }
            else if (viewModel.KeyFilter == "3")
            {
                label_All.FontAttributes = FontAttributes.None;
                label_inactive.FontAttributes = FontAttributes.Bold;
                label_potential.FontAttributes = FontAttributes.None;
                label_official.FontAttributes = FontAttributes.None;

                label_potential.TextColor = Color.FromHex("444444");
                label_official.TextColor = Color.FromHex("444444");
                label_All.TextColor = Color.FromHex("444444");
                label_inactive.TextColor = Color.FromHex("1399D5");
            }
            await viewModel.LoadOnRefreshCommandAsync();
            FilterView.IsVisible = false;
            LoadingHelper.Hide();
        }

        private async void CustumerType_Tapped(object sender, EventArgs e)
        {
            string sts = ((sender as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;
            LoadingHelper.Show();
            if (sts == "100000000") // Khach hang
            {
                rb_khachHang.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
                lb_khachHang.TextColor = Color.White;
                rb_chuDauTu.BackgroundColor = Color.White;
                lb_chuDauTu.TextColor = Color.FromHex("#444444");
                rb_sanDaiLy.BackgroundColor = Color.White;
                lb_sanDaiLy.TextColor = Color.FromHex("#444444");

            }
            else if (sts == "100000003") // Chu dau tu
            {
                rb_khachHang.BackgroundColor = Color.White;
                lb_khachHang.TextColor = Color.FromHex("#444444");
                rb_chuDauTu.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
                lb_chuDauTu.TextColor = Color.White;
                rb_sanDaiLy.BackgroundColor = Color.White;
                lb_sanDaiLy.TextColor = Color.FromHex("#444444");
            }
            else if (sts == "100000002")  // San dai ly
            {
                rb_khachHang.BackgroundColor = Color.White;
                lb_khachHang.TextColor = Color.FromHex("#444444");
                rb_chuDauTu.BackgroundColor = Color.White;
                lb_chuDauTu.TextColor = Color.FromHex("#444444");
                rb_sanDaiLy.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
                lb_sanDaiLy.TextColor = Color.White;
            }
            viewModel.FillterStatus = $@"<condition attribute='bsd_businesstype' operator='contain-values'>
                                                <value>{sts}</value>
                                              </condition>";
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
        private void RefreshLanguege()
        {
            var format = new FormattedString();
            format.Spans.Add(new Span { Text = "\uf08d ", FontFamily = "FontAwesomeSolid", FontSize = 10 });
            format.Spans.Add(new Span { Text = Language.khach_hang_filter });
            lb_khachHang.FormattedText = format;
            lb_chuDauTu.Text = Language.chu_dau_tu_filter;
            lb_sanDaiLy.Text = Language.san_dai_ly_filter;

            label_All.Text = Language.hieu_luc;
            label_inactive.Text = Language.vo_hieu_luc;
            label_official.Text = Language.chinh_thuc;
            label_potential.Text = Language.tiem_nang_sts;
        }
    }
}