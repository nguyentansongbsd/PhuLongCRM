﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.Resources;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class CustomerPage : ContentPage
    {
        public static bool? NeedToRefreshLead = null;
        public static bool? NeedToRefreshContact = null;
        public static bool? NeedToRefreshAccount = null;
        private LeadsContentView LeadsContentView;
        private ContactsContentview ContactsContentview;
        private AccountsContentView AccountsContentView;
        public CustomerPage()
        {
            LoadingHelper.Show();
            InitializeComponent();
            NeedToRefreshLead = false;
            NeedToRefreshContact = false;
            NeedToRefreshAccount = false;
            Init();
        }
        public async void Init()
        {
            VisualStateManager.GoToState(radBorderLead, "Active");
            VisualStateManager.GoToState(radBorderAccount, "InActive");
            VisualStateManager.GoToState(radBorderContact, "InActive");
            VisualStateManager.GoToState(lblLead, "Active");
            VisualStateManager.GoToState(lblAccount, "InActive");
            VisualStateManager.GoToState(lblContact, "InActive");
            if (LeadsContentView == null)
            {
                LeadsContentView = new LeadsContentView();
            }
            LeadsContentView.OnCompleted = async (IsSuccess) =>
            {
                CustomerContentView.Children.Add(LeadsContentView);
                LoadingHelper.Hide();
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (LeadsContentView != null && NeedToRefreshLead == true)
            {
                LoadingHelper.Show();
                await LeadsContentView.viewModel.LoadOnRefreshCommandAsync();
                NeedToRefreshLead = false;
                LoadingHelper.Hide();
            }

            if (ContactsContentview != null && NeedToRefreshContact == true)
            {
                LoadingHelper.Show();
                await ContactsContentview.viewModel.LoadOnRefreshCommandAsync();
                NeedToRefreshContact = false;
                LoadingHelper.Hide();
            }

            if (AccountsContentView != null && NeedToRefreshAccount == true)
            {
                LoadingHelper.Show();
                await AccountsContentView.viewModel.LoadOnRefreshCommandAsync();
                NeedToRefreshAccount = false;
                LoadingHelper.Hide();
            }
        }

        private void Lead_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderLead, "Active");
            VisualStateManager.GoToState(radBorderAccount, "InActive");
            VisualStateManager.GoToState(radBorderContact, "InActive");
            VisualStateManager.GoToState(lblLead, "Active");
            VisualStateManager.GoToState(lblAccount, "InActive");
            VisualStateManager.GoToState(lblContact, "InActive");
            LeadsContentView.IsVisible = true;
            if (AccountsContentView != null)
            {
                AccountsContentView.IsVisible = false;
            }
            if (ContactsContentview != null)
            {
                ContactsContentview.IsVisible = false;
            }
        }

        private void Account_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderLead, "InActive");
            VisualStateManager.GoToState(radBorderAccount, "Active");
            VisualStateManager.GoToState(radBorderContact, "InActive");
            VisualStateManager.GoToState(lblLead, "InActive");
            VisualStateManager.GoToState(lblAccount, "Active");
            VisualStateManager.GoToState(lblContact, "InActive");
            if (AccountsContentView == null)
            {
                LoadingHelper.Show();
                AccountsContentView = new AccountsContentView();
            }
            AccountsContentView.OnCompleted = (IsSuccess) =>
            {
                CustomerContentView.Children.Add(AccountsContentView);
                LoadingHelper.Hide();
            };
            LeadsContentView.IsVisible = false;
            AccountsContentView.IsVisible = true;
            if (ContactsContentview != null)
            {
                ContactsContentview.IsVisible = false;
            }
        }

        private void Contact_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderLead, "InActive");
            VisualStateManager.GoToState(radBorderAccount, "InActive");
            VisualStateManager.GoToState(radBorderContact, "Active");
            VisualStateManager.GoToState(lblLead, "InActive");
            VisualStateManager.GoToState(lblAccount, "InActive");
            VisualStateManager.GoToState(lblContact, "Active");
            if (ContactsContentview == null)
            {
                LoadingHelper.Show();
                ContactsContentview = new ContactsContentview();
            }
            ContactsContentview.OnCompleted = (IsSuccess) =>
            {
                CustomerContentView.Children.Add(ContactsContentview);
                LoadingHelper.Hide();
            };
            LeadsContentView.IsVisible = false;
            ContactsContentview.IsVisible = true;
            if (AccountsContentView != null)
            {
                AccountsContentView.IsVisible = false;
            }
        }

        private async void NewCustomer_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { Language.khach_hang_tiem_nang_option, Language.khach_hang_ca_nhan_option, Language.khach_hang_doanh_nghiep_option };
            string asw = await DisplayActionSheet(Language.huy, Language.huy, null, options);
            if (asw == Language.khach_hang_tiem_nang_option)
            {
                await Navigation.PushAsync(new LeadForm());
            }
            else if (asw == Language.khach_hang_ca_nhan_option)
            {
                await Navigation.PushAsync(new ContactForm());
            }
            else if (asw == Language.khach_hang_doanh_nghiep_option)
            {
                await Navigation.PushAsync(new AccountForm());
            }
            LoadingHelper.Hide();
        }
    }
}
