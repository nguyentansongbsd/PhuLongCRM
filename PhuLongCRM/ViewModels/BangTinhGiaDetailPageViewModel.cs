using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class BangTinhGiaDetailPageViewModel : BaseViewModel
    {
        private ReservationDetailPageModel _reservation;
        public ReservationDetailPageModel Reservation { get => _reservation; set { _reservation = value; OnPropertyChanged(nameof(Reservation)); } }
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; }

        public OptionSet _customer;
        public OptionSet Customer { get => _customer; set { _customer = value; OnPropertyChanged(nameof(Customer)); } }
        public ObservableCollection<ReservationCoownerModel> CoownerList { get; set; }
        public ObservableCollection<ReservationInstallmentDetailPageModel> InstallmentList { get; set; }

        private int _numberInstallment;
        public int NumberInstallment { get => _numberInstallment; set { _numberInstallment = value; OnPropertyChanged(nameof(NumberInstallment)); } }

        private bool _showInstallmentList;
        public bool ShowInstallmentList { get => _showInstallmentList; set { _showInstallmentList = value; OnPropertyChanged(nameof(ShowInstallmentList)); } }

        private StatusCodeModel _quoteStatus;
        public StatusCodeModel QuoteStatus { get=> _quoteStatus; set { _quoteStatus = value;OnPropertyChanged(nameof(QuoteStatus)); } }

        public ObservableCollection<OptionSet> ListDiscount { get; set; } = new ObservableCollection<OptionSet>();
        public ObservableCollection<OptionSet> ListPromotion { get; set; } = new ObservableCollection<OptionSet>();
        public List<OptionSet> ListSpecialDiscount { get; set; }
        
        
        public string UpdateQuote = "1";
        public string UpdateQuotation = "2";
        public string ConfirmReservation = "3";
        public string UpdateReservation = "4";
        public string CodeContact = LookUpMultipleTabs.CodeContac;
        public string CodeAccount = LookUpMultipleTabs.CodeAccount;
        public BangTinhGiaDetailPageViewModel()
        {
            CoownerList = new ObservableCollection<ReservationCoownerModel>();
            InstallmentList = new ObservableCollection<ReservationInstallmentDetailPageModel>();
            Reservation = new ReservationDetailPageModel();
            Customer = new OptionSet();
            ListSpecialDiscount = new List<OptionSet>();
            ButtonCommandList = new ObservableCollection<FloatButtonItem>();
        }

        #region Chinh Sach
        public async Task LoadReservation(Guid ReservationId)
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='quote'>
                                    <attribute name='name' />
                                    <attribute name='statecode' />
                                    <attribute name='statuscode' />
                                    <attribute name='quoteid' />
                                    <attribute name='bsd_reservationno' />  
                                    <attribute name='bsd_quotationnumber' />
                                    <attribute name='quotenumber' />
                                    <attribute name='bsd_numberofmonthspaidmf' />
                                    <attribute name='bsd_managementfee' />
                                    <attribute name='bsd_rejectdate' />
                                    <attribute name='bsd_rejectreason' />
                                    <attribute name='bsd_salesdepartmentreceiveddeposit' />
                                    <attribute name='bsd_receiptdate' />
                                    <attribute name='bsd_depositfeereceived' />
                                    <attribute name='bsd_detailamount' />
                                    <attribute name='bsd_discount' />
                                    <attribute name='bsd_packagesellingamount' />
                                    <attribute name='bsd_totalamountlessfreight' />
                                    <attribute name='bsd_landvaluededuction' />
                                    <attribute name='totaltax' />
                                    <attribute name='bsd_freightamount' />
                                    <attribute name='totalamount' />
                                    <attribute name='bsd_constructionarea' />
                                    <attribute name='bsd_followuplist' />
                                    <attribute name='bsd_unitstatus' />
                                    <attribute name='bsd_netusablearea' />
                                    <attribute name='bsd_actualarea'/>
                                    <attribute name='bsd_bookingfee' />
                                    <attribute name='bsd_depositfee' />
                                    <attribute name='bsd_totalamountpaid' />
                                    <attribute name='bsd_quotationprinteddate' />
                                    <attribute name='bsd_expireddateofsigningqf' />
                                    <attribute name='bsd_quotationsigneddate' />
                                    <attribute name='bsd_reservationtime' />
                                    <attribute name='bsd_deposittime' />
                                    <attribute name='bsd_nameofstaffagent' />
                                    <attribute name='bsd_referral' />
                                    <attribute name='bsd_reservationformstatus' />
                                    <attribute name='bsd_reservationprinteddate' />
                                    <attribute name='bsd_signingexpired' />
                                    <attribute name='bsd_rfsigneddate' />
                                    <attribute name='bsd_reservationuploadeddate' />
                                    <attribute name='bsd_discountlist' alias='bsd_discounttypeid'/>
                                    <attribute name='bsd_discounts'/>
                                    <order attribute='createdon' descending='true' />
                                    <link-entity name='account' from='accountid' to='customerid' link-type='outer' alias='aa'>
      	                                <attribute name='bsd_name' alias='purchaser_account_name'/>
    	                                <attribute name='accountid' alias='purchaser_accountid'/>
                                    </link-entity>
                                    <link-entity name='contact' from='contactid' to='customerid' link-type='outer' alias='ab'>
     	                                <attribute name='bsd_fullname' alias='purchaser_contact_name'/>
    	                                <attribute name='contactid' alias='purchaser_contactid'/>
                                    </link-entity>
                                    <link-entity name='bsd_project' from='bsd_projectid' to='bsd_projectid' link-type='outer' alias='ac'>
                                        <attribute name='bsd_name' alias='project_name'/>
                                        <attribute name='bsd_projectid' alias='project_id' />
                                        <attribute name='bsd_quotationvalidatetime' alias='quotationvalidate'/>
                                    </link-entity>
                                    <link-entity name='pricelevel' from='pricelevelid' to='pricelevelid' link-type='outer' alias='ad'>
                                        <attribute name='pricelevelid' alias='pricelevel_id_apply'/>
                                        <attribute name='name' alias='pricelevel_name_apply'/>
                                    </link-entity>
                                    <link-entity name='pricelevel' from='pricelevelid' to='bsd_pricelistphaselaunch' link-type='outer' alias='ae'>
                                        <attribute name='pricelevelid' alias='pricelevel_id_phaseslaunch'/>
                                        <attribute name='name' alias='pricelevel_name_phaseslaunch'/>
                                    </link-entity>
                                    <link-entity name='bsd_phaseslaunch' from='bsd_phaseslaunchid' to='bsd_phaseslaunchid' link-type='inner' alias='ak'>
                                        <attribute name='bsd_phaseslaunchid' alias='phaseslaunch_id'/>
                                        <attribute name='bsd_name' alias='phaseslaunch_name'/>
                                    </link-entity>
                                    <link-entity name='bsd_taxcode' from='bsd_taxcodeid' to='bsd_taxcode' link-type='outer' alias='af'>
                                        <attribute name='bsd_taxcodeid' alias='taxcode_id'/>
                                        <attribute name='bsd_name' alias='taxcode_name'/>
                                    </link-entity>
                                    <link-entity name='account' from='accountid' to='bsd_salessgentcompany' link-type='outer' alias='ag'>
                                        <attribute name='bsd_name' alias='salescompany_account_name'/>
                                        <attribute name='accountid' alias='salescompany_accountid'/>
                                    </link-entity>
                                    <link-entity name='opportunity' from='opportunityid' to='opportunityid' link-type='outer' alias='al'>
                                        <attribute name='name' alias='queue_name' />
                                        <attribute name='opportunityid' alias='queue_id' /> 
                                    </link-entity>
                                    <link-entity name='product' from='productid' to='bsd_unitno' link-type='outer' alias='ap'>
                                        <attribute name='name' alias='unit_name' />
                                        <attribute name='productid' alias='unit_id' />
                                    </link-entity>
                                    <filter type='and'>
	                                    <condition attribute='quoteid' operator='eq' uitype='quote' value='" + ReservationId + @"' />
	                                </filter>
                                  </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationDetailPageModel>>("quotes", fetchXml);
            if (result == null || result.value.Count == 0)
            {
                return;
            }

            Reservation = result.value.SingleOrDefault();

            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='quote'>
                                    <attribute name='name' />
                                    <attribute name='bsd_discounts'/>
                                    <attribute name='quoteid' />
                                    <order attribute='createdon' descending='true' />
                                    <link-entity name='bsd_paymentscheme' from='bsd_paymentschemeid' to='bsd_paymentscheme' link-type='outer' alias='apo'>
                                        <attribute name='bsd_name' alias='paymentscheme_name'/>
                                        <attribute name='bsd_paymentschemeid' alias='paymentscheme_id'/>
                                    </link-entity>
                                    <link-entity name='bsd_discounttype' from='bsd_discounttypeid' to='bsd_discountlist' link-type='outer' alias='ae'>
                                        <attribute name='bsd_name' alias='discountlist_name' />
                                    </link-entity>
                                    <filter type='and'>
	                                    <condition attribute='quoteid' operator='eq' uitype='quote' value='" + ReservationId + @"' />
	                                </filter>
                                  </entity>
                                </fetch>";

            var result2 = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationDetailPageModel>>("quotes", fetch);
            if (result2 != null && result2.value.Count > 0)
            {
                var data = result2.value.SingleOrDefault();
                Reservation.paymentscheme_id = data.paymentscheme_id;
                Reservation.paymentscheme_name = data.paymentscheme_name;
                Reservation.discountlist_name = data.discountlist_name;
            }
           
            if(!string.IsNullOrEmpty(Reservation.purchaser_account_name))
            {
                Customer.Val = Reservation.purchaser_accountid.ToString();
                Customer.Label = Reservation.purchaser_account_name;
                Customer.Title = CodeAccount;

            }
            else
            {
                Customer.Val = Reservation.purchaser_contactid.ToString();
                Customer.Label = Reservation.purchaser_contact_name;
                Customer.Title = CodeContact;
            }
            
            this.QuoteStatus = QuoteStatusCodeData.GetQuoteStatusCodeById(this.Reservation.statuscode.ToString());
        }

        public async Task LoadHandoverCondition(Guid ReservationId)
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_packageselling'>
                                        <attribute name='bsd_name' alias='Label' />
                                        <attribute name='bsd_packagesellingid' alias='Val' />
                                        <order attribute='bsd_name' descending='true' />
                                        <link-entity name='bsd_quote_bsd_packageselling' from='bsd_packagesellingid' to='bsd_packagesellingid' visible='false' intersect='true'>
                                            <link-entity name='quote' from='quoteid' to='quoteid' alias='ab'>
                                                <filter type='and'>
                                                    <condition attribute='quoteid' operator='eq' uitype='quote' value='" + ReservationId + @"' />
                                                </filter>
                                            </link-entity>
                                        </link-entity>
                                    </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_packagesellings", fetchXml);
            if (result == null || result.value.Count == 0) return;

            Reservation.handovercondition_id = Guid.Parse(result.value.FirstOrDefault().Val);
            Reservation.handovercondition_name = result.value.FirstOrDefault().Label ;
        }

        public async Task LoadSpecialDiscount(Guid ReservationId)
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_discountspecial'>
                                        <attribute name='bsd_discountspecialid' alias='Val'/>
                                        <attribute name='bsd_name'  alias='Label'/>
                                        <attribute name='createdon' />
                                        <order attribute='bsd_name' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='bsd_quote' operator='eq' uitype='quote' value='" + ReservationId + @"' />
                                        </filter>
                                    </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_discountspecials", fetchXml);
            if (result == null || result.value.Count == 0)
            {
                return;
            }
            ListSpecialDiscount = result.value;
        }

        public async Task LoadPromotions(Guid ReservationId)
        {
            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='bsd_promotion'>
                                        <attribute name='bsd_name' alias='Label' />
                                        <attribute name='bsd_promotionid' alias='Val' />
                                        <order attribute='createdon' descending='true' />
                                        <link-entity name='bsd_quote_bsd_promotion' from='bsd_promotionid' to='bsd_promotionid' visible='false' intersect='true'>
                                            <link-entity name='quote' from='quoteid' to='quoteid' alias='ab'>
                                                <filter type='and'>
                                                    <condition attribute='quoteid' operator='eq' uitype='quote' value='" + ReservationId + @"' />
                                                </filter>
                                            </link-entity>
                                        </link-entity>
                                    </entity>
                                </fetch>";

            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_promotions", fetchXml);
            if (result == null || result.value.Count == 0) return;
            foreach (var item in result.value)
            {
                this.ListPromotion.Add(item);
            }
        }

        public async Task LoadDiscounts()
        {
            if (string.IsNullOrWhiteSpace(this.Reservation.bsd_discounts)) return;
            string[] arrDiscounts = new string[] { };
            string conditionValue = string.Empty;
            arrDiscounts = this.Reservation.bsd_discounts.Split(',');
            for (int i = 0; i < arrDiscounts.Count(); i++)
            {
                conditionValue += $"<value uitype='bsd_discount'>{arrDiscounts[i]}</value>";
            }

            string fetchXml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                <entity name='bsd_discount'>
                                    <attribute name='bsd_discountid' alias='Val'/>
                                    <attribute name='bsd_name' alias='Label'/>
                                    <order attribute='bsd_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='bsd_discountid' operator='in'>
                                        {conditionValue}
                                      </condition>
                                    </filter>
                                </entity>
                            </fetch>";
            
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<OptionSet>>("bsd_discounts", fetchXml);
            if (result == null || result.value.Count == 0) return;
            foreach (var item in result.value)
            {
                this.ListDiscount.Add(item);
            }
        }

        public async Task LoadCoOwners(Guid ReservationId)
        {
            string xml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='bsd_coowner'>
                <attribute name='bsd_coownerid' />
                <attribute name='bsd_name' />
                <order attribute='bsd_name' descending='false' />
                <link-entity name='account' from='accountid' to='bsd_customer' visible='false' link-type='outer' alias='a_1324f6d5b214e911a97f000d3aa04914'>
                  <attribute name='bsd_name' alias='account_name' />
                </link-entity>
                <link-entity name='contact' from='contactid' to='bsd_customer' visible='false' link-type='outer' alias='a_6b0d05eeb214e911a97f000d3aa04914'>
                  <attribute name='bsd_fullname' alias='contact_name' />
                </link-entity>
                 <filter type='and'>
                      <condition attribute='bsd_reservation' operator='eq' uitype='quote' value='{ReservationId}' />
                  </filter>
              </entity>
            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationCoownerModel>>("bsd_coowners", xml);
            if (result != null)
            {
                var data = result.value;
                foreach (var x in result.value)
                {
                    if (!string.IsNullOrEmpty(x.account_name))
                    {
                        x.customer = x.account_name;
                    }
                    else
                    {
                        x.customer = x.contact_name;
                    }
                    CoownerList.Add(x);
                }
            }
        }

        #endregion

        #region Lich
        public async Task LoadInstallmentList(Guid ReservationId)
        {
            string xml = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='bsd_paymentschemedetail'>
                <attribute name='bsd_paymentschemedetailid' />
                <attribute name='bsd_name' />
                <attribute name='bsd_duedate' />
                <attribute name='statuscode' />
                <attribute name='bsd_amountofthisphase' />
                <attribute name='bsd_amountwaspaid' />
                <attribute name='bsd_depositamount' />
                <attribute name='bsd_ordernumber' />
                <attribute name='bsd_amountpercent' />
                <attribute name='bsd_managementamount' />
                <attribute name='bsd_maintenanceamount' />
                <order attribute='bsd_ordernumber' descending='false' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='0' />
                  <condition attribute='bsd_reservation' operator='eq' uitype='quote' value='{ReservationId}' />
                </filter>
              </entity>
            </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<ReservationInstallmentDetailPageModel>>("bsd_paymentschemedetails", xml);
            if (result == null || result.value.Count == 0)
                return;

            foreach (var x in result.value)
            {
                x.bsd_duedate = x.bsd_duedate.ToLocalTime();
                InstallmentList.Add(x);
            }
            NumberInstallment = InstallmentList.Count();
            if(NumberInstallment>0)
            {
                ShowInstallmentList = true;
            }    
            else
            {
                ShowInstallmentList = false;
            }    
        }
        #endregion

        public async Task<bool> ConfirmSinging()
        {
            string path = $"/quotes({Reservation.quoteid})";

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["bsd_quotationprinteddate"] = DateTime.Now.ToUniversalTime();
            data["bsd_expireddateofsigningqf"] = DateTime.Now.AddDays(this.Reservation.quotationvalidate).ToUniversalTime();

            CrmApiResponse apiResponse = await CrmHelper.PatchData(path, data);
            if (apiResponse.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SignQuotation()
        {
            var data = new{};
            var apiResponse = await CrmHelper.PostData($"/quotes({Reservation.quoteid})//Microsoft.Dynamics.CRM.bsd_Action_QuotationReservation_ConvertToReservation", data);

            if (apiResponse.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuotes(string option)
        {
            string path = $"/quotes({Reservation.quoteid})";

            IDictionary<string, object> data = new Dictionary<string, object>();
            if (option == UpdateQuote)
            {
                data["statecode"] = Reservation.statecode;
                data["statuscode"] = Reservation.statuscode;
            }
            
            if (option == ConfirmReservation)
            {
                data["bsd_reservationuploadeddate"] = Reservation.bsd_reservationuploadeddate.Value.ToUniversalTime(); ;
            }

            if (option == UpdateReservation)
            {
                data["bsd_reservationformstatus"] = Reservation.bsd_reservationformstatus;
                data["bsd_rfsigneddate"] = Reservation.bsd_rfsigneddate.Value.ToUniversalTime(); ;
            }

            CrmApiResponse apiResponse = await CrmHelper.PatchData(path, data);
            if (apiResponse.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> UpdatePaymentScheme()
        {
            if (Reservation.paymentscheme_id != Guid.Empty)
            {
                IDictionary<string, object> data = new Dictionary<string, object>();
                CrmApiResponse updateResponse = await CrmHelper.PostData($"/quotes({Reservation.quoteid})/Microsoft.Dynamics.CRM.bsd_Action_Resv_Gene_PMS", data);
                if (updateResponse.IsSuccess)
                {
                    return updateResponse.IsSuccess.ToString();
                }
                else
                {
                    if (updateResponse.GetErrorMessage().Contains("Localization") == true)
                    {
                        return "Localization";
                    }
                    else
                    {
                        return updateResponse.IsSuccess.ToString();
                    }
                    
                }
            }
            else
            {
                return "False";
            }
        }

        public async Task<bool> DeactiveInstallment()
        {
            if (InstallmentList == null || InstallmentList.Count == 0)
                await LoadInstallmentList(Reservation.quoteid);

            if (InstallmentList != null && InstallmentList.Count > 0)
            {
                int count = 0;
                foreach (var item in InstallmentList)
                {
                    if (item.bsd_paymentschemedetailid != Guid.Empty && await Deactive(item.bsd_paymentschemedetailid))
                        count++;
                }
                if (count == InstallmentList.Count)
                    return true;
                else
                    return false;
            } 
            else
            {
                return false;
            }    
        }

        public async Task<bool> Deactive(Guid installmentid)
        {
            string path = $"/bsd_paymentschemedetails({installmentid})";

            IDictionary<string, object> data = new Dictionary<string, object>();
            data["statecode"] = 1;
            data["statuscode"] = 2;

            CrmApiResponse apiResponse = await CrmHelper.PatchData(path, data);
            if (apiResponse.IsSuccess)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
