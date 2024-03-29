﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhuLongCRM.Models;
using PhuLongCRM.Helper;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PhuLongCRM.Resources;
using PhuLongCRM.Controls;
using System.Text.RegularExpressions;
using Xamarin.CommunityToolkit.Extensions;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeadForm : ContentPage
    {
        public Action<bool> CheckSingleLead { get; set; }
        public LeadFormViewModel viewModel;

        public LeadForm()
        {
            InitializeComponent();
            this.Title = Language.tao_moi_khach_hang;
            btn_save_lead.Text = Language.tao_moi_kh_tiem_nang;
            Init();
            
            viewModel.Rating = RatingData.GetRatingById("2");//mac dinh la warm
            viewModel.CustomerGroup = CustomerGroupData.GetCustomerGroupById("100000002"); // mac dinh la "Chua xac dinh"
        }

        public LeadForm(Guid Id)
        {
            InitializeComponent();
            this.Title = Language.cap_nhat_khach_hang;
            btn_save_lead.Text = Language.cap_nhat_kh_tiem_nang;
            Init();
            viewModel.LeadId = Id;
            InitUpdate();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new LeadFormViewModel();
            SetPreOpen();
            lookUpDanhGia.HideClearButton();
            CheckSingleLead?.Invoke(true);
        }

        public async void InitUpdate()
        {
            await viewModel.LoadOneLead();

            if (viewModel.singleLead.leadid != Guid.Empty)
            {
                datePickerNgayCap.ReSetTime();
                customerCode.IsVisible = true;
                lookUpLeadSource.IsEnabled = false;
                //lookUpLeadSource.IsEnabled = false; // Task 973 Cho phep chinh sua nguon kh sai khi tao moi thanh cong

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._campaignid_value))
                    viewModel.Campaign = new OptionSet { Val = viewModel.singleLead._campaignid_value, Label = viewModel.singleLead.campaignid_label };

                viewModel.IndustryCode = LeadIndustryCode.GetIndustryCodeById(viewModel.singleLead.industrycode);
                viewModel.Rating = RatingData.GetRatingById(viewModel.singleLead.leadqualitycode.ToString());
                viewModel.CustomerGroup = CustomerGroupData.GetCustomerGroupById(viewModel.singleLead.bsd_customergroup);

                viewModel.Topic = new OptionSet() { Val = viewModel.singleLead._bsd_topic_value, Label = viewModel.singleLead.bsd_topic_label };

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_typeofidcard))
                {
                    viewModel.TypeIdCard = TypeIdCardData.GetTypeIdCardById(viewModel.singleLead.bsd_typeofidcard);
                    if (viewModel.TypeIdCard?.Val == "100000002")
                        lb_soID.Keyboard = Keyboard.Default;
                    else
                        lb_soID.Keyboard = Keyboard.Numeric;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_area))
                {
                    viewModel.Area = AreaData.GetAreaById(viewModel.singleLead.bsd_area);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.new_gender))
                {
                    viewModel.Gender = viewModel.Genders.SingleOrDefault(x => x.Val == viewModel.singleLead.new_gender);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.leadsourcecode))
                {
                    viewModel.LeadSource = LeadSourcesData.GetLeadSourceById(viewModel.singleLead.leadsourcecode);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._transactioncurrencyid_value))
                {
                    OptionSet currency = new OptionSet()
                    {
                        Val = viewModel.singleLead._transactioncurrencyid_value,
                        Label = viewModel.singleLead.transactioncurrencyid_label
                    };
                    viewModel.SelectedCurrency = currency;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._campaignid_value))
                {
                    OptionSet campaign = new OptionSet()
                    {
                        Val = viewModel.singleLead._campaignid_value,
                        Label = viewModel.singleLead.campaignid_label
                    };
                    viewModel.SelectedCurrency = campaign;
                }
                if (viewModel.singleLead.bsd_hasguardian)
                {
                    viewModel.HasGuardian = new OptionSet("1", Language.co);
                    lookUpHasGuardian_SelectedItemChange(null, null);
                }
                if(viewModel.singleLead.new_birthday.HasValue)
                    birthday.Date = viewModel.singleLead.new_birthday.Value;

                CheckSingleLead?.Invoke(true);
            }

            else
                CheckSingleLead?.Invoke(false);
        }

        public void SetPreOpen()
        {
            lookUpKhuVuc.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.Areas = AreaData.Areas();
                LoadingHelper.Hide();
            };

            lookUpLoaiTheID.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.TypeIdCards = TypeIdCardData.TypeIdCards();
                LoadingHelper.Hide();
            };

            lookUpPhanNhom.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.CustomerGroups = CustomerGroupData.CustomerGroups();
                LoadingHelper.Hide();
            };

            lookUpTieuDe.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadTopics();
                LoadingHelper.Hide();
            };

            lookUpDanhGia.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.Ratings = RatingData.Ratings();
                LoadingHelper.Hide();
            };          

            lookUpLinhVuc.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.IndustryCodes = LeadIndustryCode.LeadIndustryCodeData();
                LoadingHelper.Hide();
            };

            lookUpChienDich.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCampainsForLookup();
                if (viewModel.list_campaign_lookup.Count == 0)
                {
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
                LoadingHelper.Hide();
            };
            lookUpLeadSource.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.LeadSources = LeadSourcesData.GetListSources();
                LoadingHelper.Hide();
            };
            lookUpGuardian.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadContactForLookup();
                LoadingHelper.Hide();
            };
        }

        #region chua dung toi
        //private async Task<String> checkData()
        //{
        //    if (viewModel.singleLead._bsd_topic_value == null || string.IsNullOrWhiteSpace(viewModel.singleLead.fullname) || string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone))
        //    {
        //        return "Vui lòng nhập các trường bắt buộc";
        //    }

        //    if(!PhoneNumberFormatVNHelper.CheckValidate(viewModel.singleLead.mobilephone))
        //    {
        //        return "Số điện thoại sai định dạng";
        //    }

        //    if (viewModel.singleLead.new_birthday != null && (DateTime.Now.Year - DateTime.Parse(viewModel.singleLead.new_birthday.ToString()).Year < 18))
        //    {
        //        return "Khách hàng phải từ 18 tuổi";
        //    }

        //    //Kiem tra trùng tên - số điện thoại, tên - email
        //    await viewModel.Checkdata_identical_lock(viewModel.singleLead.fullname, viewModel.singleLead.mobilephone, viewModel.singleLead.emailaddress1, viewModel.singleLead.leadid);
        //    if (viewModel.single_Leadcheck != null)
        //    {
        //        if (viewModel.singleLead.fullname.Trim() == viewModel.single_Leadcheck.fullname && viewModel.singleLead.mobilephone == viewModel.single_Leadcheck.mobilephone)
        //        {
        //            return "Khách hàng - Số điện thoại đã tồn tại";
        //        }
        //        else if (viewModel.singleLead.fullname.Trim() == viewModel.single_Leadcheck.fullname && viewModel.singleLead.emailaddress1 == viewModel.single_Leadcheck.emailaddress1)
        //        {
        //            return "Khách hàng - Email đã tồn tại";
        //        }
        //    }
        //    return "Sucesses";
        //}

        //private void MyNewDatePicker_DateChanged(object sender, EventArgs e)
        //{
        //    if (viewModel.singleLead.new_birthday != null && (DateTime.Now.Year - DateTime.Parse(viewModel.singleLead.new_birthday.ToString()).Year < 18))
        //    {
        //        Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Lỗi", "Khách hàng phải từ 18 tuổi", "OK");
        //        viewModel.singleLead.new_birthday = null;
        //    }
        //    viewModel.PhongThuy.gioi_tinh = viewModel.singleLead.new_gender != null ? Int32.Parse(viewModel.singleLead.new_gender) : 0;
        //    viewModel.PhongThuy.nam_sinh = viewModel.singleLead.new_birthday.HasValue ? viewModel.singleLead.new_birthday.Value.Year : 0;
        //}
        #endregion

        private async void MainEntry_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid)) return;

            if (viewModel.TypeIdCard?.Val == "100000000" && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 9))// CMND
            {
                ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 12))// CCCD
            {
                ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
            }
            if (viewModel.TypeIdCard?.Val == "100000002" && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 8))// Passport
            {
                ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
            }

            bool isValid = await viewModel.CheckIsValidID(viewModel.singleLead.bsd_identitycardnumberid);
            if (isValid)
            {
                ToastMessageHelper.Message(Language.thong_tin_id_da_ton_tai);
                return;
            }
            if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// CMND
            {
                viewModel.singleLead.bsd_placeofissue = "Công An Thành phố/ Tỉnh";
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// CCCD
            {
                viewModel.singleLead.bsd_placeofissue = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư";
            }
            if (viewModel.TypeIdCard?.Val == "100000002" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// Passport
            {
                viewModel.singleLead.bsd_placeofissue = "Phòng Quản lý xuất nhập cảnh";
            }

        }

        private void TypeIdCard_ItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            if (viewModel.TypeIdCard?.Val == "100000002")
                lb_soID.Keyboard = Keyboard.Default;
            else
                lb_soID.Keyboard = Keyboard.Numeric;

            if (string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid)) return;

            if (viewModel.TypeIdCard == null)
            {
                viewModel.singleLead.bsd_identitycardnumberid = null;
            }

            if (viewModel.TypeIdCard != null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid))
            {
                if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 9))// CMND
                {
                    ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 12))// CCCD
                {
                    ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000002" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 8))// Passport
                {
                    ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// CMND
                {
                    viewModel.singleLead.bsd_placeofissue = "Công An Thành phố/ Tỉnh";
                }
                if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// CCCD
                {
                    viewModel.singleLead.bsd_placeofissue = "Cục Cảnh sát ĐKQL cư trú và DLQG về dân cư";
                }
                if (viewModel.TypeIdCard?.Val == "100000002" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_placeofissue))// Passport
                {
                    viewModel.singleLead.bsd_placeofissue = "Phòng Quản lý xuất nhập cảnh";
                }
            }
        }

        private async void mobilephone_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
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
                else if (!PhoneNumberFormatVNHelper.CheckValidate(phone))
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                }
                else
                {
                    bool isValidPhoneNum = await viewModel.CheckIsValidPhone(phone);
                    if (isValidPhoneNum)
                    {
                        ToastMessageHelper.Message(Language.thong_tin_so_dien_thoai_da_ton_tai);
                    }
                }
            }
        }

        private void telephone1_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
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
                else if (!PhoneNumberFormatVNHelper.CheckValidate(phone) && phone.Length == 10)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                }
            }
        }

        private async void SaveLead_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (viewModel.Topic == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_tieu_de);
                    return;
                }

                if (string.IsNullOrWhiteSpace(viewModel.singleLead.lastname))
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_ho_ten);
                    return;
                }

                if (string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone))
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_so_dien_thoai);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone) && viewModel.singleLead.mobilephone.Length != 14)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone) && !PhoneNumberFormatVNHelper.CheckValidate(viewModel.singleLead.mobilephone))
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.telephone1) && viewModel.singleLead.telephone1.Length != 14 && viewModel.singleLead.telephone1.Length > 4)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_khong_hop_le_gom_10_ky_tu);
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(viewModel.singleLead.telephone1) && !PhoneNumberFormatVNHelper.CheckValidate(viewModel.singleLead.telephone1) && viewModel.singleLead.telephone1.Length == 14)
                {
                    ToastMessageHelper.Message(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                    return;
                }

                if (viewModel.CustomerGroup == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_phan_nhom);
                    return;
                }

                if (viewModel.Area == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_khu_vuc);
                    return;
                }

                if (viewModel.LeadSource == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_nguon_khach_hang);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.emailaddress1) && !ValidEmailHelper.CheckValidEmail(viewModel.singleLead.emailaddress1))
                {
                    ToastMessageHelper.Message(Language.email_sai_dinh_dang_vui_long_thu_lai);
                    return;
                }

                if(viewModel.ForQualify && viewModel.Gender == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_gioi_tinh);
                    return;
                }

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
                    if (viewModel.singleLead.new_birthday != null && CalculateYear(viewModel.singleLead.new_birthday.Value) < 18)
                    {
                        ToastMessageHelper.Message(Language.khach_hang_phai_tu_18_tuoi);
                        return;
                    }
                }
                if (viewModel.singleLead.new_birthday != null && DateTime.Compare((DateTime)viewModel.singleLead.new_birthday, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_sinh_khong_duoc_thuoc_tuong_lai);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.TypeIdCard == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_loai_the_id);
                    return;
                }

                if(viewModel.ForQualify && string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid))
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_id);
                    return;
                }

                if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 9))// CMND
                {
                    ToastMessageHelper.Message(Language.so_cmnd_khong_hop_le_gioi_han_9_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 12))// CCCD
                {
                    ToastMessageHelper.Message(Language.so_cccd_khong_hop_le_gioi_han_12_ky_tu);
                    return;
                }
                if (viewModel.TypeIdCard?.Val == "100000002" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_identitycardnumberid, 8))// Passport
                {
                    ToastMessageHelper.Message(Language.so_ho_chieu_khong_hop_le_gioi_han_8_ky_tu);
                    return;
                }
                if (viewModel.singleLead.bsd_dategrant != null && DateTime.Compare((DateTime)viewModel.singleLead.bsd_dategrant, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_khong_duoc_thuoc_tuong_lai);
                    return;
                }

                if (viewModel.ForQualify && !viewModel.singleLead.bsd_dategrant.HasValue)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_ngay_cap);
                    return;
                }

                if (viewModel.singleLead.bsd_dategrant != null && viewModel.singleLead.new_birthday.HasValue && CalculateYear(viewModel.singleLead.new_birthday.Value, (DateTime)viewModel.singleLead.bsd_dategrant) < 14)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_khong_hop_le);
                    return;
                }

                if (viewModel.ForQualify && string.IsNullOrWhiteSpace(viewModel.singleLead?.bsd_placeofissue))
                {
                    ToastMessageHelper.Message(Language.vui_long_nhap_noi_cap);
                    return;
                }

                if (viewModel.ForQualify && viewModel.Address1 == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_dia_chi_lien_lac);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_registrationcode) && !StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_registrationcode, 10))
                {
                    ToastMessageHelper.Message(Language.so_gpkd_khong_hop_le_gom_10_ky_tu);
                    return;
                }

                //if (viewModel.Address1 == null)
                //{
                //    ToastMessageHelper.ShortMessage(Language.vui_long_chon_dia_chi_lien_lac);
                //    return;
                //}
                //if (viewModel.Address2 == null)
                //{
                //    ToastMessageHelper.ShortMessage(Language.vui_long_chon_dia_chi_thuong_tru);
                //    return;
                //}

                LoadingHelper.Show();

                viewModel.singleLead.industrycode = viewModel.IndustryCode != null ? viewModel.IndustryCode.Val : null;
                viewModel.singleLead._transactioncurrencyid_value = viewModel.SelectedCurrency != null ? viewModel.SelectedCurrency.Val : null;
                viewModel.singleLead._campaignid_value = viewModel.Campaign != null ? viewModel.Campaign.Val : null;

                if (viewModel.LeadId == Guid.Empty)
                {
                    var result = await viewModel.createLead();
                    if (result.IsSuccess)
                    {
                        if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                        if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                        ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                        await Navigation.PopAsync();
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.thong_bao_that_bai);
                    }
                }
                else
                {
                    bool IsSuccess = await viewModel.updateLead();
                    if (IsSuccess)
                    {
                        if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                        if (LeadDetailPage.NeedToRefreshLeadDetail.HasValue) LeadDetailPage.NeedToRefreshLeadDetail = true;
                        await Navigation.PopAsync();
                        ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.thong_bao_that_bai);
                    }
                }
            }catch(Exception ex)
            {

            }
        }

        private void so_gpkd_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_registrationcode)) return;
            if (!StringFormatHelper.CheckValueID(viewModel.singleLead.bsd_registrationcode, 10))
            {
                ToastMessageHelper.Message(Language.so_gpkd_khong_hop_le_gom_10_ky_tu);
                return;
            }
        }

        private async void emailaddress1_text_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleLead.emailaddress1)) return;
            if (!ValidEmailHelper.CheckValidEmail(viewModel.singleLead.emailaddress1))
            {
                ToastMessageHelper.Message(Language.email_sai_dinh_dang_vui_long_thu_lai);
            }
            else
            {
                bool isValiEmail = await viewModel.CheckIsValidEmail(viewModel.singleLead.emailaddress1);
                if (isValiEmail)
                {
                    ToastMessageHelper.Message(Language.thong_tin_email_da_ton_tai);
                }
            }
        }

        private void datePickerNgayCap_Date_Selected(object sender, EventArgs e)
        {
            if (viewModel.singleLead != null && viewModel.singleLead.bsd_dategrant != null)
            {
                if (DateTime.Compare((DateTime)viewModel.singleLead.bsd_dategrant, DateTime.Now) == 1)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_khong_duoc_thuoc_tuong_lai);
                    return;
                }
                if (viewModel.singleLead.bsd_dategrant != null && viewModel.singleLead.new_birthday.HasValue && CalculateYear(viewModel.singleLead.new_birthday.Value, (DateTime)viewModel.singleLead.bsd_dategrant) < 14)
                {
                    ToastMessageHelper.Message(Language.ngay_cap_khong_hop_le);
                }
            }
        }

        private void lookUpHasGuardian_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if(viewModel.HasGuardian != null && viewModel.HasGuardian.Val == "1")
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
            if(viewModel.Guardian != null && viewModel.Guardian.contactid != Guid.Empty)
            {
                if (viewModel.Guardian.birthdate.HasValue && CalculateYear(viewModel.Guardian.birthdate.Value) < 18)
                {
                    ToastMessageHelper.Message(Language.khach_hang_chua_du_dieu_kien_lam_nguoi_bao_ho_vui_long_kiem_tra_lai);
                    return;
                }
            }    
        }
        private int CalculateYear(DateTime dateTime, DateTime? dateTime2 = null)
        {
            int age = 0;
            if(dateTime2 == null)
            {
                dateTime2 = DateTime.Now;
            }
            age = dateTime2.Value.Year - dateTime.Year;
            if (dateTime2.Value.DayOfYear < dateTime.DayOfYear)
                age = age - 1;
            return age;
        }

        private void DatePickerBorder_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                if (viewModel.singleLead != null)
                    viewModel.singleLead.new_birthday = e.NewDate;
            }
            catch(Exception ex)
            {

            }
        }

        void LeadQualify_Clicked(System.Object sender, System.EventArgs e)
        {
            viewModel.ForQualify = !viewModel.ForQualify;
            if(viewModel.ForQualify)
                menu_forqualify.IconImageSource = new FontImageSource() { Glyph = "\uf4fc", FontFamily = "FontAwesomeSolid", Size = 18, Color = Color.White };
            else
                menu_forqualify.IconImageSource = new FontImageSource() { Glyph = "\uf007", FontFamily = "FontAwesomeRegular", Size = 18, Color = Color.White };
        }
    }
}