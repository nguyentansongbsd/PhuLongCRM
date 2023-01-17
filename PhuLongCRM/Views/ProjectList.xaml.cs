using System;
using System.Collections.Generic;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class ProjectList : ContentPage
    {
        public ProjectListViewModel viewModel;
        public static bool? NeedToRefresh = null;
        public ProjectList()
        {
            LoadingHelper.Show();
            InitializeComponent();
            this.BindingContext = viewModel = new ProjectListViewModel();
            NeedToRefresh = false;
            Init();
        }

        public async void Init()
        {
            await viewModel.LoadData();
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

        private void BsdListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            var item = e.Item as ProjectListModel;
            ProjectInfo project = new ProjectInfo(Guid.Parse(item.bsd_projectid));
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
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
            
        }
    }
}
