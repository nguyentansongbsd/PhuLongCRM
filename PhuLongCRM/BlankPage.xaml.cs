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
        private DateTime? _date;
        public DateTime? Date { get => _date; set { _date = value;OnPropertyChanged(nameof(Date)); } }
        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            Date = null;// DateTime.Now; ;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {

            await DisplayAlert("", Date.Value.ToString(), "ok");
        }
    }
}
