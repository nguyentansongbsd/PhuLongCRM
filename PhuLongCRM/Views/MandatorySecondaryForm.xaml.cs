using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
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
    public partial class MandatorySecondaryForm : ContentPage
    {
        private MandatorySecondaryFormViewModel viewModel;
        public MandatorySecondaryForm(Guid id)
        {
            InitializeComponent();
            Init(id.ToString());
        }

        private async void Init(string id)
        {
            this.BindingContext = viewModel = new MandatorySecondaryFormViewModel();
            datePickerNgayHieuLucTu.DefaultDisplay = DateTime.Now;
            datePickerNgayHieuLucDen.DefaultDisplay = DateTime.Now;
            SetPreOpen();
            await viewModel.GetOneAccountById(id);
        }

        public void SetPreOpen()
        {
            Lookup_Account.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadContactsLookup();
                LoadingHelper.Hide();
            };
        }

        private async void AddMandatorySecondary_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(viewModel.mandatorySecondary.bsd_name))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập topic");
                return;
            }
            if (viewModel.Contact == null || viewModel.Contact.Id == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn người ủy quyền");
                return;
            }
            if (viewModel.mandatorySecondary.bsd_effectivedatefrom == null || viewModel.mandatorySecondary.bsd_effectivedateto == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn thời gian hiệu lực");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.mandatorySecondary.bsd_descriptionsvn))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập mô tả (VN)");
                return;
            }
            if (string.IsNullOrWhiteSpace(viewModel.mandatorySecondary.bsd_descriptionsen))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập mô tả (EN)");
                return;
            }
            LoadingHelper.Show();
            viewModel.mandatorySecondary.bsd_contactid = viewModel.Contact.Id;
            if(await viewModel.Save())
            {
                LoadingHelper.Hide();
                if (AccountDetailPage.NeedToRefreshMandatory.HasValue) AccountDetailPage.NeedToRefreshMandatory = true;
                await Navigation.PopAsync();
                ToastMessageHelper.ShortMessage("Đã tạo người uỷ quyền thành công");
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage("Tạo người uỷ quyền thất bại");
            }
        }

        private void Effectivedateto_DateSelected(object sender, EventArgs e)
        {
            if (viewModel.mandatorySecondary.bsd_effectivedatefrom == null)
                viewModel.mandatorySecondary.bsd_effectivedatefrom = DateTime.Now;
            if (this.compareDateTime(viewModel.mandatorySecondary.bsd_effectivedatefrom, viewModel.mandatorySecondary.bsd_effectivedateto) == -1)
            {
                viewModel.mandatorySecondary.bsd_effectivedateto = viewModel.mandatorySecondary.bsd_effectivedatefrom;
                ToastMessageHelper.ShortMessage("Ngày hết hiệu lực phải lớn hơn ngày bắt đầu");
            }
        }

        private void Effectivedatefrom_DateSelected(object sender, EventArgs e)
        {
            if (viewModel.mandatorySecondary.bsd_effectivedateto == null)
                viewModel.mandatorySecondary.bsd_effectivedateto = DateTime.Now;
            if (this.compareDateTime(viewModel.mandatorySecondary.bsd_effectivedatefrom,viewModel.mandatorySecondary.bsd_effectivedateto) == -1)
            {
                viewModel.mandatorySecondary.bsd_effectivedatefrom = viewModel.mandatorySecondary.bsd_effectivedateto;
                ToastMessageHelper.ShortMessage("Ngày hết hiệu lực phải lớn hơn ngày bắt đầu");
            }    
        }

        private int compareDateTime(DateTime? date, DateTime? date1)
        {
            if (date != null && date != null)
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
    }
}