﻿using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservationList : ContentPage
    {
        private readonly ReservationListViewModel viewModel;
        public static bool? NeedToRefreshReservationList = null;

        public ReservationList()
        {
            InitializeComponent();
            LoadingHelper.Show();
            BindingContext = viewModel = new ReservationListViewModel();
            NeedToRefreshReservationList = false;
            this.PropertyChanged += ReservationList_PropertyChanged;
            Init();
        }

        private void ReservationList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Title = Language.bang_tinh_gia_title;
            FiltersProject.Placeholder = Language.du_an;
            FiltersStatus.Placeholder = Language.tinh_trang;
            viewModel.FiltersStatus.Clear();
            viewModel.LoadStatus();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefreshReservationList ==true)
            {
                LoadingHelper.Show();
                await viewModel.LoadOnRefreshCommandAsync();
                NeedToRefreshReservationList = false;
                LoadingHelper.Hide();
            }
        }

        public async void Init()
        {
            await Task.WhenAll(
                  viewModel.LoadData(),
                  viewModel.LoadProject()
                  );
            viewModel.LoadStatus();
            LoadingHelper.Hide();
        }    

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            ReservationListModel item = e.Item as ReservationListModel;
            BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(item.quoteid);
            bangTinhGiaDetail.OnCompleted = async (OnCompleted) =>
            {
                if (OnCompleted == true)
                {
                    await Navigation.PushAsync(bangTinhGiaDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
        private async void FiltersProject_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }

        private async void FiltersStatus_SelectedItemChanged(object sender, LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
    }
}