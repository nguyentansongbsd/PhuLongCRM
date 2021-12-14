using System;
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
            centerModalContacAddress.Body.BindingContext = viewModel;
            centerModalPermanentAddress.Body.BindingContext = viewModel;
            viewModel.ContactType = viewModel.ContactTypes.SingleOrDefault(x => x.Val == "100000000");
            SetPreOpen();
        }

        private void Create()
        {
            this.Title = "Tạo Mới Khách Hàng Cá Nhân";
            btn_save_contact.Text = "Tạo Mới";
            datePickerNgayCap.DefaultDisplay = DateTime.Now;
            datePickerNgayCapHoChieu.DefaultDisplay = DateTime.Now;
            datePikerNgayCapTheCanCuoc.DefaultDisplay = DateTime.Now;
            btn_save_contact.Clicked += CreateContact_Clicked;
        }

        private void CreateContact_Clicked(object sender, EventArgs e)
        {
            SaveData(null);
        }

        private async void Update()
        {
            await loadData(this.Id.ToString());
            this.Title = "Cập Nhật Khách Hàng Cá Nhân";
            btn_save_contact.Text = "Cập Nhật";
            btn_save_contact.Clicked += UpdateContact_Clicked;
            if (viewModel.singleContact.contactid != Guid.Empty)
            {
                customerCode.IsVisible = true;
                viewModel.CustomerStatusReason = CustomerStatusReasonData.GetCustomerStatusReasonById(viewModel.singleContact.statuscode);
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
            LoadingHelper.Show();

            if (contactId != null)
            {
                await viewModel.LoadOneContact(contactId);
                //await viewModel.GetImageCMND();
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
            LoadingHelper.Hide();
        }

        private async void SaveData(string id)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_fullname))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập họ tên");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.mobilephone))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số điện thoại");               
                return;
            }
            if (viewModel.singleGender == null || viewModel.singleGender.Val == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn giới tính");
                return;
            }
            if (viewModel.singleLocalization == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn quốc tịch");
                return;
            }
            if (viewModel.singleContact.birthdate == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn ngày sinh");
                return;
            }
            if (DateTime.Now.Year - DateTime.Parse(viewModel.singleContact.birthdate.ToString()).Year < 18)
            {
                ToastMessageHelper.ShortMessage("Khách hàng phải từ 18 tuổi");
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.emailaddress1))
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(viewModel.singleContact.emailaddress1);
                if (!match.Success)
                {
                    ToastMessageHelper.ShortMessage("Email sai định dạng");
                    return;
                }

                if (!await viewModel.CheckEmail(viewModel.singleContact.emailaddress1, id))
                {
                    ToastMessageHelper.ShortMessage("Email đã được sử dụng");
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_contactaddress))
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn địa chỉ liên lạc");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_permanentaddress1))
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn địa chỉ thường trú");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số CMND");
                return;
            }
            if (!await viewModel.CheckCMND(viewModel.singleContact.bsd_identitycardnumber, id))
            {
                ToastMessageHelper.ShortMessage("Số CMNND đã được sử dụng");
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && !await viewModel.CheckPassport(viewModel.singleContact.bsd_passport, id))
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu đã được sử dụng");
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_identitycardnumber) && viewModel.singleContact.bsd_identitycardnumber.Length > 9)
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Giới hạn 09 ký tự).");
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_idcard) && viewModel.singleContact.bsd_idcard.Length > 12)
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Giới hạn 12 ký tự).");
                return;
            }
            if (!string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_passport) && viewModel.singleContact.bsd_passport.Length > 8)
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Giới hạn 08 ký tự).");
                return;
            }


            viewModel.singleContact.bsd_localization = viewModel.singleLocalization != null && viewModel.singleLocalization.Val != null ? viewModel.singleLocalization.Val : null;
            viewModel.singleContact.gendercode = viewModel.singleGender != null && viewModel.singleGender.Val != null ? viewModel.singleGender.Val : null;
            viewModel.singleContact._parentcustomerid_value = viewModel.Account != null && viewModel.Account.Id != null ? viewModel.Account.Id.ToString() : null;

            if (id == null)
            {
                LoadingHelper.Show();               
                var created = await viewModel.createContact(viewModel.singleContact);

                if (created != new Guid())
                {
                    if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                    //if (QueueForm.NeedToRefreshContactList.HasValue) QueueForm.NeedToRefreshContactList = true;

                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Đã tạo khách hàng cá nhân thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Tạo khách hàng cá nhân thất bại");
                }
            }
            else
            {
                LoadingHelper.Show();               
                var updated = await viewModel.updateContact(viewModel.singleContact);

                if (updated)
                {
                    LoadingHelper.Hide();
                    if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
                    //if (ContactDetailPage.NeedToRefresh.HasValue) ContactDetailPage.NeedToRefresh = true;

                    //if (viewModel.singleContact.bsd_mattruoccmnd_base64 != null)
                    //{
                    //    await viewModel.UpLoadCMNDFront();
                    //}

                    //if (viewModel.singleContact.bsd_matsaucmnd_base64 != null)
                    //{
                    //   await viewModel.UpLoadCMNDBehind();
                    //}
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Đã cập nhật thành công");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Cập nhật thất bại");
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
                ContactGender.GetGenders();
                foreach (var item in ContactGender.GenderOptions)
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
            lookUpContacAddressCountry.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                if(viewModel.list_country_lookup.Count == 0)
                {
                    await viewModel.LoadCountryForLookup();
                }
                LoadingHelper.Hide();
            };
            lookUpPermanentAddressCountry.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                if (viewModel.list_country_lookup.Count == 0)
                {
                    await viewModel.LoadCountryForLookup();
                }
                LoadingHelper.Hide();
            };
            Lookup_Account.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadAccountsLookup();
                LoadingHelper.Hide();
            };
        }

        private async void DiaChiLienLac_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.AddressLine1Contact == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_housenumberstreet))
            {
                viewModel.AddressLine1Contact = viewModel.singleContact.bsd_housenumberstreet;
            }

            if (viewModel.AddressCountryContac == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_country_label))
            {
                viewModel.AddressCountryContac = await viewModel.LoadCountryByName(viewModel.singleContact.bsd_country_label);
                await viewModel.LoadProvincesForLookup(viewModel.AddressCountryContac);
            }

            if (viewModel.AddressStateProvinceContac == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_province_label))
            {
                viewModel.AddressStateProvinceContac = await viewModel.LoadProvinceByName(viewModel.singleContact._bsd_country_value, viewModel.singleContact.bsd_province_label); ;
                await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvinceContac);
            }

            if (viewModel.AddressCityContac == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_district_label))
            {
                viewModel.AddressCityContac = await viewModel.LoadDistrictByName(viewModel.singleContact._bsd_province_value, viewModel.singleContact.bsd_district_label);
            }

            LoadingHelper.Hide();
            await centerModalContacAddress.Show();
        }

        private async void DiaChiThuongTru_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.AddressLine1Permanent == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_permanentaddress))
            {
                viewModel.AddressLine1Permanent = viewModel.singleContact.bsd_permanentaddress;
            }

            if (viewModel.AddressCountryPermanent == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_permanentcountry_label))
            {
                viewModel.AddressCountryPermanent = await viewModel.LoadCountryByName(viewModel.singleContact.bsd_permanentcountry_label);
                await viewModel.LoadProvincesForLookup(viewModel.AddressCountryPermanent);
            }

            if (viewModel.AddressStateProvincePermanent == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_permanentprovince_label))
            {
                viewModel.AddressStateProvincePermanent = await viewModel.LoadProvinceByName(viewModel.singleContact._bsd_permanentcountry_value, viewModel.singleContact.bsd_permanentprovince_label); ;
                await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvincePermanent);
            }

            if (viewModel.AddressCityPermanent == null && !string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_permanentdistrict_label))
            {
                viewModel.AddressCityPermanent = await viewModel.LoadDistrictByName(viewModel.singleContact._bsd_permanentprovince_value, viewModel.singleContact.bsd_permanentdistrict_label);
            }

            LoadingHelper.Hide();
            await centerModalPermanentAddress.Show();
        }

        private void Handle_LayoutChanged(object sender, System.EventArgs e)
        {
            var width = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 35) / 2;
            var tmpHeight = width * 2 / 3;
            MatTruocCMND.HeightRequest = tmpHeight;
            MatSauCMND.HeightRequest = tmpHeight;
        }     

        private async void ContacAddressCountry_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadProvincesForLookup(viewModel.AddressCountryContac);
        }

        private async void ContacAddressProvince_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvinceContac);
        }

        private async void CloseContacAddress_Clicked(object sender, EventArgs e)
        {
            await centerModalContacAddress.Hide();
        }

        private async void ConfirmContacAddress_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.AddressLine1Contact))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.AddressLine1Contact))
            {
                viewModel.singleContact.bsd_housenumberstreet = viewModel.AddressLine1Contact;
                address.Add(viewModel.AddressLine1Contact);
            }
            else
            {
                viewModel.singleContact.bsd_housenumberstreet = null;
            }
            if (viewModel.AddressCityContac != null)
            {
                viewModel.singleContact.bsd_district_label = viewModel.AddressCityContac.Name;
                viewModel.singleContact._bsd_district_value = viewModel.AddressCityContac.Id.ToString();
                address.Add(viewModel.AddressCityContac.Name);
            }
            else
            {
                viewModel.singleContact.bsd_district_label = null;
                viewModel.singleContact._bsd_district_value = null;
            }
            if (viewModel.AddressStateProvinceContac != null)
            {
                viewModel.singleContact.bsd_province_label = viewModel.AddressStateProvinceContac.Name;
                viewModel.singleContact._bsd_province_value = viewModel.AddressStateProvinceContac.Id.ToString();
                address.Add(viewModel.AddressStateProvinceContac.Name);
            }
            else
            {
                viewModel.singleContact.bsd_province_label = null;
                viewModel.singleContact._bsd_province_value = null;
            }

            if (viewModel.AddressCountryContac != null)
            {
                viewModel.singleContact.bsd_country_label = viewModel.AddressCountryContac.Name;
                viewModel.singleContact._bsd_country_value = viewModel.AddressCountryContac.Id.ToString();
                address.Add(viewModel.AddressCountryContac.Name);
            }
            else
            {
                viewModel.singleContact.bsd_country_label = null;
                viewModel.singleContact._bsd_country_value = null;
            }
            viewModel.singleContact.bsd_contactaddress = viewModel.AddressCompositeContac = string.Join(", ", address);

            //Address En
            List<string> addressEn = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.AddressLine1Contact))
            {
                addressEn.Add(viewModel.AddressLine1Contact);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressCityContac?.Detail))
            {
                addressEn.Add(viewModel.AddressCityContac.Detail);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressStateProvinceContac?.Detail))
            {
                addressEn.Add(viewModel.AddressStateProvinceContac.Detail);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressCountryContac?.Detail))
            {
                addressEn.Add(viewModel.AddressCountryContac.Detail);
            }
            viewModel.singleContact.bsd_diachi = string.Join(", ", addressEn);

            await centerModalContacAddress.Hide();
        }

        private async void PermanentAddressCountry_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadProvincesForLookup(viewModel.AddressCountryPermanent);
        }

        private async void PermanentAddressProvince_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadDistrictForLookup(viewModel.AddressStateProvincePermanent);
        }       

        private async void ClosePermanentAddress_Clicked(object sender, EventArgs e)
        {
            await centerModalPermanentAddress.Hide();
        }

        private async void ConfirmPermanentAddress_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.AddressLine1Permanent))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.AddressLine1Permanent))
            {
                viewModel.singleContact.bsd_permanentaddress = viewModel.AddressLine1Permanent;
                address.Add(viewModel.AddressLine1Permanent);
            }
            else
            {
                viewModel.singleContact.bsd_permanentaddress = null;
            }

            if (viewModel.AddressCityPermanent != null)
            {
                viewModel.singleContact.bsd_permanentdistrict_label = viewModel.AddressCityPermanent.Name;
                viewModel.singleContact._bsd_permanentdistrict_value = viewModel.AddressCityPermanent.Id.ToString();
                address.Add(viewModel.AddressCityPermanent.Name);
            }
            else
            {
                viewModel.singleContact.bsd_permanentdistrict_label = null;
                viewModel.singleContact._bsd_permanentdistrict_value = null;
            }
            if (viewModel.AddressStateProvincePermanent != null)
            {
                viewModel.singleContact.bsd_permanentprovince_label = viewModel.AddressStateProvincePermanent.Name;
                viewModel.singleContact._bsd_permanentprovince_value = viewModel.AddressStateProvincePermanent.Id.ToString();
                address.Add(viewModel.AddressStateProvincePermanent.Name);
            }
            else
            {
                viewModel.singleContact.bsd_permanentprovince_label = null;
                viewModel.singleContact._bsd_permanentprovince_value = null;
            }
            if (viewModel.AddressCountryPermanent != null)
            {
                viewModel.singleContact.bsd_permanentcountry_label = viewModel.AddressCountryPermanent.Name;
                viewModel.singleContact._bsd_permanentcountry_value = viewModel.AddressCountryPermanent.Id.ToString();
                address.Add(viewModel.AddressCountryPermanent.Name);
            }
            else
            {
                viewModel.singleContact.bsd_permanentcountry_label = null;
                viewModel.singleContact._bsd_permanentcountry_value = null;
            }
            viewModel.singleContact.bsd_permanentaddress1 = viewModel.AddressCompositePermanent = string.Join(", ", address);

            //addres EN
            List<string> addressPermanentEn = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.AddressLine1Permanent))
            {
                addressPermanentEn.Add(viewModel.AddressLine1Permanent);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressCityPermanent?.Detail))
            {
                addressPermanentEn.Add(viewModel.AddressCityPermanent.Detail);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressStateProvincePermanent?.Detail))
            {
                addressPermanentEn.Add(viewModel.AddressStateProvincePermanent.Detail);
            }
            if (!string.IsNullOrWhiteSpace(viewModel.AddressCountryPermanent?.Detail))
            {
                addressPermanentEn.Add(viewModel.AddressCountryPermanent.Detail);
            }
            
            viewModel.singleContact.bsd_diachithuongtru = string.Join(", ", addressPermanentEn);

            await centerModalPermanentAddress.Hide();
        }

        public void MatTruocCMND_Tapped(object sender, System.EventArgs e)
        {
            List<OptionSet> menuItem = new List<OptionSet>();
            if (viewModel.singleContact.bsd_mattruoccmnd_base64 != null)
            {
                menuItem.Add(new OptionSet { Label = "Xem ảnh mặt trước cmnd", Val = "Front" });
            }
            menuItem.Add(new OptionSet { Label = "Chụp ảnh", Val = "Front" });
            menuItem.Add(new OptionSet { Label = "Chọn ảnh từ thư viện", Val = "Front" });
            this.showMenuImageCMND(menuItem);
        }

        private void MatSauCMND_Tapped(object sender, System.EventArgs e)
        {
            List<OptionSet> menuItem = new List<OptionSet>();
            if (viewModel.singleContact.bsd_matsaucmnd_base64 != null)
            {
                menuItem.Add(new OptionSet { Label = "Xem ảnh mặt sau cmnd", Val = "Behind" });
            }
            menuItem.Add(new OptionSet { Label = "Chụp ảnh", Val = "Behind" });
            menuItem.Add(new OptionSet { Label = "Chọn ảnh từ thư viện", Val = "Behind" });
            this.showMenuImageCMND(menuItem);
        }

        private void showMenuImageCMND(List<OptionSet> listItem)
        {
            popup_menu_imageCMND.ItemSource = listItem;

            popup_menu_imageCMND.focus();
        }

        async void MenuItem_Tapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            //var item = e.Item as OptionSet;
            //popup_menu_imageCMND.unFocus();

            //Stream resultStream;
            //byte[] arrByte;
            //string base64String;

            //switch (item.Label)
            //{
            //    case "Chụp ảnh":
                    
            //        PermissionStatus cameraStatus = await PermissionHelper.RequestCameraPermission();
            //        if (cameraStatus == PermissionStatus.Granted)
            //        { 
            //            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //            {
            //                SaveToAlbum = false,
            //                PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
            //                MaxWidthHeight = 600,
            //            });

            //            if (file == null)
            //                return;

            //            resultStream = file.GetStream();
            //            using (var memoryStream = new MemoryStream())
            //            {
            //                resultStream.CopyTo(memoryStream);
            //                arrByte = memoryStream.ToArray();
            //            }
            //            base64String = Convert.ToBase64String(arrByte);
            //            var tmp1 = base64String.Length;
            //            if (item.Val == "Front") { viewModel.singleContact.bsd_mattruoccmnd_base64 = base64String; }
            //            else if (item.Val == "Behind") { viewModel.singleContact.bsd_matsaucmnd_base64 = base64String; }
            //        }

            //        break;
            //    case "Chọn ảnh từ thư viện":

            //        PermissionStatus storageStatus = await PermissionHelper.RequestPhotosPermission();
            //        if (storageStatus == PermissionStatus.Granted)
            //        {
            //            var file2 = await MediaPicker.PickPhotoAsync();
            //            if (file2 == null)
            //                return;

            //            Stream result = await file2.OpenReadAsync();
            //            if (result != null)
            //            {
            //                using (var memoryStream = new MemoryStream())
            //                {
            //                    result.CopyTo(memoryStream);
            //                    arrByte = memoryStream.ToArray();
            //                }
            //                base64String = Convert.ToBase64String(arrByte);
            //                var tmp = base64String.Length;
            //                if (item.Val == "Front") { viewModel.singleContact.bsd_mattruoccmnd_base64 = base64String; }
            //                else if (item.Val == "Behind") { viewModel.singleContact.bsd_matsaucmnd_base64 = base64String; }
            //            }
            //        }
            //        break;
            //    default:
            //        if (item.Val == "Front") { image_detailCMNDImage.Source = viewModel.singleContact.bsd_mattruoccmnd_source; }
            //        else if (item.Val == "Behind") { image_detailCMNDImage.Source = viewModel.singleContact.bsd_matsaucmnd_source; }
            //        this.showDetailCMNDImage();
            //        break;

            //}
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

        private void CMND_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.singleContact.bsd_identitycardnumber.Length > 9)
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Giới hạn 09 ký tự).");
            }
        }

        private void PassPort_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.singleContact.bsd_passport.Length > 8)
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Giới hạn 08 ký tự).");
            }
        }

        private void CCCD_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.singleContact.bsd_idcard.Length > 12)
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Giới hạn 12 ký tự).");
            }
        }
    }
}