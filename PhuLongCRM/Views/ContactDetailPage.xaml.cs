using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Stormlion.PhotoBrowser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDetailPage : ContentPage
    {
        public Action<int> OnCompleted;
        public static bool? NeedToRefresh = null;
        public static bool? NeedToRefreshQueues = null;
        public static bool? NeedToRefreshActivity = null;
        public static OptionSet FromCustomer = null;
        private ContactDetailPageViewModel viewModel;
        private Guid Id;
        public ContactDetailPage(Guid contactId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ContactDetailPageViewModel();
            NeedToRefresh = false;
            NeedToRefreshActivity = false;
            Id = contactId;
            Init();
        }
        public async void Init()
        {
            await LoadDataThongTin(Id.ToString());
            
            if (viewModel.singleContact.contactid != Guid.Empty)
            {
                await viewModel.LoadProvince();
                await viewModel.LoadProject();
                SetButtonFloatingButton();
                btn_nhaucaudientich.IsVisible = false;
                btn_tieuchichonmua.IsVisible = false;
                btn_loaibdsquantam.IsVisible = false;
                FromCustomer = new OptionSet { Val= viewModel.singleContact.contactid.ToString(), Label= viewModel.singleContact.bsd_fullname, Title= viewModel.CodeContac };
                if(viewModel.singleContact.employee_id == UserLogged.Id)
                    OnCompleted?.Invoke(1);// thanh cong
                else
                    OnCompleted?.Invoke(2);// KH khong thuoc employee dang dang nhap
            }
            else
                OnCompleted?.Invoke(3);// khong tim thay KH
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();
                viewModel.singleContact = new ContactFormModel();
                await LoadDataThongTin(this.Id.ToString());
                viewModel.PhongThuy = null;
                LoadDataPhongThuy();
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
            if (NeedToRefreshQueues == true)
            {
                LoadingHelper.Show();
                viewModel.PageDanhSachDatCho = 1;
                viewModel.list_danhsachdatcho.Clear();
                await viewModel.LoadQueuesForContactForm(viewModel.singleContact.contactid.ToString());
                NeedToRefreshQueues = false;
                LoadingHelper.Hide();
            }
            if (NeedToRefreshActivity == true && viewModel.Cares != null)
            {
                LoadingHelper.Show();
                viewModel.PageCase = 1;
                viewModel.Cares.Clear();
                await viewModel.LoadCase();
                ActivityPopup.Refresh();
                NeedToRefreshActivity = false;
                LoadingHelper.Hide();
            }
        }

        private void SetButtonFloatingButton()
        {
            if (viewModel.singleContact.contactid != Guid.Empty)
            {
                if (viewModel.IsCurrentRecordOfUser) // bat loi khi quet QR: Record co thuoc user dang dang nhap hay khong
                {
                    if (viewModel.ButtonCommandList.Count > 0)
                        viewModel.ButtonCommandList.Clear();

                    if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_qrcode))
                    {
                        viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.tao_qr_code, "FontAwesomeSolid", "\uf029", null, GenerateQRCode));
                    }

                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.tao_cuoc_hop, "FontAwesomeRegular", "\uf274", null, NewMeet));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.tao_cuoc_goi, "FontAwesomeSolid", "\uf095", null, NewPhoneCall));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.tao_cong_viec, "FontAwesomeSolid", "\uf073", null, NewTask));
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhu_cau_va_tieu_chi_title, "FontAwesomeSolid", "\uf073", null, NhuCauVaTieuChi));

                    if (viewModel.singleContact.statuscode != "100000000")
                        viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.cap_nhat, "FontAwesomeRegular", "\uf044", null, EditContact));

                    if (viewModel.singleContact.statuscode == "2")
                    {
                        floatingButtonGroup.IsVisible = false;
                        changeAvt.IsVisible = false;
                    }
                    else
                    {
                        floatingButtonGroup.IsVisible = true;
                        changeAvt.IsVisible = true;
                    }
                }
                else
                {
                    floatingButtonGroup.IsVisible = false;
                }
            }
        }

        private void NhuCauVaTieuChi(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            TieuChi_CenterPopup.ShowCenterPopup();
            LoadingHelper.Hide();
        }

        private async void NewMeet(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null)
            {
                LoadingHelper.Show();
                await Navigation.PushAsync(new MeetingForm());
                LoadingHelper.Hide();
            }
        }

        private async void NewPhoneCall(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null)
            {
                LoadingHelper.Show();
                await Navigation.PushAsync(new PhoneCallForm());
                LoadingHelper.Hide();
            }
        }

        private async void NewTask(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null)
            {
                LoadingHelper.Show();
                await Navigation.PushAsync(new TaskForm());
                LoadingHelper.Hide();
            }
        }

        private void EditContact(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ContactForm newPage = new ContactForm(Id);
            newPage.OnCompleted = async (OnCompleted) =>
            {
                if (OnCompleted == true)
                {
                    await Navigation.PushAsync(newPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        // tab thong tin
        private async Task LoadDataThongTin(string Id)
        {
            if (Id != null && viewModel.singleContact.contactid == Guid.Empty)
            {
                await viewModel.loadOneContact(Id);
                if (viewModel.singleContact?.gendercode != null)
                { 
                   viewModel.singleGender = ContactGender.GetGenderById(viewModel.singleContact.gendercode); 
                }
                if (viewModel.singleContact?.bsd_localization != null)
                {
                    viewModel.SingleLocalization = AccountLocalization.GetLocalizationById(viewModel.singleContact.bsd_localization);
                }
                else
                {
                    viewModel.SingleLocalization = null;
                }
            }
        }

        private void CMNDFront_Tapped(object sender, EventArgs e)
        {
            if(viewModel.singleContact.bsd_mattruoccmnd_source != null)
            {
                new PhotoBrowser()
                {
                    Photos = viewModel.Photos,
                    StartIndex = 0,
                    EnableGrid = true
                }.Show();
            }
        }

        private void CMNDBehind_Tapped(object sender, EventArgs e)
        {
            if (viewModel.singleContact.bsd_matsaucmnd_source != null && viewModel.singleContact.bsd_mattruoccmnd_source != null)
            {
                new PhotoBrowser()
                {
                    Photos = viewModel.Photos,
                    StartIndex = 1,
                    EnableGrid = true
                }.Show();
            }
            else
            {
                if (viewModel.singleContact.bsd_matsaucmnd_source != null && viewModel.singleContact.bsd_mattruoccmnd_source == null)
                {
                    new PhotoBrowser()
                    {
                        Photos = viewModel.Photos,
                        StartIndex = 0,
                        EnableGrid = true
                    }.Show();
                }
            }
                
        }

        #region Tab giao dich
        private async Task LoadDataGiaoDich(string Id)
        {
            if (viewModel.list_danhsachdatcho == null || viewModel.list_danhsachdatcoc == null || viewModel.list_danhsachhopdong == null || viewModel.Cares == null)
            {
                LoadingHelper.Show();
                viewModel.PageDanhSachDatCho = 1;
                viewModel.PageDanhSachDatCoc = 1;
                viewModel.PageDanhSachHopDong = 1;
                viewModel.PageCase = 1;

                viewModel.list_danhsachdatcho = new ObservableCollection<QueueFormModel>();
                viewModel.list_danhsachdatcoc = new ObservableCollection<ReservationListModel>();
                viewModel.list_danhsachhopdong = new ObservableCollection<ContractModel>();
                viewModel.Cares = new ObservableCollection<ActivityListModel>();

                await Task.WhenAll(
                   viewModel.LoadQueuesForContactForm(Id),
                   viewModel.LoadReservationForContactForm(Id),
                   viewModel.LoadOptoinEntryForContactForm(Id),
                   viewModel.LoadCase()
               );
                LoadingHelper.Hide();
            }          
        }
        // danh sach dat cho
        private async void ShowMoreDanhSachDatCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachDatCho++;
            await viewModel.LoadQueuesForContactForm(viewModel.singleContact.contactid.ToString());
            LoadingHelper.Hide();
        }

        // danh sach dat coc
        private async void ShowMoreDanhSachDatCoc_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachDatCoc++;
            await viewModel.LoadReservationForContactForm(viewModel.singleContact.contactid.ToString());
            LoadingHelper.Hide();
        }

        // danh sach hop dong
        private async void ShowMoreDanhSachHopDong_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachHopDong++;
            await viewModel.LoadOptoinEntryForContactForm(viewModel.singleContact.contactid.ToString());
            LoadingHelper.Hide();
        }

        //Cham soc khach hang
        private async void ShowMoreChamSocKhachHang_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageCase++;
            await viewModel.LoadCase();
            LoadingHelper.Hide();
        }

        private void ChiTietDatCoc_Tapped(object sender, EventArgs e)
        {
            if (viewModel.IsCurrentRecordOfUser == false) return;
            var item = (ReservationListModel)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item == null) return;

            LoadingHelper.Show();
            bool isReservation = false;
            if (item.statuscode != 100000007 && item.statuscode != 100000000 && item.statuscode != 861450001) // Qutation,Reservatoin,Supmit
                isReservation = true;
            BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(item.quoteid, isReservation);
            bangTinhGiaDetail.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(bangTinhGiaDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void ItemHopDong_Tapped(object sender, EventArgs e)
        {
            if (viewModel.IsCurrentRecordOfUser == false) return;
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            ContractDetailPage contractDetail = new ContractDetailPage(itemId);
            contractDetail.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(contractDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void CaseItem_Tapped(object sender, EventArgs e)
        {
            if (viewModel.IsCurrentRecordOfUser == false) return;
            var item = (ActivityListModel)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item != null && item.activityid != Guid.Empty)
            {
                ActivityPopup.ShowActivityPopup(item.activityid, item.activitytypecode);
            }
        }

        #endregion

        #region TabPhongThuy
        private void LoadDataPhongThuy()
        {
            if(viewModel.PhongThuy == null)
            {
               viewModel.PhongThuy = new PhongThuyModel();
               viewModel.LoadPhongThuy();
                if (viewModel.PhongThuy.gioi_tinh != 0 && viewModel.PhongThuy.gioi_tinh != 100000000 && viewModel.PhongThuy.nam_sinh != 0)//100000000
                    phongthuy_info.IsVisible = true;
                else
                    phongthuy_info.IsVisible = false;
            }
        }

        private void ShowImage_Tapped(object sender, EventArgs e)
        {
            LookUpImage.IsVisible = true;
            ImageDetail.Source = viewModel.PhongThuy.image;
        }

        protected override bool OnBackButtonPressed()
        {
            if (LookUpImage.IsVisible)
            {
                LookUpImage.IsVisible = false;
                return true;
            }
            FromCustomer = null;
            return base.OnBackButtonPressed();
        }

        private void Close_LookUpImage_Tapped(object sender, EventArgs e)
        {
            LookUpImage.IsVisible = false;
        }

        #endregion

        private async void NhanTin_Tapped(object sender, EventArgs e)
        {
            string phone = viewModel.singleContact.mobilephone.Substring(viewModel.singleContact.mobilephone.Length - 10, 10);
            if (phone != string.Empty)
            {
                SmsMessage sms = new SmsMessage(null, phone);
                await Sms.ComposeAsync(sms);
                //var checkVadate = PhoneNumberFormatVNHelper.CheckValidate(phone);
                //if (checkVadate == true)
                //{
                //    SmsMessage sms = new SmsMessage(null, phone);
                //    await Sms.ComposeAsync(sms);
                //    LoadingHelper.Hide();
                //}
                //else
                //{
                //    LoadingHelper.Hide();
                //    ToastMessageHelper.ShortMessage(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                //}
            }
            else
            {
                ToastMessageHelper.Message(Language.khach_hang_khong_co_so_dien_thoai_vui_long_kiem_tra_lai);
            }
        }

        private async void GoiDien_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(viewModel.singleContact.mobilephone) && viewModel.singleContact.mobilephone.Length >= 12)
                {
                    string phone = viewModel.singleContact.mobilephone.Substring(viewModel.singleContact.mobilephone.Length - 10, 10);
                    if (phone != string.Empty)
                    {
                        await Launcher.OpenAsync($"tel:{phone}");
                        //var checkVadate = PhoneNumberFormatVNHelper.CheckValidate(phone);
                        //if (checkVadate == true)
                        //{
                        //    await Launcher.OpenAsync($"tel:{phone}");
                        //    LoadingHelper.Hide();
                        //}
                        //else
                        //{
                        //    LoadingHelper.Hide();
                        //    ToastMessageHelper.ShortMessage(Language.so_dien_thoai_sai_dinh_dang_vui_long_kiem_tra_lai);
                        //}
                    }
                    else
                    {
                        ToastMessageHelper.Message(Language.khach_hang_khong_co_so_dien_thoai_vui_long_kiem_tra_lai);
                    }
                }
                else
                {
                    ToastMessageHelper.Message(Language.khach_hang_khong_co_so_dien_thoai_vui_long_kiem_tra_lai);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ThongTinCongTy_Tapped(object sender, EventArgs e)
        {            
            if (!string.IsNullOrEmpty(viewModel.singleContact._parentcustomerid_value))
            {
                LoadingHelper.Show();
                AccountDetailPage newPage = new AccountDetailPage(Guid.Parse(viewModel.singleContact._parentcustomerid_value));
                newPage.OnCompleted = async (OnCompleted) =>
                {
                    if (OnCompleted == 1)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else if(OnCompleted == 3 || OnCompleted == 2)
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
        }

        private void GiuChoItem_Tapped(object sender, EventArgs e)
        {
            if (viewModel.IsCurrentRecordOfUser == false) return;
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            QueuesDetialPage queuesDetialPage = new QueuesDetialPage(itemId);
            queuesDetialPage.OnCompleted = async (IsSuccess) => {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(queuesDetialPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void ActivityPopup_HidePopupActivity(object sender, EventArgs e)
        {
            OnAppearing();
        }

        private async void TabControl_IndexTab(object sender, LookUpChangeEvent e)
        {
            if (e.Item != null)
            {
                if ((int)e.Item == 0)
                {
                    TabThongTin.IsVisible = true;
                    TabGiaoDich.IsVisible = false;
                    TabPhongThuy.IsVisible = false;
                }
                else if ((int)e.Item == 1)
                {
                    await LoadDataGiaoDich(Id.ToString());
                    TabThongTin.IsVisible = false;
                    TabGiaoDich.IsVisible = true;
                    TabPhongThuy.IsVisible = false;
                }
                else if ((int)e.Item == 2)
                {
                    LoadDataPhongThuy();
                    TabThongTin.IsVisible = false;
                    TabGiaoDich.IsVisible = false;
                    TabPhongThuy.IsVisible = true;
                }
            }
        }

        private async void GenerateQRCode(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.singleContact.bsd_customercode))
            {
                ToastMessageHelper.Message(Language.vui_long_cap_nhat_ma_khach_hang_de_tao_ma_qr);
                return;
            }

            LoadingHelper.Show();
            List<string> info = new List<string>();
            info.Add(viewModel.singleContact.bsd_customercode);
            info.Add("contact");
            info.Add(viewModel.singleContact.contactid.ToString());
            string uriQrCode = $"https://api.qrserver.com/v1/create-qr-code/?size=150%C3%97150&data={string.Join(",", info)}";

            var bytearr = await DowloadImageToByteArrHelper.Download(uriQrCode);
            string base64 = System.Convert.ToBase64String(bytearr);

            bool isSuccess = await viewModel.SaveQRCode(base64);
            if (isSuccess)
            {
                viewModel.singleContact.bsd_qrcode = base64;
                ToastMessageHelper.Message(Language.tao_qr_code_thanh_cong);
                SetButtonFloatingButton();
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.tao_qr_code_that_bai);
            }
        }

        private void NhuCauDienTich_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            btn_nhaucaudientich.IsVisible = true;
        }
        private void TieuChiChonMua_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            btn_tieuchichonmua.IsVisible = true;
        }
        private void LoaiBDSQuanTam_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            btn_loaibdsquantam.IsVisible = true;
        }

        private async void LoaiBDSQuanTam_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var result = await viewModel.updateLoaiBDSQuanTam();
            if (result.IsSuccess)
            {
                btn_loaibdsquantam.IsVisible = false;
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
        }

        private async void btn_nhaucaudientich_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var result = await viewModel.updateNhuCauDienTich();
            if (result.IsSuccess)
            {
                btn_nhaucaudientich.IsVisible = false;
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
        }

        private async void btn_tieuchichonmua_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var result = await viewModel.updateTieuChiChonMua();
            if (result.IsSuccess)
            {
                btn_tieuchichonmua.IsVisible = false;
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
        }

        private async void btn_nhucaudiadiem_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var result = await viewModel.updateNhuCauDiaDiem();
            if (result)
            {
                btn_nhucaudiadiem.IsVisible = false;
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
        }

        private void Province_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            btn_nhucaudiadiem.IsVisible = true;
        }

        private void Project_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            btn_nhucauduan.IsVisible = true;
        }

        private async void btn_nhucauduan_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var result = await viewModel.updateNhuCauDuAn();
            if (result)
            {
                btn_nhucauduan.IsVisible = false;
                ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.Message(Language.thong_bao_that_bai);
            }
        }
        private void Protecter_Tapped(object sender, EventArgs e)
        {
            if (viewModel.singleContact != null && viewModel.singleContact.protecter_id != Guid.Empty)
            {
                LoadingHelper.Show();
                ContactDetailPage newPage = new ContactDetailPage(viewModel.singleContact.protecter_id);
                newPage.OnCompleted = async (OnCompleted) =>
                {
                    if (OnCompleted == 1)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
        }
        private async void ChangeAvatar_Tapped(object sender, EventArgs e)
        {
            string[] options = new string[] { Language.thu_vien, Language.chup_hinh };
            string asw = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, options);
            if (asw == Language.thu_vien)
            {
                await CrossMedia.Current.Initialize();
                PermissionStatus photostatus = await PermissionHelper.RequestPhotosPermission();
                if (photostatus == PermissionStatus.Granted)
                {
                    try
                    {
                        LoadingHelper.Show();
                        var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions() { PhotoSize = PhotoSize.MaxWidthHeight, MaxWidthHeight = 600 });
                        if (file != null)
                        {
                            viewModel.AvatarArr = System.IO.File.ReadAllBytes(file.Path);
                            string imgBase64 = Convert.ToBase64String(viewModel.AvatarArr);
                            var Avatar = imgBase64;
                            if (Avatar != viewModel.singleContact.entityimage)
                            {
                                bool isSuccess = await viewModel.ChangeAvatar();
                                if (isSuccess)
                                {
                                    viewModel.singleContact.avatar = Avatar;
                                    ToastMessageHelper.Message(Language.doi_hinh_dai_dien_thanh_cong);
                                    LoadingHelper.Hide();
                                }
                                else
                                {
                                    LoadingHelper.Hide();
                                    ToastMessageHelper.Message(Language.doi_hinh_dai_dien_that_bai);
                                }
                            }
                            LoadingHelper.Hide();
                        }
                    }
                    catch (Exception ex)
                    {
                        ToastMessageHelper.Message(ex.Message);
                        LoadingHelper.Hide();
                    }
                }
                LoadingHelper.Hide();
            }
            else if (asw == Language.chup_hinh)
            {
                await CrossMedia.Current.Initialize();
                PermissionStatus camerastatus = await PermissionHelper.RequestCameraPermission();
                if (camerastatus == PermissionStatus.Granted)
                {
                    LoadingHelper.Show();
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
                        var Avatar = Convert.ToBase64String(viewModel.AvatarArr);
                        if (Avatar != viewModel.singleContact.entityimage)
                        {
                            bool isSuccess = await viewModel.ChangeAvatar();
                            if (isSuccess)
                            {
                                viewModel.singleContact.avatar = Avatar;
                                if (AppShell.NeedToRefeshUserInfo.HasValue) AppShell.NeedToRefeshUserInfo = true;
                                ToastMessageHelper.Message(Language.doi_hinh_dai_dien_thanh_cong);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.Message(Language.doi_hinh_dai_dien_that_bai);
                            }
                        }
                    }
                }
                LoadingHelper.Hide();
            }
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.IsRefreshing = true;
                await viewModel.loadOneContact(viewModel.singleContact.contactid.ToString());
                if (viewModel.singleContact?.gendercode != null)
                {
                    viewModel.singleGender = ContactGender.GetGenderById(viewModel.singleContact.gendercode);
                }
                if (viewModel.singleContact?.bsd_localization != null)
                {
                    viewModel.SingleLocalization = AccountLocalization.GetLocalizationById(viewModel.singleContact.bsd_localization);
                }
                else
                {
                    viewModel.SingleLocalization = null;
                }
            
            if (viewModel.list_danhsachdatcho != null && viewModel.list_danhsachdatcho.Count > 0)
            {
                viewModel.list_danhsachdatcho.Clear();
                viewModel.PageDanhSachDatCho = 1;
                await viewModel.LoadQueuesForContactForm(viewModel.singleContact.contactid.ToString());
            }
            if (viewModel.list_danhsachdatcoc != null && viewModel.list_danhsachdatcoc.Count > 0)
            {
                viewModel.list_danhsachdatcoc.Clear();
                viewModel.PageDanhSachDatCoc = 1;
                await viewModel.LoadReservationForContactForm(viewModel.singleContact.contactid.ToString());
            }
            if (viewModel.list_danhsachhopdong != null && viewModel.list_danhsachhopdong.Count > 0)
            {
                viewModel.list_danhsachhopdong.Clear();
                viewModel.PageDanhSachHopDong = 1;
                await viewModel.LoadOptoinEntryForContactForm(viewModel.singleContact.contactid.ToString());
            }
            if (viewModel.Cares != null && viewModel.Cares.Count > 0)
            {
                viewModel.Cares.Clear();
                viewModel.PageCase = 1;
                await viewModel.LoadCase();
            }
            viewModel.IsRefreshing = false;
            viewModel.PhongThuy = null;
            LoadDataPhongThuy();
            LoadingHelper.Hide();
        }

        private void OriginatingLead_Tapped(object sender, EventArgs e)
        {
            if (viewModel.singleContact.leadid_originated != Guid.Empty)
            {
                LoadingHelper.Show();
                LeadDetailPage newPage = new LeadDetailPage(viewModel.singleContact.leadid_originated);
                newPage.OnCompleted = async (OnCompleted) =>
                {
                    if (OnCompleted == 1)
                    {
                        await Navigation.PushAsync(newPage);
                        LoadingHelper.Hide();
                    }
                    else if (OnCompleted == 3 || OnCompleted == 2)
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
        }
    }
}