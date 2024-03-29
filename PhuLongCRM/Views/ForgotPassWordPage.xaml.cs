﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class ForgotPassWordPage : ContentPage
    {
        public static bool? NeedRefreshForm = null;
        public ForgotPassWordPageViewModel viewModel;
        public ForgotPassWordPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ForgotPassWordPageViewModel();
            this.Title = Language.quen_mat_khau_title;
            NeedRefreshForm = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedRefreshForm == true)
            {
                this.Title = Language.mat_khau_moi_title;
                stPhone.IsVisible = false;
                stChangePassWord.IsVisible = true;
            }
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ConfirmPhone_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.UserName))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_ho_ten);
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.Phone))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_so_dien_thoai);
                return;
            }
            if (viewModel.Phone.Split('-')[1].Length != 10)
            {
                ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                return;
            }
            if(viewModel.SendToEmail && string.IsNullOrWhiteSpace(viewModel.Email))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_email);
                return;
            }

            LoadingHelper.Show();
            await viewModel.CheckUserName();

            if (viewModel.Employee == null)
            {
                ToastMessageHelper.Message(Language.tai_khoan_hoac_so_dien_thoai_da_nhap_khong_dung_vui_long_kiem_tra_lai_thong_tin);
                LoadingHelper.Hide();
                return;
            }

            if (viewModel.Employee.contact_phone != viewModel.Phone.Replace("+","").Replace("-",""))
            {
                ToastMessageHelper.Message(Language.tai_khoan_hoac_so_dien_thoai_da_nhap_khong_dung_vui_long_kiem_tra_lai_thong_tin);
                LoadingHelper.Hide();
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.Employee.contact_email))
            {
                ToastMessageHelper.Message(Language.nhan_vien_chua_co_email);
                LoadingHelper.Hide();
                return;
            }

            if (viewModel.Employee.contact_email != viewModel.Email)
            {
                ToastMessageHelper.Message(Language.tai_khoan_hoac_so_dien_thoai_da_nhap_khong_dung_vui_long_kiem_tra_lai_thong_tin);
                LoadingHelper.Hide();
                return;
            }

            try
            {
                if (viewModel.SendToEmail)
                {
                    ConformOTPPage conformOTP = new ConformOTPPage(viewModel.Phone,viewModel.Email,viewModel.SendToEmail);
                    conformOTP.OnCompeleted = async (isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            await Navigation.PushAsync(conformOTP);
                            LoadingHelper.Hide();
                        }
                        else
                        {
                            LoadingHelper.Hide();
                            ToastMessageHelper.Message(Language.loi_ket_noi_dern_server);
                        }
                    };
                }
                else
                {
                    ConformOTPPage conformOTP = new ConformOTPPage(viewModel.Phone);
                    conformOTP.OnCompeleted = async (isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            await Navigation.PushAsync(conformOTP);
                            LoadingHelper.Hide();
                        }
                        else
                        {
                            LoadingHelper.Hide();
                            ToastMessageHelper.Message(Language.loi_ket_noi_dern_server);
                        }
                    };
                }
            }
            catch(FirebaseException ex)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(ex.Message);
            }
        }

        private async void ConfirmChangedPassWord_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.NewPassword))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_mat_khau);
                return;
            }
            if (viewModel.NewPassword.Length < 6)
            {
                ToastMessageHelper.Message(Language.mat_khau_it_nhat_6_ky_tu);
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.ConfirmPassword))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_xac_nhan_mat_khau);
                return;
            }

            if (viewModel.NewPassword.Length > 15)
            {
                ToastMessageHelper.Message(Language.mat_khau_toi_da_15_ky_tu);
                return;
            }

            if (viewModel.ConfirmPassword != viewModel.NewPassword)
            {
                ToastMessageHelper.Message(Language.mat_khau_khong_khop);
                return;
            }

            if (viewModel.ConfirmPassword.Length > 15)
            {
                ToastMessageHelper.Message(Language.mat_khau_toi_da_15_ky_tu);
                return;
            }

            LoadingHelper.Show();
            string path = $"/bsd_employees({viewModel.Employee.bsd_employeeid})";
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_password"] = viewModel.ConfirmPassword;

            var content = data as object;
            CrmApiResponse apiResponse = await CrmHelper.PatchData(path, content);
            if (apiResponse.IsSuccess)
            {
                ToastMessageHelper.Message(Language.doi_mat_khau_thanh_cong);
                await Navigation.PopAsync();
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.da_co_loi_xay_ra_vui_long_thu_lai_sau);
            }
        }

        private void MainEntry_Unfocused_NewPassword(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.NewPassword.Length > 15)
            {
                ToastMessageHelper.Message(Language.mat_khau_toi_da_15_ky_tu);
            }
        }

        private void MainEntry_TextChanged_NewPassword(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.NewPassword != null && viewModel.NewPassword.Contains(" "))
            {
                ToastMessageHelper.Message(Language.mat_khau_khong_duoc_chua_ky_tu_khoan_trang);
                viewModel.NewPassword = viewModel.NewPassword.Trim();
            }
        }

        private void MainEntry_Unfocused_ConfirmPassword(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.ConfirmPassword.Length > 15)
            {
                ToastMessageHelper.Message(Language.mat_khau_toi_da_15_ky_tu);
            }
        }

        private void MainEntry_TextChanged_ConfirmPassword(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.ConfirmPassword != null && viewModel.ConfirmPassword.Contains(" "))
            {
                ToastMessageHelper.Message(Language.mat_khau_khong_duoc_chua_ky_tu_khoan_trang);
                viewModel.ConfirmPassword = viewModel.ConfirmPassword.Trim();
            }
        }
    }
}
