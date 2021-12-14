using System;
using System.Collections.Generic;
using System.IO;
using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class UserInfoPage : ContentPage
    {
        public UserInfoPageViewModel viewModel;
        public UserInfoPage()
        {
            LoadingHelper.Show();
            InitializeComponent();
            Init();
            
        }

        private async void Init()
        {
            this.BindingContext = viewModel = new UserInfoPageViewModel();
            centerModelPassword.Body.BindingContext = viewModel;
            centerModalContactAddress.Body.BindingContext = viewModel;
            await viewModel.LoadContact();
            SetPreOpen();
            LoadingHelper.Hide();
        }

        private void SetPreOpen()
        {
            lookUpContacAddressCountry.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCountryForLookup();
                LoadingHelper.Hide();
            };
            lookUpContactAddressProvice.PreOpenAsync=async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadProvincesForLookup(viewModel.AddressCountryContact);
                LoadingHelper.Hide();
            };
            lookUpContactAddressDistrict.PreOpenAsync = async () => {
                LoadingHelper.Show();
                await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvinceContact);
                LoadingHelper.Hide();
            };

        }

        private async void ChangePassword_Tapped(object sender, EventArgs e)
        {
            viewModel.OldPassword = null;
            viewModel.NewPassword = null;
            viewModel.ConfirmNewPassword = null;
            await centerModelPassword.Show();
        }

        private void OldPassword_TextChanged(object sender, EventArgs e)
        {
            if (viewModel.OldPassword != null && viewModel.OldPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                viewModel.OldPassword = viewModel.OldPassword.Trim();
            }
        }

        private void NewPassword_TextChanged(object sender, EventArgs e)
        {
            if (viewModel.NewPassword != null && viewModel.NewPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                viewModel.NewPassword = viewModel.NewPassword.Trim();
            }
        }

        private void ConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            if (viewModel.ConfirmNewPassword != null && viewModel.ConfirmNewPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                viewModel.ConfirmNewPassword = viewModel.ConfirmNewPassword.Trim();
            }
        }

        private async void SaveChangedPassword_Clicked(object sender, EventArgs e)
        {
            if (viewModel.OldPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                return;
            }

            if (viewModel.NewPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                return;
            }

            if (viewModel.ConfirmNewPassword.Contains(" "))
            {
                ToastMessageHelper.ShortMessage("Password không được chứa ký tự khoảng trắng");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.OldPassword))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập mật khẩu cũ");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.NewPassword))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập mật khẩu mới");
                return;
            }

            if (viewModel.NewPassword.Length < 6)
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập password nhiều hơn 6 ký tự");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.ConfirmNewPassword))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập xác nhận mật khẩu mới");
                return;
            }

            if (UserLogged.Password != viewModel.OldPassword)
            {
                ToastMessageHelper.ShortMessage("Mật khẩu cũ không đúng");
                return;
            }

            if (viewModel.NewPassword != viewModel.ConfirmNewPassword)
            {
                ToastMessageHelper.ShortMessage("Xác nhận mật khẩu không đúng");
                return;
            }

            if (viewModel.OldPassword == viewModel.NewPassword)
            {
                ToastMessageHelper.LongMessage("Bạn đang sử dụng mật khẩu cũ. Vui lòng nhập lại.");
                return;
            }

            LoadingHelper.Show();
            bool isSuccess = await viewModel.ChangePassword();
            if (isSuccess)
            {
                await centerModelPassword.Hide();
                UserLogged.Password = viewModel.ConfirmNewPassword;
                ToastMessageHelper.ShortMessage("Đổi mật khẩu thành công");
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Đổi mật khẩu thất bại");
            }
        }

        private async void ChangeAddress_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (!string.IsNullOrWhiteSpace(viewModel.ContactModel._bsd_country_value))
            {
                viewModel.AddressCountryContact = new LookUp() { Id = Guid.Parse(viewModel.ContactModel._bsd_country_value), Name = viewModel.ContactModel.bsd_country_label };
            }
            else
            {
                viewModel.AddressCountryContact = null;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.ContactModel._bsd_province_value))
            {
                viewModel.AddressStateProvinceContact = new LookUp() { Id = Guid.Parse(viewModel.ContactModel._bsd_province_value), Name = viewModel.ContactModel.bsd_province_label };
            }
            else
            {
                viewModel.AddressStateProvinceContact = null;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.ContactModel._bsd_district_value))
            {
                viewModel.AddressCityContact = new LookUp() { Id = Guid.Parse(viewModel.ContactModel._bsd_district_value), Name = viewModel.ContactModel.bsd_district_label };
            }
            else
            {
                viewModel.AddressCityContact = null;
            }

            viewModel.AddressLine1Contact = viewModel.ContactModel.bsd_housenumberstreet;
            viewModel.AddressPostalCodeContact = viewModel.ContactModel.bsd_postalcode;
            await centerModalContactAddress.Show();
            LoadingHelper.Hide();
        }

        private async void ContactAddressCountry_Changed(object sender, EventArgs e)
        {
            if (viewModel.list_province_lookup.Count != 0) return;
            await viewModel.LoadProvincesForLookup(viewModel.AddressCountryContact);
        }

        private async void ContactAddressProvince_Changed(object sender, EventArgs e)
        {
            if (viewModel.list_district_lookup.Count != 0) return;
            await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvinceContact);
        }

        private async void ConfirmContactAddress_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.AddressLine1Contact))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            LoadingHelper.Show();
            viewModel.ContactModel.bsd_housenumberstreet = viewModel.AddressLine1Contact;
            viewModel.ContactModel.bsd_postalcode = viewModel.AddressPostalCodeContact;
            viewModel.ContactModel._bsd_country_value = viewModel.AddressCountryContact?.Id.ToString();
            viewModel.ContactModel.bsd_country_label = viewModel.AddressCountryContact?.Name;
            viewModel.ContactModel._bsd_province_value = viewModel.AddressStateProvinceContact?.Id.ToString();
            viewModel.ContactModel.bsd_province_label = viewModel.AddressStateProvinceContact?.Name;
            viewModel.ContactModel._bsd_district_value = viewModel.AddressCityContact?.Id.ToString();
            viewModel.ContactModel.bsd_district_label = viewModel.AddressCityContact?.Name;
            viewModel.SetAddress();
            await centerModalContactAddress.Hide();
            LoadingHelper.Hide();
        }

        private async void SaveUserInfor_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.ContactModel.mobilephone))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số điện thoại");
                return;
            }

            if (viewModel.ContactModel.birthdate == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn ngày sinh");
                return;
            }

            if (viewModel.Gender == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn giới tính");
                return;
            }

            LoadingHelper.Show();

            bool isSuccess = await viewModel.UpdateUserInfor();
            if (isSuccess)
            {
                if (viewModel.ContactModel.bsd_fullname != UserLogged.ContactName)
                {
                    UserLogged.ContactName = viewModel.ContactModel.bsd_fullname;
                }
                if (AppShell.NeedToRefeshUserInfo.HasValue) AppShell.NeedToRefeshUserInfo = true;
                ToastMessageHelper.ShortMessage("Cập nhật thành công");
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Cập nhật thất bại");
            }
        }

        private async void ChangeAvatar_Tapped(object sender, EventArgs e)
        {
            string[] options = new string[] { "Thư viện", "Chụp hình" };
            string asw = await DisplayActionSheet("Tuỳ chọn", "Hủy", null, options);
            if (asw == "Thư viện")
            {
                LoadingHelper.Show();
                await CrossMedia.Current.Initialize();
                PermissionStatus photostatus = await PermissionHelper.RequestPhotosPermission();
                if (photostatus == PermissionStatus.Granted)
                {
                    try
                    {
                        var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() { PhotoSize = PhotoSize.MaxWidthHeight,MaxWidthHeight=600});
                        if (file != null)
                        {
                            viewModel.AvatarArr = System.IO.File.ReadAllBytes(file.Path);
                            string imgBase64 = Convert.ToBase64String(viewModel.AvatarArr);
                            viewModel.Avatar = imgBase64;
                            if (viewModel.Avatar != UserLogged.Avartar)
                            {
                                bool isSuccess = await viewModel.ChangeAvatar();
                                if (isSuccess)
                                {
                                    UserLogged.Avartar = viewModel.Avatar;
                                    if (AppShell.NeedToRefeshUserInfo.HasValue) AppShell.NeedToRefeshUserInfo = true;
                                    ToastMessageHelper.ShortMessage("Đổi hình đại diện thành công");
                                    LoadingHelper.Hide();
                                }
                                else
                                {
                                    LoadingHelper.Hide();
                                    ToastMessageHelper.ShortMessage("Đổi hình đại diện thất bại");
                                }
                            }
                            LoadingHelper.Hide();
                        }
                    }
                    catch(Exception ex)
                    {
                        ToastMessageHelper.LongMessage(ex.Message);
                        LoadingHelper.Hide();
                    }
                }
                LoadingHelper.Hide();
            }
            else if (asw == "Chụp hình")
            {
                LoadingHelper.Show();
                await CrossMedia.Current.Initialize();
                PermissionStatus camerastatus = await PermissionHelper.RequestCameraPermission();
                if (camerastatus == PermissionStatus.Granted)
                {
                    string fileName = $"{Guid.NewGuid()}.jpg";
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Name = fileName,
                        SaveToAlbum = false,
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 600
                    });
                    if (file != null)
                    {
                        viewModel.AvatarArr = System.IO.File.ReadAllBytes(file.Path);
                        viewModel.Avatar = Convert.ToBase64String(viewModel.AvatarArr);
                        if (viewModel.Avatar != UserLogged.Avartar)
                        {
                            bool isSuccess = await viewModel.ChangeAvatar();
                            if (isSuccess)
                            {
                                UserLogged.Avartar = viewModel.Avatar;
                                if (AppShell.NeedToRefeshUserInfo.HasValue) AppShell.NeedToRefeshUserInfo = true;
                                ToastMessageHelper.ShortMessage("Đổi hình đại diện thành công");
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage("Đổi hình đại diện thất bại");
                            }
                        }
                    }
                }
                LoadingHelper.Hide();
            }
        }
    }
}
