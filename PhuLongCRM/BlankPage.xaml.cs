using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        //private string _num;
        //public string num { get => _num; set { _num = value; OnPropertyChanged(nameof(num)); } }
        public static string tessst { get; set; }
        public BlankPage()
        {
            InitializeComponent();
            //this.BindingContext = this;
           // test.InputValue = num = "840336021479";
            //zxing.OnScanResult += (result) =>
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    lblResult.Text = result.Text;
            //});


        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            //zxing.IsScanning = false;

            base.OnDisappearing();
        }

        public static async Task<GetTokenResponse> getSharePointToken()
        {
            var client = BsdHttpClient.Instance();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/b8ff1d2e-28ba-44e6-bf5b-c96188196711/oauth2/token");//" https://login.microsoftonline.com/b8ff1d2e-28ba-44e6-bf5b-c96188196711/oauth2/token"
            var formContent = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("client_id", "bbdc1207-6048-415a-a21c-02a734872571"),
                        new KeyValuePair<string, string>("client_secret", "_~~NDM9PVbrSD22Ef-.qRnxioPHcG5xsJ8"),
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("resource", OrgConfig.Resource)
                    });
            request.Content = formContent;
            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            GetTokenResponse tokenData = JsonConvert.DeserializeObject<GetTokenResponse>(body);
            return tokenData;
        }
    }
}
