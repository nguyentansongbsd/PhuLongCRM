using PhuLongCRM.Helper;
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
    public partial class ContractDetailPage : ContentPage
    {
        private Guid ContractId;
        public Action<bool> OnCompleted;
        private ContractDetailPageViewModel viewModel;
        public ContractDetailPage(Guid id)
        {
            InitializeComponent();
            ContractId = id;
            BindingContext = viewModel = new ContractDetailPageViewModel();
            Tab_Tapped(1);
            Init();
        }

        public async void Init()
        {
            await viewModel.LoadContract(this.ContractId);
            if (viewModel.Contract.salesorderid != Guid.Empty)
            {
                OnCompleted?.Invoke(true);
            }
            else
                OnCompleted?.Invoke(false);
            LoadingHelper.Hide();
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

        //tab chính sách
        private async void LoadChinhSach()
        {
            if (viewModel.Contract.handovercondition_id == Guid.Empty)
            {
                LoadingHelper.Show();
                await viewModel.LoadHandoverCondition(this.ContractId);
                SetUpDiscount(viewModel.Contract.bsd_discounts);
                SutUpPromotions();
                SutUpSpecialDiscount();
                LoadingHelper.Hide();
            }
        }

        private void TongHop_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(1);
        }

        private void ChiTiet_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(2);
        }

        private void ChinhSach_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(3);
            LoadChinhSach();
        }

        private void Lich_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(4);
            LoadInstallmentList(this.ContractId);
        }

        private void Tab_Tapped(int tab)
        {
            if (tab == 1)
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
            if (tab == 2)
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
            if (tab == 3)
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

        private void SalesCompany_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Contract.salesagentcompany_id != Guid.Empty)
            {
                LoadingHelper.Show();
                AccountDetailPage newPage = new AccountDetailPage(viewModel.Contract.salesagentcompany_id);
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

        private async void SetUpDiscount(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                scrolltDiscount.IsVisible = true;
                if (viewModel.Contract.discountlist_id != Guid.Empty)
                {
                    await viewModel.LoadDiscounts();

                    var list_id = ids.Split(',');

                    foreach (var id in list_id)
                    {
                        OptionSet item = viewModel.ListDiscount.Single(x => x.Val == id);
                        if (item != null && !string.IsNullOrEmpty(item.Val))
                        {
                            stackLayoutDiscount.Children.Add(SetUpItemBorder(item.Label));
                        }
                    }
                }
            }
            else
            {
                scrolltDiscount.IsVisible = false;
            }
        }

        private async void SutUpSpecialDiscount()
        {
            if (viewModel.Contract.salesorderid != Guid.Empty)
            {
                await viewModel.LoadSpecialDiscount(this.ContractId);
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
        }

        private async void SutUpPromotions()
        {
            if (viewModel.Contract.salesorderid != Guid.Empty)
            {
                await viewModel.LoadPromotions(this.ContractId);
                if (viewModel.ListPromotion != null && viewModel.ListPromotion.Count > 0)
                {
                    stackLayoutPromotions.IsVisible = true;
                    foreach (var item in viewModel.ListPromotion)
                    {
                        if (!string.IsNullOrEmpty(item.Label))
                        {
                            stackLayoutPromotions.Children.Add(SetUpItem(item.Label));
                        }
                    }
                }
                else
                {
                    stackLayoutPromotions.IsVisible = false;
                }
            }
        }

        private RadBorder SetUpItemBorder(string content)
        {
            RadBorder rd = new RadBorder();
            rd.Padding = 5;
            rd.BorderColor = Color.FromHex("f1f1f1");
            rd.BorderThickness = 1;
            rd.CornerRadius = 5;
            Label lb = new Label();
            lb.Text = content;
            lb.FontSize = 15;
            lb.TextColor = Color.FromHex("1399D5");
            lb.VerticalOptions = LayoutOptions.Center;
            lb.HorizontalOptions = LayoutOptions.Center;
            lb.FontAttributes = FontAttributes.Bold;
            rd.Content = lb;
            return rd;
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

        private void UnitDetail_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var unitId = (Guid)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            UnitInfo unit = new UnitInfo(unitId);
            unit.OnCompleted = async (isSuccess) => {
                if (isSuccess)
                {
                    await Navigation.PushAsync(unit);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy thông tin sản phẩm");
                }
            };
        }
    }
}