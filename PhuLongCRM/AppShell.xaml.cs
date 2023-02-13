using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.Views;
using Xamarin.Forms;

namespace PhuLongCRM
{
    public partial class AppShell : Shell
    {
        public static bool? NeedToRefeshUserInfo = null;
        private string _avartar;
        public string Avartar { get => _avartar; set { _avartar = value; OnPropertyChanged(nameof(Avartar)); } }

        private string _userName;
        public string UserName { get => _userName; set { _userName = value; OnPropertyChanged(nameof(UserName)); } }

        private string _contactName;
        public string ContactName { get => _contactName; set { _contactName = value; OnPropertyChanged(nameof(ContactName)); } }

        private string _verApp;
        public string VerApp { get => _verApp; set { _verApp = value; OnPropertyChanged(nameof(VerApp)); } }

        private UserCRMInfoPage userCRMInfo;

        public AppShell()
        {
            InitializeComponent();
            UserName = UserLogged.User;
            ContactName = string.IsNullOrWhiteSpace(UserLogged.ContactName) ? UserLogged.User : UserLogged.ContactName;
            Avartar = UserLogged.Avartar;
            NeedToRefeshUserInfo = false;
            VerApp = Config.OrgConfig.VerApp;
            this.BindingContext = this;
            PropertyChanged += AppShell_PropertyChanged;
        }

        private void AppShell_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LoadingHelper.Show();
            RefreshLanguage();
            LoadingHelper.Hide();
        }

        public AppShell(bool isLoginByUserCrm)
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            VerApp = Config.OrgConfig.VerApp;
            this.ContactName = UserLogged.ContactName;
            this.UserName = UserLogged.UserCRM;
            this.Avartar = $"https://ui-avatars.com/api/?background=2196F3&rounded=false&color=ffffff&size=150&length=2&name={UserLogged.UserCRM}";
            this.BindingContext = this;
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (NeedToRefeshUserInfo == true)
            {
                LoadingHelper.Show();
                if (Avartar != UserLogged.Avartar)
                {
                    Avartar = UserLogged.Avartar;
                }
                ContactName = UserLogged.ContactName;
                NeedToRefeshUserInfo = false;
                LoadingHelper.Hide();
            }
        }

        private void UserInfor_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (!UserLogged.IsLoginByUserCRM && UserLogged.ContactId == Guid.Empty)
            {
                ToastMessageHelper.ShortMessage(Language.chua_co_contact_khong_the_chinh_sua_thong_tin);
                LoadingHelper.Hide();
                return;
            }
            if (UserLogged.IsLoginByUserCRM)
            {
                UserCRMInfoPage userCRMInfo = new UserCRMInfoPage();
                userCRMInfo.OnCompleted = async (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(userCRMInfo);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.khong_tim_thay_user);
                    }
                };
            }
            else
            {
                UserInfoPage userInfo = new UserInfoPage();
                userInfo.OnCompleted = async (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(userInfo);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.khong_tim_thay_user);
                    }
                };
            }
            //await Shell.Current.Navigation.PushAsync(new UserInfoPage());
            this.FlyoutIsPresented = false;
            //LoadingHelper.Hide();
        }

        private async void Logout_Clicked(System.Object sender, System.EventArgs e)
        {
            if (UserLogged.IsLoginByUserCRM)
                DependencyService.Get<IClearCookies>().ClearAllCookies(); ;
            await UpdateStateLogin(false);
            await Shell.Current.GoToAsync("//LoginPage");
        }
        public async Task UpdateStateLogin(bool isLogin)
        {
            string path = $"/bsd_employees({UserLogged.Id})";

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (isLogin)
                data["bsd_statelogin"] = "Login";
            else
                data["bsd_statelogin"] = "Logout";

            CrmApiResponse crmApiResponse = await CrmHelper.PatchData(path, data);
            if (!crmApiResponse.IsSuccess)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
                return;
            }
        }
        public void RefreshLanguage()
        {
            trangchu.Title = Language.trang_chu_title;
            congviec.Title = Language.cong_viec_menu;
            khachhang.Title = Language.khach_hang_title;
            duan.Title = Language.du_an_title;
            giohang.Title = Language.gio_hang;
            giaodich.Title = Language.giao_dich_title;
            giucho.Title = Language.giu_cho_title;
            bangtinhgia.Title = Language.bang_tinh_gia_title;
            datcoc.Title = Language.dat_coc_title;
            hopdong.Title = Language.hop_dong_title;
            chamsockhachhang.Title = Language.cham_soc_khach_hang_title;
            hoatdong.Title = Language.hoat_dong;
            phanhoi.Title = Language.phan_hoi_title;
            danhsachtheodoi.Title = Language.danh_sach_theo_doi_title;
            tinhnang.Title = Language.tinh_nang_title;
            lichlamviec.Title = Language.lich_lam_viec;
            dongbodanhba.Title = Language.dong_bo_danh_ba;
            thietlap.Title = Language.thiet_lap_title;
            dangxuat.Text = Language.dang_xuat;
        }
    }
}
