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
        private string _num;
        public string num { get=>_num; set { _num = value; OnPropertyChanged(nameof(num)); } }
        public BlankPage()
        {
            InitializeComponent();
            //this.BindingContext = this;
           // test.InputValue = num = "840336021479";
            GetListId();
        }

        private async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await DisplayAlert("", test.Mask, "ok");
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

        private async void GetListId()
        {
            string projectName = "Bcons Plaza Bình Dương_5C7ACF4A31ACEC119841002248585DEE";
            string token = (await getSharePointToken()).access_token;
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var front_request = new HttpRequestMessage(HttpMethod.Get, OrgConfig.SharePointResource +
                OrgConfig.SharePointSiteName + $"lists(guid'{OrgConfig.SharePointProjectId}')/items?$select=ID,FileRef,FileSystemObjectType&$filter=substringof('{projectName}',FileRef)");
            var front_result = await client.SendAsync(front_request);
            if (front_result.IsSuccessStatusCode)
            {
                
            }
        }
    }
}
