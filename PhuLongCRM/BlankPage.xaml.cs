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
                ToastMessageHelper.ShortMessage("vi tri " + e.ViewPosition.X + "vi tri " + e.ViewPosition.Y);
            }
        }

        private void Start_Clicked(object sender, EventArgs e)
        {
            aTimerIdle.Enabled = false;
            aTimerRun = new System.Timers.Timer();
            aTimerRun.Elapsed += new ElapsedEventHandler(Running);
            aTimerRun.Interval = 100;
            aTimerRun.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (this.StartNumIdle <= 10)
            {
                this.Img = $"Idle-{this.StartNumIdle}.png";
            }
            if (this.StartNumIdle > 10)
            {
                this.StartNumIdle = 1;
            }
            else
            {
                this.StartNumIdle++;
            }
        }

        private void Running(object source, ElapsedEventArgs e)
        {
            if (this.StartNumRun <= 8)
            {
                this.Img = $"Run-{this.StartNumRun}.png";
            }
            if (this.StartNumRun > 8)
            {
                this.StartNumRun = 1;
            }
            else
            {
                this.StartNumRun++;
            }
        }
    }
}
