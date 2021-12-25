using System;
using System.Collections.Generic;

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
            this.BindingContext = this;
            test.InputValue = num = "840336021479";
            
        }

        private async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await DisplayAlert("", test.Mask, "ok");
        }
    }
}
