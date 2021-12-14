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
                listView.ItemsSource = viewModel.Data.Where(x => x.Label.ToLower().Contains(text.ToLower()));
            }
        }
    }
}