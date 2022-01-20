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
        public ListViewMultiTabsViewModel viewModel;
        public Action<OptionSetFilter> ItemTapped { get; set; }
        public ListViewMultiTabs(string fetch, string entity, bool isSelect = false, List<OptionSetFilter> itemselected = null)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewMultiTabsViewModel(fetch,entity);
            if (itemselected != null && itemselected.Count > 0)
                viewModel.ItemSelecteds = itemselected;
            this.SetUpDataTemplate(isSelect);
            this.LoadData();
        }
        private async void LoadData()
        {
            LoadingHelper.Show();
            await viewModel.LoadData();
            LoadingHelper.Hide();
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
        private void SetUpDataTemplate(bool isSelect)
        {
            var dataTemplate = new DataTemplate(() =>
            {
                TapGestureRecognizer item = new TapGestureRecognizer();
                item.Tapped += Item_Tapped;
                item.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));

                Grid grid = new Grid();
                grid.BackgroundColor = Color.FromHex("#eeeeee");
                grid.Padding = new Thickness(1, 0, 0, 0);
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                Label lb = new Label();
                lb.TextColor = Color.FromHex("#444444");
                lb.FontSize = 15;
                lb.SetBinding(Label.TextProperty, "Label");
                lb.BackgroundColor = Color.White;
                lb.Padding = 10;

                grid.Children.Add(lb);
                Grid.SetColumn(lb, 0);
                Grid.SetRow(lb, 0);

                if (isSelect == true)
                {
                    CheckBox checkbox = new CheckBox();
                    checkbox.Color = Color.FromHex("#2196F3");
                    checkbox.SetBinding(CheckBox.IsCheckedProperty, "Selected");
                    checkbox.HorizontalOptions = LayoutOptions.End;
                    checkbox.VerticalOptions = LayoutOptions.Center;
                    checkbox.BackgroundColor = Color.White;
                    checkbox.Margin = new Thickness(0, 0, 10, 0);
                    checkbox.IsEnabled = false;

                    grid.Children.Add(checkbox);
                    Grid.SetColumn(checkbox, 0);
                    Grid.SetRow(checkbox, 0);
                }

                Grid grid_tap = new Grid();
                grid_tap.GestureRecognizers.Add(item);
                grid.Children.Add(grid_tap);
                Grid.SetColumn(grid_tap, 0);
                Grid.SetRow(grid_tap, 0);

                return new ViewCell { View = grid };
            });

            listView.ItemTemplate = dataTemplate;
        }
        private void Item_Tapped(object sender, EventArgs e)
        {
            var item = (OptionSetFilter)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (searchBar.IsFocused)
            {
                searchBar.Unfocus();
            }
            ItemTapped?.Invoke(item);
        }
    }
}