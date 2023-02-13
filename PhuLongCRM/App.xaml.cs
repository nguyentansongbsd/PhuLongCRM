using System;
using System.Globalization;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using PhuLongCRM.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            CultureInfo cultureInfo = new CultureInfo(UserLogged.Language);
            Language.Culture = cultureInfo;
            MainPage = new AppShell();
            Shell.Current.Navigation.PushAsync(new Login(), false);
            //  MainPage = new BlankPage();
            if (UserLogged.Language == "vi")
            {
                DependencyService.Register<IDatetimeService, DatetimeService>();
            }
            else
            {
                DependencyService.Register<IDatetimeService, DatetimeENService>();
            }
        }

        protected override async void OnStart()
        {
            //var appleSignInService = DependencyService.Get<IAppleSignInService>();
            //if (appleSignInService != null)
            //{
            //    userId = await SecureStorage.GetAsync(AppleUserIdKey);
            //    if (appleSignInService.IsAvailable && !string.IsNullOrEmpty(userId))
            //    {

            //        var credentialState = await appleSignInService.GetCredentialStateAsync(userId);

            //        switch (credentialState)
            //        {
            //            case AppleSignInCredentialState.Authorized:
            //                //Normal app workflow...
            //                break;
            //            case AppleSignInCredentialState.NotFound:
            //            case AppleSignInCredentialState.Revoked:
            //                //Logout;
            //                SecureStorage.Remove(AppleUserIdKey);
            //                Preferences.Set(LoggedInKey, false);
            //                MainPage = new BlankPage();
            //                break;
            //        }
            //    }
            //}
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
