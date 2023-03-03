
using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class Login : ContentPage
    {
        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(nameof(UserName)); } }
        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }
        private bool _eyePass = false;
        public bool EyePass { get => _eyePass; set { _eyePass = value; OnPropertyChanged(nameof(EyePass)); } }
        private bool _issShowPass = true;
        public bool IsShowPass { get => _issShowPass; set { _issShowPass = value; OnPropertyChanged(nameof(IsShowPass)); } }

        private string _verApp;
        public string VerApp { get => _verApp; set { _verApp = value; OnPropertyChanged(nameof(VerApp)); } }

        public string ImeiNum { get; set; }

        public static bool? LoginSession = null;

        private UserModel _admin;
        public UserModel Admin { get => _admin; set { _admin = value; OnPropertyChanged(nameof(Admin)); } }
        private bool isTimeOut {get;set;}

        public Login()
        {
            InitializeComponent();
            this.BindingContext = this;
            LoginSession = false;
            VerApp = Config.OrgConfig.VerApp;
            if (UserLogged.IsLogged && UserLogged.IsSaveInforUser)
            {
                checkboxRememberAcc.IsChecked = true;
                UserName = UserLogged.User;
                Password = UserLogged.Password;
                SetGridUserName();
                SetGridPassword();
            }
            else
            {
                checkboxRememberAcc.IsChecked = false;
            }

            if (UserLogged.Language == "vi")
            {
                flagVN.BorderColor = Color.FromHex("#2196F3");
                flagEN.BorderColor = Color.FromHex("#eeeeee");
            }

            else if (UserLogged.Language == "en")
            {
                flagVN.BorderColor = Color.FromHex("#eeeeee");
                flagEN.BorderColor = Color.FromHex("#2196F3");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            return true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(UserLogged.IsTimeOut == true)
            {
                if (UserLogged.IsLoginByUserCRM)
                    DependencyService.Get<IClearCookies>().ClearAllCookies();
                await UpdateStateLogin(false);
                TimeOut_Popup.IsVisible = true;
            }    
        }

        private void IsRemember_Tapped(object sender, EventArgs e)
        {
            checkboxRememberAcc.IsChecked = !checkboxRememberAcc.IsChecked;
        }

        private void UserName_Focused(object sender, EventArgs e)
        {
            entryUserName.Placeholder = "";
            SetGridUserName();
        }

        private void UserName_UnFocused(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                Grid.SetRow(lblUserName, 0);
                Grid.SetRow(entryUserName, 0);
                Grid.SetRowSpan(entryUserName, 2);             
                
                entryUserName.Placeholder = Language.ten_dang_nhap;               
            }
        }

        private void SetGridUserName()
        {
            Grid.SetRow(lblUserName, 0);
            Grid.SetRow(entryUserName, 1);
            Grid.SetRowSpan(entryUserName, 1);
        }

        private void Password_Focused(object sender, EventArgs e)
        {
            entryPassword.Placeholder = "";
            SetGridPassword();
            lblEyePass.Margin = new Thickness(0, 0, 0, 0);
        }

        private void Password_UnFocused(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                Grid.SetRow(lblPassword, 1);
                Grid.SetRow(entryPassword, 1);
                Grid.SetRowSpan(entryPassword, 2);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    lblEyePass.Margin = new Thickness(0, 0, 0, -13);
                }
                else
                {
                    lblEyePass.Margin = new Thickness(0, 0, 0, -10);
                }

                EyePass = false;
                entryPassword.Placeholder = Language.mat_khau;
            }
        }

        private void Password_TextChanged(object sender, EventArgs e)
        {
            if (!EyePass)
            {
                EyePass = string.IsNullOrWhiteSpace(Password) ? false : true;
            }
        }

        private void SetGridPassword()
        {
            Grid.SetRow(lblPassword, 0);
            Grid.SetRow(entryPassword, 1);
            Grid.SetRowSpan(entryPassword, 1);
        }

        private void ShowHidePass_Tapped(object sender, EventArgs e)
        {
            IsShowPass = !IsShowPass;
            if(IsShowPass)
            {
                lblEyePass.Text = "\uf070";
            }
            else
            {
                lblEyePass.Text = "\uf06e";
            }    
        }

        private void Flag_Tapped(object sender, EventArgs e)
        {
            string code = (string)((sender as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (code == UserLogged.Language) return;
            LoadingHelper.Show();
            UserLogged.Language = code;
            CultureInfo cultureInfo = new CultureInfo(UserLogged.Language);
            Language.Culture = cultureInfo;
            if (code == "vi")
            {
                flagVN.BorderColor = Color.FromHex("#2196F3");
                flagEN.BorderColor = Color.FromHex("#eeeeee");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
            }
                
            else if (code == "en")
            {
                flagVN.BorderColor = Color.FromHex("#eeeeee");
                flagEN.BorderColor = Color.FromHex("#2196F3");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            }
            ChangedLanguage();
            LoadingHelper.Hide();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                ToastMessageHelper.ShortMessage(Language.tai_khoan_khong_duoc_de_trong_vui_long_kiem_tra_lai_thong_tin);
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ToastMessageHelper.ShortMessage(Language.mat_khau_khong_duong_de_trong_vui_long_kiem_tra_lai_thong_tin);
                return;
            }

            try
            {
                LoadingHelper.Show();
                var response = await LoginHelper.Login();
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    GetTokenResponse tokenData = JsonConvert.DeserializeObject<GetTokenResponse>(body);
                    UserLogged.AccessToken = tokenData.access_token;
                    UserLogged.UserAttribute = "bsd_employee";

                    EmployeeModel employeeModel = await LoginUser();
                    if (employeeModel != null)
                    {
                        if (employeeModel.statuscode != "1")
                        {
                            LoadingHelper.Hide();
                            ToastMessageHelper.ShortMessage(Language.tai_khoan_khong_co_hieu_luc);
                            return;
                        }
                        // lưu để cập nhật và kiểm tra
                        UserLogged.Id = employeeModel.bsd_employeeid;
                        UserLogged.NumberLogin = int.Parse(DecaimalToString(employeeModel.bsd_numberlogin));
                        UserLogged.DateLoginFailed = employeeModel.bsd_logindate.ToString();
                        UserLogged.LoginLimit = employeeModel.bsd_loginlimit;

                        if (!string.IsNullOrWhiteSpace(UserLogged.DateLoginFailed) && (DateTime.Now - DateTime.Parse(UserLogged.DateLoginFailed)).TotalHours <= 24.0 && UserLogged.NumberLogin >= UserLogged.LoginLimit)
                        {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.tai_khoan_cua_ban_da_bi_khoa_vui_long_lien_he_quan_tri_he_thong);
                                return;
                        }
                        else
                        {
                            if ((DateTime.Now - DateTime.Parse(UserLogged.DateLoginFailed)).TotalHours > 24.0 && UserLogged.NumberLogin > 0)
                                UserLogged.NumberLogin = 0;
                        }

                        if (employeeModel.bsd_name != UserName)
                        {
                            LoadingHelper.Hide();
                            ToastMessageHelper.ShortMessage(Language.ten_dang_nhap_hoac_mat_khau_khong_chinh_xac);
                            return;
                        }

                        if (employeeModel.bsd_password != Password)
                        {
                            UserLogged.NumberLogin++;
                            UserLogged.DateLoginFailed = DateTime.Now.ToString();
                            await UpdateNumberLogin();
                            LoadingHelper.Hide();
                            ToastMessageHelper.ShortMessage(Language.ten_dang_nhap_hoac_mat_khau_khong_chinh_xac);
                            return;
                        }

                        ImeiNum = await DependencyService.Get<INumImeiService>().GetImei();
                        string imeiADmin = await LoadImeiAdmin();

                        if (string.IsNullOrWhiteSpace(employeeModel.bsd_imeinumber))
                        {
                            await UpdateImei(employeeModel.bsd_employeeid.ToString()) ;
                        }
                        else if (employeeModel.bsd_imeinumber != ImeiNum && employeeModel.bsd_imeinumber != imeiADmin)
                        {
                            LoadingHelper.Hide();
                            ToastMessageHelper.ShortMessage(Language.tai_khoan_khong_the_dang_nhap_tren_thiet_bi_nay);
                            return;
                        }

                        UserLogged.User = employeeModel.bsd_name;
                        UserLogged.Avartar = employeeModel.bsd_avatar;
                        UserLogged.Password = employeeModel.bsd_password;
                        UserLogged.ContactId = employeeModel.contact_id;
                        UserLogged.ContactName = employeeModel.contact_name;
                        UserLogged.ManagerId = employeeModel.manager_id;
                        UserLogged.ManagerName = employeeModel.manager_name;
                        UserLogged.AgentID = employeeModel.agent_id;
                        UserLogged.AgentName = employeeModel.agent_name;
                        UserLogged.TimeOut = employeeModel.bsd_timeoutminute.HasValue ? employeeModel.bsd_timeoutminute.Value : 0;
                        isTimeOut = employeeModel.bsd_timeoutminute.HasValue ? true : false;
                        UserLogged.IsSaveInforUser = checkboxRememberAcc.IsChecked;
                        UserLogged.IsLogged = true;
                        UserLogged.IsLoginByUserCRM = false;

                        Application.Current.MainPage = new AppShell();
                        UserLogged.NumberLogin = 0;
                        UserLogged.DateLoginFailed = DateTime.Now.ToString();
                        await UpdateNumberLogin(true);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.ten_dang_nhap_hoac_mat_khau_khong_chinh_xac);
                    }
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.loi_ket_noi_dern_server);
                    LoadingHelper.Hide();
                }    
            }
            catch (Exception ex)
            {
                LoadingHelper.Hide();
                await DisplayAlert(Language.thong_bao, $"{Language.loi_ket_noi_dern_server} \n" + ex.Message, Language.dong);
            }
            LoadingHelper.Hide();
        }

        public async Task<EmployeeModel> LoginUser()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='bsd_employee'>
                    <attribute name='bsd_employeeid' />
                    <attribute name='bsd_name' />
                    <attribute name='statuscode' />
                    <attribute name='createdon' />
                    <attribute name='bsd_password' />
                    <attribute name='bsd_imeinumber' />
                    <attribute name='bsd_manager' />
                    <attribute name='bsd_avatar' />
                    <attribute name='bsd_numberlogin' />
                    <attribute name='bsd_loginlimit' />
                    <attribute name='bsd_logindate' />
                    <attribute name='bsd_timeoutminute' />
                    <attribute name='bsd_statelogin' />
                    <order attribute='bsd_name' descending='false' />
                    <filter type='and'>
                      <condition attribute='bsd_name' operator='eq' value='{UserName}' />
                    </filter>
                    <link-entity name='systemuser' from='systemuserid' to='bsd_manager' visible='false' link-type='outer' alias='a_548d21d0fee9eb11bacb002248163181'>
                      <attribute name='fullname' alias='manager_name'/>
                      <attribute name='systemuserid' alias='manager_id' />
                    </link-entity>
                    <link-entity name='contact' from='contactid' to='bsd_contact' visible='false' link-type='outer' alias='a_5b790f4631f4eb1194ef000d3a801090'>
                      <attribute name='contactid' alias='contact_id'/>
                      <attribute name='bsd_fullname' alias='contact_name'/>
                    </link-entity>
                    <link-entity name='account' from='accountid' to='bsd_agents' link-type='outer' alias='aa'>
                        <attribute name='bsd_name' alias='agent_name'/>
                        <attribute name='accountid' alias='agent_id'/>
                    </link-entity>
                  </entity>
                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EmployeeModel>>("bsd_employees", fetchXml);
            if (result == null || result.value.Count == 0)
            {
                return null;
            }
            else
            {
                return result.value.FirstOrDefault();
            }
        }

        public async Task UpdateImei(string employeeId)
        {
            string path = $"/bsd_employees({employeeId})";
            var content = await getContent();
            CrmApiResponse crmApiResponse = await CrmHelper.PatchData(path, content);
            if (!crmApiResponse.IsSuccess)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.khong_cap_nhat_duoc_thong_tin_imei);
                return;
            }
        }

        private async Task<object> getContent()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_imeinumber"] = ImeiNum;
            return data;
        }

        private async void ForgotPassword_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            await Navigation.PushAsync(new ForgotPassWordPage());
            LoadingHelper.Hide();
        }

        private async void LoginUserCRM_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new LoginByUserCRMPage());
        }

        private void ChangedLanguage()
        {
            lblforsales.Text = Language.danh_cho_ban_hang;
            lblUserName.Text = Language.ten_dang_nhap;
            entryUserName.Placeholder = Language.ten_dang_nhap;
            lblPassword.Text = Language.mat_khau;
            entryPassword.Placeholder = Language.mat_khau;
            lbRemember.Text = Language.ghi_nho_dang_nhap;
            lbfogotPassword.Text = Language.quen_mat_khau;
            btnLogin.Text = Language.dang_nhap;
            btnLoginUserCRM.Text = Language.dang_nhap_voi_user_crm;
            Admin_CenterPopup.Title = Language.lien_he;
            admin_name.Text = Language.ho_ten;
            admin_sdt.Text = Language.so_dien_thoai;
            admin_email.Text = Language.email;
            lb_lienhe.Text = Language.lien_he;
        }
        //ghi nhận số lần login khi thành công hoặc thất bại, nếu login thành công cập nhâtj state login
        public async Task UpdateNumberLogin(bool isLogin = false)
        {
            string path = $"/bsd_employees({UserLogged.Id})";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_numberlogin"] = UserLogged.NumberLogin;
            if (isLogin)
                data["bsd_statelogin"] = "Login";
            if (!string.IsNullOrWhiteSpace(UserLogged.DateLoginFailed))
                data["bsd_logindate"] = DateTime.Parse(UserLogged.DateLoginFailed).ToUniversalTime();
            CrmApiResponse crmApiResponse = await CrmHelper.PatchData(path, data);
            if (!crmApiResponse.IsSuccess)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
                return;
            }
        }
        public async Task UpdateStateLogin(bool isLogin)
        {
            string path = $"/bsd_employees({UserLogged.Id})";

            Dictionary<string, object> data = new Dictionary<string, object>();
            if(isLogin)
                data["bsd_statelogin"] = "Login";
            else
                data["bsd_statelogin"] = "Logout";

            CrmApiResponse crmApiResponse = await CrmHelper.PatchData(path, data);
            if (!crmApiResponse.IsSuccess)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
                return;
            }
        }
        private string DecaimalToString(decimal total)
        {
            if (total > 0 && total.ToString().Length > 6)
            {
                var format = total.ToString();
                if(format.Contains(','))
                    return format.Split(',')[0];
                else
                    return format.Split('.')[0];
            }
            else
            {
                return "0";
            }
        }

        private async void LienHe_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (string.IsNullOrWhiteSpace(UserLogged.AccessToken))
            {
                var response = await LoginHelper.Login();
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    GetTokenResponse tokenData = JsonConvert.DeserializeObject<GetTokenResponse>(body);
                    UserLogged.AccessToken = tokenData.access_token;
                }
            }
            if(Admin == null)
            {
                await LoadAdmin();
            }    
            if(Admin != null)
            {
                Admin_CenterPopup.ShowCenterPopup();
            }
            LoadingHelper.Hide();
        }
        public async Task LoadAdmin()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_configureapplications'>
                                    <attribute name='bsd_configureapplicationsid' alias='systemuserid'/>
                                    <attribute name='bsd_phone' alias='mobilephone'/>
                                    <attribute name='bsd_fullname' alias='fullname'/>
                                    <attribute name='bsd_email' alias='internalemailaddress'/>
                                    <filter type='and'>
                                        <condition attribute='bsd_default' operator='eq' value='1' />
                                    </filter>
                                  </entity>
                                </fetch>";
            //< condition attribute = 'fullname' operator= 'eq' value = '# CRM Admin' />

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<UserModel>>("bsd_configureapplicationses", fetchXml);
            if (result != null && result.value.Count > 0)
            {
                Admin = result.value.FirstOrDefault();
            }
        }
        public async Task<string> LoadImeiAdmin()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_configureapplications'>
                                    <attribute name='bsd_imeinumbersystem' alias='Name'/>
                                    <filter type='and'>
                                        <condition attribute='bsd_default' operator='eq' value='1' />
                                    </filter>
                                  </entity>
                                </fetch>";
            //< condition attribute = 'fullname' operator= 'eq' value = '# CRM Admin' />

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LookUp>>("bsd_configureapplicationses", fetchXml);
            if (result != null && result.value.Count > 0)
            {
                return result.value.FirstOrDefault().Name;
            }
            else
                return null;
        }

        private void CloseTimeOut_Clicked(object sender, EventArgs e)
        {
            UserLogged.IsTimeOut = false;
            TimeOut_Popup.IsVisible = false;
        }
    }
}