using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewMultiTabs : Grid
    {
        private ListViewMultiTabsViewModel viewModel;
        public Action<OptionSet> ItemTapped { get; set; }
        public ListViewMultiTabs(string fetch, string entity)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewMultiTabsViewModel(fetch,entity);
            this.LoadData();
        }

        private async void LoadData()
        {
            LoadingHelper.Show();
            await viewModel.LoadData();
            LoadingHelper.Hide();
        }

        public void listView_ItemTapped(object sender, EventArgs e)
        {
            var item = (OptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (searchBar.IsFocused)
            {
                searchBar.Unfocus();
            }
            ItemTapped?.Invoke(item);
        }

        private void SearchBar_SearchButtonPressed(System.Object sender, System.EventArgs e)
        {
           
        }

        private void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var text = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(text))
            {
                listView.ItemsSource = viewModel.Data;
            }
            else
            {
                var list = from Item in viewModel.Data
                           where Item.Label.ToString().ToLower().Contains(text.ToLower()) ||
                           Item.SDT != null && Item.SDT.ToString().ToLower().Contains(text.ToLower()) ||
                           Item.CMND != null && Item.CMND.ToString().ToLower().Contains(text.ToLower()) ||
                           Item.CCCD != null && Item.CCCD.ToString().ToLower().Contains(text.ToLower()) ||
                           Item.HC != null && Item.HC.ToString().ToLower().Contains(text.ToLower()) ||
                           Item.SoGPKD != null && Item.SoGPKD.ToString().ToLower().Contains(text.ToLower())
                           select Item;
                listView.ItemsSource = list;
                //listView.ItemsSource = viewModel.Data.Where(x => x.Label.ToLower().Contains(text.ToLower()));
            }
        }

        public async void Refresh()
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
    }
}