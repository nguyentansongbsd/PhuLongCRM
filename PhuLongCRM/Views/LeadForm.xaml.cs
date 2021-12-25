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
            centerModalAddress1.Body.BindingContext = viewModel;
            centerModalAddress2.Body.BindingContext = viewModel;
            centerModalAddress3.Body.BindingContext = viewModel;
            SetPreOpen();
            lookUpDanhGia.HideClearButton();
            CheckSingleLead?.Invoke(true);
        }

        public async void InitUpdate()
        {
            await viewModel.LoadOneLead();

            if (viewModel.singleLead.leadid != Guid.Empty)
            {
                customerCode.IsVisible = true;
                lookUpLeadSource.IsEnabled = false;
                viewModel.CompositeAddress3 = viewModel.singleLead.bsd_accountaddressvn;
                viewModel.LineAddress3 = viewModel.singleLead.bsd_account_housenumberstreetwardvn;

                viewModel.CompositeAddress2 = viewModel.singleLead.bsd_permanentaddress1;
                viewModel.LineAddress2 = viewModel.singleLead.bsd_permanentaddress;

                viewModel.CompositeAddress1 = viewModel.singleLead.bsd_contactaddress;
                viewModel.LineAddress1 = viewModel.singleLead.bsd_housenumberstreet;

                if (!string.IsNullOrWhiteSpace(viewModel.singleLead._campaignid_value))
                    viewModel.Campaign = new OptionSet { Val = viewModel.singleLead._campaignid_value, Label = viewModel.singleLead.campaignid_label };

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

            lookUpCountryAddress1.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCountryForLookup();
                if (viewModel.list_country_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được quốc gia");
                }
                LoadingHelper.Hide();
            };

            lookUpCountryAddress2.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCountryForLookup();
                if (viewModel.list_country_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được quốc gia");
                }
                LoadingHelper.Hide();
            };

            lookUpCountryAddress3.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadCountryForLookup();
                if (viewModel.list_country_lookup.Count == 0)
                {
                    ToastMessageHelper.ShortMessage("Không load được quốc gia");
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

        private void MainEntry_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.TypeIdCard?.Val == "100000000" && viewModel.singleLead.bsd_identitycardnumberid.Length != 9)// CMND
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Gồm 09 ký tự).");
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && viewModel.singleLead.bsd_identitycardnumberid.Length > 12 ||
                viewModel.TypeIdCard?.Val == "100000001" && viewModel.singleLead.bsd_identitycardnumberid.Length < 9)// CCCD
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Gồm 12 ký tự).");
            }
            if (viewModel.TypeIdCard?.Val == "100000003" && viewModel.singleLead.bsd_identitycardnumberid.Length != 8)// Passport
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Gồm 08 ký tự).");
            }
        }

        private void mobilephone_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.singleLead.mobilephone.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Gồm 10 ký tự)");
            }
        }

        private void telephone1_text_Unfocused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (viewModel.singleLead.telephone1.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Giới hạn 10 ký tự)");
            }
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
                ToastMessageHelper.ShortMessage("Số điện thoại không hợp lệ (Gồm 10 ký tự)");
                return;
            }

            if (!string.IsNullOrWhiteSpace(viewModel.singleLead.telephone1) && viewModel.singleLead.telephone1.Length != 14)
            {
                ToastMessageHelper.ShortMessage("Số điện thoại công ty không hợp lệ (Gồm 10 ký tự)");
                return;
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

            if (viewModel.TypeIdCard?.Val == "100000000" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length != 9)// CMND
            {
                ToastMessageHelper.ShortMessage("Số CMND không hợp lệ (Gồm 09 ký tự).");
                return;
            }
            if (viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length > 12
                || viewModel.TypeIdCard?.Val == "100000001" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length < 9)// CCCD
            {
                ToastMessageHelper.ShortMessage("Số CCCD không hợp lệ (Gồm 12 ký tự).");
                return;
            }
            if (viewModel.TypeIdCard?.Val == "100000003" && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_identitycardnumberid) && viewModel.singleLead.bsd_identitycardnumberid.Length != 8)// Passport
            {
                ToastMessageHelper.ShortMessage("Số hộ chiếu không hợp lệ (Gồm 08 ký tự).");
                return;
            }

            LoadingHelper.Show();

            //viewModel.singleLead.address1_city = viewModel.AddressCity != null ? viewModel.AddressCity.Name : null;
            //viewModel.singleLead.address1_stateorprovince = viewModel.AddressStateProvince != null ? viewModel.AddressStateProvince.Name : null;
            //viewModel.singleLead.address1_country = viewModel.AddressCountry != null ? viewModel.AddressCountry.Name : null;

            //viewModel.singleLead.address1_line1 = viewModel.AddressLine1;
            //viewModel.singleLead.address1_composite = viewModel.AddressComposite;

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

        // địa chỉ liên lạc

        private async void CountryAddress1_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadProvincesForLookup(viewModel.CountryAddress1);
        }

        private async void StateProvinceAddress1_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress1);
        }

        private async void CloseAddress1_Clicked(object sender, EventArgs e)
        {
            await centerModalAddress1.Hide();
        }
        private async void ConfirmAddress1_Clicked(object sender, EventArgs e)
        {
            //bsd_country
            //bsd_province
            //bsd_district

            //bsd_housenumberstreet
            //bsd_contactaddress
            if (string.IsNullOrWhiteSpace(viewModel.LineAddress1))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.LineAddress1))
            {
                viewModel.singleLead.bsd_housenumberstreet = viewModel.LineAddress1;
                address.Add(viewModel.LineAddress1);
            }
            else
            {
                viewModel.singleLead.bsd_housenumberstreet = null;
            }

            if (viewModel.CityAddress1 != null)
            {
                viewModel.singleLead.bsd_district = viewModel.CityAddress1.Id.ToString();
                address.Add(viewModel.CityAddress1.Name);
            }
            else
            {
                viewModel.singleLead.bsd_district = null;
            }
            if (viewModel.StateProvinceAddress1 != null)
            {
                viewModel.singleLead.bsd_province = viewModel.StateProvinceAddress1.Id.ToString();
                address.Add(viewModel.StateProvinceAddress1.Name);
            }
            else
            {
                viewModel.singleLead.bsd_province = null;
            }
            if (viewModel.CountryAddress1 != null)
            {
                viewModel.singleLead.bsd_country = viewModel.CountryAddress1.Id.ToString();
                address.Add(viewModel.CountryAddress1.Name);
            }
            else
            {
                viewModel.singleLead.bsd_country = null;
            }
            viewModel.singleLead.bsd_contactaddress = viewModel.CompositeAddress1 = string.Join(", ", address);
            await centerModalAddress1.Hide();
        }

        private async void DiaChiLienLac_Tapped(object sender, EventArgs e)
        {   //bsd_country
            //bsd_province
            //bsd_district

            //bsd_housenumberstreet
            //bsd_contactaddress
            LoadingHelper.Show();
            if (viewModel.LineAddress1 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_permanentaddress))
            {
                viewModel.LineAddress1 = viewModel.singleLead.bsd_permanentaddress;
            }

            if (viewModel.CountryAddress1 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_country_name))
            {
                viewModel.CountryAddress1 = await viewModel.LoadCountryByName(viewModel.singleLead.bsd_country_name);
                await viewModel.LoadProvincesForLookup(viewModel.CountryAddress1);
            }

            if (viewModel.StateProvinceAddress1 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_province_name))
            {
                viewModel.StateProvinceAddress1 = await viewModel.LoadProvinceByName(viewModel.singleLead.bsd_country, viewModel.singleLead.bsd_province_name); ;
                await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress1);
            }

            if (viewModel.CityAddress1 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_district_name))
            {
                viewModel.CityAddress1 = await viewModel.LoadDistrictByName(viewModel.singleLead.bsd_province, viewModel.singleLead.bsd_district_name);
            }

            LoadingHelper.Hide();
            await centerModalAddress1.Show();
        }

        // địa chỉ thương trú
        private async void CountryAddress2_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadProvincesForLookup(viewModel.CountryAddress2);
        }

        private async void StateProvinceAddress2_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress2);
        }

        private async void CloseAddress2_Clicked(object sender, EventArgs e)
        {
            await centerModalAddress2.Hide();
        }

        private async void ConfirmAddress2_Clicked(object sender, EventArgs e)
        {
            //bsd_permanentcountry
            //bsd_permanentprovince
            //bsd_permanentdistrict

            //bsd_permanentaddress
            //bsd_permanentaddress1
            if (string.IsNullOrWhiteSpace(viewModel.LineAddress2))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.LineAddress2))
            {
                viewModel.singleLead.bsd_permanentaddress = viewModel.LineAddress2;
                address.Add(viewModel.LineAddress2);
            }
            else
            {
                viewModel.singleLead.bsd_permanentaddress = null;
            }

            if (viewModel.CityAddress2 != null)
            {
                viewModel.singleLead.bsd_permanentdistrict = viewModel.CityAddress2.Id.ToString();
                address.Add(viewModel.CityAddress2.Name);
            }
            else
            {
                viewModel.singleLead.bsd_permanentdistrict = null;
            }
            if (viewModel.StateProvinceAddress2 != null)
            {
                viewModel.singleLead.bsd_permanentprovince = viewModel.StateProvinceAddress2.Id.ToString();
                address.Add(viewModel.StateProvinceAddress2.Name);
            }
            else
            {
                viewModel.singleLead.bsd_permanentprovince = null;
            }
            if (viewModel.CountryAddress2 != null)
            {
                viewModel.singleLead.bsd_permanentcountry = viewModel.CountryAddress2.Id.ToString();
                address.Add(viewModel.CountryAddress2.Name);
            }
            else
            {
                viewModel.singleLead.bsd_permanentcountry = null;
            }
            viewModel.singleLead.bsd_permanentaddress1 = viewModel.CompositeAddress2 = string.Join(", ", address);
            await centerModalAddress2.Hide();
        }

        private async void DiaChiThuongTru_Tapped(object sender, EventArgs e)
        {
            //bsd_permanentcountry
            //bsd_permanentprovince
            //bsd_permanentdistrict

            //bsd_permanentaddress
            //bsd_permanentaddress1
            LoadingHelper.Show();
            if (viewModel.LineAddress2 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_permanentaddress))
            {
                viewModel.LineAddress2 = viewModel.singleLead.bsd_permanentaddress;
            }

            if (viewModel.CountryAddress2 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_permanentcountry_name))
            {
                viewModel.CountryAddress2 = await viewModel.LoadCountryByName(viewModel.singleLead.bsd_permanentcountry_name);
                await viewModel.LoadProvincesForLookup(viewModel.CountryAddress2);
            }

            if (viewModel.StateProvinceAddress2 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_permanentprovince_name))
            {
                viewModel.StateProvinceAddress2 = await viewModel.LoadProvinceByName(viewModel.singleLead.bsd_permanentcountry, viewModel.singleLead.bsd_permanentprovince_name); ;
                await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress2);
            }

            if (viewModel.CityAddress2 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_permanentdistrict_name))
            {
                viewModel.CityAddress2 = await viewModel.LoadDistrictByName(viewModel.singleLead.bsd_permanentprovince, viewModel.singleLead.bsd_permanentdistrict_name);
            }

            LoadingHelper.Hide();
            await centerModalAddress2.Show();
        }

        // địa chỉ công ty
        private async void CountryAddress3_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadProvincesForLookup(viewModel.CountryAddress3);
        }

        private async void StateProvinceAddress3_Changed(object sender, LookUpChangeEvent e)
        {
            await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress3);
        }

        private async void CloseAddress3_Clicked(object sender, EventArgs e)
        {
            await centerModalAddress3.Hide();
        }

        private async void ConfirmAddress3_Clicked(object sender, EventArgs e)
        {
            //bsd_accountcountry 
            //bsd_accountprovince
            //bsd_accountdistrict

            //bsd_account_housenumberstreetwardvn
            //bsd_accountaddressvn

            if (string.IsNullOrWhiteSpace(viewModel.LineAddress3))
            {
                ToastMessageHelper.ShortMessage("Vui lòng nhập số nhà/đường/phường");
                return;
            }

            List<string> address = new List<string>();
            if (!string.IsNullOrWhiteSpace(viewModel.LineAddress3))
            {
                viewModel.singleLead.bsd_account_housenumberstreetwardvn = viewModel.LineAddress3;
                address.Add(viewModel.LineAddress3);
            }
            else
            {
                viewModel.singleLead.bsd_account_housenumberstreetwardvn = null;
            }

            if (viewModel.CityAddress3 != null)
            {
                viewModel.singleLead.bsd_accountdistrict = viewModel.CityAddress3.Id.ToString();
                address.Add(viewModel.CityAddress3.Name);
            }
            else
            {
                viewModel.singleLead.bsd_accountdistrict = null;
            }
            if (viewModel.StateProvinceAddress3 != null)
            {
                viewModel.singleLead.bsd_accountprovince = viewModel.StateProvinceAddress3.Id.ToString();
                address.Add(viewModel.StateProvinceAddress3.Name);
            }
            else
            {
                viewModel.singleLead.bsd_accountprovince = null;
            }
            if (viewModel.CountryAddress3 != null)
            {
                viewModel.singleLead.bsd_accountcountry = viewModel.CountryAddress3.Id.ToString();
                address.Add(viewModel.CountryAddress3.Name);
            }
            else
            {
                viewModel.singleLead.bsd_accountcountry = null;
            }
            viewModel.singleLead.bsd_accountaddressvn = viewModel.CompositeAddress3 = string.Join(", ", address);
            await centerModalAddress3.Hide();
        }

        private async void DiaChiCongTy_Tapped(object sender, EventArgs e)
        {
            //bsd_accountcountry 
            //bsd_accountprovince
            //bsd_accountdistrict

            //bsd_account_housenumberstreetwardvn
            //bsd_accountaddressvn
            LoadingHelper.Show();
            if (viewModel.LineAddress3 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_account_housenumberstreetwardvn))
            {
                viewModel.LineAddress3 = viewModel.singleLead.bsd_account_housenumberstreetwardvn;
            }

            if (viewModel.CountryAddress3 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_accountcountry_name))
            {
                viewModel.CountryAddress3 = await viewModel.LoadCountryByName(viewModel.singleLead.bsd_accountcountry_name);
                await viewModel.LoadProvincesForLookup(viewModel.CountryAddress3);
            }

            if (viewModel.StateProvinceAddress3 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_accountprovince_name))
            {
                viewModel.StateProvinceAddress3 = await viewModel.LoadProvinceByName(viewModel.singleLead.bsd_accountcountry, viewModel.singleLead.bsd_accountprovince_name); ;
                await viewModel.LoadDistrictForLookup(viewModel.StateProvinceAddress3);
            }

            if (viewModel.CityAddress3 == null && !string.IsNullOrWhiteSpace(viewModel.singleLead.bsd_accountdistrict_name))
            {
                viewModel.CityAddress3 = await viewModel.LoadDistrictByName(viewModel.singleLead.bsd_accountprovince, viewModel.singleLead.bsd_accountdistrict_name);
            }

            LoadingHelper.Hide();
            await centerModalAddress3.Show();
        }
    }
}