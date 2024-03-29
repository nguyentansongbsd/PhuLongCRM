﻿using PhuLongCRM.Helper;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using FFImageLoading.Forms;
using System.Collections.ObjectModel;
using PhuLongCRM.Resources;
using System.Linq;
using Stormlion.PhotoBrowser;
using Xamarin.Essentials;
using PhuLongCRM.IServices;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitInfo : ContentPage
    {
        public Action<bool> OnCompleted;
        public static bool? NeedToRefreshQueue = null;
        public static bool? NeedToRefreshQuotation = null;
        public static bool? NeedToRefreshReservation = null;
        public static bool? NeedToRefresh = null;
        private UnitInfoViewModel viewModel;

        public UnitInfo(Guid id)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new UnitInfoViewModel();
            NeedToRefreshQueue = false;
            NeedToRefreshQuotation = false;
            NeedToRefreshReservation = false;
            NeedToRefresh = false;
            viewModel.UnitId = id;
            Init();
        }
        public async void Init()
        {
            await viewModel.LoadUnit();
            await viewModel.CheckShowBtnBangTinhGia();
            
            if (viewModel.UnitInfo != null)
            {
                viewModel.StatusCode = StatusCodeUnit.GetStatusCodeById(viewModel.UnitInfo.statuscode.ToString());
                if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_direction)) 
                {
                    viewModel.Direction = DirectionData.GetDiretionById(viewModel.UnitInfo.bsd_direction);
                }

                if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_viewphulong))
                {
                    viewModel.View = ViewData.GetViewByIds(viewModel.UnitInfo.bsd_viewphulong);
                }

                if (viewModel.UnitInfo.statuscode == 1 || viewModel.UnitInfo.statuscode == 100000000 || viewModel.UnitInfo.statuscode == 100000004 || viewModel.UnitInfo.statuscode == 100000007)
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
                var width = ((DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) - 35) / 2;
                var tmpHeight = width * 2 / 3;
                collection.HeightRequest = (tmpHeight + 15) * ((viewModel.Collections.Count + 2) / 3);
                SetButton();
                viewModel.IsShowBtn = viewModel.UnitInfo.bsd_vippriority ? false : true;
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
                if (viewModel.BangTinhGiaList != null)
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
            if (NeedToRefresh == true)
            {
                LoadingHelper.Show();
                await viewModel.LoadUnit();
                await viewModel.CheckShowBtnBangTinhGia();
                if (viewModel.UnitInfo != null)
                {
                    viewModel.StatusCode = StatusCodeUnit.GetStatusCodeById(viewModel.UnitInfo.statuscode.ToString());
                    if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_direction))
                    {
                        viewModel.Direction = DirectionData.GetDiretionById(viewModel.UnitInfo.bsd_direction);
                    }

                    if (!string.IsNullOrWhiteSpace(viewModel.UnitInfo.bsd_viewphulong))
                    {
                        viewModel.View = ViewData.GetViewByIds(viewModel.UnitInfo.bsd_viewphulong);
                    }

                    if (viewModel.UnitInfo.statuscode == 1 || viewModel.UnitInfo.statuscode == 100000000 || viewModel.UnitInfo.statuscode == 100000004 || viewModel.UnitInfo.statuscode == 100000007)
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

                    SetButton();
                }
                NeedToRefresh = false;
                LoadingHelper.Hide();
            }
            //await CrossMediaManager.Current.Stop();
        }

        public void SetButton()
        {
            gridbtn = new Grid();
            if (btnGiuCho.IsVisible == false && viewModel.IsShowBtnBangTinhGia == false)
            {
                gridbtn.IsVisible = false;
            }
            gridbtn.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), });
            if (btnGiuCho.IsVisible == true && viewModel.IsShowBtnBangTinhGia == true)
            {
                gridbtn.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), });
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
            gridbtn.IsVisible = viewModel.UnitInfo.bsd_vippriority ? false : true;
            viewModel.IsShowBtn = viewModel.UnitInfo.bsd_vippriority ? false : true;
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
            QueueUnitModel queueUnit = new QueueUnitModel
            {
                unit_id = Guid.Parse(viewModel.UnitInfo.productid),
                unit_name = viewModel.UnitInfo.name,
                project_id = viewModel.UnitInfo.bsd_project_id,
                project_name = viewModel.UnitInfo.bsd_project_name,
                phaseslaunch_id = viewModel.UnitInfo.bsd_phaseslaunch_id,
                phaseslaunch_name = viewModel.UnitInfo.bsd_phaseslaunch_name,
                bsd_queuesperunit = viewModel.UnitInfo.project_queuesperunit,
                bsd_unitspersalesman = viewModel.UnitInfo.project_unitspersalesman,
                bsd_queueunitdaysaleman = viewModel.UnitInfo.project_queueunitdaysaleman,
                bsd_bookingfee = viewModel.UnitInfo.project_bookingfee,
                bsd_queuingfee = viewModel.UnitInfo.bsd_queuingfee,
            };
            QueueForm2 queue = new QueueForm2(queueUnit);
            queue.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Shell.Current.Navigation.PushAsync(queue);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    // hiện câu thông báo bên queue form
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
                    ToastMessageHelper.Message(Language.san_pham_khong_the_tao_bang_tinh_gia);
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_san_pham);
                }
            };
        }

        private void GiuChoItem_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
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
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void ChiTietDatCoc_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            BangTinhGiaDetailPage bangTinhGiaDetail = new BangTinhGiaDetailPage(itemId, true);
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
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
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
            var itemId = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
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
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
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

        private async void TabControl_IndexTab(object sender, LookUpChangeEvent e)
        {
            if (e.Item != null)
            {
                if ((int)e.Item == 0)
                {
                    stackThongTinCanHo.IsVisible = true;
                    stackGiaoDich.IsVisible = false;
                }
                else if ((int)e.Item == 1)
                {
                    LoadingHelper.Show();
                    stackThongTinCanHo.IsVisible = false;
                    stackGiaoDich.IsVisible = true;

                    if (viewModel.IsLoaded == false)
                    {
                        await Task.WhenAll(
                            viewModel.LoadQueues(),
                            viewModel.LoadDanhSachDatCoc(),
                            viewModel.LoadDanhSachBangTinhGia(),
                            viewModel.LoadOptoinEntry()
                        );
                    }
                    LoadingHelper.Hide();
                }
            }
        }

        private void ChiTietBTG_Tapped(object sender, EventArgs e)
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
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }
        private void ItemSlider_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
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
                ShowMedia showMedia = new ShowMedia(Config.OrgConfig.SP_ProjectID, item.MediaSourceId);
                showMedia.OnCompleted = async (isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await Navigation.PushAsync(showMedia);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        ToastMessageHelper.Message(Language.khong_tai_duoc_video);
                    }
                };
            }
            LoadingHelper.Hide();
        }
        private async void OpenPdfDocxFile_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var item = (CollectionData)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await DependencyService.Get<IOpenFileService>().OpenFile(item.PdfName, null, item.UrlPdfFile);
        }
    }
}