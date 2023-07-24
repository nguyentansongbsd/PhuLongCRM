using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewDemand : Grid
    {
        public ListViewDemandViewModel viewModel;
        public Action<OptionSet> ItemTapped { get; set; }
        public ListViewDemand(string fetch, string entity, List<OptionSet> itemselected = null)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewDemandViewModel(fetch, entity);
            if (itemselected != null && itemselected.Count > 0)
                viewModel.ItemSelecteds.AddRange(itemselected);
            this.SetUpDataTemplate();
            this.LoadData();
        }
        private async void LoadData()
        {
            LoadingHelper.Show();
            await viewModel.LoadData();
            LoadingHelper.Hide();
        }
        private async void SearchBar_SearchButtonPressed(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
        private void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Key))
            {
                SearchBar_SearchButtonPressed(null, EventArgs.Empty);
            }
        }
        public async void Refresh()
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }
        private void SetUpDataTemplate()
        {
            var dataTemplate = new DataTemplate(() =>
            {
                TapGestureRecognizer item = new TapGestureRecognizer();
                item.Tapped += Item_Tapped;
                item.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));

                Grid grid = new Grid();
                grid.BackgroundColor = Color.White;
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                grid.Margin = new Thickness(0, 1, 0, 0);
                grid.Padding = new Thickness(10, 5, 10, 5);

                Label lb = new Label();
                lb.TextColor = Color.FromHex("#444444");
                lb.FontSize = 15;
                lb.SetBinding(Label.TextProperty, "Label");
                lb.BackgroundColor = Color.White;
                lb.VerticalOptions = LayoutOptions.Center;
                lb.LineBreakMode = LineBreakMode.TailTruncation;
                //  lb.Padding = 10;

                grid.Children.Add(lb);
                Grid.SetColumn(lb, 0);
                Grid.SetRow(lb, 0);

                CheckBox checkbox = new CheckBox();
                checkbox.Color = Color.FromHex("#777777");
                checkbox.SetBinding(CheckBox.IsCheckedProperty, "Selected");
                checkbox.HorizontalOptions = LayoutOptions.End;
                checkbox.VerticalOptions = LayoutOptions.Center;
                checkbox.BackgroundColor = Color.White;
                checkbox.Margin = new Thickness(0, 0, 10, 0);
                checkbox.IsEnabled = false;
                checkbox.Margin = 0;

                grid.Children.Add(checkbox);
                Grid.SetColumn(checkbox, 1);
                Grid.SetRow(checkbox, 0);


                Grid grid_tap = new Grid();
                grid_tap.GestureRecognizers.Add(item);
                grid.Children.Add(grid_tap);
                Grid.SetColumn(grid_tap, 0);
                Grid.SetRow(grid_tap, 0);
                Grid.SetColumnSpan(grid_tap, 2);

                return new ViewCell { View = grid };
            });

            listView.ItemTemplate = dataTemplate;
            listView.BackgroundColor = Color.FromHex("#eeeeee");
        }
        private void Item_Tapped(object sender, EventArgs e)
        {
            var item = (OptionSet)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (searchBar.IsFocused)
            {
                searchBar.Unfocus();
            }
            ItemTapped?.Invoke(item);
        }
    }
}