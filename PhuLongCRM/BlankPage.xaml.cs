using Firebase.Database;
using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
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
                Firebase.Database.
                
                var options = FirebaseOptions.Builder()
                            .SetApplicationId("1:998946335721:android:94837bae80edd709fcb978")
                            .SetApiKey("AIzaSyC1p-TwiVz5Tb2Cj8zCN3iy4O-8qfeV1Kg")
                            .SetDatabaseUrl("https://sundihome-bsd.firebaseio.com")
                            .Build();


                var collection = firebaseClient
                        .Child("test").Child("DirectSaleData")
                        .
                        .Subscribe(async (dbevent) =>
                        {

                        });
            }
            catch (Exception ex)
            {

            }

        }


        private void AbsoluteLayout_Tapped(object sender, MR.Gestures.TapEventArgs e)
        {
            if (e != null && e.ViewPosition != null)
            {
                ToastMessageHelper.ShortMessage("vi tri " + e.Touches);
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
