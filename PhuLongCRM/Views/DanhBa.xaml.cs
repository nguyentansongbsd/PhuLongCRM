using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;
using System.Linq;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Xamarin.Essentials;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Views
{
    public partial class DanhBa : ContentPage
    {
        public DanhBaViewModel viewModel;
        public DanhBa()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DanhBaViewModel();
            Init();
        }

        public async void Init()
        {
            LoadingHelper.Show();
            viewModel.isCheckedAll = false;
            viewModel.total = 0;
            viewModel.numberChecked = 0;
            await viewModel.LoadContacts();
            LoadingHelper.Hide();
        }

        public void reset()
        {
            LoadingHelper.Show();
            button_toLead.isVisible = false;
            viewModel.reset();
            viewModel.LoadContacts().GetAwaiter();
            LoadingHelper.Hide();
        }

        private void checkAll_IsCheckedChanged(object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            if (viewModel.Contacts != null && viewModel.Contacts.Count > 0)
            {
                foreach (var item in viewModel.Contacts)
                {
                    if (item.numberFormated != null && item.IsConvertToLead == false)
                    {
                        item.IsSelected = e.NewValue.Value;
                    }
                }
                if (e.NewValue.Value)
                {
                    viewModel.numberChecked = (int)(viewModel.Contacts.Where(x => x.IsSelected == true && x.IsConvertToLead == false)?.Count());
                    button_toLead.isVisible = viewModel.numberChecked > 0 ? true : false;
                }
                else
                {
                    viewModel.numberChecked = 0;
                    button_toLead.isVisible = false;
                }
            }

        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                listView.ItemsSource = viewModel.Contacts;
            }
            else
            {
                listView.ItemsSource = viewModel.Contacts.Where(x => x.numberFormated == null ||
                    x.Name.Replace("-","").Replace(" ","").Replace("(","").Replace(")","").IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0 
                    || x.numberFormated.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").IndexOf(e.NewTextValue, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var tapGes = (TapGestureRecognizer)grid.GestureRecognizers[0];
            var item = (Models.DanhBaItemModel)tapGes.CommandParameter;
            if (item.IsConvertToLead == false)
            {
                item.IsSelected = !item.IsSelected;
                viewModel.numberChecked = (int)(viewModel.Contacts.Where(x => x.IsSelected == true && x.IsConvertToLead == false)?.Count());
                button_toLead.isVisible = viewModel.numberChecked > 0 ? true : false;
                if (viewModel.numberChecked == viewModel.totalConactActive)
                    viewModel.isCheckedAll = true;
                else
                {
                    viewModel.isCheckedAll = false;
                }
            }
        }

        private async void ConvertToLead_Clicked(object sender, EventArgs e)
        {
            var SelectedContact = this.viewModel.Contacts.Where(x => x.IsSelected == true && x.IsConvertToLead == false);
            if (SelectedContact.Any() == false)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_contact_de_chuyen_sang_khach_hang_tiem_nang);
                return;
            }

            var choice = await DisplayAlert("", Language.chuyen_lien_he_thanh_khach_hang_tiem_nang, Language.chuyen, Language.huy);
            if (choice)
            {
                this.ConvertToLead(SelectedContact);
            }
        }

        private async void ConvertToLead(IEnumerable<DanhBaItemModel> SelectedContact)
        {
            LoadingHelper.Show();
            foreach (var i in SelectedContact)
            {
                var re = await createLead(new LeadFormModel()
                {
                    leadid = Guid.NewGuid(),
                    bsd_topic_label = Language.khach_hang_tiem_nang_tu_danh_ba,
                    lastname = i.Name,
                    mobilephone = i.numberFormated,
                });

                if (!re.IsSuccess)
                {
                    ToastMessageHelper.ShortMessage(re.ErrorResponse.error.message);
                    LoadingHelper.Hide();
                    return;
                }
            }
            ToastMessageHelper.ShortMessage(Language.chuyen_thanh_cong);
            if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
            this.reset();
        }

        public async Task<CrmApiResponse> createLead(LeadFormModel leadFormModel)
        {
            string path = "/leads";
            var content = await this.getContent(leadFormModel);
            CrmApiResponse result = await CrmHelper.PostData(path, content);
            return result;
        }

        private async Task<object> getContent(LeadFormModel leadFormModel)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            data["leadid"] = leadFormModel.leadid;
            data["subject"] = leadFormModel.bsd_topic_label;
            data["lastname"] = leadFormModel.lastname;
            data["bsd_topic@odata.bind"] = $"/bsd_topics({Config.OrgConfig.Lead_Topic})";
            data["leadsourcecode"] = "10"; //Orther
            data["telephone1"] = "+84";
            data["bsd_PermanentCountry@odata.bind"] = "/bsd_countries(" + Config.OrgConfig.PermanentCountry + ")";
            data["bsd_PermanentProvince@odata.bind"] = "/new_provinces(" + Config.OrgConfig.PermanentProvice + ")";
            data["bsd_PermanentDistrict@odata.bind"] = "/new_districts(" + Config.OrgConfig.PermanentDistrict + ")";
            data["bsd_permanenthousenumber"] = $"{Config.OrgConfig.PermanentHouseNumber}";
            data["bsd_permanentaddress1"] = $"{Config.OrgConfig.PermanentAddress}";

            if (leadFormModel.mobilephone.StartsWith("+84"))
            {
                data["mobilephone"] = leadFormModel.mobilephone.Replace("+","");
            }
            else if (leadFormModel.mobilephone.StartsWith("84"))
            {
                data["mobilephone"] = leadFormModel.mobilephone;
            }
            else if (leadFormModel.mobilephone.StartsWith("0"))
            {
                data["mobilephone"] = "84" + leadFormModel.mobilephone;
            }
            else
            {
                data["mobilephone"] = leadFormModel.mobilephone;
            }
            
            if (UserLogged.Id != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["bsd_employee@odata.bind"] = "/bsd_employees(" + UserLogged.Id + ")";
            }
            if (UserLogged.ManagerId != Guid.Empty && !UserLogged.IsLoginByUserCRM)
            {
                data["ownerid@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
                //data["bsd_importer@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
                data["createdby@odata.bind"] = "/systemusers(" + UserLogged.ManagerId + ")";
            }
            return data;
        }
    }
}
