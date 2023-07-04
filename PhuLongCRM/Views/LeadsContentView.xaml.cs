using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeadsContentView : ContentView
    {
        public LeadsContentViewViewModel viewModel;
        public Action<bool> OnCompleted;
        public LeadsContentView()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LeadsContentViewViewModel();
            PropertyChanged += LeadsContentView_PropertyChanged;
            LoadingHelper.Show();
            Init();
        }

        private void LeadsContentView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshLanguege();
        }

        public async void Init()
        {
            rb_moi.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
            lb_moi.TextColor = Color.White;
            viewModel.FillterStatus = $@"<condition attribute='statuscode' operator='in'>
                                            <value>1</value>
                                          </condition>";
            await viewModel.LoadData();
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
            var item = e.Item as LeadListModel;
            LeadDetailPage newPage = new LeadDetailPage(item.leadid);
            newPage.OnCompleted = async (OnCompleted) =>
            {
                if (OnCompleted == 1)
                {
                    await Navigation.PushAsync(newPage);
                    LoadingHelper.Hide();
                }
                else if(OnCompleted == 3)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private async void Search_Pressed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            if (viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var item in viewModel.Data)
                {
                    if (item.new_birthday.Day == DateTime.Today.Day && item.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }
            LoadingHelper.Hide();
        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Keyword))
            {
                Search_Pressed(null, EventArgs.Empty);
            }
        }

        private void Sort_Tapped(object sender, EventArgs e)
        {
            SortView.IsVisible = !SortView.IsVisible;
        }

        private async void SelectSort_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (TapGestureRecognizer)((Label)sender).GestureRecognizers[0];
            viewModel.KeySort = item.CommandParameter as string;
            // thay đổi icon
            if (viewModel.KeySort == "1")
            {
                viewModel.Create_on_sort = !viewModel.Create_on_sort;
                if (viewModel.Create_on_sort)
                {
                    icon_createon.Text = "\uf15d";
                    label_createon.Text = Language.ngay_tao_a_z;
                }
                else
                {
                    icon_createon.Text = "\uf882";
                    label_createon.Text = Language.ngay_tao_z_a;
                }
                label_createon.TextColor = Color.FromHex("1399D5");
                label_rating.TextColor = Color.FromHex("444444");
                label_status.TextColor = Color.FromHex("444444");
            }
            else if (viewModel.KeySort == "2")
            {
                viewModel.Rating_sort = !viewModel.Rating_sort;
                if (viewModel.Rating_sort)
                {
                    icon_rating.Text = "\uf15d";
                    label_rating.Text = Language.danh_gia_a_z;
                }
                else
                {
                    icon_rating.Text = "\uf882";
                    label_rating.Text = Language.danh_gia_z_a;
                }
                label_rating.TextColor = Color.FromHex("1399D5");
                label_createon.TextColor = Color.FromHex("444444");
                label_status.TextColor = Color.FromHex("444444");
            }
            else if (viewModel.KeySort == "3")
            {
                viewModel.Allocation_sort = !viewModel.Allocation_sort;
                if (viewModel.Allocation_sort)
                {
                    icon_phanbo.IsChecked = true;
                    label_status.TextColor = Color.FromHex("1399D5");
                    label_rating.TextColor = Color.FromHex("444444");
                    label_createon.TextColor = Color.FromHex("444444");
                }
                else
                {
                    icon_phanbo.IsChecked = false;
                    label_status.TextColor = Color.FromHex("444444");
                    label_rating.TextColor = Color.FromHex("444444");
                    label_createon.TextColor = Color.FromHex("444444");
                }
            }
            await viewModel.LoadOnRefreshCommandAsync();
            if (viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var i in viewModel.Data)
                {
                    if (i.new_birthday.Day == DateTime.Today.Day && i.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(i);
                    }
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }
            SortView.IsVisible = false;
            LoadingHelper.Hide();
        }

        private async void Moi_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            rb_moi.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
            lb_moi.TextColor =Color.White;
            rb_dachuyendoi.BackgroundColor = Color.White;
            lb_dachuyendoi.TextColor = Color.FromHex("#444444");
            rb_khongchuyendoi.BackgroundColor = Color.White;
            lb_khongchuyendoi.TextColor = Color.FromHex("#444444");
            viewModel.FillterStatus = $@"<condition attribute='statuscode' operator='in'>
                                            <value>1</value>
                                          </condition>";
            await viewModel.LoadOnRefreshCommandAsync();
            if (viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var item in viewModel.Data)
                {
                    if (item.new_birthday.Day == DateTime.Today.Day && item.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }
            LoadingHelper.Hide();
        }
        private async void DaChuyenDoi_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            rb_moi.BackgroundColor = Color.White;
            lb_moi.TextColor = Color.FromHex("#444444");
            rb_dachuyendoi.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
            lb_dachuyendoi.TextColor = Color.White;
            rb_khongchuyendoi.BackgroundColor = Color.White;
            lb_khongchuyendoi.TextColor = Color.FromHex("#444444");
            viewModel.FillterStatus = $@"<condition attribute='statuscode' operator='in'>
                                            <value>3</value>
                                          </condition>";
            await viewModel.LoadOnRefreshCommandAsync();
            if (viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var item in viewModel.Data)
                {
                    if (item.new_birthday.Day == DateTime.Today.Day && item.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }
            LoadingHelper.Hide();
        }
        private async void KhongChuyenDoi_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            rb_moi.BackgroundColor = Color.White;
            lb_moi.TextColor = Color.FromHex("#444444");
            rb_dachuyendoi.BackgroundColor = Color.White;
            lb_dachuyendoi.TextColor = Color.FromHex("#444444");
            rb_khongchuyendoi.BackgroundColor = (Color)App.Current.Resources["NavigationPrimary"];
            lb_khongchuyendoi.TextColor = Color.White;
            viewModel.FillterStatus = $@"<condition attribute='statuscode' operator='in'>
                                            <value>2</value>
                                            <value>4</value>
                                            <value>5</value>
                                            <value>7</value>
                                            <value>6</value>
                                          </condition>";
            await viewModel.LoadOnRefreshCommandAsync();
            if (viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach (var item in viewModel.Data)
                {
                    if (item.new_birthday.Day == DateTime.Today.Day && item.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }
            LoadingHelper.Hide();
        }
        private void RefreshLanguege()
        {
            var format = new FormattedString();
            format.Spans.Add(new Span { Text = "\uf08d ", FontFamily = "FontAwesomeSolid", FontSize = 10 });
            format.Spans.Add(new Span { Text = Language.new_sts });
            lb_moi.FormattedText = format;
            lb_dachuyendoi.Text = Language.da_chuyen;
            lb_khongchuyendoi.Text = Language.khong_chuyen;

            if (viewModel.Create_on_sort)
            {
                label_createon.Text = Language.ngay_tao_a_z;
            }
            else
            {
                label_createon.Text = Language.ngay_tao_z_a;
            }
            if (viewModel.Rating_sort)
            {
                label_rating.Text = Language.danh_gia_a_z;
            }
            else
            {
                label_rating.Text = Language.danh_gia_z_a;
            }
            label_status.Text = Language.phan_bo;
        }
        async void Birtday_Tapped(System.Object sender, System.EventArgs e)
        {
            viewModel.FillterBirtday = !viewModel.FillterBirtday;
            if (viewModel.FillterBirtday)
            {
                filtter_birtday.TextColor = Color.FromHex("#1399D5");
            }
            else
            {
                filtter_birtday.TextColor = Color.FromHex("#444444");
            }
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            if(viewModel.Data != null && viewModel.Data.Count > 0 && viewModel.FillterBirtday)
            {
                List<LeadListModel> list = new List<LeadListModel>();
                foreach(var item in viewModel.Data)
                {
                    if(item.new_birthday.Day == DateTime.Today.Day && item.new_birthday.Month == DateTime.Today.Month)
                    {
                        list.Add(item);
                    }    
                }
                viewModel.Data.Clear();
                viewModel.Data.AddRange(list);
            }    
            LoadingHelper.Hide();
        }
    }
}