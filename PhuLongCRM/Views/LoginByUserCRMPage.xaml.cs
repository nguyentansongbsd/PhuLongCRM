using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class LoginByUserCRMPage : ContentPage
    {
        private string loginUrl = "https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize?client_id={1}&response_type=code&redirect_uri={2}&response_mode=query&scope={3}&state=12345";
        
        public LoginByUserCRMPage()
        {
            LoadingHelper.Show();
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            await Task.Delay(1);
            loginWeb.Source = string.Format(loginUrl,Config.OrgConfig.TeantId,Config.OrgConfig.ClientId_ForUserCRM,Config.OrgConfig.Redirect_Uri,Config.OrgConfig.Scope);
            LoadingHelper.Hide();
        }

        private async void loginWeb_Navigating(System.Object sender, Xamarin.Forms.WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith(Config.OrgConfig.Redirect_Uri))
            {
                LoadingHelper.Show();
                string code = e.Url.Split('?', '&')[1].Split('=')[1];
                var response = await LoginHelper.LoginByUserCRM(code);
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    GetTokenResponse tokenData = JsonConvert.DeserializeObject<GetTokenResponse>(body);
                    UserLogged.AccessToken = tokenData.access_token;
                    UserLogged.IsLoginByUserCRM = true;
                    UserLogged.IsLogged = true;

                    Application.Current.MainPage = new AppShell();
                    await Task.Delay(1);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Lỗi đăng nhập");
                }
            }
        }
    }
}
