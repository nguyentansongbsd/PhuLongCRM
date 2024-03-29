﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using PhuLongCRM.ViewModels;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System.Linq;
using PhuLongCRM.Resources;
using PhuLongCRM.Controls;
using System.Net.Http;
using Newtonsoft.Json;
using Plugin.Media;
using System.Net.Http.Headers;
using System.Data;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactForm : ContentPage
    {
        public Action<bool> OnCompleted;      
        private ContactFormViewModel viewModel;
        private Guid Id;

        public ContactForm()
        {
            InitializeComponent();
            this.Id = Guid.Empty;
            Init();
            Create();
        }

        public ContactForm(Guid contactId)
        {
            InitializeComponent();
            this.Id = contactId;
            Init();
            Update();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new ContactFormViewModel();
            viewModel.ContactType = viewModel.ContactTypes.SingleOrDefault(x => x.Val == "100000000");
            SetPreOpen();
        }

        private void Create()
        {
            this.Title = Language.tao_moi_khach_hang;
            btn_save_contact.Text = Language.tao_moi_khach_hang_ca_nhan;
            btn_save_contact.Clicked += CreateContact_Clicked;
            viewModel.CustomerStatusReason = CustomerStatusReasonData.GetCustomerStatusReasonById("1");
            lookUpTinhTrang.IsEnabled = false;
        }

        private void CreateContact_Clicked(object sender, EventArgs e)
        {
            SaveData(null);
        }

        private async void Update()
        {
            await loadData(this.Id.ToString());
            this.Title = Language.cap_nhat_khach_hang;
            btn_save_contact.Text = Language.cap_nhat_khach_hang_ca_nhan;
            btn_save_contact.Clicked += UpdateContact_Clicked;
            lookUpTinhTrang.IsEnabled = false;

            if (viewModel.singleContact.contactid != Guid.Empty)
            {
                datePickerNgayCap.ReSetTime();
                datePickerNgayCapHoChieu.ReSetTime();
                customerCode.IsVisible = true;
                viewModel.CustomerStatusReason = CustomerStatusReasonData.GetCustomerStatusReasonById(viewModel.singleContact.statuscode);
                TypeIdCard_ItemChange(null,null);
                if (viewModel.singleContact.bsd_haveprotector)
                {
                    viewModel.HasGuardian = new OptionSet("1", Language.co);
                    lookUpHasGuardian_SelectedItemChange(null, null);
                }
                OnCompleted?.Invoke(true);
            }
            else
                OnCompleted?.Invoke(false);
        }

        private void UpdateContact_Clicked(object sender, EventArgs e)
        {
            SaveData(this.Id.ToString());
        }

        public async Task loadData(string contactId)
        {
            if (contactId != null)
            {
                await viewModel.LoadOneContact(contactId);
                if (viewModel.singleContact.gendercode != null)
                {
                    viewModel.singleGender = ContactGender.GetGenderById(viewModel.singleContact.gendercode);
                }
                if (viewModel.singleContact.bsd_localization != null)
                {
                    viewModel.singleLocalization = AccountLocalization.GetLocalizationById(viewModel.singleContact.bsd_localization);
                }
                if (viewModel.singleContact._parentcustomerid_value != null)
                {
                    viewModel.Account = new PhuLongCRM.Models.LookUp
                    {
                        Name = viewModel.singleContact.parentcustomerid_label,
                        Id = Guid.Parse(viewModel.singleContact._parentcustomerid_value)
                    };
                }
            }
        }

        private async void SaveData(string id)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_fullname))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_ho_ten);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singleContact.mobilephone))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_so_dien_thoai);               
                return;
            }

            string phone = string.Empty;
            phone = viewModel.singleContact.mobilephone.Contains("-") ? viewModel.singleContact.mobilephone.Split('-')[1] : viewModel.singleContact.mobilephone;

            if (phone.Length != 10)
            {
                ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                return;
            }

            if (!PhoneNumberFormatVNHelper.CheckValidate(phone))
            {
                ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                return;
            }

            if (viewModel.singleGender == null || viewModel.singleGender.Val == null)
            {
                ToastMessageHelper.Message(Language.vui_long_chon_gioi_tinh);
                return;
            }
            if (viewModel.singleLocalization == null)
            {
                ToastMessageHelper.Message(Language.vui_long_chon_quoc_tich);
                return;
            }
            if (viewModel.singleContact.birthdate == null)
            {
                ToastMessageHelper.Message(Language.vui_long_chon_ngay_sinh);
                return;
            }
            //if (CalculateYear(viewModel.singleContact.birthdate.Value) < 18)
            //{
            //    ToastMessageHelper.ShortMessage(Language.khach_hang_phai_tu_18_tuoi);
            //    return;
            //}
            if (viewModel.HasGuardian != null && viewModel.HasGuardian.Val == "1")
            {
                if (viewModel.Guardian == null || viewModel.Guardian.contactid == Guid.Empty)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_nguoi_bao_ho);
                    return;
                }
                else
                {
                    if (viewModel.Guardian.birthdate.HasValue && CalculateYear(viewModel.Guardian.birthdate.Value) < 18)
                    {
                        ToastMessageHelper.Message(Language.khach_hang_chua_du_dieu_kien_lam_nguoi_bao_ho_vui_long_kiem_tra_lai);
                        return;
                    }
                }
            }
            else
            {
                if (viewModel.singleContact.birthdate != null && CalculateYear(viewModel.singleContact.birthdate.Value) < 18)
                {
                    ToastMessageHelper.Message(Language.khach_hang_phai_tu_18_tuoi);
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.emailaddress1))
            {
                if (!ValidEmailHelper.CheckValidEmail(viewModel.singleContact.emailaddress1))
                {
                    ToastMessageHelper.Message(Language.email_sai_dinh_dang);
                    return;
                }

                if (!await viewModel.CheckEmail(viewModel.singleContact.emailaddress1, id))
                {
                    ToastMessageHelper.Message(Language.email_da_duoc_su_dung);
                    return;
                }
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.telephone1) && viewModel.singleContact.telephone1 != "+84")
            {
                string telephone = string.Empty;
                telephone = viewModel.singleContact.telephone1.Contains("-") ? viewModel.singleContact.telephone1.Split('-')[1] : viewModel.singleContact.telephone1;

                if (telephone.Length != 10)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(viewModel.Address1?.lineaddress))
            {
                ToastMessageHelper.Message(Language.vui_long_chon_dia_chi_lien_lac);
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.Address2?.lineaddress))
            {
                ToastMessageHelper.Message(Language.vui_long_chon_dia_chi_thuong_tru);
                return;
            }
            if (viewModel.TypeIdCard != null)
            {
                if (viewModel.TypeIdCard?.Val == "100000000" && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber))// CMND
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_so_cmnd);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000001" && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard))// CCCD
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_so_cccd);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000002" && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport))// Passport
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_so_ho_chieu);
                    return;
                }
            }
            else
            {
                ToastMessageHelper.Message(Language.vui_long_chon_loai_the_id);
                return;
            }
            if (!await viewModel.CheckCCCD(viewModel.singleContact.bsd_identitycard,id))
            {
                ToastMessageHelper.Message(Language.so_cccd_da_duoc_su_dung);
                return;
            }

            if (!await viewModel.CheckCMND(viewModel.singleContact.bsd_identitycardnumber, id))
            {
                ToastMessageHelper.Message(Language.so_cmnd_da_duoc_su_dung);
                viewModel.checkCMND = viewModel.singleContact.bsd_identitycardnumber;
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && !await viewModel.CheckPassport(viewModel.singleContact.bsd_passport, id))
            {
                ToastMessageHelper.Message(Language.so_ho_chieu_da_duoc_su_sung);
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycard, 12))
            {
                ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycardnumber, 9))
            {
                ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_passport, 8))
            {
                ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
                return;
            }

            if (viewModel.singleContact.bsd_identitycarddategrant.HasValue && DateTime.Compare((DateTime)viewModel.singleContact.bsd_identitycarddategrant, DateTime.Now) == 1)
            {
                ToastMessageHelper.Message(Language.ngay_cap_cccd_khong_duoc_thuoc_tuong_lai);
                return;
            }

            if (viewModel.singleContact.bsd_dategrant.HasValue && DateTime.Compare((DateTime)viewModel.singleContact.bsd_dategrant, DateTime.Now) == 1)
            {
                ToastMessageHelper.Message(Language.ngay_cap_cmnd_khong_duoc_thuoc_tuong_lai);
                return;
            }

            if (viewModel.singleContact.bsd_issuedonpassport.HasValue && DateTime.Compare((DateTime)viewModel.singleContact.bsd_issuedonpassport, DateTime.Now) == 1)
            {
                ToastMessageHelper.Message(Language.ngay_cap_ho_chieu_khong_duoc_thuoc_tuong_lai);
                return;
            }


            viewModel.singleContact.bsd_localization = viewModel.singleLocalization != null && viewModel.singleLocalization.Val != null ? viewModel.singleLocalization.Val : null;
            viewModel.singleContact.gendercode = viewModel.singleGender != null && viewModel.singleGender.Val != null ? viewModel.singleGender.Val : null;
            viewModel.singleContact._parentcustomerid_value = viewModel.Account != null && viewModel.Account.Id != null ? viewModel.Account.Id.ToString() : null;

            if (id == null)
            {
                LoadingHelper.Show();               
                var result = await viewModel.createContact(viewModel.singleContact);

                if (result.IsSuccess)
                {
                    if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (QueueForm.NeedToRefresh.HasValue) QueueForm.NeedToRefresh = true;
                    await viewModel.PostCMND();
                    await Navigation.PopAsync();
                    ToastMessageHelper.Message(Language.tao_khach_hang_ca_nhan_thanh_cong);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(result.ErrorResponse.error.message);
                }
            }
            else
            {
                LoadingHelper.Show();               
                var result = await viewModel.updateContact(viewModel.singleContact);

                if (result.IsSuccess)
                {
                    if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                    if (ContactDetailPage.NeedToRefresh.HasValue) ContactDetailPage.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    await viewModel.PostCMND();
                    await Navigation.PopAsync();
                    ToastMessageHelper.Message(Language.cap_nhat_thanh_cong);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(result.ErrorResponse.error.message);
                }
            }
        }

        public void SetPreOpen()
        {
            lookUpTinhTrang.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.CustomerStatusReasons = CustomerStatusReasonData.CustomerStatusReasons();
                LoadingHelper.Hide();
            };

            Lookup_GenderOptions.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                foreach (var item in ContactGender.GenderData())
                {
                    viewModel.GenderOptions.Add(item);
                }
                LoadingHelper.Hide();
            };
            Lookup_LocalizationOptions.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                AccountLocalization.Localizations();
                foreach (var item in AccountLocalization.LocalizationOptions)
                {
                    viewModel.LocalizationOptions.Add(item);
                }
                LoadingHelper.Hide();                
            }; 
           Lookup_Account.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadAccountsLookup();
                LoadingHelper.Hide();
            };
            lookUpLoaiTheID.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.TypeIdCards = TypeIdCardData.TypeIdCards();
                LoadingHelper.Hide();
            };
            lookUpGuardian.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadContactForLookup();
                LoadingHelper.Hide();
            };
        }

        private void Handle_LayoutChanged(object sender, System.EventArgs e)
        {
            var width = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 35) / 2;
            var tmpHeight = width * 2 / 3;
            MatTruocCMND.HeightRequest = tmpHeight;
            MatSauCMND.HeightRequest = tmpHeight;
        }

        public void MatTruocCMND_Tapped(object sender, System.EventArgs e)
        {
            List<OptionSet> menuItem = new List<OptionSet>();
            if (viewModel.singleContact.bsd_mattruoccmnd_source != null)
            {
                menuItem.Add(new OptionSet { Label = Language.xem_anh_mat_truoc_cmnd, Val = "Front" ,Title="Show"});
            }
            menuItem.Add(new OptionSet { Label = Language.chup_anh, Val = "Front", Title = "Take" });
            menuItem.Add(new OptionSet { Label = Language.chon_anh_ty_thu_vien, Val = "Front", Title = "Select" });
            if (viewModel.singleContact.front_id != null)
            {
                menuItem.Add(new OptionSet { Label = Language.xoa, Val = "Front", Title = "Delete" });
            }
            this.showMenuImageCMND(menuItem);
        }

        private void MatSauCMND_Tapped(object sender, System.EventArgs e)
        {
            List<OptionSet> menuItem = new List<OptionSet>();
            if (viewModel.singleContact.bsd_matsaucmnd_source != null)
            {
                menuItem.Add(new OptionSet { Label = Language.xem_anh_mat_sau_cmnd, Val = "Behind", Title = "Show" });
            }
            menuItem.Add(new OptionSet { Label = Language.chup_anh, Val = "Behind", Title = "Take" });
            menuItem.Add(new OptionSet { Label = Language.chon_anh_ty_thu_vien, Val = "Behind", Title = "Select" });
            if (viewModel.singleContact.behind_id != null)
            {
                menuItem.Add(new OptionSet { Label = Language.xoa, Val = "Behind", Title = "Delete" });
            }
            this.showMenuImageCMND(menuItem);
        }

        private void showMenuImageCMND(List<OptionSet> listItem)
        {
            popup_menu_imageCMND.ItemSource = listItem;

            popup_menu_imageCMND.focus();
        }

        async void MenuItem_Tapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var item = e.Item as OptionSet;
            popup_menu_imageCMND.unFocus();

            Stream resultStream;
            byte[] arrByte;
            string base64String;

            switch (item.Title)
            {
                case "Take":

                    PermissionStatus cameraStatus = await PermissionHelper.RequestCameraPermission();
                    if (cameraStatus == PermissionStatus.Granted)
                    {
                        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            SaveToAlbum = false,
                            PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 600,
                        });

                        if (file == null)
                            return;

                        resultStream = file.GetStream();
                        using (var memoryStream = new MemoryStream())
                        {
                            resultStream.CopyTo(memoryStream);
                            arrByte = memoryStream.ToArray();
                        }
                        base64String = Convert.ToBase64String(arrByte);
                        if (item.Val == "Front") { viewModel.singleContact.bsd_mattruoccmnd_source = base64String; viewModel.singleContact.bsd_mattruoccmnd_base64 = base64String; }
                        else if (item.Val == "Behind") { viewModel.singleContact.bsd_matsaucmnd_source = base64String; viewModel.singleContact.bsd_matsaucmnd_base64 = base64String; }
                    }

                    break;
                case "Select":

                    PermissionStatus storageStatus = await PermissionHelper.RequestPhotosPermission();
                    if (storageStatus == PermissionStatus.Granted)
                    {
                        var file2 = await MediaPicker.PickPhotoAsync();
                        if (file2 == null)
                            return;

                        Stream result = await file2.OpenReadAsync();
                        if (result != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                result.CopyTo(memoryStream);
                                arrByte = memoryStream.ToArray();
                            }
                            base64String = Convert.ToBase64String(arrByte);
                            if (item.Val == "Front") { viewModel.singleContact.bsd_mattruoccmnd_source  = base64String; viewModel.singleContact.bsd_mattruoccmnd_base64 = base64String; }
                            else if (item.Val == "Behind") { viewModel.singleContact.bsd_matsaucmnd_source = base64String; viewModel.singleContact.bsd_matsaucmnd_base64 = base64String; }
                        }
                    }
                    break;
                case "Delete":
                    bool asw = await DisplayAlert(Language.xoa, Language.xoa_hinh_anh, Language.dong_y, Language.huy);
                    if (asw == true)
                    {
                        LoadingHelper.Show();
                        if (item.Val == "Front")
                        {
                            var result = await viewModel.DeleteImageCMND(item.Val);
                            if (result)
                            {
                                viewModel.singleContact.bsd_mattruoccmnd_source = null;
                                viewModel.singleContact.bsd_mattruoccmnd_base64 = "";
                                viewModel.singleContact.front_id = null;
                                if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                                if (ContactDetailPage.NeedToRefresh.HasValue) ContactDetailPage.NeedToRefresh = true;
                            }
                        }
                        else if (item.Val == "Behind")
                        {
                            var result = await viewModel.DeleteImageCMND(item.Val);
                            if (result)
                            {
                                viewModel.singleContact.bsd_matsaucmnd_source = null;
                                viewModel.singleContact.bsd_matsaucmnd_base64 = "";
                                viewModel.singleContact.behind_id = null;
                                if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                                if (ContactDetailPage.NeedToRefresh.HasValue) ContactDetailPage.NeedToRefresh = true;
                            }
                        }
                        LoadingHelper.Hide();
                    }
                    break;
                default:
                    if (item.Val == "Front") { image_detailCMNDImage.Source = viewModel.singleContact.bsd_mattruoccmnd_source; }
                    else if (item.Val == "Behind") { image_detailCMNDImage.Source = viewModel.singleContact.bsd_matsaucmnd_source; }
                    this.showDetailCMNDImage();
                    break;
            }
        }

        private void showDetailCMNDImage()
        {

            NavigationPage.SetHasNavigationBar(this, false);
            popup_detailCMNDImage.IsVisible = true;
        }

        void BtnCloseModalImage_Clicked(object sender, System.EventArgs e)
        {
            NavigationPage.SetHasNavigationBar(this, true);
            popup_detailCMNDImage.IsVisible = false;
        }

        private void CMND_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber)) return;
            if (!StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycardnumber, 9))
            {
                ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissue))
            {
                viewModel.singleContact.bsd_placeofissueidentitycard = ""; //can cuoc
                viewModel.singleContact.bsd_placeofissue = "Công An Thành phố/ Tỉnh"; //cmnd
                viewModel.singleContact.bsd_placeofissuepassport = ""; //pp
            }
        }
        private void Phone_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            var num = sender as PhoneEntryControl;
            if (!string.IsNullOrWhiteSpace(num.Text))
            {
                string phone = num.Text;
                phone = phone.Contains("-") ? phone.Split('-')[1] : phone;

                if (phone.Length != 10)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                }
                if (!PhoneNumberFormatVNHelper.CheckValidate(phone))
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                }
            }
        }
        private void CCCD_Unfocused(object sender, FocusEventArgs e)
        {
            if (!StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycard, 12))
            {
                ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
                return;
            }
            else if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissueidentitycard))
            {
                viewModel.singleContact.bsd_placeofissueidentitycard = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư"; //can cuoc
                viewModel.singleContact.bsd_placeofissue = ""; //cmnd
                viewModel.singleContact.bsd_placeofissuepassport = ""; //pp
            }
        }
        private void PassPort_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport)) return;
            if (!StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_passport, 8))
            {
                ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissuepassport))
            {
                viewModel.singleContact.bsd_placeofissueidentitycard = ""; //can cuoc
                viewModel.singleContact.bsd_placeofissue = ""; //cmnd
                viewModel.singleContact.bsd_placeofissuepassport = "Phòng Quản lý xuất nhập cảnh"; //pp
            }
        }

        private void datePickerNgayCapHoChieu_Date_Selected(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null && viewModel.singleContact.bsd_issuedonpassport != null)
            {
                if (DateTime.Compare((DateTime)viewModel.singleContact.bsd_issuedonpassport, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_ho_chieu_khong_duoc_thuoc_tuong_lai);
                }
            }
        }

        private void datePickerNgayCap_Date_Selected(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null && viewModel.singleContact.bsd_dategrant != null)
            {
                if (DateTime.Compare((DateTime)viewModel.singleContact.bsd_dategrant, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_cmnd_khong_duoc_thuoc_tuong_lai);
                }
            }
        }

        private void datePickerNgayCapCCCD_Date_Selected(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null && viewModel.singleContact.bsd_identitycarddategrant != null)
            {
                if (DateTime.Compare((DateTime)viewModel.singleContact.bsd_identitycarddategrant, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_cccd_khong_duoc_thuoc_tuong_lai);
                }
            }
        }
        private void TypeIdCard_ItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport)
                && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber)
                && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard)) return;

            if (viewModel.TypeIdCard == null)
            {
                viewModel.singleContact.bsd_passport = null;
                viewModel.singleContact.bsd_identitycardnumber = null;
                viewModel.singleContact.bsd_identitycard = null;
            }

            if (viewModel.TypeIdCard != null)
            {
                if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycardnumber, 9))// CMND
                {
                    lb_cccd.IsVisible = true;
                    lb_cmnd.IsVisible = false;
                    lb_ho_chieu.IsVisible = true;
                    ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_identitycard, 12))// CCCD
                {
                    lb_cccd.IsVisible = false;
                    lb_cmnd.IsVisible = true;
                    lb_ho_chieu.IsVisible = true;
                    ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000002" && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport) && !StringFormatHelper.CheckValueID(viewModel.singleContact.bsd_passport, 8))// Passport
                {
                    lb_cccd.IsVisible = true;
                    lb_cmnd.IsVisible = true;
                    lb_ho_chieu.IsVisible = false;
                    ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000000")
                {
                    lb_cccd.IsVisible = false;
                    lb_cmnd.IsVisible = true;
                    lb_ho_chieu.IsVisible = false;
                    if(!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissue))
                    {
                        viewModel.singleContact.bsd_placeofissueidentitycard = ""; //can cuoc
                        viewModel.singleContact.bsd_placeofissue = "Công An Thành phố/ Tỉnh"; //cmnd
                        viewModel.singleContact.bsd_placeofissuepassport = ""; //pp
                    }
                }else if (viewModel.TypeIdCard?.Val == "100000001")// CCCD
                {
                    lb_cccd.IsVisible = true;
                    lb_cmnd.IsVisible = false;
                    lb_ho_chieu.IsVisible = false;
                    if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycard) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissueidentitycard))
                    {
                        viewModel.singleContact.bsd_placeofissueidentitycard = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư"; //can cuoc
                        viewModel.singleContact.bsd_placeofissue = ""; //cmnd
                        viewModel.singleContact.bsd_placeofissuepassport = ""; //pp
                    }
                }
                else if (viewModel.TypeIdCard?.Val == "100000002")// Passport
                {
                    lb_cccd.IsVisible = false;
                    lb_cmnd.IsVisible = false;
                    lb_ho_chieu.IsVisible = true;
                    if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport) && string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_placeofissuepassport))
                    {
                        viewModel.singleContact.bsd_placeofissueidentitycard = ""; //can cuoc
                        viewModel.singleContact.bsd_placeofissue = ""; //cmnd
                        viewModel.singleContact.bsd_placeofissuepassport = "Phòng Quản lý xuất nhập cảnh"; //pp
                    }
                }
            }
        }
        private void lookUpHasGuardian_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.HasGuardian != null && viewModel.HasGuardian.Val == "1")
            {
                lbGuardian.IsVisible = true;
                lookUpGuardian.IsVisible = true;
            }
            else
            {
                lbGuardian.IsVisible = false;
                lookUpGuardian.IsVisible = false;
            }
        }

        private void lookUpGuardian_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.Guardian != null && viewModel.Guardian.contactid != Guid.Empty)
            {
                if (viewModel.Guardian.birthdate.HasValue && CalculateYear(viewModel.Guardian.birthdate.Value) < 18)
                {
                    ToastMessageHelper.Message(Language.khach_hang_chua_du_dieu_kien_lam_nguoi_bao_ho_vui_long_kiem_tra_lai);
                    return;
                }
            }
        }
        private int CalculateYear(DateTime dateTime)
        {
            int age = 0;
            age = DateTime.Now.Year - dateTime.Year;
            if (DateTime.Now.DayOfYear < dateTime.DayOfYear)
                age = age - 1;
            return age;
        }
    }
}