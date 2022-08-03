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
        private DateTime? _myDate;
        public DateTime? MyDate { get => _myDate; set { _myDate = value; OnPropertyChanged(nameof(MyDate)); } }

        private string _img;
        public string Img { get => _img; set { _img = value; OnPropertyChanged(nameof(Img)); } }

        public int StartNumIdle { get; set; } = 1;
        public int StartNumRun { get; set; } = 1;

        private System.Timers.Timer aTimerIdle;
        private System.Timers.Timer aTimerRun;
        private int NumIdle = 10;

        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            //MyDate = null;// new DateTime(2022, 06, 18, 22, 00, 00);
            aTimerIdle = new System.Timers.Timer();
            aTimerIdle.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimerIdle.Interval = 100;
            aTimerIdle.Enabled = true;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            //this.MyDate = null;
            //this.MyDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 00, 00);
            //await DisplayAlert("", MyDate.ToString(), "ok");

            //var task = Task.Run(() => {
            //    System.Diagnostics.Debug.WriteLine("ok okookokokok");
            //});

            //if (task.Wait(TimeSpan.FromSeconds(10)))
            //{
            //    System.Diagnostics.Debug.WriteLine("alkjshdflaksd");
            //}
            //else
            //    throw new Exception("Timed out");

            //for (int i = 0; i < 9; i++)
            //{
            //    Dinosaur.Source = $"Idle-{i+1}.png";
            //    await Task.Delay(100);
            //}

            //await Task.Delay()

            

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
