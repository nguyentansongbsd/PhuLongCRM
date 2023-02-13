using Firebase.Database.Query;
using PhuLongCRM.Helper;
using PhuLongCRM.Helpers;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using Stormlion.PhotoBrowser;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

        public ProjectInfo(Guid projectId, string projectName = null)
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
            await viewModel.LoadData();

            if (viewModel.Project != null)
            {
                viewModel.ProjectType = ProjectTypeData.GetProjectType(viewModel.Project.bsd_projecttype);
                //viewModel.PropertyUsageType = PropertyUsageTypeData.GetPropertyUsageTypeById(viewModel.Project.bsd_propertyusagetype.ToString());
                //if (viewModel.Project.bsd_handoverconditionminimum.HasValue)
                //{
                //    viewModel.HandoverCoditionMinimum = HandoverCoditionMinimumData.GetHandoverCoditionMinimum(viewModel.Project.bsd_handoverconditionminimum.Value.ToString());
                //}

                await Task.WhenAll(
                        viewModel.LoadAllCollection(),
                        viewModel.CheckEvent(),
                        viewModel.LoadThongKe(),
                        viewModel.LoadThongKeGiuCho(),
                        viewModel.LoadThongKeHopDong(),
                        viewModel.LoadThongKeSoLuong(),
                        viewModel.CheckPhasesLaunch()
                       // viewModel.LoadThongKeDatCoc()
                    );
                var width = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 35) / 2;
                var tmpHeight = width * 2 / 3;
                collection.HeightRequest = (tmpHeight + 15 ) * ((viewModel.Collections.Count+2)/3);
                SetRealTime();
                try
                {
                    if (viewModel.Project.bsd_projectslogo == null)
                    {
                        avataProject.Source = StringAvata(viewModel.ProjectName);
                    }
                    if (viewModel.Project.bsd_queueproject && viewModel.Project.statuscode == "861450002") // sts = publish
                    {
                        viewModel.IsShowBtnGiuCho = true;
                    }
                    else
                    {
                        viewModel.IsShowBtnGiuCho = false;
                    }
                }
                catch (Exception ex)
                {

                }
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
            if (NeedToRefreshQueue == true)
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

        private void SetRealTime()
        {
            bool temp = false;
            var collection = RealTimeHelper.firebaseClient.Child("test").Child("DirectSaleNew").AsObservable<ResponseRealtime>()
                .Subscribe(async (dbevent) =>
                {
                    try
                    {
                        if (dbevent.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate && dbevent.Object != null && temp == true)
                        {
                            var item = dbevent.Object as ResponseRealtime;
                            if (item.ProjectId == viewModel.ProjectId.ToString())
                            {
                                viewModel.SoGiuCho++;
                                viewModel.ResetNumStatus(item.StatusNew, item.StatusOld);
                            }
                        }
                        temp = true;
                    }
                    catch (Exception ex)
                    {

                    }
                });
        }

        private void GiuCho_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Project.statuscode != "861450002") // 861450002 == publish
            {
                ToastMessageHelper.ShortMessage(Language.du_an_chua_duoc_cong_bo_ban_khong_the_thuc_hien_giu_cho);
                return;
            }
            LoadingHelper.Show();
            QueueUnitModel queueUnit = new QueueUnitModel
            {
                project_id = viewModel.Project.bsd_projectid,
                project_name = viewModel.Project.bsd_name,
                bsd_queuesperunit = viewModel.Project.bsd_queuesperunit,
                bsd_unitspersalesman = viewModel.Project.bsd_unitspersalesman,
                bsd_queueunitdaysaleman = viewModel.Project.bsd_queueunitdaysaleman,
                bsd_bookingfee = viewModel.Project.bsd_bookingfee.HasValue ? viewModel.Project.bsd_bookingfee.Value : 0,
            };
            QueueForm2 queue = new QueueForm2(queueUnit, true);
            queue.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(queue);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    // hiện câu thông báo bên queue form
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
            accountDetailPage.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess == 1)
                {
                    await Navigation.PushAsync(accountDetailPage);
                    LoadingHelper.Hide();
                }
                else if (IsSuccess == 3)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void GiuChoItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            QueuesDetialPage queuesDetialPage = new QueuesDetialPage(itemId);
            queuesDetialPage.OnCompleted = async (IsSuccess) =>
            {
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
            var item = (CollectionData)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item.SharePointType == SharePointType.Image)
            {
                var img = viewModel.Photos.SingleOrDefault(x => x.URL == item.ImageSource);
                var index = viewModel.Photos.IndexOf(img);

                new PhotoBrowser()
                {
                    Photos = viewModel.Photos,
                    StartIndex = index,
                    EnableGrid = true
                }.Show();
            }
            else if (item.SharePointType == SharePointType.Video)
            {
                LoadingHelper.Show();
                ShowMedia showMedia = new ShowMedia(Config.OrgConfig.SP_ProjectID, item.MediaSourceId);
                showMedia.OnCompleted = async (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(showMedia);
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.ShortMessage(Language.khong_tai_duoc_video);
                    }
                };
            }
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

        private async void TabControl_IndexTab(object sender, LookUpChangeEvent e)
        {
            if (e.Item != null)
            {
                if ((int)e.Item == 0)
                {
                    stackThongKe.IsVisible = true;
                    stackThongTin.IsVisible = false;
                    stackGiuCho.IsVisible = false;
                }
                else if ((int)e.Item == 1)
                {
                    stackThongKe.IsVisible = false;
                    stackThongTin.IsVisible = true;
                    stackGiuCho.IsVisible = false;
                }
                else if ((int)e.Item == 2)
                {
                    stackThongKe.IsVisible = false;
                    stackThongTin.IsVisible = false;
                    stackGiuCho.IsVisible = true;
                    LoadingHelper.Show();
                    if (viewModel.IsLoadedGiuCho == false)
                    {
                        await viewModel.LoadGiuCho();
                    }
                    LoadingHelper.Hide();
                }
            }
        }

        private string StringAvata(string projectName)
        {
            if (projectName == null) return null;
            string nameAvata = null;

            var name = projectName.Split(' ');
            if (name != null && name.Length > 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    nameAvata += name[i].Substring(0, 1).ToUpper();
                }
            }
            else
            {
                if (projectName.Length > 1)
                {
                    nameAvata = projectName.Substring(0, 2).ToUpper();
                }
                else
                    nameAvata = projectName.ToUpper();
            }
            return $"https://ui-avatars.com/api/?background=2196F3&rounded=false&color=ffffff&size=150&length=2&name={nameAvata}";
        }

        private async void OpenPdfDocxFile_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (CollectionData)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await DependencyService.Get<IOpenFileService>().OpenFile(item.PdfName,null, item.UrlPdfFile);
        }
    }
}