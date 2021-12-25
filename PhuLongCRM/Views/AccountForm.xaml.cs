using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountForm : ContentPage
    {
        public Action<bool> OnCompleted;
        private Guid AccountId;
        private AccountFormViewModel viewModel;

        public AccountForm()
        {
            InitializeComponent();
            AccountId = Guid.Empty;
            Init();
            Create();
        }

        public AccountForm(Guid accountId)
        {
            InitializeComponent();
            AccountId = accountId;
            Init();
            Update();
        }

        public void Init()
        {
            this.BindingContext = viewModel = new AccountFormViewModel();
            //Lookup_BusinessType.BindingContext = viewModel;
            SetPreOpen();
        }

        private void Create()
        {
            this.Title = "Tạo Mới Khách Hàng Doanh Nghiệp";
            btnSave.Text = "Tạo Mới";
            datePickerNgayCap.DefaultDisplay = DateTime.Now;
            btnSave.Clicked += CreateContact_Clicked;
            viewModel.BusinessType = viewModel.BusinessTypeOptionList.SingleOrDefault(x => x.Val == "100000000");
        }

        private void CreateContact_Clicked(object sender, EventArgs e)
        {
            SaveData(null);
        }

        private async void Update()
        {
            viewModel.singleAccount = new AccountFormModel();
            this.Title = "Cập Nhật Khách Hàng Doanh Nghiệp";
            btnSave.Text = "Cập Nhật";
            btnSave.Clicked += UpdateContact_Clicked;

            await viewModel.LoadOneAccount(this.AccountId);

            if (viewModel.singleAccount.accountid != Guid.Empty)
            {
                customerCode.IsVisible = true;
                viewModel.CustomerStatusReason = CustomerStatusReasonData.GetCustomerStatusReasonById(viewModel.singleAccount.statuscode);
                viewModel.OperationScope = OperationScopeData.GetOperationScopeById(viewModel.singleAccount.bsd_operationscope);
                viewModel.BusinessType = viewModel.BusinessTypeOptionList.SingleOrDefault(x => x.Val == "100000000");

                if (viewModel.singleAccount != null && viewModel.singleAccount.bsd_localization != null)
                {
                    viewModel.Localization = AccountLocalization.GetLocalizationById(viewModel.singleAccount.bsd_localization);
                }

                if (viewModel.singleAccount != null && viewModel.singleAccount.primarycontactname != null)
                {
                    viewModel.GetPrimaryContactByID();
                }

                viewModel.singleAccount.bsd_address = await SetAddress();

                OnCompleted?.Invoke(true);
            }
                
            else
                OnCompleted?.Invoke(false);
        }

        private async Task<string> SetAddress()
        {
            List<string> listaddress = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.bsd_housenumberstreet))
            {
                listaddress.Add(viewModel.singleAccount.bsd_housenumberstreet);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.district_name))
            {
                listaddress.Add(viewModel.singleAccount.district_name);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.province_name))
            {
                listaddress.Add(viewModel.singleAccount.province_name);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.bsd_postalcode))
            {
                listaddress.Add(viewModel.singleAccount.bsd_postalcode);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.country_name))
            {
                listaddress.Add(viewModel.singleAccount.country_name);
            }

            string address = string.Join(", ", listaddress);

            return address;
        }

        private void UpdateContact_Clicked(object sender, EventArgs e)
        {
            SaveData(this.AccountId.ToString());
        }

        public void SetPreOpen()
        {
            lookUpTinhTrang.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.CustomerStatusReasons = CustomerStatusReasonData.CustomerStatusReasons();
                LoadingHelper.Hide();
            };

            lookUpOperationScope.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.OperationScopes = OperationScopeData.OperationScopes();
                LoadingHelper.Hide();
            };

            Lookup_Localization.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                AccountLocalization.Localizations();
                foreach (var item in AccountLocalization.LocalizationOptions)
                {
                    viewModel.LocalizationOptionList.Add(item);
                }
                LoadingHelper.Hide();
            };         
            //Lookup_BusinessType.PreShow = async () =>
            //{
            //    LoadingHelper.Show();
            //    viewModel.LoadBusinessTypeForLookup();
            //    LoadingHelper.Hide();
            //};            
            Lookup_PrimaryContact.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadContactForLookup();
                LoadingHelper.Hide();
            };
        }

        private async void SaveData(string id)
        {
            if (viewModel.Localization == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn loại khách hàng");
                return;
            }
            if (viewModel.singleAccount.bsd_name == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập tên công ty");
                return;
            }
            if (viewModel.OperationScope == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn operation scope");
                return;
            }

            if (viewModel.PrimaryContact == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn người đại diện");
                return;
            }
            if (viewModel.singleAccount.telephone1 == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số điện thoại công ty");
                return;
            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.emailaddress1))
            {
                Match match = regex.Match(viewModel.singleAccount.emailaddress1);
                if (!match.Success)
                {
                    ToastMessageHelper.ShortMessage("Email sai địng dạng. Vui lòng thử lại");
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleAccount.bsd_email2))
            {
                Match match = regex.Match(viewModel.singleAccount.bsd_email2);
                if (!match.Success)
                {
                    ToastMessageHelper.ShortMessage("Email 2 sai địng dạng. Vui lòng thử lại");
                    return;
                }
            }
            if (viewModel.singleAccount.bsd_registrationcode == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số giấy phép kinh doanh");
                return;
            }
            if (!await viewModel.Check_form_keydata(null, viewModel.singleAccount.bsd_registrationcode, viewModel.singleAccount.accountid.ToString()))
            {
                ToastMessageHelper.ShortMessage("Số giấy phép kinh doanh đã tạo trong dữ liệu doanh nghiệp");
                return;
            }
            if (viewModel.singleAccount.bsd_vatregistrationnumber != null)
            {
                if (!await viewModel.Check_form_keydata(viewModel.singleAccount.bsd_vatregistrationnumber, null, viewModel.singleAccount.accountid.ToString()))
                {
                    ToastMessageHelper.ShortMessage("Mã số thuế đã tạo trong dữ liệu doanh nghiệp");
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(viewModel.singleAccount.bsd_address))
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn địa chỉ liên lạc");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singleAccount.bsd_permanentaddress1))
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn địa chỉ trụ sở chính");
                return;
            }

            LoadingHelper.Show();
            if (viewModel.Localization != null && viewModel.Localization.Val != null)
            {
                viewModel.singleAccount.bsd_localization = viewModel.Localization.Val;
            }
            if (viewModel.PrimaryContact != null && viewModel.PrimaryContact.Id != null)
            {
                viewModel.singleAccount._primarycontactid_value = viewModel.PrimaryContact.Id;
            }
            //if (viewModel.BusinessType != null && viewModel.BusinessType.Count > 0)
            //{
            //    viewModel.singleAccount.bsd_businesstypesys = string.Join(", ", viewModel.BusinessType);
            //}
            if (id == null)
            {
                var created = await viewModel.createAccount();
                if (created)
                {
                    if (QueueForm.NeedToRefresh.HasValue) QueueForm.NeedToRefresh = true;
                    if (CustomerPage.NeedToRefreshAccount.HasValue) CustomerPage.NeedToRefreshAccount = true;
                    ToastMessageHelper.ShortMessage("Tạo khách hàng doanh nghiệp thành công");
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Tạo khách hàng doanh nghiệp thất bại");
                }
            }
            else
            {
                var updated = await viewModel.updateAccount();
                if (updated)
                {
                    if (CustomerPage.NeedToRefreshAccount.HasValue) CustomerPage.NeedToRefreshAccount = true;
                    //if (AccountDetailPage.NeedToRefreshAccount.HasValue) AccountDetailPage.NeedToRefreshAccount = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Cập nhật khách hàng doanh nghiệp thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Cập nhật khách hàng doanh nghiệp thất bại");
                }
            }
        }
    }
}