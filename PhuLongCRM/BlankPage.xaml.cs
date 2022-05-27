using System;
using System.Collections.Generic;

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
    }
}
