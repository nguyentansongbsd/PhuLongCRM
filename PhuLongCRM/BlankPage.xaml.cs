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
        private DateTime? _myDate;
        public DateTime? MyDate { get => _myDate; set { _myDate = value;OnPropertyChanged(nameof(MyDate)); } }
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            MyDate = null;// new DateTime(2022, 06, 18, 22, 00, 00);
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            //this.MyDate = null;
            //this.MyDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 00, 00);
            await DisplayAlert("", MyDate.ToString(), "ok");
        }
    }
}
