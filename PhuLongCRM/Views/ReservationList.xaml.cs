using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
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
            Init();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefreshReservationList ==true)
            {
                await viewModel.LoadOnRefreshCommandAsync();
                NeedToRefreshReservationList = false;
            }
        }

        public async void Init()
        {
            await viewModel.LoadData();
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
                    ToastMessageHelper.ShortMessage("Không tìm thấy thông tin bảng tính giá");
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
    }
}