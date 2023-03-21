using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupPage : ContentPage
    {

        public SetupPage()
        {
            InitializeComponent();
            if (UserLogged.Language == "en")
                language.SelectedItem = "English";
            else
                language.SelectedItem = "Việt Nam";

            if (UserLogged.Currency == "en")
            {
                currency.SelectedItem = "English (United States)";
                text_number.Text = "123,456,789.00";
                text_curency.Text = "123,456,789.00 đ";
            }
            else
            {
                currency.SelectedItem = "Vietnamese (VietNam)";
                text_number.Text = "123.456.789,00";
                text_curency.Text = "123.456.789,00 đ";
            }

            if (UserLogged.Notification == true)
                noti.IsToggled = true;
            else
                noti.IsToggled = false;
        }

        private void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
        //edit o day
            var item = language.SelectedItem as string;
            if(item == "English")
            {
                UserLogged.Language = "en";
            }    
            else
            {
                UserLogged.Language = "vi";
            }
            CultureInfo cultureInfo = new CultureInfo(UserLogged.Language);
            Language.Culture = cultureInfo;
            if (UserLogged.Language == "vi")
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
            }
            else if (UserLogged.Language == "en")
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            }
            Refresh();
            RefreshLanguage();
        }

        private void Currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = currency.SelectedItem as string;
            if (item == "English (United States)")
            {
                UserLogged.Currency = "en";
                text_number.Text = "123,456,789.00";
                text_curency.Text = "123,456,789.00 đ";
            }
            else
            {
                UserLogged.Currency = "vi";
                text_number.Text = "123.456.789,00";
                text_curency.Text = "123.456.789,00 đ";
            }
            Refresh();
        }
        private void Refresh()
        {
            //trang chu
            if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
            if (Dashboard.NeedToRefreshMeet.HasValue) Dashboard.NeedToRefreshMeet = true;
            if (Dashboard.NeedToRefreshPhoneCall.HasValue) Dashboard.NeedToRefreshPhoneCall = true;
            if (Dashboard.NeedToRefreshQueue.HasValue) Dashboard.NeedToRefreshQueue = true;
            if (Dashboard.NeedToRefreshTask.HasValue) Dashboard.NeedToRefreshTask = true;

            //khachhang
            if (CustomerPage.NeedToRefreshAccount.HasValue) CustomerPage.NeedToRefreshAccount = true;
            if (CustomerPage.NeedToRefreshContact.HasValue) CustomerPage.NeedToRefreshContact = true;
            if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;

            //du an
            if (ProjectList.NeedToRefresh.HasValue) ProjectList.NeedToRefresh = true;

            //giu cho
            if (QueueList.NeedToRefresh.HasValue) QueueList.NeedToRefresh = true;

            //btg
            if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;

            //dat coc
            if (DatCocList.NeedToRefresh.HasValue) DatCocList.NeedToRefresh = true;

            // hop dong
            if (ContractList.NeedToRefresh.HasValue) ContractList.NeedToRefresh = true;

            // hoat dong
            if (ActivityList.NeedToRefreshMeet.HasValue) ActivityList.NeedToRefreshMeet = true;
            if (ActivityList.NeedToRefreshPhoneCall.HasValue) ActivityList.NeedToRefreshPhoneCall = true;
            if (ActivityList.NeedToRefreshTask.HasValue) ActivityList.NeedToRefreshTask = true;

            //phan hoi
            if (ListPhanHoi.NeedToRefresh.HasValue) ListPhanHoi.NeedToRefresh = true;

            //danh sach theo doi
            if (FollowUpListPage.NeedToRefresh.HasValue) FollowUpListPage.NeedToRefresh = true;

            //lich lam viec
        }
        private void RefreshLanguage()
        {
            this.Title = Language.thiet_lap_title;
            ngonngu.Text = Language.ngon_ngu;
            tiente.Text = Language.tien_te;
            thongbao.Text = Language.thong_bao;
            lb_number.Text = Language.so;
            lb_curency.Text = Language.tien_te;
        }

        private void noti_Toggled(object sender, ToggledEventArgs e)
        {
            if (noti.IsToggled == true)
            {
                UserLogged.Notification = true;
                CrossFirebasePushNotification.Current.Subscribe("all");
            }
            else
            {
                UserLogged.Notification = false;
                CrossFirebasePushNotification.Current.Unsubscribe("all");
            }
        }
    }
}
