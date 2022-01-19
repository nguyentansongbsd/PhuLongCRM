using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Stormlion.PhotoBrowser;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProjectInfo : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefreshQueue = null;
        public static bool? NeedToRefreshNumQueue = null;
        public ProjectInfoViewModel viewModel;

        public ProjectInfo(Guid projectId,string projectName = null)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ProjectInfoViewModel();
            NeedToRefreshQueue = false;
            NeedToRefreshNumQueue = false;
            viewModel.ProjectId = projectId;
            viewModel.ProjectName = projectName;
            Init();
        }

        public async void Init()
        {
            VisualStateManager.GoToState(radborderThongKe, "Active");
            VisualStateManager.GoToState(radborderThongTin, "InActive");
            VisualStateManager.GoToState(radborderGiuCho, "InActive");
            VisualStateManager.GoToState(lblThongKe, "Active");
            VisualStateManager.GoToState(lblThongTin, "InActive");
            VisualStateManager.GoToState(lblGiuCho, "InActive");

            await Task.WhenAll(
                viewModel.LoadData(),
                viewModel.LoadAllCollection(),
                viewModel.CheckEvent(),
                viewModel.LoadThongKe(),
                viewModel.LoadThongKeGiuCho(),
                viewModel.LoadThongKeHopDong(),
                viewModel.LoadThongKeBangTinhGia()
            ) ;

            if (viewModel.Project != null)
            {
                viewModel.ProjectType = ProjectTypeData.GetProjectType(viewModel.Project.bsd_projecttype);
                viewModel.PropertyUsageType = PropertyUsageTypeData.GetPropertyUsageTypeById(viewModel.Project.bsd_propertyusagetype.ToString());
                //if (viewModel.Project.bsd_handoverconditionminimum.HasValue)
                //{
                //    viewModel.HandoverCoditionMinimum = HandoverCoditionMinimumData.GetHandoverCoditionMinimum(viewModel.Project.bsd_handoverconditionminimum.Value.ToString());
                //}
                OnCompleted?.Invoke(true);
            }
            else
            {
                OnCompleted?.Invoke(false);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if(NeedToRefreshQueue == true)
            {
                LoadingHelper.Show();
                viewModel.PageListGiuCho = 1;
                viewModel.ListGiuCho.Clear();
                await viewModel.LoadGiuCho();
                NeedToRefreshQueue = false;
                LoadingHelper.Hide();
            }

            if (NeedToRefreshNumQueue == true)
            {
                LoadingHelper.Show();
                viewModel.SoGiuCho = 0;
                await viewModel.LoadThongKeGiuCho();
                NeedToRefreshNumQueue = false;
                LoadingHelper.Hide();
            }
        }

        private async void ThongKe_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radborderThongKe, "Active");
            VisualStateManager.GoToState(radborderThongTin, "InActive");
            VisualStateManager.GoToState(radborderGiuCho, "InActive");
            VisualStateManager.GoToState(lblThongKe, "Active");
            VisualStateManager.GoToState(lblThongTin, "InActive");
            VisualStateManager.GoToState(lblGiuCho, "InActive");
            stackThongKe.IsVisible = true;
            stackThongTin.IsVisible = false;
            stackGiuCho.IsVisible = false;
        }

        private async void ThongTin_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radborderThongKe, "InActive");
            VisualStateManager.GoToState(radborderThongTin, "Active");
            VisualStateManager.GoToState(radborderGiuCho, "InActive");
            VisualStateManager.GoToState(lblThongKe, "InActive");
            VisualStateManager.GoToState(lblThongTin, "Active");
            VisualStateManager.GoToState(lblGiuCho, "InActive");
            stackThongKe.IsVisible = false;
            stackThongTin.IsVisible = true;
            stackGiuCho.IsVisible = false;
        }

        private async void GiuCho_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radborderThongKe, "InActive");
            VisualStateManager.GoToState(radborderThongTin, "InActive");
            VisualStateManager.GoToState(radborderGiuCho, "Active");
            VisualStateManager.GoToState(lblThongKe, "InActive");
            VisualStateManager.GoToState(lblThongTin, "InActive");
            VisualStateManager.GoToState(lblGiuCho, "Active");
            stackThongKe.IsVisible = false;
            stackThongTin.IsVisible = false;
            stackGiuCho.IsVisible = true;
            if (viewModel.IsLoadedGiuCho == false)
            {
                await viewModel.LoadGiuCho();
            }            
            LoadingHelper.Hide();
        }

        private void GiuCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            QueueForm queue = new QueueForm(viewModel.ProjectId, false);
            queue.OnCompleted = async (IsSuccess) => {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(queue);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_san_pham);
                }
            };
        }

        private async void ShowMoreListDatCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageListGiuCho++;
            await viewModel.LoadGiuCho();
            LoadingHelper.Hide();
        }

        private void ChuDauTu_Tapped(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            var id = (Guid)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            AccountDetailPage accountDetailPage = new AccountDetailPage(id);
            accountDetailPage.OnCompleted= async (IsSuccess) => {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(accountDetailPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void GiuChoItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            QueuesDetialPage queuesDetialPage = new QueuesDetialPage(itemId);
            queuesDetialPage.OnCompleted = async (IsSuccess) => {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(queuesDetialPage);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void ItemSlider_Tapped(object sender, EventArgs e)
        {
            // khoa lai vi phu long chua co hinh anh va video

            //LoadingHelper.Show();
            //var item = (CollectionData)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            //if (item.SharePointType == SharePointType.Image)
            //{
            //    var img = viewModel.Photos.SingleOrDefault(x => x.URL == item.ImageSource);
            //    var index = viewModel.Photos.IndexOf(img);

            //    new PhotoBrowser()
            //    {
            //        Photos = viewModel.Photos,
            //        StartIndex = index,
            //        EnableGrid = true
            //    }.Show();
            //}
            //else if (item.SharePointType == SharePointType.Video)
            //{
            //    ShowMedia showMedia = new ShowMedia(Config.OrgConfig.SharePointProjectId, item.MediaSourceId);
            //    showMedia.OnCompleted = async (isSuccess) => {
            //        if (isSuccess)
            //        {
            //            await Navigation.PushAsync(showMedia);
            //            LoadingHelper.Hide();
            //        }
            //        else
            //        {
            //            LoadingHelper.Hide();
            //            ToastMessageHelper.ShortMessage("Không lấy được video");
            //        }
            //    };
            //}
            //LoadingHelper.Hide();
        }

        private void ScollTo_Video_Tapped(object sender, EventArgs e)
        {
            var index = viewModel.Collections.IndexOf(viewModel.Collections.FirstOrDefault(x => x.SharePointType == SharePointType.Video));
            carouseView.ScrollTo(index, position: ScrollToPosition.End);
        }

        private void ScollTo_Image_Tapped(object sender, EventArgs e)
        {
            var index = viewModel.Collections.IndexOf(viewModel.Collections.FirstOrDefault(x => x.SharePointType == SharePointType.Image));
            carouseView.ScrollTo(index, position: ScrollToPosition.End);
        }

        private async void OpenEvent_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Event == null)
            {
                await viewModel.LoadDataEvent();
            }
            ContentEvent.IsVisible = true;
        }

        private void CloseContentEvent_Tapped(object sender, EventArgs e)
        {
            ContentEvent.IsVisible = false;
        }
    }
}