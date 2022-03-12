using PhuLongCRM.Helper;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using FFImageLoading.Forms;
using System.Collections.ObjectModel;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitInfo : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefreshQueue = null;
        public static bool? NeedToRefreshQuotation = null;
        public static bool? NeedToRefreshReservation = null;
        private UnitInfoViewModel viewModel;

        public UnitInfo(Guid id)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new UnitInfoViewModel();
            NeedToRefreshQueue = false;
            NeedToRefreshQuotation = false;
            NeedToRefreshReservation = false;
            viewModel.UnitId = id;
            Init();
        }
        public async void Init()
        {
            await Task.WhenAll(
                viewModel.LoadUnit(),
                viewModel.CheckShowBtnBangTinhGia()
                );
            
            if (viewModel.UnitInfo != null)
            {
                VisualStateManager.GoToState(radborderThongTin, "Active");
                VisualStateManager.GoToState(radborderGiaoDich, "InActive");
                VisualStateManager.GoToState(lblThongTin, "Active");
                VisualStateManager.GoToState(lblGiaoDich, "InActive");

                viewModel.StatusCode = StatusCodeUnit.GetStatusCodeById(viewModel.UnitInfo.statuscode.ToString());
                if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_direction)) 
                {
                    viewModel.Direction = DirectionData.GetDiretionById(viewModel.UnitInfo.bsd_direction);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_viewphulong))
                {
                    viewModel.View = ViewData.GetViewByIds(viewModel.UnitInfo.bsd_viewphulong);
                }

                if (viewModel.UnitInfo.statuscode == 1 || viewModel.UnitInfo.statuscode == 100000000 || viewModel.UnitInfo.statuscode == 100000004)
                {
                    btnGiuCho.IsVisible = viewModel.UnitInfo.bsd_vippriority ? false : true;
                    if (viewModel.UnitInfo.statuscode != 1 && viewModel.IsShowBtnBangTinhGia == true)
                    {
                        viewModel.IsShowBtnBangTinhGia = true;
                    }
                    else
                    {
                        viewModel.IsShowBtnBangTinhGia = false;
                    }
                }
                else
                {
                    btnGiuCho.IsVisible = false;
                    viewModel.IsShowBtnBangTinhGia = false;
                }

                gridButton.IsVisible = !viewModel.UnitInfo.bsd_vippriority;
                SetButton();
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
                viewModel.PageDanhSachDatCho = 1;
                viewModel.list_danhsachdatcho.Clear();
                await viewModel.LoadQueues();
                NeedToRefreshQueue = false;
                LoadingHelper.Hide();
            }
            if (NeedToRefreshQuotation == true)
            {
                LoadingHelper.Show();
                viewModel.PageBangTinhGia = 1;
                viewModel.BangTinhGiaList.Clear();
                await viewModel.LoadDanhSachBangTinhGia();
                NeedToRefreshQuotation = false;
                LoadingHelper.Hide();
            }
            if (NeedToRefreshReservation == true)
            {
                LoadingHelper.Show();
                viewModel.PageDanhSachDatCoc = 1;
                viewModel.list_danhsachdatcoc.Clear();
                await viewModel.LoadDanhSachDatCoc();
                NeedToRefreshReservation = false;
                LoadingHelper.Hide();
            }
            //await CrossMediaManager.Current.Stop();
        }

        public void SetButton()
        {
            gridButton = new Grid();
            if (btnGiuCho.IsVisible == false && viewModel.IsShowBtnBangTinhGia == false)
            {
                gridButton.IsVisible = false;
            }
            gridButton.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), });
            if (btnGiuCho.IsVisible == true && viewModel.IsShowBtnBangTinhGia == true)
            {
                gridButton.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), });
                Grid.SetColumn(btnGiuCho, 0);
                Grid.SetColumn(btnBangTinhGia, 1);
            }
            else if (btnGiuCho.IsVisible == true && viewModel.IsShowBtnBangTinhGia == false)
            {
                Grid.SetColumn(btnGiuCho, 0);
                Grid.SetColumn(btnBangTinhGia, 0);
            }
            else if (btnGiuCho.IsVisible == false && viewModel.IsShowBtnBangTinhGia == true)
            {
                Grid.SetColumn(btnBangTinhGia, 0);
                Grid.SetColumn(btnGiuCho, 0);
            }
        }

        private void ThongTin_Tapped(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(radborderThongTin, "Active");
            VisualStateManager.GoToState(radborderGiaoDich, "InActive");
            VisualStateManager.GoToState(lblThongTin, "Active");
            VisualStateManager.GoToState(lblGiaoDich, "InActive");
            stackThongTinCanHo.IsVisible = true;
            stackGiaoDich.IsVisible = false;
        }

        private async void GiaoDich_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radborderThongTin, "InActive");
            VisualStateManager.GoToState(radborderGiaoDich, "Active");
            VisualStateManager.GoToState(lblThongTin, "InActive");
            VisualStateManager.GoToState(lblGiaoDich, "Active");
            stackThongTinCanHo.IsVisible = false;
            stackGiaoDich.IsVisible = true;

            if (viewModel.IsLoaded == false)
            {
                viewModel.BangTinhGiaList = new ObservableCollection<ReservationListModel>();
                await Task.WhenAll(
                    viewModel.LoadQueues(),
                    viewModel.LoadDanhSachDatCoc(),
                    viewModel.LoadDanhSachBangTinhGia(),
                    viewModel.LoadOptoinEntry()
                );
            }
            LoadingHelper.Hide();
        }

        private async void ShowMoreDanhSachDatCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachDatCho++;
            await viewModel.LoadQueues();
            LoadingHelper.Hide();
        }

        private async void ShowMoreDanhSachDatCoc_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachDatCoc++;
            await viewModel.LoadDanhSachDatCoc();
            LoadingHelper.Hide();
        }

        private async void ShowMoreDanhSachHopDong_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageDanhSachHopDong++;
            await viewModel.LoadOptoinEntry();
            LoadingHelper.Hide();
        }

        private void GiuCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            QueueForm queue = new QueueForm(viewModel.UnitId, true);
            queue.OnCompleted = async (IsSuccess) => {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(queue);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                   // ToastMessageHelper.ShortMessage(Language.khong_tim_thay_san_pham);
                }
            };
        }

        private void BangTinhGia_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ReservationForm reservationForm = new ReservationForm(Guid.Parse(viewModel.UnitInfo.productid), null, null, null,null);
            reservationForm.CheckReservation = async (isSuccess) => {
                if (isSuccess == 0)
                {
                    await Navigation.PushAsync(reservationForm);
                    LoadingHelper.Hide();
                }
                else if (isSuccess == 1)
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.san_pham_khong_the_tao_bang_tinh_gia);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_san_pham);
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

        private void ChiTietDatCoc_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(itemId);
            bangTinhGiaDetail.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(bangTinhGiaDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private async void ShowMoreBangTinhGia_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            viewModel.PageBangTinhGia++;
            await viewModel.LoadDanhSachBangTinhGia();
            LoadingHelper.Hide();
        }

        private void ItemHopDong_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            ContractDetailPage contractDetail = new ContractDetailPage(itemId);
            contractDetail.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
                {
                    await Navigation.PushAsync(contractDetail);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private async void Meida_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            Grid mediaElement = (Grid)sender;
            var a = (TapGestureRecognizer)mediaElement.GestureRecognizers[0];
            CollectionData item = a.CommandParameter as CollectionData;
            if (item != null)
            {
                LoadingHelper.Show();
                //await Navigation.PushAsync(new ShowMedia(item.MediaSourceId));
                LoadingHelper.Hide();

            }
            LoadingHelper.Hide();
        }

        private void Image_Tapped(object sender, EventArgs e)
        {
            CachedImage image = (CachedImage)sender;
            var a = (TapGestureRecognizer)image.GestureRecognizers[0];
            CollectionData item = a.CommandParameter as CollectionData;
            if (item != null)
            {
                viewModel.photoBrowser.StartIndex = item.Index;
                viewModel.photoBrowser.Show();
            }
        }
        private async void OpenEvent_Tapped(object sender, EventArgs e)
        {
            if (viewModel.Event == null)
                await viewModel.LoadDataEvent();
            ContentEvent.IsVisible = true;
        }
        private void CloseContentEvent_Tapped(object sender, EventArgs e)
        {
            ContentEvent.IsVisible = false;
        }
    }
}