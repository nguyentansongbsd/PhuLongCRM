using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhanHoiForm : ContentPage
    {
        public Action<bool> CheckPhanHoi;
        public PhanHoiFormViewModel viewModel;

        public PhanHoiForm()
        {
            InitializeComponent();
            Init();
        }
        public PhanHoiForm(Guid incidentid)
        {
            InitializeComponent();
            Init();
            viewModel.IncidentId = incidentid;
            InitUpdate();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new PhanHoiFormViewModel();
            viewModel.TabsDoiTuong = new List<string>() { "Giữ chỗ,", "Bảng Tính Giá", "Hợp đồng" };
            SerPreOpen();
        }

        public async void InitUpdate()
        {
            await viewModel.LoadCase();
            if (viewModel.singlePhanHoi != null)
            {
                this.Title = "CẬP NHẬT PHẢN HỒI";
                buttonSave.Text = "Cập nhật phản hồi";
                CheckPhanHoi?.Invoke(true);
            }
            else
            {
                CheckPhanHoi?.Invoke(false);
            }
        }

        private void SerPreOpen()
        {
            lookupCaseType.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.CaseTypes = CaseTypeData.CasesData();
                LoadingHelper.Hide();
            };

            lookupSubjects.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadSubjects();
                LoadingHelper.Hide();
            };

            lookupCaseOrigin.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.CaseOrigins = CaseOriginData.Origins();
                LoadingHelper.Hide();
            };

            lookupCaseLienQuan.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                if (viewModel.IncidentId != Guid.Empty)//cap nhat load danh sach caselienquan khong chua case hien tai
                {
                    await viewModel.LoadCaseLienQuan(viewModel.IncidentId.ToString());
                }
                else
                {
                    await viewModel.LoadCaseLienQuan();
                }

                LoadingHelper.Hide();
            };

            lookupProjects.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.Projects = await viewModel.LoadProjects();
                LoadingHelper.Hide();
            };

            lookupUnits.PreOpenOneTime = false;
            lookupUnits.PreOpen = async () => {
                if (viewModel.Project == null)
                {
                    ToastMessageHelper.ShortMessage("Vui lòng chọn dự án");
                }
            };

            //multiTabsDoiTuong.PreOpenOneTime = false;
            //multiTabsDoiTuong.PreOpen = () =>
            //{
            //    LoadingHelper.Show();
            //    if (viewModel.Customer == null)
            //    {
            //        LoadingHelper.Hide();
            //        ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng");
            //        return;
            //    }
            //    LoadingHelper.Hide();
            //};
        }

        private async void CustomerItem_Changed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.DoiTuong = null;
            viewModel.Queues = null;
            viewModel.Quotes = null;
            viewModel.OptionEntries = null;
            if (viewModel.AllItemSourceDoiTuong != null)
            {
                viewModel.AllItemSourceDoiTuong.Clear();
            }

            await Task.WhenAll(
                    viewModel.LoadQueues(),
                    viewModel.LoadQuotes(),
                    viewModel.LoadOptionEntries()
                    );
            viewModel.AllItemSourceDoiTuong = new List<List<OptionSet>>() { viewModel.Queues, viewModel.Quotes, viewModel.OptionEntries };
            LoadingHelper.Hide();
        }

        private async void ProjectItem_Changed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.Unit = null;
            viewModel.Units = null;
            await viewModel.LoadUnits();
            LoadingHelper.Hide();
        }

        private async void SaveCase_Clicked(object sender, EventArgs e)
        {
            if (viewModel.CaseType == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn loại phản hồi");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singlePhanHoi.title))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập tiêu đề");
                return;
            }

            if (viewModel.Customer == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng");
                return;
            }

            LoadingHelper.Show();
            if (viewModel.singlePhanHoi.incidentid == Guid.Empty)
            {
                bool isSuccess = await viewModel.CreateCase();
                if (isSuccess)
                {
                    if (ListPhanHoi.NeedToRefresh.HasValue) ListPhanHoi.NeedToRefresh = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Tạo phản hồi thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Tạo phản hồi thất bại");
                }
            }
            else
            {
                bool isSuccess = await viewModel.UpdateCase();
                if (isSuccess)
                {
                    if (PhanHoiDetailPage.NeedToRefresh.HasValue) PhanHoiDetailPage.NeedToRefresh = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Cập nhật phản hồi thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Cập nhật phản hồi thất bại");
                }
            }
        }
    }
}
