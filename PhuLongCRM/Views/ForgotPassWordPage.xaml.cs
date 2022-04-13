using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class ForgotPassWordPage : ContentPage
    {
        public ForgotPassWordPage()
        {
            InitializeComponent();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void ConfirmPhone_Clicked(object sender, EventArgs e)
        {

        }
    }
}
