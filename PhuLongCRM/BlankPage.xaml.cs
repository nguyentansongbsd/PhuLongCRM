using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
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
        public BlankPage()
        {
            InitializeComponent();
        }

        private void AbsoluteLayout_Tapped(object sender, MR.Gestures.TapEventArgs e)
        {
            if (e != null && e.ViewPosition != null)
            {
                ToastMessageHelper.ShortMessage("vi tri " + e.Touches);
            }
        }
    }
}
