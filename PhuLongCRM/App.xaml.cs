﻿using System;
using PhuLongCRM.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new Login();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
