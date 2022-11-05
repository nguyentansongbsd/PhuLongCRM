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

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://phulongcrm-590ff-default-rtdb.asia-southeast1.firebasedatabase.app/",
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("6kEMlaIMDuxmqsmPDZR4BO3wshQvh7hJiTp8xaMr") }); //https://phulong-aff10-default-rtdb.firebaseio.com/ VhuPY1prumruPs8Vgxuj1P1NIIsqnvzZ8tycOuIK



        public OptionSet DiscountList { get; set; }
        public List<OptionSet> DiscountLists { get; set; }
        public bool IsLocked { get; set; }

        private string _num;
        public string Num { get=>_num; set { _num = value;OnPropertyChanged(nameof(Num)); } } 

        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            //Init();
        }

        private async void Init()
        {
            try
            {
                var collection = firebaseClient
                    .Child("test").Child("directsale")
                    .AsObservable<ResponseRealtime>()
                    .Subscribe(async (dbevent) =>
                    {
                        if (dbevent.EventType != Firebase.Database.Streaming.FirebaseEventType.Delete && dbevent.Object != null )
                        {
                            
                        }
                    });

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
            //firebaseClient.Child("test").Child("directsale").PostAsync(new UnitStatus { id = Guid.NewGuid(), status ="12"}) ;
        }

        void Button_Clicked_1(System.Object sender, System.EventArgs e)
        {
            
        }
    }
    public class test
    {
        public string id { get; set; }
        public string status { get; set; }
    }
}
