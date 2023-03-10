using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
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
    public partial class QueueForm2 : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefresh;
        public QueueFormViewModel2 viewModel;

        public QueueForm2(QueueUnitModel queueUnit, bool fromProject = false)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new QueueFormViewModel2();

            if (UserLogged.AgentID != Guid.Empty)
            {
                LookUp agent = new LookUp { Id = UserLogged.AgentID, Name = UserLogged.AgentName };
                viewModel.DailyOption = agent;
                nhanVienDaiLy.IsEnabled = true;
            }
            viewModel.queueProject = fromProject;
            viewModel.QueueUnit = queueUnit;
            Init();
            CheckLimit();
        }

        public void Init()
        {
            NeedToRefresh = false;
            SetPreOpen();
        }
        public void SetPreOpen()
        {
            lookUpDaiLy.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadSalesAgentCompany();
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
        public async void CheckLimit()
        {
            var result = await viewModel.CheckLimit();
            if (result == 1)
            {
                OnCompleted?.Invoke(false);
                ToastMessageHelper.ShortMessage(Language.vuot_qua_so_luong_giu_cho_tren_san_pham_co_the_thuc_hien_duoc_cho_du_an_trong_hom_nay);
            }
            else if (result == 2)
            {
                OnCompleted?.Invoke(false);
                ToastMessageHelper.ShortMessage(Language.vuot_qua_so_luong_giu_cho_tren_san_pham_co_the_thuc_hien_trong_hom_nay);
            }
            else if (result == 3)
            {
                OnCompleted?.Invoke(false);
                ToastMessageHelper.ShortMessage(Language.vuot_qua_so_luong_giu_cho_co_the_thuc_hien_duoc_cho_1_san_pham);
            }
            else if (result == 4)
            {
                OnCompleted?.Invoke(false);
                ToastMessageHelper.ShortMessage(Language.khong_tim_thay_du_an);
            }
            else if (result == 0)
            {
                Create();
            }
        }
        public void Create()
        {
            this.Title = btnSave.Text = Language.tao_giu_cho;
            btnSave.Clicked += Create_Clicked;
            if (viewModel.queueProject)
            {
                if (viewModel.QueueUnit.project_id == Guid.Empty)
                {
                    OnCompleted?.Invoke(false);
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_du_an);
                }
                topic.Text = viewModel.QueueUnit.project_name + " - " + DateTime.Now.ToString("dd/MM/yyyy");
                viewModel.QueueUnit.bsd_queuingfee_format = StringFormatHelper.FormatCurrency(viewModel.QueueUnit.bsd_bookingfee);
                if (viewModel.Queue != null) viewModel.Queue.bsd_queuingfee = viewModel.QueueUnit.bsd_bookingfee;
                OnCompleted?.Invoke(true);
            } 
            else
            {
                if (viewModel.QueueUnit.unit_id == Guid.Empty)
                {
                    OnCompleted?.Invoke(false);
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_san_pham);
                }
                topic.Text = viewModel.QueueUnit.unit_name;
                if (viewModel.QueueUnit.phaseslaunch_id == Guid.Empty) // giữ chỗ sản phẩm có đợt mở bán, tiền giữ chỗ = 0
                {
                    if (viewModel.QueueUnit.bsd_queuingfee > 0)
                    {
                        viewModel.QueueUnit.bsd_queuingfee_format = StringFormatHelper.FormatCurrency(viewModel.QueueUnit.bsd_queuingfee);
                        if (viewModel.Queue != null) viewModel.Queue.bsd_queuingfee = viewModel.QueueUnit.bsd_queuingfee;
                    }
                    else if (viewModel.QueueUnit.bsd_bookingfee > 0)
                    {
                        viewModel.QueueUnit.bsd_queuingfee_format = StringFormatHelper.FormatCurrency(viewModel.QueueUnit.bsd_bookingfee);
                        if (viewModel.Queue != null) viewModel.Queue.bsd_queuingfee = viewModel.QueueUnit.bsd_bookingfee;
                    }
                }
                else
                {
                    viewModel.QueueUnit.bsd_queuingfee_format = StringFormatHelper.FormatCurrency(0);
                    if (viewModel.Queue != null) viewModel.Queue.bsd_queuingfee = 0;
                }
                OnCompleted?.Invoke(true);
            }
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            btnSave.Text = Language.dang_tao_giu_cho;
            await SaveData();
        }

        private async Task SaveData()
        {
            if (string.IsNullOrWhiteSpace(viewModel.Queue.name))
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_nhap_tieu_de_giu_cho);
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                return;
            }
            if (viewModel.Customer == null || string.IsNullOrWhiteSpace(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_khach_hang);
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                return;
            }
            if (viewModel.queueProject == false)
            {
                if (!await viewModel.CheckCustomer())// chỉ kiểm tra kh cho giữ chỗ sản phẩm
                {
                    ToastMessageHelper.ShortMessage(Language.khach_hang_da_tham_gia_giu_cho_cho_san_pham_nay);
                    LoadingHelper.Hide();
                    btnSave.Text = Language.tao_giu_cho;
                    return;
                }
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.DailyOption != null && viewModel.DailyOption.Id != Guid.Empty && viewModel.DailyOption.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage(Language.khach_hang_phai_khac_dai_ly_ban_hang);
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                return;
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.Collaborator != null && viewModel.Collaborator.Id != Guid.Empty && viewModel.Collaborator.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage(Language.khach_hang_phai_khac_cong_tac_vien);
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                return;
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.CustomerReferral != null && viewModel.CustomerReferral.Id != Guid.Empty && viewModel.CustomerReferral.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage(Language.khach_hang_phai_khac_khach_hang_gioi_thieu);
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                return;
            }
            string created = null;
            if (viewModel.queueProject)
                created = await viewModel.createQueueDraft(true,viewModel.QueueUnit.project_id);
            else
                created = await viewModel.createQueueDraft(false, viewModel.QueueUnit.unit_id);

            if (string.IsNullOrWhiteSpace(created))
            {
                if (ProjectInfo.NeedToRefreshQueue.HasValue) ProjectInfo.NeedToRefreshQueue = true;
                if (ProjectInfo.NeedToRefreshNumQueue.HasValue) ProjectInfo.NeedToRefreshNumQueue = true;
                if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                if (DirectSaleDetailTest.NeedToRefreshDirectSale.HasValue) DirectSaleDetailTest.NeedToRefreshDirectSale = true;
                if (UnitInfo.NeedToRefreshQueue.HasValue) UnitInfo.NeedToRefreshQueue = true;
                if (UnitInfo.NeedToRefresh.HasValue) UnitInfo.NeedToRefresh = true;
                if (Dashboard.NeedToRefreshQueue.HasValue) Dashboard.NeedToRefreshQueue = true;
                if (QueueList.NeedToRefresh.HasValue) QueueList.NeedToRefresh = true;
                await Navigation.PopAsync();
                ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                btnSave.Text = Language.tao_giu_cho;
                if (!string.IsNullOrWhiteSpace(viewModel.Error_message))
                    ToastMessageHelper.ShortMessage(viewModel.Error_message);
                else
                    ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai);
            }
        }
        private void lookUpDaiLy_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.DailyOption != null && viewModel.DailyOption.Id != Guid.Empty)
            {
                viewModel.Collaborator = null;
                viewModel.CustomerReferral = null;
            }
        }
        private void lookUpCollaborator_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.Collaborator != null && viewModel.Collaborator.Id != Guid.Empty)
            {
                viewModel.DailyOption = null;
                viewModel.CustomerReferral = null;
            }
        }
        private void lookUpCustomerReferral_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.CustomerReferral != null && viewModel.CustomerReferral.Id != Guid.Empty)
            {
                viewModel.DailyOption = null;
                viewModel.Collaborator = null;
            }
        }
    }
}