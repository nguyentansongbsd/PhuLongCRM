using PhuLongCRM.Helper;
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
    public partial class ContractList : ContentPage
    {
        public ContractListViewModel viewModel;
        public static bool? NeedToRefresh = null;
        public ContractList()
        {
            InitializeComponent(); 
            BindingContext = viewModel = new ContractListViewModel();
            NeedToRefresh = false;
            this.PropertyChanged += ContractList_PropertyChanged;
            LoadingHelper.Show();
            Init();
        }

        private void ContractList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangLanguege();
        }

        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadData(),viewModel.LoadProject());
            viewModel.LoadStatus();
            LoadingHelper.Hide();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();
                await viewModel.LoadOnRefreshCommandAsync();
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
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

        private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            ContractModel item = e.Item as ContractModel;
            ContractDetailPage contractDetailPage = new ContractDetailPage(item.salesorderid);
            await Navigation.PushAsync(contractDetailPage);
            //contractDetailPage.OnCompleted = async (OnCompleted) =>
            //{
            //    if (OnCompleted == true)
            //    {
            //        await Navigation.PushAsync(contractDetailPage);
            //        LoadingHelper.Hide();
            //    }
            //    else
            //    {
            //        LoadingHelper.Hide();
            //        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin hợp đồng");
            //    }

            //};
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
        private void ChangLanguege()
        {
            FiltersProject.Placeholder = Language.du_an;
            FiltersStatus.Placeholder = Language.tinh_trang;
            this.Title = Language.hop_dong_title;
            viewModel.FiltersStatus.Clear();
            viewModel.LoadStatus();
        }    
    }
}