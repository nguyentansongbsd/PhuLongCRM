using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BangTinhGiaDetailPage : ContentPage
    {
        private BangTinhGiaDetailPageViewModel viewModel;
        public Action<bool> OnCompleted;
        private Guid ReservationId;
        public static bool? NeedToRefresh = null; 
        public static bool? NeedToRefreshInstallment = null;

        public BangTinhGiaDetailPage(Guid id)
        {
            InitializeComponent();
            ReservationId = id;
            BindingContext = viewModel = new BangTinhGiaDetailPageViewModel();
            NeedToRefresh = false;
            NeedToRefreshInstallment = false;
            Tab_Tapped(1);
            Init();
        }

        public async void Init()
        {
            await Task.WhenAll(
                LoadDataChinhSach(ReservationId),
                viewModel.LoadCoOwners(ReservationId)
                );

            SetUpButtonGroup();
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                OnCompleted?.Invoke(true);
            }
            else
                OnCompleted?.Invoke(false);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();

                viewModel.CoownerList.Clear();
                viewModel.ListDiscount.Clear();
                viewModel.ListPromotion.Clear();

                await Task.WhenAll(
                    LoadDataChinhSach(ReservationId),
                    viewModel.LoadCoOwners(ReservationId)
                );
                viewModel.ButtonCommandList.Clear();
                SetUpButtonGroup();
                NeedToRefresh = false;

                LoadingHelper.Hide();
            }
            if (NeedToRefreshInstallment == true)
            {
                LoadingHelper.Show();

                viewModel.ShowInstallmentList = false;
                viewModel.NumberInstallment = 0;
                viewModel.InstallmentList.Clear();
                viewModel.ButtonCommandList.Clear();

                await viewModel.LoadInstallmentList(ReservationId);    
                SetUpButtonGroup();
                NeedToRefreshInstallment = false;

                LoadingHelper.Hide();
            }
        }

        //tab chinh sach

        private async Task LoadDataChinhSach(Guid id)
        {
            if (id != Guid.Empty)
            {
                await Task.WhenAll(
                    viewModel.LoadReservation(id),
                    viewModel.LoadPromotions(ReservationId),
                    viewModel.LoadSpecialDiscount(ReservationId),
                    viewModel.LoadInstallmentList(ReservationId)
                    );
                await viewModel.LoadHandoverCondition(ReservationId);
                await viewModel.LoadDiscounts();
                SutUpSpecialDiscount();
            }
        }

        // tab lich
        private async void LoadInstallmentList(Guid id)
        {
            if (id != Guid.Empty && viewModel.InstallmentList != null && viewModel.InstallmentList.Count == 0)
            {
                LoadingHelper.Show();
                await viewModel.LoadInstallmentList(id);
                LoadingHelper.Hide();
            }
        }

        private void ChinhSach_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(1);
        }

        private void TongHop_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(2);
        }

        private void ChiTiet_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(3);
        }

        private void Lich_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(4);
            if (viewModel.InstallmentList.Count == 0)
            {
                LoadInstallmentList(ReservationId);
            }
        }

        private void Tab_Tapped(int tab)
        {
            if (tab == 1)
            {
                VisualStateManager.GoToState(radBorderChinhSach, "Selected");
                VisualStateManager.GoToState(lbChinhSach, "Selected");
                TabChinhSach.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderChinhSach, "Normal");
                VisualStateManager.GoToState(lbChinhSach, "Normal");
                TabChinhSach.IsVisible = false;
            }
            if (tab == 2)
            {
                VisualStateManager.GoToState(radBorderTongHop, "Selected");
                VisualStateManager.GoToState(lbTongHop, "Selected");
                TabTongHop.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderTongHop, "Normal");
                VisualStateManager.GoToState(lbTongHop, "Normal");
                TabTongHop.IsVisible = false;
            }
            if (tab == 3)
            {
                VisualStateManager.GoToState(radBorderChiTiet, "Selected");
                VisualStateManager.GoToState(lbChiTiet, "Selected");
                TabChiTiet.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderChiTiet, "Normal");
                VisualStateManager.GoToState(lbChiTiet, "Normal");
                TabChiTiet.IsVisible = false;
            }
            if (tab == 4)
            {
                VisualStateManager.GoToState(radBorderLich, "Selected");
                VisualStateManager.GoToState(lbLich, "Selected");
                TabLich.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderLich, "Normal");
                VisualStateManager.GoToState(lbLich, "Normal");
                TabLich.IsVisible = false;
            }
        }

        private void SutUpSpecialDiscount()
        {
            if (viewModel.ListSpecialDiscount != null && viewModel.ListSpecialDiscount.Count > 0)
            {
                stackLayoutSpecialDiscount.IsVisible = true;
                foreach (var item in viewModel.ListSpecialDiscount)
                {
                    if (!string.IsNullOrEmpty(item.Label))
                    {
                        stackLayoutSpecialDiscount.Children.Add(SetUpItem(item.Label));
                    }
                }
            }
            else
            {
                stackLayoutSpecialDiscount.IsVisible = false;
            }
        }

        private void SetUpButtonGroup()
        {
            if (viewModel.Reservation.statuscode == 100000007)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Cập Nhật Bảng Tính Giá", "FontAwesomeRegular", "\uf044", null, EditQuotes));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Hủy Bảng Tính Giá", "FontAwesomeRegular", "\uf273", null, CancelQuotes));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Xác nhận in", "FontAwesomeSolid", "\uf02f", null, ConfirmSigning));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Xóa Lịch Thanh Toán", "FontAwesomeRegular", "\uf1c3", null, CancelInstallment));
            }
            if (viewModel.Reservation.statuscode == 100000007 && viewModel.InstallmentList.Count == 0)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Tạo Lịch Thanh Toán", "FontAwesomeRegular", "\uf271", null, CreatePaymentScheme));
            }
            if (viewModel.Reservation.statuscode == 100000007 && viewModel.InstallmentList.Count > 0 )
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Ký Bảng Tính Giá", "FontAwesomeRegular", "\uf274", null, SignQuotationClicked));
            }
            if (viewModel.Reservation.bsd_reservationformstatus == 100000001 && viewModel.Reservation.bsd_reservationprinteddate != null && viewModel.Reservation.bsd_reservationuploadeddate == null && viewModel.Reservation.bsd_rfsigneddate == null)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Xác Nhận Tải PĐC", "FontAwesomeRegular", "\uf15c", null, ConfirmReservation));
            }
            if (viewModel.Reservation.bsd_reservationformstatus == 100000001 && viewModel.Reservation.bsd_reservationprinteddate != null && viewModel.Reservation.bsd_reservationuploadeddate != null && viewModel.Reservation.bsd_rfsigneddate == null)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Ký Phiếu Đặt Cọc", "FontAwesomeRegular", "\uf274", null, CompletedReservation));
            }
            if (viewModel.ButtonCommandList.Count > 0)
            {
                floatingButtonGroup.IsVisible = true;
            }
            else
            {
                floatingButtonGroup.IsVisible = false;
            }
        }

        private async void CancelInstallment(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                if (await viewModel.DeactiveInstallment())
                {
                    NeedToRefreshInstallment = true;
                    OnAppearing();
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Đã xóa lịch thanh toán");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Xóa lịch thanh toán thất bại. Vui lòng thử lại");
                }
            }
        }

        private async void ConfirmSigning(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Reservation.bsd_quotationprinteddate.HasValue)
            {
                ToastMessageHelper.ShortMessage("Đã xác nhận in");
                LoadingHelper.Hide();
                return;
            }
            if (viewModel.InstallmentList.Count == 0)
            {
                ToastMessageHelper.ShortMessage("Vui lòng tạo lịch thanh toán");
                LoadingHelper.Hide();
                return;
            }
            bool isSuccess = await viewModel.ConfirmSinging();
            if (isSuccess)
            {
                NeedToRefresh = true;
                OnAppearing();
                ToastMessageHelper.ShortMessage("Xác nhận in thành công");
            }
            else
            {
                ToastMessageHelper.ShortMessage("Xác nhận in thất bại");
            }
            LoadingHelper.Hide();
        }

        private async void ConfirmReservation(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                viewModel.Reservation.bsd_reservationuploadeddate = DateTime.Now;
                if (await viewModel.UpdateQuotes(viewModel.ConfirmReservation))
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Đã xác nhận tải phiếu đặt cọc");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Xác nhận tải phiếu đặt cọc thất bại. Vui lòng thử lại");
                }
            }
        }

        private void EditQuotes(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ReservationForm reservation = new ReservationForm(this.ReservationId);
            reservation.CheckReservation = async (isSuccess) =>
            {
                if (isSuccess == 0)
                {
                    await Navigation.PushAsync(reservation);
                    LoadingHelper.Hide();
                }
                else if (isSuccess == 1)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Sản phẩm đang ở trạng thái Reserve không thể tạo bảng tính giá");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không có thông tin bảng tính giá");
                }
            };
        } 

        private async void CreatePaymentScheme(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string IsSuccess = await viewModel.UpdatePaymentScheme();
            if (IsSuccess == "True")
            {
                NeedToRefreshInstallment = true;
                OnAppearing();
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Tạo lịch thanh toán thành công");
            }
            else
            {
                if (IsSuccess == "Localization")
                {
                    string asw = await App.Current.MainPage.DisplayActionSheet("Khách hàng chưa chọn quốc tịch", "Hủy", "Thêm quốc tịch");
                    if (asw == "Thêm quốc tịch")
                    {
                        if (!string.IsNullOrEmpty(viewModel.Reservation.purchaser_contact_name))
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new ContactForm(Guid.Parse(viewModel.Customer.Val)));
                        }
                        else
                        {
                            await App.Current.MainPage.Navigation.PushAsync(new AccountForm(Guid.Parse(viewModel.Customer.Val)));
                        }
                    }
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Tạo lịch thanh toán thất bại. Vui lòng thử lại");
                }    
            }
        }

        private async void CompletedReservation(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                viewModel.Reservation.bsd_reservationformstatus = 100000002;
                viewModel.Reservation.bsd_rfsigneddate = DateTime.Now;
                if (await viewModel.UpdateQuotes(viewModel.UpdateReservation))
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Phiếu đặt cọc đã được ký");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Ký phiếu đặt cọc thất bại. Vui lòng thử lại");
                }
            }
        }

        private async void SignQuotationClicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.InstallmentList.Count == 0)
            {
                ToastMessageHelper.ShortMessage("Vui lòng tạo lịch thanh toán");
                LoadingHelper.Hide();
                return;
            }
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                if (await viewModel.SignQuotation())
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Bảng tính giá đã được ký");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Ký bảng tính giá thất bại. Vui lòng thử lại");
                }
            }
        }

        private async void CancelQuotes(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string options = await DisplayActionSheet("Hủy Bảng Tính Giá", "Đóng", "Xác nhận");
            if (options == "Xác nhận")
            {
                viewModel.Reservation.statecode = 3;
                viewModel.Reservation.statuscode = 6;
                if (await viewModel.UpdateQuotes(viewModel.UpdateQuote))
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Đã hủy bảng tính giá");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Hủy bảng tính giá thất bại. Vui lòng thử lại");
                }
            }
            LoadingHelper.Hide();
        }

        private Grid SetUpItem(string content)
        {
            Grid grid = new Grid();
            Label lb = new Label();
            lb.Text = content;
            lb.FontSize = 15;
            lb.TextColor = Color.FromHex("1399D5");
            lb.VerticalOptions = LayoutOptions.Center;
            lb.HorizontalOptions = LayoutOptions.End;
            lb.FontAttributes = FontAttributes.Bold;
            grid.Children.Add(lb);
            return grid;
        }

        private void Project_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Reservation.project_id != Guid.Empty)
            {
                LoadingHelper.Show();
                ProjectInfo projectInfo = new ProjectInfo(viewModel.Reservation.project_id);
                projectInfo.OnCompleted = async (IsSuccess) =>
                {
                    if (IsSuccess == true)
                    {
                        await Navigation.PushAsync(projectInfo);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin dự án");
                    }
                };
            }
        }

        private void SalesCompany_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Reservation.salescompany_accountid != Guid.Empty)
            {
                LoadingHelper.Show();
                AccountDetailPage newPage = new AccountDetailPage(viewModel.Reservation.salescompany_accountid);
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
                        ToastMessageHelper.ShortMessage("Không tìm thấy thông tin đại lý/sàn giao dịch");
                    }
                };
            }
        }

        private void Customer_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if(viewModel.Customer != null)
            {
                if(viewModel.Customer.Title == viewModel.CodeAccount)
                {
                    AccountDetailPage newPage = new AccountDetailPage(viewModel.Reservation.purchaser_accountid);
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
                else if (viewModel.Customer.Title == viewModel.CodeContact)
                {
                    ContactDetailPage newPage = new ContactDetailPage(viewModel.Reservation.purchaser_contactid);
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