﻿using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhanHoiForm : ContentPage
    {
        public Action<bool> CheckPhanHoi;
        public PhanHoiFormViewModel viewModel;

        public PhanHoiForm(bool fromfeedback = false, bool fromRequest = false)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new PhanHoiFormViewModel();
            if (fromfeedback)
            {
                multiTabsCustomer.IsEnabled = false;
                viewModel.Customer = new OptionSet {Val = UserLogged.ContactId.ToString(), Label = UserLogged.ContactName, Title = "2" };
                viewModel.fromFeedback = fromfeedback;

                if (fromRequest)
                {
                    viewModel.CaseType = CaseTypeData.GetCaseById("3");
                    lookupCaseType.IsEnabled = false;
                    viewModel.singlePhanHoi.title += "Chuyển đổi thiết bị đăng nhập";
                    viewModel.singlePhanHoi.description += "Chuyển đổi thiết bị để đăng nhập employee: " + UserLogged.User;
                }
            }
            Init();
        }
        public PhanHoiForm(Guid incidentid)
        {
            InitializeComponent();
            Init();
            viewModel.IncidentId = incidentid;
            InitUpdate();
        }

        public async void Init()
        {
            viewModel.TabsDoiTuong = new List<string>() { Language.giu_cho_btn, Language.bang_tinh_gia_btn, Language.hop_dong };
            SerPreOpen();
        }

        public async void InitUpdate()
        {
            await viewModel.LoadCase();
            if (viewModel.singlePhanHoi != null)
            {
                this.Title = Language.cap_nhat_phan_hoi;
                buttonSave.Text = Language.cap_nhat;
                CheckPhanHoi?.Invoke(true);
            }
            else
            {
                CheckPhanHoi?.Invoke(false);
            }
        }

        private void SerPreOpen()
        {
            lookupCaseType.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.CaseTypes = CaseTypeData.CasesData().Where(x=>x.Val != "0").ToList();
                LoadingHelper.Hide();
            };

            lookupSubjects.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                await viewModel.LoadSubjects();
                LoadingHelper.Hide();
            };

            lookupCaseOrigin.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.CaseOrigins = CaseOriginData.Origins().Where(x => x.Val != "0").ToList();
                LoadingHelper.Hide();
            };

            lookupCaseLienQuan.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                if (viewModel.IncidentId != Guid.Empty)//cap nhat load danh sach caselienquan khong chua case hien tai
                {
                    await viewModel.LoadCaseLienQuan(viewModel.IncidentId.ToString());
                }
                else
                {
                    await viewModel.LoadCaseLienQuan();
                }

                LoadingHelper.Hide();
            };

            lookupProjects.PreOpenAsync = async () =>
            {
                LoadingHelper.Show();
                viewModel.Projects = await viewModel.LoadProjects();
                LoadingHelper.Hide();
            };

            lookupUnits.PreOpenOneTime = false;
            lookupUnits.PreOpen = async () => {
                if (viewModel.Project == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_du_an);
                }
            };

            //multiTabsDoiTuong.PreOpenOneTime = false;
            //multiTabsDoiTuong.PreOpen = () =>
            //{
            //    LoadingHelper.Show();
            //    if (viewModel.Customer == null)
            //    {
            //        LoadingHelper.Hide();
            //        ToastMessageHelper.ShortMessage("Vui lòng chọn khách hàng");
            //        return;
            //    }
            //    LoadingHelper.Hide();
            //};
        }

        private async void CustomerItem_Changed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.DoiTuong = null;
            viewModel.Queues = null;
            viewModel.Quotes = null;
            viewModel.OptionEntries = null;
            if (viewModel.AllItemSourceDoiTuong != null)
            {
                viewModel.AllItemSourceDoiTuong.Clear();
            }

            await Task.WhenAll(
                    viewModel.LoadQueues(),
                    viewModel.LoadQuotes(),
                    viewModel.LoadOptionEntries()
                    );
            viewModel.AllItemSourceDoiTuong = new List<List<OptionSet>>() { viewModel.Queues, viewModel.Quotes, viewModel.OptionEntries };
            LoadingHelper.Hide();
        }

        private async void ProjectItem_Changed(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.Unit = null;
            viewModel.Units = null;
            await viewModel.LoadUnits();
            LoadingHelper.Hide();
        }

        private async void SaveCase_Clicked(object sender, EventArgs e)
        {
            if (viewModel.CaseType == null)
            {
                ToastMessageHelper.Message(Language.vui_long_chon_loai_phan_hoi);
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.singlePhanHoi.title))
            {
                ToastMessageHelper.Message(Language.vui_long_nhap_tieu_de);
                return;
            }

            if (viewModel.fromFeedback == false)
            {
                if (viewModel.Customer == null)
                {
                    ToastMessageHelper.Message(Language.vui_long_chon_khach_hang);
                    return;
                }
            }
            else
            {
                if (viewModel.Customer == null)
                {
                    ToastMessageHelper.Message(Language.nhan_vien_chua_co_contact);
                    return;
                }
            }    

            LoadingHelper.Show();
            if (viewModel.singlePhanHoi.incidentid == Guid.Empty)
            {
                bool isSuccess = await viewModel.CreateCase();
                if (isSuccess)
                {
                    if (ListPhanHoi.NeedToRefresh.HasValue) ListPhanHoi.NeedToRefresh = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.Message(Language.thong_bao_thanh_cong);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.thong_bao_that_bai);
                }
            }
            else
            {
                bool isSuccess = await viewModel.UpdateCase();
                if (isSuccess)
                {
                    if (PhanHoiDetailPage.NeedToRefresh.HasValue) PhanHoiDetailPage.NeedToRefresh = true;
                    await Navigation.PopAsync();
                    ToastMessageHelper.Message(Language.cap_nhat_thanh_cong);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.cap_nhat_that_bai);
                }
            }
        }
    }
}
