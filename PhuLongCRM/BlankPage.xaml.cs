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
using Xamarin.Forms;

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        public OptionSet DiscountList { get; set; }
        public List<OptionSet> DiscountLists { get; set; }
        public bool IsLocked { get; set; }
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            GetImageCMND();
        }
        public async Task GetImageCMND()
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
    }
}
