using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        public NotificationPageViewModel viewModel;
        public NotificationPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new NotificationPageViewModel();
            Init();
        }
        private async void Init()
        {
            await viewModel.LoadData();
        }
        private async void ReadAll_Clicked(object sender, EventArgs e)
        {
            var accept = await DisplayAlert("", Language.ban_co_muon_danh_dau_tat_ca_thong_bao_la_da_doc_khong, Language.dong_y, Language.huy);
            if (accept)
            {
                LoadingHelper.Show();
                foreach (var item in viewModel.Notifications)
                {
                    if (item.IsBusy == false)
                    {
                        await viewModel.UpdateStatus(item.Key, item);
                    }
                }
                if (Dashboard.NeedToRefreshNoti.HasValue) Dashboard.NeedToRefreshNoti = true;
                viewModel.Notifications.Clear();
                await viewModel.LoadData();
                LoadingHelper.Hide();
            }
        }
        private async void ReadNoti(NotificaModel item)
        {
            try
            {
                LoadingHelper.Show();
                if (item.IsRead == true) return;
                await viewModel.UpdateStatus(item.Key, item);
                if (Dashboard.NeedToRefreshNoti.HasValue) Dashboard.NeedToRefreshNoti = true;
                viewModel.Notifications.Clear();
                await viewModel.LoadData();
            }
            catch (Exception ex)
            { }
        }

        private async void DeleteNotification_Invoked(object sender, EventArgs e)
        {
            try
            {
                LoadingHelper.Show();
                var tap = sender as SwipeItemView;
                var item = (NotificaModel)tap.CommandParameter;
                if (item != null)
                {
                    await viewModel.DeleteNotification(item.Key);
                    if (Dashboard.NeedToRefreshNoti.HasValue) Dashboard.NeedToRefreshNoti = true;
                    viewModel.Notifications.Clear();
                    await viewModel.LoadData();
                }
                LoadingHelper.Hide();
            }
            catch(Exception ex)
            { }
        }

        private void Notification_Tapped(object sender, EventArgs e)
        {
            var tap = sender as Grid;
            var item = (NotificaModel)(tap.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item != null)
            {
                if (item.NotificationType == NotificationType.Project
                    || item.NotificationType == NotificationType.PhaseLaunch
                    || item.NotificationType == NotificationType.Event)
                {
                    if (item.ProjectId != Guid.Empty)
                    {
                        LoadingHelper.Show();
                        ProjectInfo project = new ProjectInfo(item.ProjectId);
                        project.OnCompleted = async (isSuccess) =>
                        {
                            if (isSuccess)
                            {
                                await Navigation.PushAsync(project);
                                ReadNoti(item);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                }
                else if (item.NotificationType == NotificationType.QueueCancel
                    || item.NotificationType == NotificationType.QueueSuccess
                    || item.NotificationType == NotificationType.QueueRefunded
                    || item.NotificationType == NotificationType.MatchUnit)
                {
                    if (item.QueueId != Guid.Empty)
                    {
                        LoadingHelper.Show();
                        QueuesDetialPage queuesDetialPage = new QueuesDetialPage(item.QueueId);
                        queuesDetialPage.OnCompleted = async (isSuccess) => {
                            if (isSuccess)
                            {
                                await Navigation.PushAsync(queuesDetialPage);
                                ReadNoti(item);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                }
                else if (item.NotificationType == NotificationType.Quote
                    || item.NotificationType == NotificationType.SpecialDiscount)
                {
                    if (item.QuoteId != Guid.Empty)
                    {
                        LoadingHelper.Show();
                        BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(item.QuoteId);
                        bangTinhGiaDetail.OnCompleted = async (OnCompleted) =>
                        {
                            if (OnCompleted == true)
                            {
                                await Navigation.PushAsync(bangTinhGiaDetail);
                                ReadNoti(item);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }

                        };
                    }
                }
                else if (item.NotificationType == NotificationType.Reservation)
                {
                    if (item.ReservationId != Guid.Empty)
                    {
                        LoadingHelper.Show();
                        BangTinhGiaDetailPage newPage = new BangTinhGiaDetailPage(item.ReservationId, true) { Title = Language.dat_coc_title };
                        newPage.OnCompleted = async (OnCompleted) =>
                        {
                            if (OnCompleted == true)
                            {
                                await Navigation.PushAsync(newPage);
                                ReadNoti(item);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                }
                else if (item.NotificationType == NotificationType.ContractHDMB
                    || item.NotificationType == NotificationType.ContractTTDC)
                {
                    if (item.ContractId != Guid.Empty)
                    {
                        LoadingHelper.Show();
                        ContractDetailPage contractDetailPage = new ContractDetailPage(item.ContractId);
                        contractDetailPage.OnCompleted = async (OnCompleted) =>
                        {
                            if (OnCompleted == true)
                            {
                                await Navigation.PushAsync(contractDetailPage);
                                ReadNoti(item);
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                            }
                        };
                    }
                }
            }
        }
    }
}