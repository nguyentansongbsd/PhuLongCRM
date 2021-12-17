using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class QueuesDetialPage : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefreshBTG = null;
        public QueuesDetialPageViewModel viewModel;
        public QueuesDetialPage(Guid queueId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new QueuesDetialPageViewModel();
            viewModel.QueueId = queueId;
            NeedToRefreshBTG = false;
            Init();
        }

        public async void Init()
        {
            await viewModel.LoadQueue();
            SetButtons();

            VisualStateManager.GoToState(radBorderThongTin, "Active");
            VisualStateManager.GoToState(radBorderGiaoDich, "InActive");
            VisualStateManager.GoToState(lbThongTin, "Active");
            VisualStateManager.GoToState(lbGiaoDich, "InActive");

            if (viewModel.Queue != null)
            {
                OnCompleted?.Invoke(true);
            }
            else
            {
                OnCompleted?.Invoke(false);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.BangTinhGiaList != null && NeedToRefreshBTG == true)
            {
                LoadingHelper.Show();
                viewModel.BangTinhGiaList.Clear();
                viewModel.PageBangTinhGia = 1;
                await viewModel.LoadDanhSachBangTinhGia();
                NeedToRefreshBTG = false;
                LoadingHelper.Hide();
            }
        }

        private void SetButtons()
        {
            viewModel.ShowButtons = (viewModel.ShowBtnHuyGiuCho == false && viewModel.ShowBtnBangTinhGia == false) ? false : true;
            gridButtons.ColumnDefinitions.Clear();
            var btns = gridButtons.Children.Where(x => x.IsVisible == true).ToList();
            for (int i = 0; i < btns.Count(); i++)
            {
                gridButtons.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star),
                });
                Grid.SetColumn(btns[i], i);
            }
        }

        private void GoToProject_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ProjectInfo projectInfo = new ProjectInfo(viewModel.Queue._bsd_project_value);
            projectInfo.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(projectInfo);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy dự án");
                }
            };
        }

        private void GoToPhaseLaunch_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();

            LoadingHelper.Hide();
        }

        private void GoToUnit_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            UnitInfo unitInfo = new UnitInfo(viewModel.Queue._bsd_units_value);
            unitInfo.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(unitInfo);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy sản phẩm");
                }
            };
        }

        private void GoToAcount_Tapped(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            AccountDetailPage accountDetail = new AccountDetailPage(viewModel.Queue._bsd_salesagentcompany_value);
            accountDetail.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(accountDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy đại lý bán hàng ");
                }
            };
        }

        private async void NhanTin_Tapped(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            try
            {
                if (!string.IsNullOrWhiteSpace(viewModel.NumPhone))
                {
                    LoadingHelper.Hide();
                    SmsMessage sms = new SmsMessage(null, viewModel.NumPhone);
                    await Sms.ComposeAsync(sms);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không có số điện thoại");
                }
            }
            catch (Exception ex)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(ex.Message);
            }
        }

        private async void GoiDien_Tapped(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            try
            {
                if (!string.IsNullOrWhiteSpace(viewModel.NumPhone))
                {
                    await Launcher.OpenAsync($"tel:{viewModel.NumPhone}");
                }
                else
                {
                    ToastMessageHelper.ShortMessage("Không có số điện thoại");
                }
                LoadingHelper.Hide();
            }
            catch (Exception ex)
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(ex.Message);
            }
        }

        private async void HuyGiuCho_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Xác nhận", "Bạn có muốn hủy giữ chỗ này không ?", "Đồng ý", "Hủy");
            if (confirm == false) return;

            LoadingHelper.Show();
            // string url_action = $"/opportunities({this.viewModel.QueueId})/Microsoft.Dynamics.CRM.bsd_Action_Queue_CancelQueuing";
            string url_action = $"/opportunities({this.viewModel.QueueId})/Microsoft.Dynamics.CRM.bsd_Action_Opportunity_HuyGiuChoCoTien";
            var content = new { };
            CrmApiResponse res = await CrmHelper.PostData(url_action, content);
            if (res.IsSuccess)
            {
                await viewModel.LoadQueue();
                SetButtons();
                if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                if (ProjectInfo.NeedToRefreshQueue.HasValue) ProjectInfo.NeedToRefreshQueue = true;
                if (UnitInfo.NeedToRefreshQueue.HasValue) UnitInfo.NeedToRefreshQueue = true;
                if (AccountDetailPage.NeedToRefreshQueues.HasValue) AccountDetailPage.NeedToRefreshQueues = true;
                if (ContactDetailPage.NeedToRefreshQueues.HasValue) ContactDetailPage.NeedToRefreshQueues = true;
                ToastMessageHelper.ShortMessage("Hủy giữ chỗ thành công");
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Hủy giữ chỗ thất bại");
            }
        }

        private void CreateQuotation_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            OptionSet Queue = new OptionSet(viewModel.Queue.opportunityid, viewModel.Queue.name);
            OptionSet SaleAgentCompany = new OptionSet(viewModel.Queue._bsd_salesagentcompany_value.ToString(), viewModel.Queue.salesagentcompany_name);
            string NameOfStaffAgent = viewModel.Queue.bsd_nameofstaffagent;

            ReservationForm reservationForm = new ReservationForm(viewModel.Queue._bsd_units_value, Queue, SaleAgentCompany, NameOfStaffAgent, viewModel.Customer);
            reservationForm.CheckReservation = async (isSuccess) =>
            {
                if (isSuccess == 0)
                {
                    await Navigation.PushAsync(reservationForm);
                    LoadingHelper.Hide();
                }
                else if (isSuccess == 1)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Sản phẩm không thể tạo bảng tính giá");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy sản phẩm");
                }
            };
        }

        private void ThongTin_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderThongTin, "Active");
            VisualStateManager.GoToState(radBorderGiaoDich, "InActive");
            VisualStateManager.GoToState(lbThongTin, "Active");
            VisualStateManager.GoToState(lbGiaoDich, "InActive");
            stThongTin.IsVisible = true;
            stGiaoDich.IsVisible = false;
        }

        private async void GiaoDich_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderThongTin, "InActive");
            VisualStateManager.GoToState(radBorderGiaoDich, "Active");
            VisualStateManager.GoToState(lbThongTin, "InActive");
            VisualStateManager.GoToState(lbGiaoDich, "Active");
            stThongTin.IsVisible = false;
            stGiaoDich.IsVisible = true;
            LoadingHelper.Show();
            if (viewModel.BangTinhGiaList == null && viewModel.DatCocList == null && viewModel.HopDongList == null)
            {
                viewModel.BangTinhGiaList = new ObservableCollection<ReservationListModel>();
                viewModel.DatCocList = new ObservableCollection<ReservationListModel>();
                viewModel.HopDongList = new ObservableCollection<ContractModel>();
                await Task.WhenAll(
                    viewModel.LoadDanhSachBangTinhGia(),
                    viewModel.LoadDanhSachDatCoc(),
                    viewModel.LoadDanhSachHopDong()
                    );
            }
            LoadingHelper.Hide();
        }

        private async void ShowMoreBangTinhGia_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageBangTinhGia++;
            await viewModel.LoadDanhSachBangTinhGia();
            LoadingHelper.Hide();
        }

        private async void ShowMoreDatCoc_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDatCoc++;
            await viewModel.LoadDanhSachDatCoc();
            LoadingHelper.Hide();
        }

        private async void ShowMoreHopDong_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageHopDong++;
            await viewModel.LoadDanhSachHopDong();
            LoadingHelper.Hide();
        }

        private void ItemReservation_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(itemId);
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
                    ToastMessageHelper.ShortMessage("Không tìm thấy thông tin");
                }
            };
        }

        private void ItemHopDong_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
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
                    ToastMessageHelper.ShortMessage("Không tìm thấy thông tin");
                }
            };
        }

        private void Customer_Tapped(object sender, EventArgs e)
        {
            if(viewModel.Customer!= null)
            {
                if(viewModel.Customer.Title == viewModel.CodeContact)
                {
                    ContactDetailPage newPage = new ContactDetailPage(Guid.Parse(viewModel.Customer.Val));
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };
                }
                else if (viewModel.Customer.Title == viewModel.CodeAccount)
                {
                    AccountDetailPage newPage = new AccountDetailPage(Guid.Parse(viewModel.Customer.Val));
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
                            ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại.");
                        }
                    };
                }
            }
        }
    }
}
