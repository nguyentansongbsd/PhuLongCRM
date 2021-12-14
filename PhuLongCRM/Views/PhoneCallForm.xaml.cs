using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PhoneCallForm : ContentPage
	{
        public Action<bool> OnCompleted;
        private bool IsInit;
        public PhoneCallViewModel viewModel;
        private Guid PhoneCallId;

        public PhoneCallForm()
        {
            InitializeComponent();            
            Init();
            Create();
        }
        public PhoneCallForm(Guid id)
        {
            InitializeComponent();
            Init();           
            PhoneCallId = id;
            Update();
        }

        public PhoneCallForm(Guid idCustomer, string nameCustomer, string codeCustomer)
        {
            InitializeComponent();
            Init();
            Create();
            viewModel.CallTo = new OptionSet { Val = idCustomer.ToString(), Label = nameCustomer, Title = codeCustomer };
            Lookup_CallTo.IsVisible = false;
            CustomerMapping.IsVisible = true;
            Lookup_CallTo_SelectedItemChange(null, null);
        }

        private void Init()
        {
            LoadingHelper.Show();
            BindingContext = viewModel = new PhoneCallViewModel();
            DatePickerStart.DefaultDisplay = DateTime.Now;
            DatePickerEnd.DefaultDisplay = DateTime.Now;
        }

        private void Create()
        {
            this.Title = "Tạo mới cuộc gọi";
            BtnSave.Text = "Thêm cuộc gọi";
            IsInit = true;
            BtnSave.Clicked += Create_Clicked;
        }

        private void Create_Clicked(object sender, EventArgs e)
        {
            SaveData(Guid.Empty);
        }

        private async void Update()
        {
            BtnSave.Text = "Cập nhật";
            BtnSave.Clicked += Update_Clicked;
            await viewModel.loadPhoneCall(this.PhoneCallId);
            await viewModel.loadFromTo(this.PhoneCallId);
            this.Title = "Cập nhật cuộc gọi";
            if (viewModel.PhoneCellModel.activityid != Guid.Empty)
            {
                OnCompleted?.Invoke(true);
                IsInit = true;
            }
            else
                OnCompleted?.Invoke(false);
        }

        private void Update_Clicked(object sender, EventArgs e)
        {
            SaveData(this.PhoneCallId);
        }

        private async void SaveData(Guid id)
        {
            if (string.IsNullOrWhiteSpace(viewModel.PhoneCellModel.subject))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập chủ đề cuộc gọi");
                return;
            }         
            if (viewModel.CallTo == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn người nhận cuộc gọi");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.PhoneCellModel.phonenumber))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số điện thoại");
                return;
            }
            if (viewModel.PhoneCellModel.scheduledstart == null || viewModel.PhoneCellModel.scheduledend == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian kết thúc và thời gian bắt đầu");
                    return;
            }
            if (viewModel.PhoneCellModel.scheduledstart != null && viewModel.PhoneCellModel.scheduledend != null)
            {
                if (this.compareDateTime(viewModel.PhoneCellModel.scheduledstart, viewModel.PhoneCellModel.scheduledend) != -1)
                {
                    ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian kết thúc lớn hơn thời gian bắt đầu");
                    return;
                }
            }

            LoadingHelper.Show();

            if (id == Guid.Empty)
            {
                if (await viewModel.createPhoneCall())
                {
                    if (Dashboard.NeedToRefreshPhoneCall.HasValue) Dashboard.NeedToRefreshPhoneCall = true;
                    if (ActivityList.NeedToRefreshPhoneCall.HasValue) ActivityList.NeedToRefreshPhoneCall = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
                    ToastMessageHelper.ShortMessage("Đã thêm cuộc gọi");                   
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Thêm cuộc gọi thất bại");
                }
            }
            else
            {
                if (await viewModel.UpdatePhoneCall(id))
                {
                    if (Dashboard.NeedToRefreshPhoneCall.HasValue) Dashboard.NeedToRefreshPhoneCall = true;
                    if (ActivityList.NeedToRefreshPhoneCall.HasValue) ActivityList.NeedToRefreshPhoneCall = true;
                    if (LichLamViecTheoThang.NeedToRefresh.HasValue) LichLamViecTheoThang.NeedToRefresh = true;
                    if (LichLamViecTheoTuan.NeedToRefresh.HasValue) LichLamViecTheoTuan.NeedToRefresh = true;
                    if (LichLamViecTheoNgay.NeedToRefresh.HasValue) LichLamViecTheoNgay.NeedToRefresh = true;
                    if (ContactDetailPage.NeedToRefreshActivity.HasValue) ContactDetailPage.NeedToRefreshActivity = true;
                    if (AccountDetailPage.NeedToRefreshActivity.HasValue) AccountDetailPage.NeedToRefreshActivity = true;
                    ToastMessageHelper.ShortMessage("Cập nhật thành công");
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Cập nhật cuộc gọi thất bại");
                }
            }
        }

        private int compareDateTime(DateTime? date, DateTime? date1)
        {
            if (date != null && date1 != null )
            {
                int result = DateTime.Compare(date.Value, date1.Value);
                if (result < 0)
                    return -1;
                else if (result == 0)
                    return 0;
                else
                    return 1;
            }
            if (date == null && date1 != null)
                return -1;
            if (date1 == null && date != null)
                return 1;
            return 0;
        }     

        private void DatePickerStart_DateSelected(object sender, EventArgs e)
        {
            if (IsInit)
            {
                if (viewModel.PhoneCellModel.scheduledend != null)
                {
                    if (this.compareDateTime(viewModel.PhoneCellModel.scheduledstart, viewModel.PhoneCellModel.scheduledend) != -1)
                    {
                        ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian kết thúc lớn hơn thời gian bắt đầu");
                        viewModel.PhoneCellModel.scheduledstart = viewModel.PhoneCellModel.scheduledend;
                    }
                }
            }
        }

        private void DatePickerEnd_DateSelected(object sender, EventArgs e)
        {
            if (IsInit)
            {
                if (viewModel.PhoneCellModel.scheduledstart != null)
                {
                    if (this.compareDateTime(viewModel.PhoneCellModel.scheduledstart, viewModel.PhoneCellModel.scheduledend) != -1)
                    {
                        ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian kết thúc lớn hơn thời gian bắt đầu");
                        viewModel.PhoneCellModel.scheduledend = viewModel.PhoneCellModel.scheduledstart;
                    }    
                }
                else
                {
                    ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian bắt đầu");
                }
            }
        }

        private async void Lookup_CallTo_SelectedItemChange(object sender, LookUpChangeEvent e)
        {
            if (viewModel.CallTo != null)
            {
                if (viewModel.CallTo.Title == viewModel.CodeLead)
                {
                    await viewModel.LoadOneLead(viewModel.CallTo.Val);
                }
                else if (viewModel.CallTo.Title == viewModel.CodeContac)
                {
                    await viewModel.loadOneContact(viewModel.CallTo.Val);
                }
                else if (viewModel.CallTo.Title == viewModel.CodeAccount)
                {
                    await viewModel.LoadOneAccount(viewModel.CallTo.Val);
                }
            }
            else
            {
                viewModel.PhoneCellModel.phonenumber = string.Empty;
            }
        }
    }
}