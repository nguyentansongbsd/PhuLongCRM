using System;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class ViewPDFFilePage : ContentPage
    {
        public ViewPDFFilePage(string fileUrl)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                webView.Source = fileUrl;
            }
            else
            {
                //webView.Uri = fileUrl;
            }
        }
    }
}
