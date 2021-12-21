using PhuLongCRM.Helper;
using PhuLongCRM.ViewModels;
using System;
using System.Linq;
using PhuLongCRM.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservationForm : ContentPage
    {
        public Action<int> CheckReservation;
        public ReservationFormViewModel viewModel;
        private List<string> newSelectedPromotionIds;
        private bool _isEnableCheck { get; set; }

        public ReservationForm(Guid quoteId)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ReservationFormViewModel();
            centerModalPromotions.Body.BindingContext = viewModel;
            centerModalCoOwner.Body.BindingContext = viewModel;
            viewModel.QuoteId = quoteId;
            InitUpdate();
        }

        public ReservationForm(Guid productId, OptionSet queue, OptionSet saleAgentCompany, string nameOfStaffAgent, OptionSet customer)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ReservationFormViewModel();
            centerModalPromotions.Body.BindingContext = viewModel;
            centerModalCoOwner.Body.BindingContext = viewModel;
            viewModel.ProductId = productId;
            viewModel.Queue = queue;

            if (viewModel.Queue == null)
            {
                lblGiuCho.IsVisible = false;
                lookupGiuCho.IsVisible = false;
            }

            if (saleAgentCompany != null && Guid.Parse(saleAgentCompany.Val) != Guid.Empty)
            {
                viewModel.SalesAgent = saleAgentCompany;
                lookupDaiLySanGiaoDich.IsEnabled = false;
            }
            if (!string.IsNullOrWhiteSpace(nameOfStaffAgent))
            {
                viewModel.Quote.bsd_nameofstaffagent = nameOfStaffAgent;
                entryNhanVienDaiLy.IsEnabled = false;
            }
            if (customer != null)
            {
                viewModel.Buyer = customer;
                lookupNguoiMua.IsEnabled = false;
                lookupNguoiMua.HideClearButton();
            }

            Init();
        }

        public async void Init()
        {
            await viewModel.LoadUnitInfor();
            if (viewModel.UnitInfor != null)
            {
                if (viewModel.UnitInfor.statuscode == "100000006") // 100000006 :  Reserve
                {
                    CheckReservation?.Invoke(1);
                    return;
                }
                if (viewModel.Queue != null)
                {
                    lookupGiuCho.IsEnabled = false;
                    lookupGiuCho.HideClearButton();
                }
                viewModel.PaymentSchemeType = PaymentSchemeTypeData.GetPaymentSchemeTypeById("100000000");
                await viewModel.LoadTaxCode();
                SetPreOpen();
                CheckReservation?.Invoke(0);
            }
            else
            {
                CheckReservation?.Invoke(2);
            }
        }

        public async void InitUpdate()
        {
            await viewModel.LoadQuote();
            if (viewModel.Quote != null)
            {
                this.Title = "CẬP NHẬT BẢNG TÍNH GIÁ";
                buttonSave.Text = "CẬP NHẬT BẢNG TÍNH GIÁ";
                lookupNguoiMua.IsEnabled = false;
                lookupGiuCho.IsEnabled = false;
                lookupDaiLySanGiaoDich.IsEnabled = false;
                lookUpCollaborator.IsEnabled = false;
                lookUpCustomerReferral.IsEnabled = false;
                entryNhanVienDaiLy.IsEnabled = false;
                _isEnableCheck = true;

                if (viewModel.Queue == null)
                { 
                    lblGiuCho.IsVisible = false;
                    lookupGiuCho.IsVisible = false;
                }

                await viewModel.CheckTaoLichThanhToan();
                Guid id = await viewModel.GetDiscountPamentSchemeListId(viewModel.PaymentScheme.Val);
                await Task.WhenAll(
                    viewModel.LoadDiscountChilds(),
                    viewModel.LoadDiscountChildsPaymentSchemes(id.ToString()),
                    viewModel.LoadDiscountChildsInternel(),
                    viewModel.LoadDiscountChildsExchange(),
                    viewModel.LoadHandoverCondition(),
                    viewModel.LoadPromotionsSelected(),
                    viewModel.LoadPromotions(),
                    viewModel.LoadCoOwners()
                    ) ;
                
                SetPreOpen();

                viewModel.PaymentSchemeType = PaymentSchemeTypeData.GetPaymentSchemeTypeById(viewModel.Quote.bsd_paymentschemestype);

                if (viewModel.IsHadLichThanhToan == true)
                {
                    lookupDieuKienBanGiao.IsEnabled = true;
                    lookupPhuongThucThanhToan.IsEnabled = true;
                    lookupChieuKhau.IsEnabled = true;
                }
                if (!string.IsNullOrWhiteSpace(viewModel.Quote.bsd_discounts))
                {
                    List<string> arrDiscounts = viewModel.Quote.bsd_discounts.Split(',').ToList();
                    for (int i = 0; i < viewModel.DiscountChilds.Count; i++)
                    {
                        for (int j = 0; j < arrDiscounts.Count; j++)
                        {
                            if (viewModel.DiscountChilds[i].Val == arrDiscounts[j])
                            {
                                viewModel.DiscountChilds[i].Selected = true;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.Quote.bsd_selectedchietkhaupttt))
                {
                    List<string> arrCKPTTT = viewModel.Quote.bsd_selectedchietkhaupttt.Split(',').ToList();
                    for (int i = 0; i < viewModel.DiscountChildsPaymentSchemes.Count; i++)
                    {
                        for (int j = 0; j < arrCKPTTT.Count; j++)
                        {
                            if (viewModel.DiscountChildsPaymentSchemes[i].Val == arrCKPTTT[j])
                            {
                                viewModel.DiscountChildsPaymentSchemes[i].Selected = true;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.Quote.bsd_interneldiscount))
                {
                    List<string> arrCKNoiBo = viewModel.Quote.bsd_interneldiscount.Split(',').ToList();
                    for (int i = 0; i < viewModel.DiscountChildsInternel.Count; i++)
                    {
                        for (int j = 0; j < arrCKNoiBo.Count; j++)
                        {
                            if (viewModel.DiscountChildsInternel[i].Val == arrCKNoiBo[j])
                            {
                                viewModel.DiscountChildsInternel[i].Selected = true;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(viewModel.Quote.bsd_exchangediscount))
                {
                    List<string> arrCKQuyDoi = viewModel.Quote.bsd_exchangediscount.Split(',').ToList();
                    for (int i = 0; i < viewModel.DiscountChildsExchanges.Count; i++)
                    {
                        for (int j = 0; j < arrCKQuyDoi.Count; j++)
                        {
                            if (viewModel.DiscountChildsExchanges[i].Val == arrCKQuyDoi[j])
                            {
                                viewModel.DiscountChildsExchanges[i].Selected = true;
                            }
                        }
                    }
                }
                this.CheckReservation?.Invoke(0);
                _isEnableCheck = false;
            }
            else
            {
                this.CheckReservation?.Invoke(2);
            }
        }

        private void SetPreOpen()
        {
            lookupDieuKienBanGiao.HideClearButton();
            lookupPhuongThucThanhToan.HideClearButton();
            lookupChieuKhau.PreOpenOneTime = false;

            if (viewModel.IsHadLichThanhToan)
            {
                lookupDieuKienBanGiao.PreOpenOneTime = false;
                lookupPhuongThucThanhToan.PreOpenOneTime = false;
                lookupChieuKhau.HideClearButton();

                lookupDieuKienBanGiao.PreOpen = () =>
                {
                    ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                };

                lookupPhuongThucThanhToan.PreOpen = () =>
                {
                    ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                };

                lookupChieuKhau.PreOpen = () =>
                {
                    ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                };
            }
            else
            {
                lookupDieuKienBanGiao.PreOpenAsync = async () =>
                {
                    LoadingHelper.Show();
                    await viewModel.LoadHandoverConditions();
                    LoadingHelper.Hide();
                };

                lookupPhuongThucThanhToan.PreOpenAsync = async () =>
                {
                    LoadingHelper.Show();
                    await viewModel.LoadPaymentSchemes();
                    LoadingHelper.Hide();
                };

                lookupChieuKhau.PreOpenAsync = async () =>
                {
                    LoadingHelper.Show();
                    if (viewModel.DiscountLists == null)
                    {
                        await viewModel.LoadDiscountList();
                    }

                    if (viewModel.DiscountLists == null) // dot mo ban khong co chieu khau
                    {
                        ToastMessageHelper.ShortMessage("Không có chiết khấu");
                    }
                    LoadingHelper.Hide();
                };
            }

            lookupChieuKhauQuyDoi.PreOpenAsync = async () => {
                LoadingHelper.Show();
                await viewModel.LoadDiscountExchangeList();
                LoadingHelper.Hide();
            };

            lookupChieuKhauNoiBo.PreOpenAsync = async () => {
                LoadingHelper.Show();
                await viewModel.LoadDiscountInternelList();
                LoadingHelper.Hide();
            };

            lookupLoaiGopDot.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.PaymentSchemeTypes = PaymentSchemeTypeData.PaymentSchemeTypes();
                LoadingHelper.Hide();
            };

            lookupQuanHe.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.Relationships = RelationshipCoOwnerData.RelationshipData();
                LoadingHelper.Hide();
            };

            lookupGiuCho.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadQueues();
                LoadingHelper.Hide();
            };

            lookupDaiLySanGiaoDich.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadSalesAgents();
                LoadingHelper.Hide();
            };

            lookUpCollaborator.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCollaboratorLookUp();
                LoadingHelper.Hide();
            };
            lookUpCustomerReferral.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCustomerReferralLookUp();
                LoadingHelper.Hide();
            };
        }

        private void ChinhSach_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderChinhSach, "Active");
            VisualStateManager.GoToState(radBorderTongHop, "InActive");
            VisualStateManager.GoToState(radBorderChiTiet, "InActive");
            VisualStateManager.GoToState(lblChinhSach, "Active");
            VisualStateManager.GoToState(lblTongHop, "InActive");
            VisualStateManager.GoToState(lblChiTiet, "InActive");
            contentChinhSach.IsVisible = true;
            contentTongHop.IsVisible = false;
            contentChiTiet.IsVisible = false;
        }

        private void TongHop_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderChinhSach, "InActive");
            VisualStateManager.GoToState(radBorderTongHop, "Active");
            VisualStateManager.GoToState(radBorderChiTiet, "InActive");
            VisualStateManager.GoToState(lblChinhSach, "InActive");
            VisualStateManager.GoToState(lblTongHop, "Active");
            VisualStateManager.GoToState(lblChiTiet, "InActive");
            contentChinhSach.IsVisible = false;
            contentTongHop.IsVisible = true;
            contentChiTiet.IsVisible = false;
        }

        private void ChiTiet_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radBorderChinhSach, "InActive");
            VisualStateManager.GoToState(radBorderTongHop, "InActive");
            VisualStateManager.GoToState(radBorderChiTiet, "Active");
            VisualStateManager.GoToState(lblChinhSach, "InActive");
            VisualStateManager.GoToState(lblTongHop, "InActive");
            VisualStateManager.GoToState(lblChiTiet, "Active");
            contentChinhSach.IsVisible = false;
            contentTongHop.IsVisible = false;
            contentChiTiet.IsVisible = true;
        }

        #region Handover Condition // Dieu kien ban giao
        private void HandoverCondition_SelectedItemChange(object sender, EventArgs e)
        {
            if (viewModel.IsHadLichThanhToan == true)
            {
                ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                return;
            }
            if (viewModel.HandoverCondition.bsd_byunittype == true && (viewModel.HandoverCondition._bsd_unittype_value != viewModel.UnitType))
            {
                ToastMessageHelper.ShortMessage("Không thể thêm điều kiện bàn giao");
                viewModel.HandoverCondition = null;
                return;
            }
        }
        #endregion

        #region PaymentScheme
        private async void PTTT_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            if (viewModel.PaymentScheme.Val != viewModel.Quote.paymentscheme_id.ToString())
            {
                if (viewModel.DiscountChildsPaymentSchemes.Any(x => x.Selected))
                {
                    var answer = await DisplayAlert("", "Bạn đang tích chọn chiết khấu theo PTTT, bạn có chắc chắn muốn thay đổi PTTT này?", "Đồng ý", "Hủy");
                    if (answer == false)
                    {
                        LoadingHelper.Hide();
                        return;
                    }
                }
                viewModel.DiscountChildsPaymentSchemes.Clear();
                var id = await viewModel.GetDiscountPamentSchemeListId(viewModel.PaymentScheme.Val);
                await viewModel.LoadDiscountChildsPaymentSchemes(id.ToString());
            }
            LoadingHelper.Hide();
        }

        private void DiscountChildPaymentSchemeItem_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (_isEnableCheck) return;
            if (viewModel.IsHadLichThanhToan == true)
            {
                ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                return;
            }
        }

        private void DiscountChildPaymentSchemeItem_Tapped(object sender, EventArgs e)
        {
            var item = (DiscountChildOptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item.IsExpired == true || item.IsNotApplied == true) return;

            item.Selected = !item.Selected;
        }

        private void LoaiGopDot_SelectedItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            if (viewModel.PaymentSchemeType?.Val == "100000001") // Type = Gop dau
            {
                datePickerNgayBatDauTinhLTT.IsVisible = true;
            }
            else {
                datePickerNgayBatDauTinhLTT.IsVisible = false;
            }
        }
        #endregion

        #region Discount list // Chiet Khau
        private async void DiscountListItem_Changed(object sender, EventArgs e)
        {
            LoadingHelper.Show();

            if (viewModel.DiscountList == null)
            {
                viewModel.DiscountChilds.Clear();
            }
            if (viewModel.DiscountChilds.Count == 0)
            {
                viewModel.DiscountChilds.Clear();
                await viewModel.LoadDiscountChilds();
            }
            LoadingHelper.Hide();
        }

        private void DiscountChildItem_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (_isEnableCheck) return;
            if (viewModel.IsHadLichThanhToan == true)
            {
                ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                return;
            }
        }

        private void DiscountChildItem_Tapped(object sender, EventArgs e)
        {
            var item = (DiscountChildOptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item.IsExpired == true || item.IsNotApplied == true) return;

            item.Selected = !item.Selected;
        }
        #endregion

        #region Discount Internel List / Chieu khau noi bo
        private async void DiscountInternelListItem_Changed(object sende, EventArgs e)
        {
            LoadingHelper.Show();

            if (viewModel.DiscountInternelList == null)
            {
                viewModel.DiscountChildsInternel.Clear();
            }
            if (viewModel.DiscountChildsInternel.Count == 0)
            {
                viewModel.DiscountChildsInternel.Clear();
                await viewModel.LoadDiscountChildsInternel();
            }
            LoadingHelper.Hide();
        }

        private void DiscountChildInternelItem_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (_isEnableCheck) return;
            if (viewModel.IsHadLichThanhToan == true)
            {
                ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                return;
            }
        }

        private void DiscountChildInternelItem_Tapped(object sender, EventArgs e)
        {
            var item = (DiscountChildOptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item.IsExpired == true || item.IsNotApplied == true) return;

            item.Selected = !item.Selected;
        }
        #endregion

        #region Promotion // Khuyen mai
        private async void Promotion_Tapped(object sender, EventArgs e)
        {
            //if (viewModel.IsHadLichThanhToan == true)
            //{
            //    ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
            //    return;
            //}
            LoadingHelper.Show();
            this.newSelectedPromotionIds = new List<string>();
            if (viewModel.Promotions.Count == 0)
            {
                await viewModel.LoadPromotions();
            }
            else
            {
                foreach (var itemPromotion in viewModel.Promotions)
                {
                    if (viewModel.SelectedPromotionIds.Count != 0 && viewModel.SelectedPromotionIds.Any(x => x == itemPromotion.Val))
                    {
                        itemPromotion.Selected = true;
                    }
                    else
                    {
                        itemPromotion.Selected = false;
                    }
                }
            }
            await centerModalPromotions.Show();
            LoadingHelper.Hide();
        }

        private void PromotionItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemPromotion = (OptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            itemPromotion.Selected = !itemPromotion.Selected;
            LoadingHelper.Hide();
        }

        private async void SaveSelectedPromotion_CLicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.QuoteId == Guid.Empty)
            {
                viewModel.PromotionsSelected.Clear();
                viewModel.SelectedPromotionIds.Clear();

                foreach (var itemPromotion in viewModel.Promotions)
                {
                    if (itemPromotion.Selected)
                    {
                        viewModel.PromotionsSelected.Add(itemPromotion);
                        viewModel.SelectedPromotionIds.Add(itemPromotion.Val);
                    }
                }
            }
            else
            {
                foreach (var item in viewModel.Promotions)
                {
                    if (item.Selected == true)
                    {
                        if (viewModel.SelectedPromotionIds.Count != 0 && viewModel.SelectedPromotionIds.Any(x => x != item.Val))
                        {
                            this.newSelectedPromotionIds.Add(item.Val);
                        }
                        else
                        {
                            this.newSelectedPromotionIds.Add(item.Val);
                        }
                    }
                }

                if (this.newSelectedPromotionIds.Count != 0)
                {
                    bool IsSuccess = await viewModel.AddPromotion(this.newSelectedPromotionIds);
                    if (IsSuccess)
                    {
                        this.newSelectedPromotionIds.Clear();
                        viewModel.PromotionsSelected.Clear();

                        foreach (var itemPromotion in viewModel.Promotions)
                        {
                            if (itemPromotion.Selected)
                            {
                                viewModel.PromotionsSelected.Add(itemPromotion);
                            }
                        }
                        if (BangTinhGiaDetailPage.NeedToRefresh.HasValue) BangTinhGiaDetailPage.NeedToRefresh = true;
                        ToastMessageHelper.ShortMessage("Thêm khuyến mãi thành công");
                    }
                    else
                    {
                        ToastMessageHelper.ShortMessage("Thêm khuyến mãi thất bại");
                    }
                }
            }
            await centerModalPromotions.Hide();
            LoadingHelper.Hide();
        }

        private async void UnSelect_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var conform = await DisplayAlert("Xác nhận", "Bạn có muốn xóa khuyến mãi không ?", "Đồng ý", "Hủy");
            if (conform == false)
            {
                LoadingHelper.Hide();
                return;
            }
            var item = (OptionSet)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (viewModel.QuoteId != Guid.Empty)
            {
                CrmApiResponse apiResponse = await viewModel.DeletePromotion(item.Val);
                if (apiResponse.IsSuccess)
                {
                    if (BangTinhGiaDetailPage.NeedToRefresh.HasValue) BangTinhGiaDetailPage.NeedToRefresh = true;
                    viewModel.PromotionsSelected.Remove(item);
                    viewModel.SelectedPromotionIds.Remove(item.Val);
                    ToastMessageHelper.ShortMessage("Xoá khuyễn mãi thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.LongMessage(apiResponse.ErrorResponse.error.message);
                }
            }
            else
            {
                viewModel.PromotionsSelected.Remove(item);
                viewModel.SelectedPromotionIds.Remove(item.Val);
            }
            LoadingHelper.Hide();
        }

        private void SearchPromotion_Pressed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var data = viewModel.Promotions.Where(x => x.Label.ToLower().Contains(viewModel.KeywordPromotion.ToLower())).ToList();
            viewModel.Promotions.Clear();
            foreach (var item in data)
            {
                viewModel.Promotions.Add(item);
            }
            LoadingHelper.Hide();
        }

        private async void SearchPromotion_TexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.KeywordPromotion))
            {
                LoadingHelper.Show();
                viewModel.Promotions.Clear();
                await viewModel.LoadPromotions();
                LoadingHelper.Hide();
            }
        }
        #endregion

        #region CK quy doi
        private async void DiscountListExchangeItem_Changed(object sende, EventArgs e)
        {
            LoadingHelper.Show();

            if (viewModel.DiscountExchangeList == null)
            {
                viewModel.DiscountChildsExchanges.Clear();
            }
            if (viewModel.DiscountChildsExchanges.Count == 0)
            {
                viewModel.DiscountChildsExchanges.Clear();
                await viewModel.LoadDiscountChildsExchange();
            }
            LoadingHelper.Hide();
        }

        private void DiscountChildExchangeItem_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (_isEnableCheck) return;
            if (viewModel.IsHadLichThanhToan == true)
            {
                ToastMessageHelper.ShortMessage("Đã có lịch thanh toán, không được chỉnh sửa");
                return;
            }
        }

        private void DiscountChildExchangeItem_Tapped(object sender, EventArgs e)
        {
            var item = (DiscountChildOptionSet)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item.IsExpired == true || item.IsNotApplied == true) return;

            item.Selected = !item.Selected;
        }
        #endregion

        #region Co Owner // Dong so huu
        private async void CoOwner_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.CoOwner = new CoOwnerFormModel();
            viewModel.TitleCoOwner = null;
            viewModel.CustomerCoOwner = null;
            viewModel.Relationship = null;
            await centerModalCoOwner.Show();
            LoadingHelper.Hide();
        }

        private async void UnSelectCoOwner_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (CoOwnerFormModel)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

            if (viewModel.QuoteId != Guid.Empty)
            {
                var conform = await DisplayAlert("Xác nhận", "Bạn có muốn xóa người đồng sở hữu này không ?", "Đồng ý", "Hủy");
                if (conform == false)
                {
                    LoadingHelper.Hide();
                    return;
                }
                var deleteResponse = await CrmHelper.DeleteRecord($"/bsd_coowners({item.bsd_coownerid})");
                if (deleteResponse.IsSuccess)
                {
                    viewModel.CoOwnerList.Remove(item);
                    ToastMessageHelper.ShortMessage("Xóa người đồng sở hữu thành công");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Xóa người đồng sở hữu thất bại");
                    return;
                }
            }
            else
            {
                viewModel.CoOwnerList.Remove(item);
            }
            LoadingHelper.Hide();
        }

        private async void UpdateCoOwner_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            centerModalCoOwner.Title = "Cập nhật đồng sở hữu";
            var item = (CoOwnerFormModel)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            viewModel.CoOwner = new CoOwnerFormModel();
            viewModel.TitleCoOwner = null;
            viewModel.CustomerCoOwner = null;
            viewModel.Relationship = null;
            viewModel.CoOwner = item;
            if (viewModel.CoOwner.contact_id != Guid.Empty)
            {
                viewModel.CustomerCoOwner = new OptionSet(viewModel.CoOwner.contact_id.ToString(), viewModel.CoOwner.contact_name) { Title = "2" };
            }

            if (viewModel.CoOwner.account_id != Guid.Empty)
            {
                viewModel.CustomerCoOwner = new OptionSet(viewModel.CoOwner.account_id.ToString(), viewModel.CoOwner.account_name) { Title = "3" };
            }

            viewModel.TitleCoOwner = viewModel.CoOwner.bsd_name;
            viewModel.Relationship = new OptionSet(viewModel.CoOwner.bsd_relationshipId.ToString(), viewModel.CoOwner.bsd_relationship);
            await centerModalCoOwner.Show();
            LoadingHelper.Hide();
        }

        private async void SaveCoOwner_CLicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.TitleCoOwner))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập tiêu đề");
                return;
            }

            if (viewModel.CustomerCoOwner == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng");
                return;
            }

            if (viewModel.Relationship == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn quan hệ");
                return;
            }

            if (viewModel.CustomerCoOwner?.Val == viewModel.Buyer?.Val)
            {
                ToastMessageHelper.ShortMessage("Khách hàng không được trùng với người mua");
                viewModel.CustomerCoOwner = null;
                return;
            }

            if (viewModel.CoOwner.bsd_coownerid == Guid.Empty && viewModel.CoOwnerList.Any(x => x.contact_id == Guid.Parse(viewModel.CustomerCoOwner?.Val) || x.account_id == Guid.Parse(viewModel.CustomerCoOwner?.Val)))
            {
                ToastMessageHelper.ShortMessage("Khách hàng đã được chọn");
                return;
            }

            LoadingHelper.Show();
            if (viewModel.CustomerCoOwner.Title == "2")
            {
                viewModel.CoOwner.contact_id = Guid.Parse(viewModel.CustomerCoOwner.Val);
                viewModel.CoOwner.contact_name = viewModel.CustomerCoOwner.Label;
            }

            if (viewModel.CustomerCoOwner.Title == "3")
            {
                viewModel.CoOwner.account_id = Guid.Parse(viewModel.CustomerCoOwner.Val);
                viewModel.CoOwner.account_name = viewModel.CustomerCoOwner.Label;
            }
            viewModel.CoOwner.bsd_name = viewModel.TitleCoOwner;
            viewModel.CoOwner.bsd_relationshipId = viewModel.Relationship.Val;
            viewModel.CoOwner.bsd_relationship = viewModel.Relationship.Label;

            if (viewModel.CoOwner.bsd_coownerid == Guid.Empty)
            {
                viewModel.CoOwner.bsd_coownerid = Guid.NewGuid();

                viewModel.CoOwnerList.Add(viewModel.CoOwner);
                if (viewModel.QuoteId != Guid.Empty)
                {
                    bool IsSuccess = await viewModel.AddCoOwer();
                    if (IsSuccess)
                    {
                        if (BangTinhGiaDetailPage.NeedToRefresh.HasValue) BangTinhGiaDetailPage.NeedToRefresh = true;
                        ToastMessageHelper.ShortMessage("Thêm đồng sở hữu thành công");
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage("Thêm đồng sở hữu thất bại");
                    }
                }
                await centerModalCoOwner.Hide();
                LoadingHelper.Hide();
            }
            else
            {
                if (viewModel.QuoteId == Guid.Empty)
                {
                    List<CoOwnerFormModel> coOwnerList = new List<CoOwnerFormModel>();
                    foreach (var item in viewModel.CoOwnerList)
                    {
                        if (viewModel.CoOwner.bsd_coownerid == item.bsd_coownerid)
                        {
                            item.bsd_name = viewModel.CoOwner.bsd_name;
                            item.contact_id = viewModel.CoOwner.contact_id;
                            item.contact_name = viewModel.CoOwner.contact_name;
                            item.account_id = viewModel.CoOwner.account_id;
                            item.account_name = viewModel.CoOwner.account_name;
                            item.bsd_relationshipId = viewModel.CoOwner.bsd_relationshipId;
                            item.bsd_relationship = viewModel.CoOwner.bsd_relationship;
                        }
                        coOwnerList.Add(item);
                    }
                    viewModel.CoOwnerList.Clear();
                    coOwnerList.ForEach(x => viewModel.CoOwnerList.Add(x));
                    await centerModalCoOwner.Hide();
                    LoadingHelper.Hide();
                }
                else
                {
                    CrmApiResponse response = await viewModel.UpdateCoOwner();
                    if (response.IsSuccess)
                    {
                        viewModel.CoOwnerList.Clear();
                        await viewModel.LoadCoOwners();
                        await centerModalCoOwner.Hide();
                        ToastMessageHelper.ShortMessage("Cập nhật đồng sở hữu thành công");
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.LongMessage(response.ErrorResponse.error.message);
                    }
                }
            }
        }
        #endregion

        private void Buyer_SelectedItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            if (viewModel.SalesAgent != null && (viewModel.SalesAgent.Val == viewModel.Buyer?.Val))
            {
                ToastMessageHelper.LongMessage("Người mua không được trùng với Đại lý/Sàn giao dịch. Vui lòng chọn lại.");
                viewModel.Buyer = null;
            }
            if (viewModel.CoOwnerList.Any(x => x.contact_id == Guid.Parse(viewModel.Buyer?.Val)) || viewModel.CoOwnerList.Any(x => x.account_id == Guid.Parse(viewModel.Buyer?.Val)))
            {
                ToastMessageHelper.ShortMessage("Khách hàng Co-Owner và khách hàng không được trùng.");
                viewModel.Buyer = null;
            }
            LoadingHelper.Hide();
        }

        private void SalesAgent_SelectedItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            if (viewModel.SalesAgent != null && viewModel.Buyer != null && viewModel.SalesAgent == viewModel.Buyer)
            {
                ToastMessageHelper.LongMessage("Đại lý/Sàn giao dịch không được trùng với Người mua. Vui lòng chọn lại.");
                viewModel.SalesAgent = null;
                return;
            }
            if (viewModel.SalesAgent != null && !string.IsNullOrWhiteSpace(viewModel.SalesAgent?.Val))
            {
                viewModel.Collaborator = null;
                viewModel.CustomerReferral = null;
            }
            LoadingHelper.Hide();
        }

        private void lookUpCollaborator_SelectedItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            if (viewModel.Collaborator != null && viewModel.Buyer != null && viewModel.Collaborator?.Id.ToString() == viewModel.Buyer?.Val)
            {
                ToastMessageHelper.LongMessage("Cộng tác viên không được trùng với Người mua. Vui lòng chọn lại.");
                viewModel.Collaborator = null;
                return;
            }
            if (viewModel.Collaborator != null && viewModel.Collaborator?.Id != Guid.Empty)
            {
                viewModel.SalesAgent = null;
                viewModel.CustomerReferral = null;
            }
            LoadingHelper.Hide();
        }

        private void lookUpCustomerReferral_SelectedItemChange(System.Object sender, PhuLongCRM.Models.LookUpChangeEvent e)
        {
            LoadingHelper.Show();
            if (viewModel.CustomerReferral != null && viewModel.Buyer != null &&  viewModel.CustomerReferral?.Id.ToString() == viewModel.Buyer?.Val)
            {
                ToastMessageHelper.LongMessage("Khách hàng giới thiệu không được trùng với Người mua. Vui lòng chọn lại.");
                viewModel.CustomerReferral = null;
                return;
            }
            if (viewModel.CustomerReferral != null && viewModel.CustomerReferral?.Id != Guid.Empty)
            {
                viewModel.SalesAgent = null;
                viewModel.Collaborator = null;
            }
            LoadingHelper.Hide();
        }

        private async void SaveQuote_Clicked(object sender, EventArgs e)
        {
            if (viewModel.HandoverCondition == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn điều kiện bàn giao");
                return;
            }
            if (viewModel.PaymentScheme == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn phương thức thanh toán");
                return;
            }
            if (viewModel.PaymentSchemeType?.Val == "100000001" && viewModel.Quote.bsd_startingdatecalculateofps == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn ngày bắt đầu tính LTT");
                return;
            }
            if (viewModel.Buyer == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.Quote.name))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập tiêu đề");
                return;
            }

            LoadingHelper.Show();

            if (viewModel.Quote.quoteid == Guid.Empty)
            {
                CrmApiResponse response = await viewModel.CreateQuote();
                if (response.IsSuccess)
                {
                    await Task.WhenAll(
                            viewModel.AddCoOwer(),
                            viewModel.AddPromotion(viewModel.SelectedPromotionIds),
                            viewModel.AddHandoverCondition()
                            );
                    viewModel.QuoteId = viewModel.Quote.quoteid;
                    CrmApiResponse responseQuoteProduct = await viewModel.CreateQuoteProduct();
                    if (responseQuoteProduct.IsSuccess)
                    {
                        CrmApiResponse responseGetTotal = await viewModel.GetTotal(viewModel.Quote.quoteid.ToString());
                        if (responseGetTotal.IsSuccess)
                        {
                            viewModel.TotalReservation = JsonConvert.DeserializeObject<TotalReservationModel>(responseGetTotal.Content);
                            CrmApiResponse apiResponse = await viewModel.UpdateQuote();
                            if (apiResponse.IsSuccess)
                            {
                                CrmApiResponse apiResponseQuoteProduct = await viewModel.UpdateQuoteProduct();
                                if (apiResponseQuoteProduct.IsSuccess == false)
                                {
                                    ToastMessageHelper.LongMessage(apiResponseQuoteProduct.ErrorResponse.error.message);
                                    LoadingHelper.Hide();
                                    return;
                                }
                                if (QueuesDetialPage.NeedToRefreshBTG.HasValue) QueuesDetialPage.NeedToRefreshBTG = true;
                                if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                                this.Title = buttonSave.Text = "CẬP NHẬT BẢNG TÍNH GIÁ";
                                ToastMessageHelper.ShortMessage("Tạo bảng tính giá thành công");
                                LoadingHelper.Hide();
                            }
                            else
                            {
                                ToastMessageHelper.LongMessage(apiResponse.ErrorResponse.error.message);
                                LoadingHelper.Hide();
                            }
                        }
                        else
                        {
                            ToastMessageHelper.LongMessage(responseGetTotal.ErrorResponse.error.message);
                            LoadingHelper.Hide();
                        }

                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.LongMessage(responseQuoteProduct.ErrorResponse.error.message);
                    }
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.LongMessage(response.ErrorResponse.error.message);
                }
            }
            else
            {
                CrmApiResponse responseGetTotal = await viewModel.GetTotal(viewModel.Quote.quoteid.ToString());
                if (responseGetTotal.IsSuccess)
                {
                    viewModel.TotalReservation = JsonConvert.DeserializeObject<TotalReservationModel>(responseGetTotal.Content);
                    if (viewModel.HandoverCondition_Update != null && (viewModel.HandoverCondition_Update?.Val != viewModel.HandoverCondition.Val))
                    {
                        var response = await CrmHelper.DeleteRecord($"/quotes({viewModel.QuoteId})/bsd_quote_bsd_packageselling({viewModel.HandoverCondition_Update.Val})/$ref");
                        if (response.IsSuccess)
                        {
                            bool isSuccess_Update = await viewModel.AddHandoverCondition();
                            if (isSuccess_Update)
                            {
                                viewModel.HandoverCondition_Update = viewModel.HandoverCondition;
                            }
                            else
                            {
                                LoadingHelper.Hide();
                                ToastMessageHelper.ShortMessage("Cập nhật điều kiện bàn giao thất bại");
                                return;
                            }
                        }
                        else
                        {
                            viewModel.HandoverCondition = viewModel.HandoverCondition_Update;
                        }
                    }

                    CrmApiResponse apiResponse = await viewModel.UpdateQuote();
                    if (apiResponse.IsSuccess)
                    {
                        CrmApiResponse apiResponseQuoteProduct = await viewModel.UpdateQuoteProduct();
                        if (apiResponseQuoteProduct.IsSuccess == false)
                        {
                            ToastMessageHelper.LongMessage(apiResponseQuoteProduct.ErrorResponse.error.message);
                            LoadingHelper.Hide();
                            return;
                        }
                        if (QueuesDetialPage.NeedToRefreshBTG.HasValue) QueuesDetialPage.NeedToRefreshBTG = true;
                        if (BangTinhGiaDetailPage.NeedToRefresh.HasValue) BangTinhGiaDetailPage.NeedToRefresh = true;
                        if (ReservationList.NeedToRefreshReservationList.HasValue) ReservationList.NeedToRefreshReservationList = true;
                        this.Title = buttonSave.Text = "CẬP NHẬT BẢNG TÍNH GIÁ";
                        ToastMessageHelper.ShortMessage("Cập nhật bảng tính giá thành công");
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        ToastMessageHelper.LongMessage(apiResponse.ErrorResponse.error.message);
                        LoadingHelper.Hide();
                    }
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.LongMessage(responseGetTotal.ErrorResponse.error.message);
                }
            }
        }

    }
}