using PhuLongCRM.Helper;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
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
            LoadingHelper.Show();
            Init();
        }
        public async void Init()
        {
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
                if (OnCompleted == true)
                {
                    await Navigation.PushAsync(newPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private async void Search_Pressed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }

        private void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Keyword))
            {
                Search_Pressed(null, EventArgs.Empty);
            }
        }
    }
}