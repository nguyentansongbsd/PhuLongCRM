using System;
using System.Collections.Generic;
using System.Linq;
using PhuLongCRM.Models;
using PhuLongCRM.Helper;
using PhuLongCRM.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeadForm : ContentPage
    {
        public Action<bool> CheckSingleLead { get; set; }
        public LeadFormViewModel viewModel;

        public LeadForm()
        {
            InitializeComponent();
            this.Title = "TẠO MỚI KH TIỀM NĂNG";
            Init();
            datePickerNgaySinh.DefaultDisplay = DateTime.Now;
            datePickerNgayCap.DefaultDisplay = DateTime.Now;
            
            viewModel.Rating = RatingData.GetRatingById("2");//mac dinh la warm
            viewModel.CustomerGroup = CustomerGroupData.GetCustomerGroupById("100000002"); // mac dinh la "Chua xac dinh"
        }
        public LeadForm(Guid Id)
        {
            InitializeComponent();
            this.Title = "CẬP NHẬT KH TIỀM NĂNG";
            btn_save_lead.Text = "CẬP NHẬT KHÁCH HÀNG";
            Init();
            viewModel.LeadId = Id;
            InitUpdate();
        }

        public async void Init()
        {
            this.BindingContext = viewModel = new LeadFormViewModel();
            centerModalAddress.Body.BindingContext = viewModel;
            SetPreOpen();
            lookUpDanhGia.HideClearButton();
            if (string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone))
            {
                mobilephone_text.Text = "+84-";
            }
            if (string.IsNullOrWhiteSpace(viewModel.singleLead.telephone1))
            {
                telephone1_text.Text = "+84-";
            }
            
            CheckSingleLead?.Invoke(true);
        }

        public async void InitUpdate()
        {
            await viewModel.LoadOneLead();

            if (viewModel.singleLead.leadid != Guid.Empty)
            {
                customerCode.IsVisible = true;
                viewModel.AddressComposite = viewModel.singleLead.address1_composite;
                viewModel.AddressLine1 = viewModel.singleLead.address1_line1;

                viewModel.IndustryCode = viewModel.list_industrycode_optionset.SingleOrDefault(x => x.Val == viewModel.singleLead.industrycode);
                viewModel.Rating = RatingData.GetRatingById(viewModel.singleLead.leadqualitycode.ToString());
                viewModel.CustomerGroup = CustomerGroupData.GetCustomerGroupById(viewModel.singleLead.bsd_customergroup);

                viewModel.Topic = new OptionSet() { Val = viewModel.singleLead._bsd_topic_value, Label = viewModel.singleLead.bsd_topic_label };

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_typeofidcard))
                {
                    viewModel.TypeIdCard = TypeIdCardData.GetTypeIdCardById(viewModel.singleLead.bsd_typeofidcard);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_area))
                {
                    viewModel.Area = AreaData.GetAreaById(viewModel.singleLead.bsd_area);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.new_gender))
                {
                    viewModel.Gender = viewModel.Genders.SingleOrDefault(x => x.Val == viewModel.singleLead.new_gender);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead.leadsourcecode))
                {
                    viewModel.LeadSource = LeadSourcesData.GetLeadSourceById(viewModel.singleLead.leadsourcecode);
                }

                if (!viewModel.singleLead.new_birthday.HasValue)
                {
                    datePickerNgaySinh.DefaultDisplay = DateTime.Now;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._transactioncurrencyid_value))
                {
                    OptionSet currency = new OptionSet()
                    {
                        Val = viewModel.singleLead._transactioncurrencyid_value,
                        Label = viewModel.singleLead.transactioncurrencyid_label
                    };
                    viewModel.SelectedCurrency = currency;
                }

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._campaignid_value))
                {
                    OptionSet campaign = new OptionSet()
                    {
                        Val = viewModel.singleLead._campaignid_value,
                        Label = viewModel.singleLead.campaignid_label
                    };
                    viewModel.SelectedCurrency = campaign;
                }

                CheckSingleLead?.Invoke(true);
            }

            else
                CheckSingleLead?.Invoke(false);
        }

        public void SetPreOpen()
        {
            lookUpKhuVuc.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.Areas = AreaData.Areas();
                LoadingHelper.Hide();
            };

            lookUpLoaiTheID.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.TypeIdCards = TypeIdCardData.TypeIdCards();
                LoadingHelper.Hide();
            };

            lookUpPhanNhom.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.CustomerGroups = CustomerGroupData.CustomerGroups();
                LoadingHelper.Hide();
            };

            lookUpTieuDe.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadTopics();
                LoadingHelper.Hide();
            };

            lookUpDanhGia.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.Ratings = RatingData.Ratings();
                LoadingHelper.Hide();
            };          

            lookUpLinhVuc.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.loadIndustrycode();
                LoadingHelper.Hide();
            };

            lookUpChienDich.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCampainsForLookup();
                if (viewModel.list_campaign_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được chiến dịch");
                }
                LoadingHelper.Hide();
            };

            lookUpCountry.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCountryForLookup();
                if (viewModel.list_country_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được quốc gia");
                }
                LoadingHelper.Hide();
            };

            lookUpProvince.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.loadProvincesForLookup();
                if (viewModel.list_province_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được tỉnh thành");
                }
                LoadingHelper.Hide();
            };

            lookUpDistrict.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.loadDistrictForLookup();
                if (viewModel.list_district_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được quận/huyện");
                }
                LoadingHelper.Hide();
            };

            lookUpLeadSource.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.LeadSources = LeadSourcesData.GetListSources();
                LoadingHelper.Hide();
            };
        }

        #region chua dung toi
        //private async Task<String> checkData()
        //{
        //    if (viewModel.singleLead._bsd_topic_value == null || string.IsNullOrWhiteSpace(viewModel.singleLead.fullname) || string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone))
        //    {
        //        return "Vui lòng nhập các trường bắt buộc";
        //    }

        //    if(!PhoneNumberFormatVNHelper.CheckValidate(viewModel.singleLead.mobilephone))
        //    {
        //        return "Số điện thoại sai định dạng";
        //    }

        //    if (viewModel.singleLead.new_birthday != null && (DateTime.Now.Year - DateTime.Parse(viewModel.singleLead.new_birthday.ToString()).Year < 18))
        //    {
        //        return "Khách hàng phải từ 18 tuổi";
        //    }

        //    //Kiem tra trùng tên - số điện thoại, tên - email
        //    await viewModel.Checkdata_identical_lock(viewModel.singleLead.fullname, viewModel.singleLead.mobilephone, viewModel.singleLead.emailaddress1, viewModel.singleLead.leadid);
        //    if (viewModel.single_Leadcheck != null)
        //    {
        //        if (viewModel.singleLead.fullname.Trim() == viewModel.single_Leadcheck.fullname && viewModel.singleLead.mobilephone == viewModel.single_Leadcheck.mobilephone)
        //        {
        //            return "Khách hàng - Số điện thoại đã tồn tại";
        //        }
        //        else if (viewModel.singleLead.fullname.Trim() == viewModel.single_Leadcheck.fullname && viewModel.singleLead.emailaddress1 == viewModel.single_Leadcheck.emailaddress1)
        //        {
        //            return "Khách hàng - Email đã tồn tại";
        //        }
        //    }
        //    return "Sucesses";
        //}

        //private void MyNewDatePicker_DateChanged(object sender, EventArgs e)
        //{
        //    if (viewModel.singleLead.new_birthday != null && (DateTime.Now.Year - DateTime.Parse(viewModel.singleLead.new_birthday.ToString()).Year < 18))
        //    {
        //        Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Lỗi", "Khách hàng phải từ 18 tuổi", "OK");
        //        viewModel.singleLead.new_birthday = null;
        //    }
        //    viewModel.PhongThuy.gioi_tinh = viewModel.singleLead.new_gender != null ? Int32.Parse(viewModel.singleLead.new_gender) : 0;
        //    viewModel.PhongThuy.nam_sinh = viewModel.singleLead.new_birthday.HasValue ? viewModel.singleLead.new_birthday.Value.Year : 0;
        //}
        #endregion

        private void TypeIdCard_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (viewModel.TypeIdCard?.Val == "100000000" && viewModel.singleLead.bsd_identitycardnumberid.Length > 9)// CMND
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Giới hạn 09 ký tự).");
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && viewModel.singleLead.bsd_identitycardnumberid.Length > 12)// CCCD
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Giới hạn 12 ký tự).");
            }
            if (viewModel.TypeIdCard?.Val == "100000003" && viewModel.singleLead.bsd_identitycardnumberid.Length > 8)// Passport
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Giới hạn 08 ký tự).");
            }
        }

        private void mobilephone_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.singleLead.mobilephone.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Giới hạn 10 ký tự)");
            }
        }

        private void telephone1_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.singleLead.telephone1.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Giới hạn 10 ký tự)");
            }
        }

        private async void Address_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.AddressCountry == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.address1_country))
            {
                viewModel.AddressCountry = await viewModel.LoadCountryByName();
            }

            if (viewModel.AddressStateProvince == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.address1_stateorprovince))
            {
                viewModel.AddressStateProvince = await viewModel.LoadProvinceByName(); ;
            }

            if (viewModel.AddressCity == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.address1_city))
            {
                viewModel.AddressCity = await viewModel.LoadDistrictByName();
            }

            await centerModalAddress.Show();
            LoadingHelper.Hide();
        }

        private async void CloseAddress_Clicked(object sender, EventArgs e)
        {
            await centerModalAddress.Hide();
        }

        private async void Country_Changed(object sender, EventArgs e)
        {
            await viewModel.loadProvincesForLookup();
        }

        private async void Province_Changed(object sender, EventArgs e)
        {
            await viewModel.loadDistrictForLookup();
        }

        private async void District_Changed(object sender, EventArgs e)
        {

        }

        private async void ConfirmAddress_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(viewModel.AddressLine1))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.AddressLine1))
            {
                address.Add(viewModel.AddressLine1);
            }

            if (viewModel.AddressCity != null)
            {
                address.Add(viewModel.AddressCity.Name);
            }

            if (viewModel.AddressStateProvince != null)
            {
                address.Add(viewModel.AddressStateProvince.Name);
            }           

            if (viewModel.AddressCountry != null)
            {
                address.Add(viewModel.AddressCountry.Name);
            }

            viewModel.AddressComposite = string.Join(",", address);
            await centerModalAddress.Hide();
        }

        private void ClearAddress_Tapped(object sender, EventArgs e)
        {
            viewModel.AddressComposite = null;
            viewModel.AddressLine1 = null;
            viewModel.AddressCity = null;
            viewModel.AddressStateProvince = null;
            viewModel.AddressCountry = null;

            viewModel.singleLead.address1_line1 = null;
            viewModel.singleLead.address1_city = null;
            viewModel.singleLead.address1_stateorprovince = null;
            viewModel.singleLead.address1_country = null;
            viewModel.singleLead.address1_composite = null;
        }

        private async void SaveLead_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Topic == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn tiêu đề");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singleLead.lastname))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập họ tên");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số điện thoại");
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleLead.mobilephone) && viewModel.singleLead.mobilephone.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Giới hạn 10 ký tự)");
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleLead.telephone1) && viewModel.singleLead.telephone1.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại công ty không hợp lệ (Giới hạn 10 ký tự)");
            }

            if (viewModel.CustomerGroup == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn phân nhóm");
                return;
            }

            if (viewModel.Area == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn khu vực");
                return;
            }

            if (viewModel.LeadSource == null)
            {
                ToastMessageHelper.ShortMessage("Vui lòng chọn nguồn khách hàng");
                return;
            }

            if (viewModel.singleLead.new_birthday != null && (DateTime.Now.Year - DateTime.Parse(viewModel.singleLead.new_birthday.ToString()).Year < 18))
            {
                ToastMessageHelper.ShortMessage("Khách hàng phải từ 18 tuổi");
                return ;
            }

            if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length > 9)// CMND
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Giới hạn 09 ký tự).");
                return;
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length > 12)// CCCD
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Giới hạn 12 ký tự).");
                return;
            }
            if (viewModel.TypeIdCard?.Val == "100000003" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length > 8)// Passport
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Giới hạn 08 ký tự).");
                return;
            }

            LoadingHelper.Show();

            viewModel.singleLead.address1_city = viewModel.AddressCity != null ? viewModel.AddressCity.Name : null;
            viewModel.singleLead.address1_stateorprovince = viewModel.AddressStateProvince != null ? viewModel.AddressStateProvince.Name : null;
            viewModel.singleLead.address1_country = viewModel.AddressCountry != null ? viewModel.AddressCountry.Name : null;

            viewModel.singleLead.address1_line1 = viewModel.AddressLine1;
            viewModel.singleLead.address1_composite = viewModel.AddressComposite;

            viewModel.singleLead.industrycode = viewModel.IndustryCode != null ? viewModel.IndustryCode.Val : null;
            viewModel.singleLead._transactioncurrencyid_value = viewModel.SelectedCurrency != null ? viewModel.SelectedCurrency.Val : null;
            viewModel.singleLead._campaignid_value = viewModel.Campaign != null ? viewModel.Campaign.Val : null;

            if (viewModel.LeadId == Guid.Empty)
            {
                var result = await viewModel.createLead();
                if (result.IsSuccess)
                {
                    if (Dashboard.NeedToRefreshLeads.HasValue) Dashboard.NeedToRefreshLeads = true;
                    if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                    ToastMessageHelper.ShortMessage("Thành công");
                    await Navigation.PopAsync();
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không thêm được khách hàng. Vui lòng thử lại");
                }
            }
            else
            {
                bool IsSuccess = await viewModel.updateLead();
                if (IsSuccess)
                {
                    if (CustomerPage.NeedToRefreshLead.HasValue) CustomerPage.NeedToRefreshLead = true;
                    if (LeadDetailPage.NeedToRefreshLeadDetail.HasValue) LeadDetailPage.NeedToRefreshLeadDetail = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.ShortMessage("Thành công");
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage("Không cập nhật được khách hàng. Vui lòng thử lại");
                }
            }
        }
    }
}