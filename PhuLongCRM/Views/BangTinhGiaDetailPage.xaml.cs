using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Threading.Tasks;
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
                viewModel.ShowInstallmentList = false;
                viewModel.NumberInstallment = 0;
                viewModel.InstallmentList.Clear();

                await Task.WhenAll(
                    LoadDataChinhSach(ReservationId),
                    viewModel.LoadCoOwners(ReservationId)
                );
                viewModel.ButtonCommandList.Clear();
                SetUpButtonGroup();
                if (QueuesDetialPage.NeedToRefreshBTG.HasValue) QueuesDetialPage.NeedToRefreshBTG = true;
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
                await Task.WhenAll(
                    viewModel.LoadDiscounts(),
                    viewModel.LoadDiscountsPaymentScheme(),
                    viewModel.LoadDiscountsInternel(),
                    viewModel.LoadDiscountsExChange()
                    ) ;
                await viewModel.LoadHandoverCondition(ReservationId);
               // SutUpSpecialDiscount();
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

        private void SetUpButtonGroup()
        {
            //if (viewModel.Reservation.statuscode == 100000007 || viewModel.Reservation.statuscode == 100000000)
            //{
            //    viewModel.ButtonCommandList.Add(new FloatButtonItem("Hủy Đặt Cọc", "FontAwesomeSolid", "\uf05e", null, CancelDeposit));
            //}

            if (viewModel.Reservation.statuscode == 100000007)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.cap_nhat_bang_tinh_gia, "FontAwesomeRegular", "\uf044", null, EditQuotes));
                if (viewModel.InstallmentList.Count == 0)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.tao_lich_thanh_toan, "FontAwesomeRegular", "\uf271", null, CreatePaymentScheme));
                }
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xoa_lich_thanh_toan, "FontAwesomeRegular", "\uf1c3", null, CancelInstallment));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xac_nhan_in, "FontAwesomeSolid", "\uf02f", null, ConfirmSigning));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.huy_bang_tinh_gia, "FontAwesomeRegular", "\uf273", null, CancelQuotes));
                if (viewModel.InstallmentList.Count > 0 && viewModel.Reservation.bsd_quotationprinteddate != null)
                {
                    viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.ky_bang_tinh_gia, "FontAwesomeRegular", "\uf274", null, SignQuotationClicked));
                }
            }

            if (viewModel.Reservation.bsd_reservationformstatus == 100000001 && viewModel.Reservation.bsd_reservationprinteddate != null && viewModel.Reservation.bsd_reservationuploadeddate == null && viewModel.Reservation.bsd_rfsigneddate == null)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xac_nhan_tai_pdc, "FontAwesomeRegular", "\uf15c", null, ConfirmReservation));
            }
            if (viewModel.Reservation.bsd_reservationformstatus == 100000001 && viewModel.Reservation.bsd_reservationprinteddate != null && viewModel.Reservation.bsd_reservationuploadeddate != null && viewModel.Reservation.bsd_rfsigneddate == null)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.ky_phieu_dat_coc, "FontAwesomeRegular", "\uf274", null, CompletedReservation));
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
                    ToastMessageHelper.ShortMessage(Language.da_xoa_lich_thanh_toan);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.xoa_lich_thanh_toan_that_bai_vui_long_thu_lai);
                }
            }
        }

        private async void ConfirmSigning(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Reservation.bsd_quotationprinteddate.HasValue)
            {
                ToastMessageHelper.ShortMessage(Language.da_xac_nhan_in);
                LoadingHelper.Hide();
                return;
            }
            if (viewModel.InstallmentList.Count == 0)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_tao_lich_thanh_toan);
                LoadingHelper.Hide();
                return;
            }
            bool isSuccess = await viewModel.ConfirmSinging();
            if (isSuccess)
            {
                NeedToRefresh = true;
                NeedToRefreshInstallment = true;
                if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                OnAppearing();
                ToastMessageHelper.ShortMessage(Language.xac_nhan_in_thanh_cong);
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.xac_nhan_in_that_bai);
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
                    ToastMessageHelper.ShortMessage(Language.da_xac_nhan_tai_phieu_dat_coc);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.xac_nhan_tao_phieu_dat_coc_tat_bai_vui_long_thu_lai);
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
                    ToastMessageHelper.ShortMessage(Language.san_pham_dang_o_trang_thai_reserve_khong_the_tao_bang_tinh_gia);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_co_thong_tin_bang_tinh_gia);
                }
            };
        } 

        private async void CreatePaymentScheme(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            CrmApiResponse response = await viewModel.UpdatePaymentScheme();
            if (response.IsSuccess == true)
            {
                NeedToRefreshInstallment = true;
                OnAppearing();
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.tao_lich_thanh_toan_thanh_cong);
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.LongMessage(response.ErrorResponse.error.message);
                //if (IsSuccess == "Localization")
                //{
                //    string asw = await App.Current.MainPage.DisplayActionSheet("Khách hàng chưa chọn quốc tịch", "Hủy", "Thêm quốc tịch");
                //    if (asw == "Thêm quốc tịch")
                //    {
                //        if (!string.IsNullOrEmpty(viewModel.Reservation.purchaser_contact_name))
                //        {
                //            await App.Current.MainPage.Navigation.PushAsync(new ContactForm(Guid.Parse(viewModel.Customer.Val)));
                //        }
                //        else
                //        {
                //            await App.Current.MainPage.Navigation.PushAsync(new AccountForm(Guid.Parse(viewModel.Customer.Val)));
                //        }
                //    }
                //    LoadingHelper.Hide();
                //}
                //else
                //{
                //    LoadingHelper.Hide();
                //    ToastMessageHelper.ShortMessage("Tạo lịch thanh toán thất bại. Vui lòng thử lại");
                //}    
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
                    ToastMessageHelper.ShortMessage(Language.phieu_dat_coc_da_duoc_ky);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.ky_phieu_dat_coc_that_bai_vui_long_thu_lai);
                }
            }
        }

        private async void SignQuotationClicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.InstallmentList.Count == 0)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_tao_lich_thanh_toan);
                LoadingHelper.Hide();
                return;
            }
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                if (await viewModel.SignQuotation())
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                    if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                    if (QueuesDetialPage.NeedToRefreshBTG.HasValue) QueuesDetialPage.NeedToRefreshBTG = true;
                    if (QueuesDetialPage.NeedToRefreshDC.HasValue) QueuesDetialPage.NeedToRefreshDC = true;
                    if (UnitInfo.NeedToRefreshQuotation.HasValue) UnitInfo.NeedToRefreshQuotation = true;
                    if (UnitInfo.NeedToRefreshReservation.HasValue) UnitInfo.NeedToRefreshReservation = true;
                    this.Title = Language.dat_coc;
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.bang_tinh_gia_da_duoc_ky);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.ky_bang_tinh_gia_that_bai_vui_long_thu_lai);
                }
            }
        }

        private async void CancelQuotes(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string options = await DisplayActionSheet(Language.huy_bang_tinh_gia, Language.dong, Language.xac_nhan);
            if (options == Language.xac_nhan)
            {
                viewModel.Reservation.statecode = 3;
                viewModel.Reservation.statuscode = 6;
                if (await viewModel.UpdateQuotes(viewModel.UpdateQuote))
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                    if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                    if (QueuesDetialPage.NeedToRefreshBTG.HasValue) QueuesDetialPage.NeedToRefreshBTG = true;
                    if (UnitInfo.NeedToRefreshQuotation.HasValue) UnitInfo.NeedToRefreshQuotation = true;
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.da_huy_bang_tinh_gia);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.huy_bang_tinh_gia_that_bai_vui_long_thu_lai);
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
                        ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
                        ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
        }

        private void Collaborator_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ContactDetailPage contactDetailPage = new ContactDetailPage(viewModel.Reservation.collaborator_id);
            contactDetailPage.OnCompleted = async (isSuccess) => {
                if (isSuccess)
                {
                    await Navigation.PushAsync(contactDetailPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
            LoadingHelper.Hide();
        }

        private void CustomerReferral_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.CustomerReferral?.Title == "3")
            {
                ContactDetailPage contactDetailPage = new ContactDetailPage(Guid.Parse(viewModel.CustomerReferral.Val));
                contactDetailPage.OnCompleted = async (isSuccess) => {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(contactDetailPage);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    }
                };
            }
            else if (viewModel.CustomerReferral?.Title == "2")
            {
                AccountDetailPage accountDetailPage = new AccountDetailPage(Guid.Parse(viewModel.CustomerReferral.Val));
                accountDetailPage.OnCompleted = async (isSuccess) => {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(accountDetailPage);
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
                            ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
                            ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                        }
                    };
                }
            }
        }

        private async void CancelDeposit(object sender, EventArgs e)
        {
            if (viewModel.Reservation.quoteid != Guid.Empty)
            {
                if (await viewModel.CancelDeposit())
                {
                    NeedToRefresh = true;
                    OnAppearing();
                    if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                    if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                    if (DatCocList.NeedToRefresh.HasValue) DatCocList.NeedToRefresh = true;
                    if (QueuesDetialPage.NeedToRefreshDC.HasValue) QueuesDetialPage.NeedToRefreshDC = true;
                    if (UnitInfo.NeedToRefreshQuotation.HasValue) UnitInfo.NeedToRefreshQuotation = true;
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.da_huy_dat_coc);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.huy_dat_coc_that_bai_vui_long_thu_lai);
                }
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
        private void CloseContentPromotion_Tapped(object sender, EventArgs e)
        {
            ContentPromotion.IsVisible = false;
        }

        private async void stackLayoutPromotions_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = ((TapGestureRecognizer)((Label)sender).GestureRecognizers[0]).CommandParameter as OptionSet;
            if (item != null && item.Val != string.Empty)
            {
                if (viewModel.PromotionItem == null)
                {
                    await viewModel.LoadPromotionItem(item.Val);
                }
                else if (viewModel.PromotionItem.bsd_promotionid.ToString() != item.Val)
                {
                    await viewModel.LoadPromotionItem(item.Val);
                }
            }
            if (viewModel.PromotionItem != null)
                KhuyenMai_CenterPopup.ShowCenterPopup();
            LoadingHelper.Hide();
        }
        private void ContentHandoverCondition_Tapped(object sender, EventArgs e)
        {
            ContentHandoverCondition.IsVisible = false;
        }

        private async void HandoverConditionItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.HandoverConditionItem == null && viewModel.Reservation.handovercondition_id != Guid.Empty)
            {
                await viewModel.LoadHandoverConditionItem(viewModel.Reservation.handovercondition_id);
            }
            if (viewModel.HandoverConditionItem != null)
                ContentHandoverCondition.IsVisible = true;
            LoadingHelper.Hide();
        }
        private void ContentSpecialDiscount_Tapped(object sender, EventArgs e)
        {
            ContentSpecialDiscount.IsVisible = false;
        }

        private async void stackLayoutSpecialDiscount_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = ((TapGestureRecognizer)((Label)sender).GestureRecognizers[0]).CommandParameter as OptionSet;
            if (item != null && item.Val != string.Empty)
            {
                if (viewModel.DiscountSpecialItem == null)
                {
                    await viewModel.LoadDiscountSpecialItem(item.Val);
                }
                else if (viewModel.DiscountSpecialItem.bsd_discountspecialid.ToString() != item.Val)
                {
                    await viewModel.LoadDiscountSpecialItem(item.Val);
                }
            }
            if (viewModel.DiscountSpecialItem != null)
                ContentSpecialDiscount.IsVisible = true;
            LoadingHelper.Hide();
        }

        private void CloseContentDiscount_Tapped(object sender, EventArgs e)
        {
            ContentDiscount.IsVisible = false;
        }

        private async void Discount_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (Guid)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await viewModel.LoadDiscountItem(item);
            if (viewModel.Discount != null)
                ContentDiscount.IsVisible = true;
            LoadingHelper.Hide();
        }

        private void Unit_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var unitId = (Guid)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            UnitInfo unit = new UnitInfo(unitId);
            unit.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(unit);
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