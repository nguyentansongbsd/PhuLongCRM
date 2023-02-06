using PhuLongCRM.Helper;
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
    public partial class DirectSale : ContentPage
    {
        public DirectSaleViewModel viewModel;
        public DirectSale()
        {
            LoadingHelper.Show();
            InitializeComponent();
            this.BindingContext = viewModel = new DirectSaleViewModel();
            PropertyChanged += DirectSale_PropertyChanged;
            Init();
            LoadingHelper.Hide();
        }

        private void DirectSale_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ChangLanguege();
        }

        public async void Init()
        {
            lookupNetArea.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.NetAreas = NetAreaDirectSaleData.NetAreaData();
                LoadingHelper.Hide();
            };
            lookupPrice.PreOpenAsync = async () => {
                LoadingHelper.Show();
                viewModel.Prices = PriceDirectSaleData.PriceData();
                LoadingHelper.Hide();
            };
            lookupMultipleDirection.PreShow = async () => {
                LoadingHelper.Show();
                viewModel.DirectionOptions = DirectionData.Directions();
                LoadingHelper.Hide();
            };
            lookupMultipleViews.PreShow =async () => {
                LoadingHelper.Show();
                viewModel.ViewOptions = ViewData.Views();
                LoadingHelper.Hide();
            };
            lookupMultipleUnitStatus.PreShow= async () => {
                LoadingHelper.Show();
                var unitStatus = StatusCodeUnit.StatusCodes();
                unitStatus.RemoveAt(0);
                viewModel.UnitStatusOptions = new List<OptionSetFilter>();
                foreach (var item in unitStatus)
                {
                    viewModel.UnitStatusOptions.Add(new OptionSetFilter { Val = item.Id, Label = item.Name });
                }
                LoadingHelper.Hide();
            };
            
        }

        private async void LoadProject_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.Projects.Clear();
            await viewModel.LoadProject();
            listviewProject.ItemsSource = viewModel.Projects;
            await bottomModalProject.Show();
            LoadingHelper.Hide();
        }

        private void SearchBar_SearchButtonPressed(object sender,EventArgs e)
        {
            LoadingHelper.Show();
            listviewProject.ItemsSource = viewModel.Projects.Where(x=>x.bsd_name.ToLower().Contains(searchProject.Text.Trim().ToLower()) || x.bsd_projectcode.ToLower().Contains(searchProject.Text.Trim().ToLower()));
            LoadingHelper.Hide();
        }

        private async void SearchBar_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchProject.Text))
            {
                LoadingHelper.Show();
                viewModel.Projects.Clear();
                await viewModel.LoadProject();
                listviewProject.ItemsSource = viewModel.Projects;
                LoadingHelper.Hide();
            }
        }

        private async void ProjectItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            var item = e.Item as ProjectListModel;
            viewModel.Project = item;
            await Task.WhenAll(
                viewModel.LoadPhasesLanch()
                //viewModel.LoadBlocks()
                ) ;
            await bottomModalProject.Hide();
            LoadingHelper.Hide();
        }

        private async void PhaseLaunchItem_SelectedChange(object sender,EventArgs e)
        {
            //LoadingHelper.Show();
            //await viewModel.LoadBlocks();
            //LoadingHelper.Hide();
        }

        private void SearchClicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Project == null)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_du_an);
                LoadingHelper.Hide();
            }
            else
            {
                string directions = (viewModel.SelectedDirections != null && viewModel.SelectedDirections.Count != 0) ? string.Join(",", viewModel.SelectedDirections) : null;
                string views = (viewModel.SelectedViews != null && viewModel.SelectedViews.Count != 0) ? string.Join(",", viewModel.SelectedViews) : null;
                string unitStatus = (viewModel.SelectedUnitStatus != null && viewModel.SelectedUnitStatus.Count != 0) ? string.Join(",", viewModel.SelectedUnitStatus) : null;

                DirectSaleSearchModel filter = null;
                if (viewModel.isOwner)
                {
                    filter = new DirectSaleSearchModel(viewModel.Project.bsd_projectid, viewModel.PhasesLaunch?.Val, viewModel.IsEvent, viewModel.UnitCode, directions, views, unitStatus, viewModel.NetArea?.Id, viewModel.Price?.Id, viewModel.isOwner, UserLogged.Id.ToString());
                }
                else
                {
                    filter = new DirectSaleSearchModel(viewModel.Project.bsd_projectid, viewModel.PhasesLaunch?.Val, viewModel.IsEvent, viewModel.UnitCode, directions, views, unitStatus, viewModel.NetArea?.Id, viewModel.Price?.Id, viewModel.isOwner);
                }

                //DirectSaleDetail directSaleDetail = new DirectSaleDetail(filter);//,viewModel.Blocks
                //directSaleDetail.OnCompleted = async (Success) =>
                //{
                //    if (Success == 0)
                //    {
                //        await Navigation.PushAsync(directSaleDetail);
                //        LoadingHelper.Hide();
                //    }
                //    else if (Success == 1)
                //    {
                //        LoadingHelper.Hide();
                //        ToastMessageHelper.LongMessage(Language.khong_co_san_pham);
                //    }
                //    else if (Success == 2)
                //    {
                //        LoadingHelper.Hide();
                //        ToastMessageHelper.LongMessage(Language.khong_co_san_pham);
                //    }
                //};

                DirectSaleDetailTest directSaleDetail = new DirectSaleDetailTest(filter);//,viewModel.Blocks
                directSaleDetail.OnCompleted = async (Success) =>
                {
                    if (Success == 0)
                    {
                        await Navigation.PushAsync(directSaleDetail);
                        LoadingHelper.Hide();
                    }
                    else if (Success == 1)
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.LongMessage(Language.khong_co_san_pham);
                    }
                    else if (Success == 2)
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.LongMessage(Language.khong_co_san_pham);
                    }
                };
            }
        }

        private void ShowInfo(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            if (viewModel.Project == null)
            {
                ToastMessageHelper.ShortMessage(Language.vui_long_chon_du_an);
                LoadingHelper.Hide();
                return;
            }

            ProjectInfo projectInfo = new ProjectInfo(Guid.Parse(viewModel.Project.bsd_projectid),viewModel.Project.bsd_name);
            projectInfo.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess == true)
                {
                    await Navigation.PushAsync(projectInfo);
                    LoadingHelper.Hide();
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                    LoadingHelper.Hide();
                }
            };
        }

        private void Clear_Clicked(object sender, EventArgs e)
        {
            viewModel.Project = null;
           viewModel.PhasesLaunch = null;
            viewModel.IsEvent = false;
            viewModel.UnitCode = null;
            viewModel.SelectedDirections = null;
            viewModel.SelectedViews = null;
            viewModel.SelectedUnitStatus = null;
            viewModel.NetArea = null;
            viewModel.Price = null;
        }
        private void ChangLanguege()
        {
            this.Title = Language.gio_hang;
            lb_duan.Text = Language.du_an;
            entry_duan.Placeholder = Language.chon_du_an;
            lb_dotmoban.Text = Language.dot_mo_ban;
            lookupPhasesLaunch.Placeholder = Language.chon_dot_mo_ban;
            lb_sukien.Text = Language.su_kien;
            lb_masanpham.Text = Language.ma_san_pham;
            entry_masanpham.Placeholder = Language.ma_san_pham;
            lb_huong.Text = Language.huong;
            lookupMultipleDirection.Placeholder = Language.chon_huong;
            lb_huongnhin.Text = Language.huong_nhin;
            lookupMultipleViews.Placeholder = Language.chon_huong_nhin;
            lb_tinhtrangsanpham.Text = Language.tinh_trang_san_pham;
            lookupMultipleUnitStatus.Placeholder = Language.chon_tinh_trang_san_pham;
            lb_dientichsudung.Text = Language.dien_tich_su_dung;
            lookupNetArea.Placeholder = Language.chon_dien_tich_su_dung;
            lb_giaban.Text = Language.gia_ban_vnd;
            lookupPrice.Placeholder = Language.chon_gia_ban;
            lb_thuocnhanvien.Text = Language.thuoc_nhan_vien;
            btn_thongtinduan.Text = Language.thong_tin_du_an;
            btn_timkiem.Text = Language.tim_kiem;
        }
    }
}