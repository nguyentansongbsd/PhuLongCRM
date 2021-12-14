using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeadDetailPage : ContentPage
    {
        public static bool? NeedToRefreshLeadDetail = null;
        public Action<bool> OnCompleted;
        private LeadDetailPageViewModel viewModel;
        private Guid Id;
        public LeadDetailPage(Guid id)
        {
            InitializeComponent();
            this.Title = "THÔNG TIN KHÁCH HÀNG";
            this.Id = id;
            this.BindingContext = viewModel = new LeadDetailPageViewModel();
            NeedToRefreshLeadDetail = false;
            Init();
        }

        public async void Init()
        {
            await LoadDataThongTin(Id.ToString());
            SetButtonFloatingButton();

            if (viewModel.singleLead.leadid != Guid.Empty)
            {
                viewModel.CustomerGroup = CustomerGroupData.GetCustomerGroupById(viewModel.singleLead.bsd_customergroup);
                viewModel.Area = AreaData.GetAreaById(viewModel.singleLead.bsd_area);
                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_typeofidcard))
                {
                    viewModel.TypeIdCard = TypeIdCardData.GetTypeIdCardById(viewModel.singleLead.bsd_typeofidcard);
                }
                OnCompleted?.Invoke(true);
            }
            else
                OnCompleted?.Invoke(false);
        }

        protected async override void OnAppearing()
        {
            if (NeedToRefreshLeadDetail==true)
            {
                await viewModel.LoadOneLead(Id.ToString()) ;
                if (viewModel.singleLead.new_gender != null) { await viewModel.loadOneGender(viewModel.singleLead.new_gender); }
                if (viewModel.singleLead.industrycode != null) { await viewModel.loadOneIndustrycode(viewModel.singleLead.industrycode); }
                NeedToRefreshLeadDetail = false;
            }
            base.OnAppearing();
        }

        private void SetButtonFloatingButton()
        {
            if (viewModel.singleLead.statuscode == "3") // qualified
            {
                floatingButtonGroup.IsVisible = false;
            }
            else if (viewModel.singleLead.statuscode == "4" || viewModel.singleLead.statuscode == "5" || viewModel.singleLead.statuscode == "6"|| viewModel.singleLead.statuscode == "7")
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Kích hoạt lại KH", "FontAwesomeSolid", "\uf1b8", null, ReactivateLead));
            }
            else
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Chuyển đổi khách hàng", "FontAwesomeSolid", "\uf542", null, LeadQualify));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Không chuyển đổi", "FontAwesomeSolid", "\uf05e", null, LeadDisQualify));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Chỉnh sửa", "FontAwesomeRegular", "\uf044", null, Update));
            }
        }

        private async void Update(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            LeadForm leadForm = new LeadForm(viewModel.singleLead.leadid);
            leadForm.CheckSingleLead = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(leadForm);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Đã xảy ra lỗi. Vui lòng thử lại.");
                }
            };
            
        }

        private async void LeadQualify(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            bool IsSuccessQualify = await viewModel.Qualify(viewModel.singleLead.leadid);
            if (IsSuccessQualify == true)
            {
                if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                if (CustomerPage.NeedToRefreshAccount.HasValue) CustomerPage.NeedToRefreshAccount = true;
                if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Thành công");
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Không thể qualify");
            }

        }

        private async void LeadDisQualify(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { "Mất khách hàng", "Không liên hệ được", "Không quan tâm", "Đã hủy" };
            
            string aws = await DisplayActionSheet("Tuỳ chọn", "Huỷ", null, options);

            if (aws == "Mất khách hàng")
            {
                viewModel.LeadStatusCode = 4;
            }
            else if (aws == "Không liên hệ được")
            {
                viewModel.LeadStatusCode = 5;
            }
            else if (aws == "Không quan tâm")
            {
                viewModel.LeadStatusCode = 6;
            }
            else if (aws == "Đã hủy")
            {
                viewModel.LeadStatusCode = 7;
            }

            if (viewModel.LeadStatusCode != 0)
            {
                viewModel.LeadStateCode = 2;
                bool isSuccess = await viewModel.UpdateStatusCodeLead();
                if (isSuccess)
                {
                    if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                    await viewModel.LoadOneLead(Id.ToString());
                    viewModel.ButtonCommandList.Clear();
                    SetButtonFloatingButton();
                    ToastMessageHelper.ShortMessage("Thành công");
                }
                else
                {
                    ToastMessageHelper.ShortMessage("Thất bại");
                }
            }
            
            LoadingHelper.Hide();
        }

        private async void ReactivateLead(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.LeadStateCode = 0;
            viewModel.LeadStatusCode = 1;
            bool isSuccess = await viewModel.UpdateStatusCodeLead();
            if (isSuccess)
            {
                if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                await viewModel.LoadOneLead(Id.ToString());
                viewModel.ButtonCommandList.Clear();
                SetButtonFloatingButton();
                ToastMessageHelper.ShortMessage("Thành công");
            }
            else
            {
                ToastMessageHelper.ShortMessage("Thất bại");
            }
            LoadingHelper.Hide();
        }

        private async void NhanTin_Tapped(object sender, EventArgs e)
        {
            string phone = viewModel.singleLead.mobilephone.Replace(" ", ""); // thêm sdt ở đây
            if (phone != string.Empty)
            {              
                var checkVadate = PhoneNumberFormatVNHelper.CheckValidate(phone);
                if (checkVadate == true)
                {
                    SmsMessage sms = new SmsMessage(null, phone);
                    await Sms.ComposeAsync(sms);                   
                }
                else
                {
                    ToastMessageHelper.ShortMessage("Số điện thoại sai định dạng. Vui lòng kiểm tra lại");
                }
            }
            else
            {
                ToastMessageHelper.ShortMessage("Khách hàng không có số điện thoại. Vui lòng kiểm tra lại");
            }
        }

        private async void GoiDien_Tapped(object sender, EventArgs e)
        {
            string phone = viewModel.singleLead.mobilephone.Replace(" ",""); // thêm sdt ở đây
            if (phone != string.Empty)
            {              
                var checkVadate = PhoneNumberFormatVNHelper.CheckValidate(phone);
                if (checkVadate == true)
                {
                    await Launcher.OpenAsync($"tel:{phone}");                   
                }
                else
                {
                    ToastMessageHelper.ShortMessage("Số điện thoại sai định dạng. Vui lòng kiểm tra lại");
                }
            }
            else
            {
                ToastMessageHelper.ShortMessage("Khách hàng không có số điện thoại. Vui lòng kiểm tra lại");
            }
        }
        // Tab Thong tin
        private async Task LoadDataThongTin(string leadid)
        {
            if (leadid != null && viewModel.singleLead == null)
            {
                await viewModel.LoadOneLead(leadid);
                if (viewModel.singleLead.new_gender != null) { await viewModel.loadOneGender(viewModel.singleLead.new_gender); }
                if (viewModel.singleLead.industrycode != null) { await viewModel.loadOneIndustrycode(viewModel.singleLead.industrycode); }                
            }
        }

        #region TabPhongThuy
        private void LoadDataPhongThuy()
        {
            if (viewModel.PhongThuy == null)
            {
                viewModel.LoadPhongThuy();
            }
        }

        private void ShowImage_Tapped(object sender, EventArgs e)
        {
            LookUpImagePhongThuy.IsVisible = true;
        }

        protected override bool OnBackButtonPressed()
        {
            if (LookUpImagePhongThuy.IsVisible)
            {
                LookUpImagePhongThuy.IsVisible = false;
                return true;
            }
            return base.OnBackButtonPressed();
        }

        private void Close_LookUpImagePhongThuy_Clicked(object sender, EventArgs e)
        {
            LookUpImagePhongThuy.IsVisible = false;
        }

        #endregion
    }
}