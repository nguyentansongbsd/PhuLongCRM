﻿using PhuLongCRM.Helper;
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
    public partial class QueueList : ContentPage
    {
        public static bool? NeedToRefresh = null;
        private readonly QueuListViewModel viewModel;
        public QueueList()
        {
            InitializeComponent();
            LoadingHelper.Show();
            NeedToRefresh = false;
            BindingContext = viewModel = new QueuListViewModel();
            this.PropertyChanged += QueueList_PropertyChanged;
            Init();
        }

        private void QueueList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Title = Language.giu_cho_btn;
            FiltersProject.Placeholder = Language.du_an;
            FiltersStatus.Placeholder = Language.tinh_trang;
            FiltersQueueForProject.Placeholder = Language.thien_chi;
            FiltersRapCan.Placeholder = Language.rap_can;
            viewModel.FiltersQueueForProject.Clear();
            viewModel.FiltersStatus.Clear();
            viewModel.RapCans.Clear();
            viewModel.LoadQueueForProject();
            viewModel.LoadStatus();
            viewModel.LoadRapCan();
        }

        public async void Init()
        {
            await Task.WhenAll(
                  viewModel.LoadData(),
                  viewModel.LoadProject()
                  );
            viewModel.LoadQueueForProject();
            viewModel.LoadStatus();
            viewModel.LoadRapCan();
            LoadingHelper.Hide();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh != null && NeedToRefresh == true)
            {
                LoadingHelper.Show();
                await viewModel.LoadOnRefreshCommandAsync();
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
        }
        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            QueuesModel val = e.Item as QueuesModel;
            QueuesDetialPage queuesDetialPage = new QueuesDetialPage(val.opportunityid);
            queuesDetialPage.OnCompleted = async (isSuccess) => {
                if (isSuccess)
                {
                    await Navigation.PushAsync(queuesDetialPage);
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

        private async void FiltersQueueForProject_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }

        private async void FiltersRapCan_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
    }
}