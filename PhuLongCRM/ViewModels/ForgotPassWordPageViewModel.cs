using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;

namespace PhuLongCRM.ViewModels
{
    public class ForgotPassWordPageViewModel :BaseViewModel
    {
        private string _userName;
        public string UserName { get => _userName; set { _userName = value;OnPropertyChanged(nameof(UserName)); } }

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(nameof(NewPassword)); } }

        private string _confiemPassword;
        public string ConfirmPassword { get => _confiemPassword; set { _confiemPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); } }

        public EmployeeModel Employee { get; set; }

        private bool _sendToEmail;
        public bool SendToEmail { get => _sendToEmail; set { _sendToEmail = value; OnPropertyChanged(nameof(SendToEmail)); } }

        public ForgotPassWordPageViewModel()
        {

        }

        public async Task CheckUserName()
        {
            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='bsd_employee'>
                    <attribute name='bsd_employeeid' />
                    <attribute name='bsd_name' />
                    <filter type='and'>
                      <condition attribute='bsd_name' operator='eq' value='{UserName}' />
                    </filter>
                    <link-entity name='contact' from='contactid' to='bsd_contact' visible='false' link-type='outer' alias='a_d839fe53e057ec118f8f000d3ac949ac'>
                      <attribute name='mobilephone' alias='contact_phone'/>
                      <attribute name='emailaddress1' alias='contact_email'/>
                    </link-entity>
                  </entity>
                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<EmployeeModel>>("bsd_employees", fetchXml);
            if (result == null || result.value.Count == 0)
                Employee = null;
            else
                Employee = (result.value as List<EmployeeModel>).SingleOrDefault();
        }
    }
}
