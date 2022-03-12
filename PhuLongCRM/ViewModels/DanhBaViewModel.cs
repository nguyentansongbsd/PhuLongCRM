using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;

namespace PhuLongCRM.ViewModels
{
    public class DanhBaViewModel : BaseViewModel
    {
        public ObservableCollection<DanhBaItemModel> Contacts { get; set; }

        public ObservableCollection<LeadListModel> LeadConvert { get; set; } = new ObservableCollection<LeadListModel>();

        private bool _isCheckedAll;
        public bool isCheckedAll { get => _isCheckedAll; set { _isCheckedAll = value; OnPropertyChanged(nameof(isCheckedAll)); } }

        private int _numberChecked;
        public int numberChecked { get => _numberChecked; set { _numberChecked = value; totalChecked = value.ToString() + "/" + total.ToString(); OnPropertyChanged(nameof(numberChecked)); } }

        private int _total;
        public int total { get => _total; set { _total = value; totalChecked = numberChecked.ToString() + "/" + value.ToString(); OnPropertyChanged(nameof(total)); } }

        private string _totalChecked;
        public string totalChecked { get => _totalChecked; set { _totalChecked = Language.da_chon + " " + value; OnPropertyChanged(nameof(totalChecked)); } }

        public DanhBaViewModel()
        {
            Contacts = new ObservableCollection<DanhBaItemModel>();
            isCheckedAll = false;
            numberChecked = 0;
            total = 0;
        }

        public void reset()
        {
            Contacts.Clear();
            isCheckedAll = false;
            numberChecked = 0;
            total = 0;
        }

        public async Task LoadLeadConvert()
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='lead'>
                        <attribute name='lastname' />
                        <attribute name='subject' />
                        <attribute name='mobilephone'/>
                        <attribute name='emailaddress1' />
                        <attribute name='createdon' />
                        <attribute name='leadid' />
                        <attribute name='leadqualitycode' />
                        <order attribute='createdon' descending='true' />                      
                      </entity>
                    </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<LeadListModel>>("leads", fetch);
            if (result == null || result.value == null)
                return;
            var data = result.value;
            foreach (var item in data)
            {
                LeadConvert.Add(item);
            }
        }
    }
}
