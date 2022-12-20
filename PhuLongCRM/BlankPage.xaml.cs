using Firebase.Database;
using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Firebase.Database.Query;
using PhuLongCRM.ViewModels;
using System.Globalization;
using System.IO;
using ZXing.Aztec.Internal;
using Telerik.Windows.Documents.Fixed.Model;
using PhuLongCRM.IServices;
using PhuLongCRM.Settings;
using System.ComponentModel;

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            Init();
        }

        public async void Init()
        {
            
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            string a = "5bf7d1ff-a776-ed11-81ac-0022485935f1";
            ContentActionReservationModel data = new ContentActionReservationModel();
            if (false)
            {
                data.Command = "Reservation";
            }
            else
            {
                Paramster paramster = new Paramster();
                paramster.action = "Reservation";
                paramster.name = "opportunity";
                paramster.value = a;
                object[] paramasters = new object[] { paramster };
                data.Command = "ReservationQueue";
                //data.Parameters = paramasters; //"[{\"action\":\"Reservation\",\"name\":\"opportunity\",\"value\":\"{a}\"}]";// JsonConvert.SerializeObject(paramster);
            }

            string content = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine(content);
        }

        private async void SignIn_Event(object sender, EventArgs e)
        {
            try
            {
                var appleSignIn = Xamarin.Forms.DependencyService.Get<IAppleSignInService>();

                var account = await appleSignIn.SignInAsync();
            }
            catch (Exception ex)
            {

            }
            

            //labelText.Text = $"Signed in!\n  Name: {account.Name}\n  Email: {account.Email}\n  UserId: {account.UserId}";
        }
    }
}
