using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
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
                currency.SelectedItem = "English";
            else
                currency.SelectedItem = "Việt Nam";
        }

        private void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        }

        private void Currency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = currency.SelectedItem as string;
            if (item == "English")
            {
                UserLogged.Currency = "en";
            }
            else
            {
                UserLogged.Currency = "vi";
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
    }
}