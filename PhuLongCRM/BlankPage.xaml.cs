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
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            Init();
        }
        public async void Init()
        {
            webview.Uri = "https://diaocphulong.sharepoint.com/sites/PhuLong-UAT/_layouts/15/download.aspx?UniqueId=ce918e10-82f2-4995-9b98-e91e14fd1880&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvZGlhb2NwaHVsb25nLnNoYXJlcG9pbnQuY29tQDg3YmJkYjA4LTQ4YmEtNGRiZi05YzUzLTkyY2VhZTE2YzM1MyIsImlzcyI6IjAwMDAwMDAzLTAwMDAtMGZmMS1jZTAwLTAwMDAwMDAwMDAwMCIsIm5iZiI6IjE2Njg3NTgzNzEiLCJleHAiOiIxNjY4NzYxOTcxIiwiZW5kcG9pbnR1cmwiOiJkeDVPcHlmblhMNlR3T2hCbWd0OE0xbk9oOUM4dGNiQ0VIQ3l4OE5XSVY0PSIsImVuZHBvaW50dXJsTGVuZ3RoIjoiMTQxIiwiaXNsb29wYmFjayI6IlRydWUiLCJjaWQiOiJOR1ZrTUdNNVltSXRPVEkzTWkwME4yTmxMVGcwWkRrdE5UUXhNelUwWldJMFkyUTQiLCJ2ZXIiOiJoYXNoZWRwcm9vZnRva2VuIiwic2l0ZWlkIjoiTnpSbU9HWmhaRGd0TVROak1TMDBNamhsTFdGa1pHVXRNakk1TkRNMFl6WmhNemRtIiwiYXBwX2Rpc3BsYXluYW1lIjoiQXp1cmUgQXBwIENSTSBCU0QiLCJuYW1laWQiOiJhNzU0NGE1OC1iN2JiLTQ1NTMtOTU0OC1kNTZkMWNmYmVjNTVAODdiYmRiMDgtNDhiYS00ZGJmLTljNTMtOTJjZWFlMTZjMzUzIiwicm9sZXMiOiJhbGxzaXRlcy5yZWFkIGFsbHNpdGVzLndyaXRlIiwidHQiOiIxIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbCwiaXBhZGRyIjoiMjAuMTkwLjE0NC4xNjkifQ.Q2NRVjdPU1ordkdwTzM1d0tnOVdPUXlPTklLZTF0RlpwVXU1VEp2L0tYMD0&ApiVersion=2.0";

            decimal a = 10000000;
            string b = string.Format("{0:#,##}", a);
        }
    }
    public class CustomWebView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri",
                returnType: typeof(string),
                declaringType: typeof(CustomWebView),
                defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

    }
}
