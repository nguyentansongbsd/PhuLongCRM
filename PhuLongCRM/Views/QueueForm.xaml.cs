using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QueueForm : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefresh;
        public QueueFormViewModel viewModel;
        public Guid QueueId;
        private bool from;
        public QueueForm(Guid unitId, bool fromDirectSale) // Direct Sales (add)
        {
            InitializeComponent();   
            Init();
            viewModel.UnitId = unitId;
            from = fromDirectSale;
            Create();
        }

        public void Init()
        {          
            this.BindingContext = viewModel = new QueueFormViewModel();
            NeedToRefresh = false;
            SetPreOpen();            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();
                Lookup_KhachHang.Refresh();
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
        }

        public async void SetPreOpen()
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
            if (viewModel.AccountsLookUp.Count <= 0)
            {
               // LoadingHelper.Show();
                await viewModel.LoadAccountsLookUp();
               // LoadingHelper.Hide();
            }
            if (viewModel.ContactsLookUp.Count <= 0)
            {
                //LoadingHelper.Show();
                await viewModel.LoadContactsLookUp();
               // LoadingHelper.Hide();
            }
        }

        public async void Create()
        {
            btnSave.Text = "Tạo Giữ Chỗ";
            btnSave.Clicked += Create_Clicked; ;
            this.Title = "Tạo Giữ Chỗ";
            viewModel.isUpdate = false;
            if (from)
            {
                await viewModel.LoadFromUnit(viewModel.UnitId);
                viewModel.createQueueDraft(false, viewModel.UnitId);
                topic.Text = viewModel.QueueFormModel.bsd_units_name;              
                if (viewModel.QueueFormModel.bsd_units_id != Guid.Empty)
                    OnCompleted?.Invoke(true);
                else
                    OnCompleted?.Invoke(false);
            }
            else
            {
                await viewModel.LoadFromProject(viewModel.UnitId);
                viewModel.createQueueDraft(true, viewModel.UnitId);
                topic.Text = viewModel.QueueFormModel.bsd_project_name +" - "+ DateTime.Now.ToString("dd/MM/yyyyy");                
                if (viewModel.QueueFormModel.bsd_project_id != Guid.Empty)
                    OnCompleted?.Invoke(true);
                else
                    OnCompleted?.Invoke(false);
            }            
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            btnSave.Text = "Đang Tạo Giữ Chỗ...";
            await SaveData(null);
        }

        private async Task SaveData(string id)
        {
            if (string.IsNullOrWhiteSpace(viewModel.QueueFormModel.name))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập tiêu đề của giữ chỗ");
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                return;
            }
            if (viewModel.Customer == null || string.IsNullOrWhiteSpace(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng tiềm năng");
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                return;
            }
            if (from)
            {
                if (!await viewModel.SetQueueTime())
                {
                    ToastMessageHelper.ShortMessage("Khách hàng đã tham gia giữ chỗ cho dự án này");
                    LoadingHelper.Hide();
                    btnSave.Text = "Tạo Giữ Chỗ";
                    return;
                }
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.DailyOption != null && viewModel.DailyOption.Id != Guid.Empty && viewModel.DailyOption.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage("Khách hàng phải khác Đại lý bán hàng");
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                return;
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.Collaborator != null && viewModel.Collaborator.Id != Guid.Empty && viewModel.Collaborator.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage("Khách hàng phải khác Cộng tác viên");
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                return;
            }
            if (viewModel.Customer != null && !string.IsNullOrWhiteSpace(viewModel.Customer.Val) && viewModel.CustomerReferral != null && viewModel.CustomerReferral.Id != Guid.Empty && viewModel.CustomerReferral.Id == Guid.Parse(viewModel.Customer.Val))
            {
                ToastMessageHelper.ShortMessage("Khách hàng phải khác Khách hàng giới thiệu");
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                return;
            }
            var created = await viewModel.UpdateQueue(viewModel.idQueueDraft);
            if (created)
            {
                if (ProjectInfo.NeedToRefreshQueue.HasValue) ProjectInfo.NeedToRefreshQueue = true;
                if (DirectSaleDetail.NeedToRefreshDirectSale.HasValue) DirectSaleDetail.NeedToRefreshDirectSale = true;
                if (UnitInfo.NeedToRefreshQueue.HasValue) UnitInfo.NeedToRefreshQueue = true;
                if (Dashboard.NeedToRefreshQueue.HasValue) Dashboard.NeedToRefreshQueue = true;
                await Navigation.PopAsync();       
                ToastMessageHelper.ShortMessage("Tạo giữ chỗ thành công");
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                btnSave.Text = "Tạo Giữ Chỗ";
                ToastMessageHelper.ShortMessage("Tạo giữ chỗ thất bại");
            }
        }

        private void lookUpDaiLy_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if(viewModel.DailyOption != null && viewModel.DailyOption.Id != Guid.Empty)
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