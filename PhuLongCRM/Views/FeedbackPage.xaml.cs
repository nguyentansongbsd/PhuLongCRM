using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedbackPage : ContentPage
    {
        private FeedbackviewModel viewModel;
        public FeedbackPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new FeedbackviewModel();
            PropertyChanged += FeedbackPage_PropertyChanged;
            Init();
        }

        private void FeedbackPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshLanguage();
        }

        private async void Init()
        {
           await viewModel.LoadRating();
            ratingbar.InitStar(viewModel.AppRating);
        }

        private void RatingControl_StarChanged(object sender, int e)
        {
            viewModel.AppRating = e;
            btn_sendfeedback.IsVisible = true;
        }

        private async void SendFeedback_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var res = await viewModel.UpdateRating();
            if(res)
            {
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                btn_sendfeedback.IsVisible = false;
            }   
            else
            {
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
            LoadingHelper.Hide();
        }

        private async void Feedback_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new PhanHoiForm(true));
            LoadingHelper.Hide();
        }

        private async void Request_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new PhanHoiForm(true,true));
            LoadingHelper.Hide();
        }
        private void RefreshLanguage()
        {
            this.Title = Language.danh_gia_title;
            danhgia.Text = Language.danh_gia;
            btn_sendfeedback.Text = Language.gui_danh_gia;
            yeucau.Text = Language.yeu_cau;
            phanhoi.Text = Language.phan_hoi;
        }
    }
    public class FeedbackviewModel : BaseViewModel
    {
        public int AppRating { get; set; }
        public FeedbackviewModel()
        {
        }

        public async Task LoadRating()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='bsd_employee'>
                    <attribute name='bsd_employeeid' />
                    <attribute name='bsd_apprating' />
                    <filter type='and'>
                      <condition attribute='bsd_employeeid' operator='eq' value='{UserLogged.Id}' />
                    </filter>
                  </entity>
                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EmployeeModel>>("bsd_employees", fetchXml);
            if (result == null || result.value.Count == 0)
            {
                return;
            }
            else
            {
                AppRating = result.value.FirstOrDefault().bsd_apprating;
            }
        }
        public async Task<bool> UpdateRating()
        {
            string path = $"/bsd_employees({UserLogged.Id})";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_apprating"] = AppRating;
            CrmApiResponse crmApiResponse = await CrmHelper.PatchData(path, data);
            if (crmApiResponse.IsSuccess)
                return true;
            else
                return false;
        }
    }
}