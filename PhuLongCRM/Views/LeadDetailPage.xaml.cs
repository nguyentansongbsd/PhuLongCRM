using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
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
            this.Title = Language.thong_tin_khach_hang;
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
                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_typeofidcard ))
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
                if (viewModel.singleLead.account_id != Guid.Empty)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.di_den_kh_doanh_nghiep, "FontAwesomeRegular", "\uf1ad", null, GoToAccount));
                    floatingButtonGroup.IsVisible = true;
                }
                if (viewModel.singleLead.contact_id != Guid.Empty)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.di_den_kh_ca_nhan, "FontAwesomeRegular", "\uf2c1", null, GoToContact));
                    floatingButtonGroup.IsVisible = true;
                }
            }
            else if (viewModel.singleLead.statuscode == "4" || viewModel.singleLead.statuscode == "5" || viewModel.singleLead.statuscode == "6"|| viewModel.singleLead.statuscode == "7")
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.kich_hoat_lai_kh, "FontAwesomeSolid", "\uf1b8", null, ReactivateLead));
            }
            else
            {
                if (viewModel.singleLead.leadqualitycode == 3)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.khong_chuyen_doi, "FontAwesomeSolid", "\uf05e", null, LeadDisQualify));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.cap_nhat, "FontAwesomeRegular", "\uf044", null, Update));
                }
                else
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chuyen_doi_khach_hang, "FontAwesomeSolid", "\uf542", null, LeadQualify));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.khong_chuyen_doi, "FontAwesomeSolid", "\uf05e", null, LeadDisQualify));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.cap_nhat, "FontAwesomeRegular", "\uf044", null, Update));
                }
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
                    ToastMessageHelper.ShortMessage(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
                }
            };
            
        }

        private async void LeadQualify(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            CrmApiResponse apiResponse = await viewModel.Qualify(viewModel.singleLead.leadid);
            if (apiResponse.IsSuccess == true)
            {
                if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                if (CustomerPage.NeedToRefreshAccount.HasValue) CustomerPage.NeedToRefreshAccount = true;
                if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                await viewModel.LoadOneLead(Id.ToString());
                viewModel.ButtonCommandList.Clear();
                SetButtonFloatingButton();
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.LongMessage(apiResponse.ErrorResponse.error.message);
            }
        }

        private async void LeadDisQualify(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { Language.mat_khach_hang, Language.khong_lien_lac_duoc, Language.khong_quan_tam, Language.da_huy };
            
            string aws = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, options);

            if (aws == Language.mat_khach_hang)
            {
                viewModel.LeadStatusCode = 4;
            }
            else if (aws == Language.khong_lien_lac_duoc)
            {
                viewModel.LeadStatusCode = 5;
            }
            else if (aws == Language.khong_quan_tam)
            {
                viewModel.LeadStatusCode = 6;
            }
            else if (aws == Language.da_huy)
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
                    ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
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
                ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
            }
            LoadingHelper.Hide();
        }

        private async void NhanTin_Tapped(object sender, EventArgs e)
        {
            string phone = viewModel.singleLead.mobilephone.Replace(" ", "").Replace("+84-", "").Replace("84",""); // thêm sdt ở đây
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
                    ToastMessageHelper.ShortMessage(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                }
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.khach_hang_khong_co_so_dien_thoai_vui_long_kiem_tra_lai);
            }
        }

        private async void GoiDien_Tapped(object sender, EventArgs e)
        {
            string phone = viewModel.singleLead.mobilephone.Replace(" ","").Replace("+84-","").Replace("84", ""); // thêm sdt ở đây
            if (phone != string.Empty)
            {              
                var checkVadate = PhoneNumberFormatVNHelper.CheckValidate(phone);
                if (checkVadate == true)
                {
                    await Launcher.OpenAsync($"tel:{phone}");                   
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                }
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.khach_hang_khong_co_so_dien_thoai_vui_long_kiem_tra_lai);
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
        private void GoToContact(object sender, EventArgs e)
        {
            if (viewModel.singleLead != null && viewModel.singleLead.contact_id != Guid.Empty)
            {
                LoadingHelper.Show();
                ContactDetailPage newPage = new ContactDetailPage(viewModel.singleLead.contact_id);
                newPage.OnCompleted = async (IsSuccess) =>
                {
                    if (IsSuccess)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
                    }
                };
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
            }
        }
        private void GoToAccount(object sender, EventArgs e)
        {
            if (viewModel.singleLead != null && viewModel.singleLead.account_id != Guid.Empty)
            {
                LoadingHelper.Show();
                AccountDetailPage newPage = new AccountDetailPage(viewModel.singleLead.account_id);
                newPage.OnCompleted = async (IsSuccess) =>
                {
                    if (IsSuccess)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
                    }
                };
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
            }
        }
    }
}