using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
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
    public partial class PhanHoiDetailPage : ContentPage
    {
        public Action<bool> OnCompleted;
        private Guid CaseId;
        PhanHoiDetailPageViewModel viewModel;
        public static bool? NeedToRefresh = null;
        public PhanHoiDetailPage(Guid id)
        {
            InitializeComponent();
            LoadingHelper.Show();
            CaseId = id;
            BindingContext = viewModel = new PhanHoiDetailPageViewModel();
            centerModalUpdateCase.Body.BindingContext = viewModel;
            NeedToRefresh = false;
            Tab_Tapped(1);
            Init();
        }

        public async void Init()
        {
            await LoadDataThongTin(CaseId);
            SetPreOpen();
            viewModel.ButtonCommandList.Add(new FloatButtonItem("Hủy phản hồi", "FontAwesomeRegular", "\uf273", null, CancelCase));
            viewModel.ButtonCommandList.Add(new FloatButtonItem("Giải quyết phản hồi", "FontAwesomeRegular", "\uf274", null, CompletedCase));
            viewModel.ButtonCommandList.Add(new FloatButtonItem("Chỉnh sửa", "FontAwesomeRegular", "\uf044", null,Update));

            if (viewModel.Case.incidentid != Guid.Empty)
                OnCompleted?.Invoke(true);
            else
                OnCompleted?.Invoke(false);
            LoadingHelper.Hide();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if(NeedToRefresh == true)
            {
                LoadingHelper.Show();
                await viewModel.LoadCase(CaseId);
                LoadingHelper.Hide();
                NeedToRefresh = false;
            }    
        }

        public void SetPreOpen()
        {
            Lookup_ResolutionType.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.ResolutionTypes = CaseResolutionType.CaseResolutionTypeData();
                LoadingHelper.Hide();
            };
            Lookup_BillableTime.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.BillableTimes = CaseBillableTime.CaseBillableTimeData();
                LoadingHelper.Hide();
            };
        }

        // tab thong tin
        private async Task LoadDataThongTin(Guid Id)
        {
            if (Id != Guid.Empty && viewModel.Case.incidentid == Guid.Empty)
            {
                await viewModel.LoadCase(Id);
            }
        }

        // phan hoi lien quan
        private async Task LoadDataPhanHoiLienQuan(Guid Id)
        {
            if (viewModel.ListCase != null && viewModel.ListCase.Count <= 0)
            {
                await viewModel.LoadListCase(Id);
            }
        }

        private async void ShowMoreCase_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageCase++;
            await viewModel.LoadListCase(CaseId);
            LoadingHelper.Hide();
        }

        private void Update(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            PhanHoiForm newPage = new PhanHoiForm(viewModel.Case.incidentid);
            newPage.CheckPhanHoi = async (OnCompleted) =>
            {
                if (OnCompleted == true)
                {
                    await Navigation.PushAsync(newPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không tìm thấy thông tin. Vui lòng thử lại");
                }
            };
        }

        private async void CancelCase(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string options = await DisplayActionSheet("Hủy phản hồi", "Không", "Có", "Xác nhận hủy phản hồi");
            if (options == "Có")
            {
                viewModel.Case.statecode = 2;
                viewModel.Case.statuscode = 6;
                if (await viewModel.UpdateCase())
                {
                    await viewModel.LoadCase(viewModel.Case.incidentid);
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Đã hủy phản hồi");
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Hủy phản hồi thất bại. Vui lòng thử lại");
                }
            }
            else if (options == "Không")
            {
                LoadingHelper.Hide();
            }
        }

        private async void CompletedCase(object sender, EventArgs e)
        {
            await centerModalUpdateCase.Show();
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await centerModalUpdateCase.Hide();
        }

        private async void Confirm_Clicked(object sender, EventArgs e)
        {
            if(viewModel.ResolutionType == null || viewModel.ResolutionType.Val == string.Empty)
            {
                ToastMessageHelper.ShortMessage("Chưa chọn hướng giải quyết");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.subject))
            {
                ToastMessageHelper.ShortMessage("Chưa nhập phương án");
                return;
            }

            if (viewModel.BillableTime == null || viewModel.BillableTime.Val == string.Empty)
            {
                ToastMessageHelper.ShortMessage("Chưa chọn billable time");
                return;
            }

            if (await viewModel.UpdateCaseResolution())
            {
                await viewModel.LoadCase(viewModel.Case.incidentid);
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Phản hồi đã được giải quyết");
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Giải quyết phản hồi thất bại. Vui lòng thử lại");
            }
            await centerModalUpdateCase.Hide();
        }

        private async void MoLaiPhanHoi_Clicked(object sender, EventArgs e)
        {
            viewModel.Case.statecode = 0;
            viewModel.Case.statuscode = 1;
            if (await viewModel.UpdateCase())
            {
                await viewModel.LoadCase(viewModel.Case.incidentid);
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Đã mở lại phản hồi");
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Mở lại phản hồi thất bại. Vui lòng thử lại");
            }
        }

        private void ThongTin_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(1);
        }

        private async void PhanHoiLienQuan_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(2);
            await LoadDataPhanHoiLienQuan(CaseId);
        }

        private void Tab_Tapped(int tab)
        {
            if (tab == 1)
            {
                VisualStateManager.GoToState(radBorderThongTin, "Selected");
                VisualStateManager.GoToState(lbThongTin, "Selected");
                TabThongTin.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderThongTin, "Normal");
                VisualStateManager.GoToState(lbThongTin, "Normal");
                TabThongTin.IsVisible = false;
            }
            if (tab == 2)
            {
                VisualStateManager.GoToState(radBorderPhanHoiLienQuan, "Selected");
                VisualStateManager.GoToState(lbPhanHoiLienQuan, "Selected");
                TabPhanHoiLienQuan.IsVisible = true;
            }
            else
            {
                VisualStateManager.GoToState(radBorderPhanHoiLienQuan, "Normal");
                VisualStateManager.GoToState(lbPhanHoiLienQuan, "Normal");
                TabPhanHoiLienQuan.IsVisible = false;
            }
        }

        private void Customer_Tapped(object sender, EventArgs e)
        {
            if(viewModel.Case != null)
            {
                if(!string.IsNullOrWhiteSpace(viewModel.Case.accountId))
                {
                    AccountDetailPage newPage = new AccountDetailPage(Guid.Parse(viewModel.Case.accountId));
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
                else if (!string.IsNullOrWhiteSpace(viewModel.Case.contactId))
                {
                    ContactDetailPage newPage = new ContactDetailPage(Guid.Parse(viewModel.Case.contactId));
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