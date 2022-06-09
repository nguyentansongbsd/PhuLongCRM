using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.Views;
using Xamarin.Forms;

namespace PhuLongCRM
{
    public partial class AppShell : Shell
    {
        public static bool? NeedToRefeshUserInfo = null;
        private string _avartar;
        public string Avartar { get => _avartar; set { _avartar = value; OnPropertyChanged(nameof(Avartar)); } }

        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(nameof(UserName)); } }

        private string _contactName;
        public string ContactName { get => _contactName; set { _contactName = value; OnPropertyChanged(nameof(ContactName)); } }

        private string _verApp;
        public string VerApp { get => _verApp; set { _verApp = value; OnPropertyChanged(nameof(VerApp)); } }

        public AppShell()
        {
            InitializeComponent();
            UserName = UserLogged.User;
            ContactName = string.IsNullOrWhiteSpace(UserLogged.ContactName) ? UserLogged.User : UserLogged.ContactName;
            Avartar = UserLogged.Avartar;
            NeedToRefeshUserInfo = false;
            VerApp = Config.OrgConfig.VerApp;
            this.BindingContext = this;
        }

        public AppShell(bool isLoginByUserCrm)
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            await LoadUserCRM();
            VerApp = Config.OrgConfig.VerApp;
            this.BindingContext = this;
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (NeedToRefeshUserInfo == true)
            {
                LoadingHelper.Show();
                if (Avartar != UserLogged.Avartar)
                {
                    Avartar = UserLogged.Avartar;
                }
                ContactName = UserLogged.ContactName;
                NeedToRefeshUserInfo = false;
                LoadingHelper.Hide();
            }
        }

        private void UserInfor_Tapped(object sender, EventArgs e)
        {
            if (UserLogged.IsLoginByUserCRM)
            {
                ToastMessageHelper.ShortMessage("Tính năng đang phát triển");
                this.FlyoutIsPresented = false;
                return;
            }

            LoadingHelper.Show();
            if (UserLogged.ContactId == Guid.Empty)
            {
                ToastMessageHelper.ShortMessage(Language.chua_co_contact_khong_the_chinh_sua_thong_tin);
                LoadingHelper.Hide();
                return;
            }

            UserInfoPage userInfo = new UserInfoPage();
            userInfo.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(userInfo);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_user);
                }
            };

            //await Shell.Current.Navigation.PushAsync(new UserInfoPage());
            this.FlyoutIsPresented = false;
            //LoadingHelper.Hide();
        }

        private async void Logout_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task LoadUserCRM()
        {
            LoadingHelper.Show();
            var decodedToken = Decoder.DecodeToken(UserLogged.AccessToken);
            JwtModel model = JsonConvert.DeserializeObject<JwtModel>(decodedToken.Payload);
            
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='systemuser'>
                                    <attribute name='fullname' />
                                    <attribute name='systemuserid' />
                                    <attribute name='domainname' />
                                    <order attribute='fullname' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='domainname' operator='eq' value='{model.unique_name}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<UserModel>>("systemusers", fetchXml);
            if (result != null || result.value.Count > 0)
            {
                UserModel user = result.value.SingleOrDefault();
                this.ContactName = user.domainname;
                UserLogged.ContactName = this.UserName = model.name;
                UserLogged.Id = user.systemuserid;

                LoadingHelper.Hide();
            }
            else
            {
                this.ContactName = model.unique_name;
                UserLogged.User = this.UserName = model.name;
                LoadingHelper.Hide();
            }
        }

       
    }
}
