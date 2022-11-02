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

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://phulong-aff10-default-rtdb.firebaseio.com/",
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("VhuPY1prumruPs8Vgxuj1P1NIIsqnvzZ8tycOuIK") });



        public OptionSet DiscountList { get; set; }
        public List<OptionSet> DiscountLists { get; set; }
        public bool IsLocked { get; set; }
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            Init();
        }

        private async void Init()
        {
            try
            {
                
            }
            catch (Exception ex)
            {}

        }


        private async void AbsoluteLayout_Tapped(object sender, MR.Gestures.TapEventArgs e)
        {
            string id = "08E25682-23E9-41EB-A1B7-6B166562F8AE";
            GetTokenResponse getTokenResponse = await LoginHelper.getSharePointToken();
            var client = BsdHttpClient.Instance();
            string fileListUrl = $"https://diaocphulong.sharepoint.com/sites/PhuLong/_layouts/15/download.aspx?UniqueId={id}&Translate=false&tempauth={getTokenResponse.access_token}&ApiVersion=2.0";
            var request = new HttpRequestMessage(HttpMethod.Get, fileListUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getTokenResponse.access_token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var a = Convert.ToBase64String(response.Content.ReadAsByteArrayAsync().Result);
                ToastMessageHelper.ShortMessage(a);
            }

        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            firebaseClient.Child("test").Child("DirectSaleData").PostAsync(new test { Name ="NgSong"}) ;
        }
    }
    public class test
    {
        public string Name { get; set; }
    }
}
