using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Models;

namespace PhuLongCRM.Helper
{
    public class LoginHelper
    {
        public static async Task<HttpResponseMessage> Login()
        {
            var client = BsdHttpClient.Instance();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/87bbdb08-48ba-4dbf-9c53-92ceae16c353/oauth2/token");//OrgConfig.LinkLogin
            var formContent = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("resource", OrgConfig.Resource),
                        new KeyValuePair<string, string>("client_secret", OrgConfig.ClientSecret),
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", OrgConfig.ClientId),
                        //new KeyValuePair<string, string>("username", OrgConfig.UserName),
                        //new KeyValuePair<string, string>("password", OrgConfig.Password),
                        //new KeyValuePair<string, string>("grant_type", "password"),

                    });
            request.Content = formContent;
            var response = await client.SendAsync(request);
            return response;
        }

        //public static async Task<GetTokenResponse> getSharePointToken()
        //{
        //    var client = BsdHttpClient.Instance();
        //    var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/common/oauth2/token");//" https://login.microsoftonline.com/b8ff1d2e-28ba-44e6-bf5b-c96188196711/oauth2/token"
        //    var formContent = new FormUrlEncodedContent(new[]
        //        {
        //                new KeyValuePair<string, string>("client_id", "2ad88395-b77d-4561-9441-d0e40824f9bc"),
        //                new KeyValuePair<string, string>("username","bsddev@conasi.vn"), // UserLogged.User), sai thông tin login, là user app chứ không phải admin
        //                new KeyValuePair<string, string>("password", "admin123$5"), // UserLogged.Password),
        //                new KeyValuePair<string, string>("grant_type", "password"),
        //                //new KeyValuePair<string, string>("client_id", "bbdc1207-6048-415a-a21c-02a734872571"),
        //                //new KeyValuePair<string, string>("client_secret", "_~~NDM9PVbrSD22Ef-.qRnxioPHcG5xsJ8"),
        //                //new KeyValuePair<string, string>("grant_type", "client_credentials"),
        //                new KeyValuePair<string, string>("resource", OrgConfig.GraphReSource)
        //            });
        //    request.Content = formContent;
        //    var response = await client.SendAsync(request);
        //    var body = await response.Content.ReadAsStringAsync();
        //    GetTokenResponse tokenData = JsonConvert.DeserializeObject<GetTokenResponse>(body);
        //    return tokenData;
        //}
    }
}
