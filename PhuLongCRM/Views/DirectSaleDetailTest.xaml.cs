﻿using PhuLongCRM.Controls;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ToolbarItem = Xamarin.Forms.ToolbarItem;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectSaleDetailTest : ContentPage
    {
        public static bool? NeedToRefreshDirectSale = null;
        private bool RefreshDirectSale { get; set; }
        public Action<int> OnCompleted;
        private DirectSaleDetailTestViewModel viewModel;
        private Grid grid;
        private Grid gridBtn;
        private Button btnQueue;
        private Button btnQuote;
        private Label labelName;
        public DirectSaleDetailTest(DirectSaleSearchModel filter)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DirectSaleDetailTestViewModel();
            NeedToRefreshDirectSale = false;
            viewModel.Filter = filter;
            if (viewModel.Filter.isOwner)
            {
                menu_item.IconImageSource = new FontImageSource() { Glyph = "\uf4fc", FontFamily = "FontAwesomeSolid",Size = 18,Color= Color.White };

                //menu_item.Text = "\uf4fc";
            }
            else
            {
                menu_item.IconImageSource = new FontImageSource() { Glyph = "\uf007", FontFamily = "FontAwesomeSolid", Size = 18, Color = Color.White };
                //menu_item.Text = "\uf007";
            }
            viewModel.CreateFilterXml();
            Init();
        }

        public async void Init()
        {
           // await viewModel.LoadTotalDirectSale();
            await viewModel.LoadTotalDirectSale2();
            if (viewModel.Blocks != null && viewModel.Blocks.Count != 0)
            {
                var rd = stackBlocks.Children[0] as RadBorder;
                var lb = rd.Content as Label;
                VisualStateManager.GoToState(rd, "Selected");
                VisualStateManager.GoToState(lb, "Selected");
                viewModel.Block = viewModel.Blocks[0];
                if (viewModel.Block.Floors.Count != 0)
                {
                    var floor = viewModel.Block.Floors[0];
                    floor.iShow = true;
                    await viewModel.LoadUnitByFloor(floor.bsd_floorid);
                    AddToolTip();
                    SetRealTimeData();
                    OnCompleted?.Invoke(0);
                }
                else
                {
                    OnCompleted?.Invoke(1);
                }
            }
            else
            {
                OnCompleted?.Invoke(2);
                return;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // giu cho thanh cong hoac huy giu cho thanh cong
            if (NeedToRefreshDirectSale == true)
            {
                await ShowUnit(viewModel.Unit.productid);
                RefreshDirectSale = true;
                NeedToRefreshDirectSale = false;
            }
        }

        public void SetRealTimeData()
        {
            bool temp = false;
            //int _currentNumQuese = 0;
            var condition = RealTimeHelper.firebaseClient.Child("test").Child("DirectSaleNew").AsObservable<ResponseRealtime>()
                .Subscribe(async (dbevent) =>
                {
                    try
                    {
                        if (dbevent.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate && dbevent.Object != null && temp == true)
                        {
                            try
                            {
                                var item = dbevent.Object as ResponseRealtime;
                                viewModel.Block.Floors.Where(x => x.bsd_floorid == Guid.Parse(item.FloorId)).ToList().ForEach(x =>
                                {
                                    var _unit = x.Units.SingleOrDefault(y => y.productid.ToString().ToLower() == item.UnitId.ToLower());
                                    if (_unit != null)
                                    {
                                        _unit.statuscode = int.Parse(item.StatusNew);
                                        _unit.NumQueses++;
                                        viewModel.SetNumStatus(item.StatusNew, item.StatusOld ,Guid.Parse(item.FloorId));
                                        viewModel.ChangeStatusUnitPopup(item);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                ToastMessageHelper.Message(ex.Message);
                            }
                        }
                        temp = true;
                    }
                    catch (Exception ex)
                    {

                    }

                });
        }

        public void Block_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            var blockChoosed = sender as RadBorder;
            if (blockChoosed != null)
            {
                for (int i = 0; i < stackBlocks.Children.Count; i++)
                {
                    if (stackBlocks.Children[i] == blockChoosed)
                    {
                        var rd = stackBlocks.Children[i] as RadBorder;
                        var lb = rd.Content as Label;
                        VisualStateManager.GoToState(rd, "Selected");// UI trong app.xaml
                        VisualStateManager.GoToState(lb, "Selected");// UI trong app.xaml
                    }
                    else
                    {
                        var rd = stackBlocks.Children[i] as RadBorder;
                        var lb = rd.Content as Label;
                        VisualStateManager.GoToState(rd, "Normal");// UI trong app.xaml
                        VisualStateManager.GoToState(lb, "Normal");// UI trong app.xaml
                    }
                }
                var item = (Block)(blockChoosed.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                viewModel.Block = item;
            }
            LoadingHelper.Hide();
        }

        private async void ItemFloor_Tapped(object sender, EventArgs e)
        {
            var item = sender as Grid;
            var collectionFloor = ((StackLayout)item.Parent).Children[1] as FlexLayout;
            if (collectionFloor != null)
            {
                LoadingHelper.Show();
                var floor = (Floor)(item.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                if (floor != null)
                {
                    if (floor.Units.Count == 0)
                        await viewModel.LoadUnitByFloor(floor.bsd_floorid);
                    //BindableLayout.SetItemsSource(collectionFloor, floor.Units);
                    floor.iShow = !floor.iShow;
                }
                LoadingHelper.Hide();
            }
            //(((RadBorder)((StackLayout)item.Parent).Parent).Parent as ViewCell).ForceUpdateSize();
        }

        private async void UnitItem_Tapped(object sender, EventArgs e)
        {
            var unitId = (Guid)((sender as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await ShowUnit(unitId);
            PopupUnit.IsVisible = true;
        }

        private async Task ShowUnit(Guid unitId)
        {
            await LoadUnit(unitId);
            CreatePopupUnit(unitId);
        }

        private async Task LoadUnit(Guid unitId)
        {
            LoadingHelper.Show();
            await viewModel.LoadUnitById(unitId);
            LoadingHelper.Hide();
        }

        private async void ScrollView_Scrolled(System.Object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            //if (!(sender is ScrollView scrollView))
            //    return;

            //var scrollingSpace = scrollView.ContentSize.Height - scrollView.Height;

            //if (scrollingSpace > e.ScrollY)
            //    return;
            //await viewModel.LoadFloor();
            //await DisplayAlert("", "asdfasd", "ok");
            //return;
        }

        private void UnitInfor_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            UnitInfo unitInfo = new UnitInfo(viewModel.Unit.productid);
            unitInfo.OnCompleted = async (IsSuccess) =>
            {
                if (IsSuccess)
                {
                    await Navigation.PushAsync(unitInfo);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.Message(Language.khong_tim_thay_san_pham);
                }
            };
        }

        private void BangTinhGia_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            ReservationForm reservationForm = new ReservationForm(viewModel.Unit.productid, null, null, null, null);
            reservationForm.CheckReservation = async (isSuccess) =>
            {
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
                    ToastMessageHelper.Message(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }

        private void GiuCho_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            //QueueForm queue = new QueueForm(viewModel.Unit.productid, true);
            //queue.OnCompleted = async (IsSuccess) =>
            //{
            //    if (IsSuccess)
            //    {
            //        await Shell.Current.Navigation.PushAsync(queue);
            //        LoadingHelper.Hide();
            //    }
            //    else
            //    {
            //        LoadingHelper.Hide();
            //        // hiện câu thông báo bên queue form
            //    }
            //};
            QueueUnitModel queueUnit = new QueueUnitModel
            {
                unit_id = viewModel.Unit.productid,
                unit_name = viewModel.Unit.name,
                project_id = viewModel.Unit._bsd_projectcode_value,
                project_name = viewModel.Unit.project_name,
                phaseslaunch_id = viewModel.Unit._bsd_phaseslaunchid_value,
                phaseslaunch_name = viewModel.Unit.phaseslaunch_name,
                bsd_queuesperunit = viewModel.Unit.project_queuesperunit,
                bsd_unitspersalesman = viewModel.Unit.project_unitspersalesman,
                bsd_queueunitdaysaleman = viewModel.Unit.project_queueunitdaysaleman,
                bsd_bookingfee = viewModel.Unit.project_bookingfee,
                bsd_queuingfee = viewModel.Unit.bsd_queuingfee,
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

        private void CreatePopupUnit(Guid unit_id)
        {
            if (grid == null)
            {
                grid = new Grid
                {
                    RowDefinitions = {
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                    }
                    ,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = GridLength.Auto},
                        new ColumnDefinition{ Width = GridLength.Auto},
                        new ColumnDefinition{ Width = GridLength.Auto},
                        new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                        new ColumnDefinition{ Width = GridLength.Auto},
                    }
                };
                PopupUnit.Body = grid;

                //status
                RadBorder radBorder = new RadBorder();
                radBorder.CornerRadius = 5;
                radBorder.SetBinding(RadBorder.BackgroundColorProperty, "Unit.statuscode_color");
                Label label = new Label();
                label.SetBinding(Label.TextProperty, "Unit.statuscode_format");
                label.FontSize = 14;
                label.FontAttributes = FontAttributes.Bold;
                label.TextColor = Color.White;
                label.Margin = 5;
                radBorder.Content = label;
                grid.Children.Add(radBorder);
                Grid.SetColumn(radBorder, 0);
                Grid.SetRow(radBorder, 0);

                // ten
                labelName = new Label { FontSize = 16, TextColor = Color.FromHex("#1C78C2"), FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, "Unit.name");
                grid.Children.Add(labelName);
                Grid.SetRow(labelName, 0);

                //vip
                BoxView boxView = new BoxView();
                boxView.HeightRequest = 20;
                boxView.WidthRequest = 1;
                boxView.BackgroundColor = Color.Gray;
                boxView.VerticalOptions = LayoutOptions.Center;
                boxView.HorizontalOptions = LayoutOptions.Center;
                boxView.SetBinding(BoxView.IsVisibleProperty, "Unit.bsd_vippriority");
                grid.Children.Add(boxView);
                Grid.SetColumn(boxView, 1);
                Grid.SetRow(boxView, 0);

                RadBorder radBorderVip = new RadBorder();
                radBorderVip.CornerRadius = 5;
                radBorderVip.BackgroundColor = Color.FromHex("#FEC93D");
                radBorderVip.SetBinding(RadBorder.IsVisibleProperty, "Unit.bsd_vippriority");
                Label labelVip = new Label();
                labelVip.FontSize = 14;
                labelVip.TextColor = Color.White;
                labelVip.Margin = 5;
                radBorderVip.Content = labelVip;

                var formattedString = new FormattedString();
                formattedString.Spans.Add(new Span { Text = "\uf005", FontFamily = "FontAwesomeSolid" });
                formattedString.Spans.Add(new Span { Text = "VIP", FontAttributes = FontAttributes.Bold });
                labelVip.FormattedText = formattedString;
                grid.Children.Add(radBorderVip);
                Grid.SetColumn(radBorderVip, 2);
                Grid.SetRow(radBorderVip, 0);

                // su kien
                var formattedStringEvent = new FormattedString();
                formattedStringEvent.Spans.Add(new Span { Text = Language.su_kien, FontAttributes = FontAttributes.Bold });
                formattedStringEvent.Spans.Add(new Span { Text = "\uf005", FontFamily = "FontAwesomeSolid" });
                Label labelEvent = new Label { FontSize = 14, TextColor = Color.FromHex("#FEC93D"), HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, FormattedText = formattedStringEvent };
                labelEvent.SetBinding(Label.IsVisibleProperty, "Unit.has_event");
                grid.Children.Add(labelEvent);
                Grid.SetColumn(labelEvent, 4);
                Grid.SetRow(labelEvent, 0);

                // gia
                Label labelPrice = new Label { FontSize = 16, TextColor = Color.Red, FontAttributes = FontAttributes.Bold };
                labelPrice.SetBinding(Label.TextProperty, new Binding("Unit.price_format") { StringFormat = "{0} đ" });
                grid.Children.Add(labelPrice);
                Grid.SetColumn(labelPrice, 0);
                Grid.SetRow(labelPrice, 1);
                Grid.SetColumnSpan(labelPrice, 5);

                //dien tich su dung
                FieldListViewItem field = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.dien_tich_su_dung, FontAttributes = FontAttributes.Bold, TextColor = Color.FromHex("#444444") };
                field.SetBinding(FieldListViewItem.TextProperty, "Unit.bsd_netsaleablearea_format");
                grid.Children.Add(field);
                Grid.SetColumn(field, 0);
                Grid.SetRow(field, 2);
                Grid.SetColumnSpan(field, 5);

                //loai unit
                FieldListViewItem fieldUnitType = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.loai_unit, TextColor = Color.FromHex("#444444") };
                fieldUnitType.SetBinding(FieldListViewItem.TextProperty, "Unit.bsd_unittype_name");
                grid.Children.Add(fieldUnitType);
                Grid.SetColumn(fieldUnitType, 0);
                Grid.SetRow(fieldUnitType, 3);
                Grid.SetColumnSpan(fieldUnitType, 5);

                // grid huong nhin
                Grid gridhuong = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = GridLength.Auto},
                        new ColumnDefinition{ Width = GridLength.Auto}
                    }
                };

                //huong
                FieldListViewItem fieldhuong = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.huong, TextColor = Color.FromHex("#444444") };
                fieldhuong.SetBinding(FieldListViewItem.TextProperty, "Unit.bsd_direction_format");
                gridhuong.Children.Add(fieldhuong);
                Grid.SetColumn(fieldhuong, 0);
                Grid.SetRow(fieldhuong, 0);

                //huong nhin
                FieldListViewItem fieldhuongnhin = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.huong_nhin, TextColor = Color.FromHex("#444444") };
                fieldhuongnhin.SetBinding(FieldListViewItem.TextProperty, "Unit.bsd_viewphulong_format");
                gridhuong.Children.Add(fieldhuongnhin);
                Grid.SetColumn(fieldhuongnhin, 1);
                Grid.SetRow(fieldhuongnhin, 0);

                grid.Children.Add(gridhuong);
                Grid.SetColumn(gridhuong, 0);
                Grid.SetRow(gridhuong, 4);
                Grid.SetColumnSpan(gridhuong, 5);

                // btn xem thong tin
                RadBorder radBorderUnitInf = new RadBorder { CornerRadius = 10, BorderColor = Color.FromHex("#2196F3"), BorderThickness = 1, BackgroundColor = Color.White, Padding = 8, HorizontalOptions = LayoutOptions.Fill, Margin = new Thickness(0, 5) };
                TapGestureRecognizer tapUnitInf = new TapGestureRecognizer();
                tapUnitInf.Tapped += UnitInfor_Clicked;
                radBorderUnitInf.GestureRecognizers.Add(tapUnitInf);

                var formattedlabelUnitInfo = new FormattedString();
                formattedlabelUnitInfo.Spans.Add(new Span { Text = Language.xem_thong_tin, FontAttributes = FontAttributes.Bold, FontSize = 15 });
                formattedlabelUnitInfo.Spans.Add(new Span { Text = "   " });
                formattedlabelUnitInfo.Spans.Add(new Span { Text = "\uf101", FontFamily = "FontAwesomeSolid", FontSize = 18 });
                Label labelUnitInfo = new Label { TextColor = Color.FromHex("#2196F3"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, FormattedText = formattedlabelUnitInfo };
                radBorderUnitInf.Content = labelUnitInfo;
                grid.Children.Add(radBorderUnitInf);
                Grid.SetColumn(radBorderUnitInf, 0);
                Grid.SetRow(radBorderUnitInf, 5);
                Grid.SetColumnSpan(radBorderUnitInf, 5);

                Label labelQueue = new Label { Text = Language.giu_cho_title, FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalTextAlignment = TextAlignment.Center, Padding = new Thickness(10, 8), BackgroundColor = Color.FromHex("#f4fafe"), TextColor = Color.FromHex("#145a92"), Margin = new Thickness(-10, 0) };
                grid.Children.Add(labelQueue);
                Grid.SetColumn(labelQueue, 0);
                Grid.SetRow(labelQueue, 6);
                Grid.SetColumnSpan(labelQueue, 5);

                //create button
                gridBtn = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(),
                        new ColumnDefinition()
                    },
                    HorizontalOptions = LayoutOptions.Fill
                };
                grid.Children.Add(gridBtn);
                Grid.SetColumn(gridBtn, 0);
                Grid.SetRow(gridBtn, 8);
                Grid.SetColumnSpan(gridBtn, 5);

                btnQueue = new Button { Text = Language.giu_cho_btn, TextColor = Color.White, BackgroundColor = (Color)Application.Current.Resources["NavigationPrimary"], CornerRadius = 10, FontAttributes = FontAttributes.Bold, HeightRequest = 45, Padding = 5 };
                btnQueue.Clicked += GiuCho_Clicked;
                gridBtn.Children.Add(btnQueue);
                btnQuote = new Button { Text = Language.bang_tinh_gia_btn, BackgroundColor = Color.White, TextColor = (Color)Application.Current.Resources["NavigationPrimary"], BorderWidth = 1, BorderColor = (Color)Application.Current.Resources["NavigationPrimary"], CornerRadius = 10, FontAttributes = FontAttributes.Bold, HeightRequest = 45, Padding = 5 };
                btnQuote.Clicked += BangTinhGia_Clicked;
                gridBtn.Children.Add(btnQuote);
            }
            QueuesControl queuesControl = new QueuesControl(unit_id);
            queuesControl.HeightRequest = 400;
            grid.Children.Add(queuesControl);
            Grid.SetColumn(queuesControl, 0);
            Grid.SetRow(queuesControl, 7);
            Grid.SetColumnSpan(queuesControl, 5);

            // hiện btn giữ chỗ availabe, queuing, preparing, booking
            if (viewModel.Unit.statuscode == 1 || viewModel.Unit.statuscode == 100000000
                || viewModel.Unit.statuscode == 100000004 || viewModel.Unit.statuscode == 100000007)
            {
                btnQueue.IsVisible = viewModel.Unit.bsd_vippriority ? false : true;
                if (viewModel.Unit.statuscode != 1 && viewModel.IsShowBtnBangTinhGia == true)
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
                btnQueue.IsVisible = false;
                viewModel.IsShowBtnBangTinhGia = false;
            }
           
            btnQuote.IsVisible = viewModel.IsShowBtnBangTinhGia;

            //set button
            if (btnQueue.IsVisible == false && btnQuote.IsVisible == false)
            {
                gridBtn.IsVisible = false;
            }
            else if (btnQueue.IsVisible == true && btnQuote.IsVisible == true)
            {
                gridBtn.IsVisible = true;
                Grid.SetColumnSpan(btnQueue, 1);
                Grid.SetColumnSpan(btnQuote, 1);
                Grid.SetColumn(btnQueue, 0);
                Grid.SetColumn(btnQuote, 1);
                
            }
            else if (btnQueue.IsVisible == true && btnQuote.IsVisible == false)
            {
                gridBtn.IsVisible = true;
                Grid.SetColumn(btnQueue, 0);
                Grid.SetColumnSpan(btnQueue, 2);
            }
            else if (btnQueue.IsVisible == false && btnQuote.IsVisible == true)
            {
                gridBtn.IsVisible = true;
                Grid.SetColumn(btnQuote, 0);
                Grid.SetColumnSpan(btnQuote, 2);
            }
            // set vip
            if (viewModel.Unit.bsd_vippriority)
            {
                Grid.SetColumn(labelName, 3);
                Grid.SetColumnSpan(labelName, 1);
            }
            else
            {
                Grid.SetColumn(labelName, 1);
                Grid.SetColumnSpan(labelName, 3);
            }
            gridBtn.IsVisible = viewModel.Unit.bsd_vippriority ? false : true;
        }

        private async void PopupUnit_Close(object sender, EventArgs e)
        {
            try
            {
                if (RefreshDirectSale == true)
                {
                    if (viewModel.Block != null && viewModel.Block.Floors != null && viewModel.Block.Floors.Count != 0)
                    {
                        LoadingHelper.Show();
                        var floor = viewModel.Block.Floors.SingleOrDefault(x => x.bsd_floorid == viewModel.Unit.floorid);
                        floor.Units.Clear();
                        await viewModel.LoadUnitByFloor(floor.bsd_floorid);
                        await viewModel.UpdateTotalDirectSale(floor);
                        LoadingHelper.Hide();
                    }
                    RefreshDirectSale = false;
                }
            }catch(Exception ex)
            {

            }
        }

        private void CloseToolTips_Tapped(object sender, EventArgs e)
        {
            foreach (var c in gridStatus.Children)
            {
                if (TooltipEffect.GetHasTooltip(c))
                {
                    TooltipEffect.SetHasTooltip(c, false);
                    TooltipEffect.SetHasTooltip(c, true);
                }
            }
        }

        public void AddToolTip()
        {
            foreach (var c in gridStatus.Children)
            {
                ListToolTip.ToolTips.Add(c);
            }
        }

        private async void Owner_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            try
            {
                if (viewModel.Filter.isOwner)
                {
                    viewModel.Filter.Employee = null;
                    viewModel.Filter.isOwner = false;
                    //menu_item.Text = "\uf007";
                    menu_item.IconImageSource = new FontImageSource() { Glyph = "\uf007", FontFamily = "FontAwesomeSolid", Size = 18, Color = Color.White };
                }
                else
                {
                    viewModel.Filter.Employee = UserLogged.Id.ToString();
                    viewModel.Filter.isOwner = true;
                    //menu_item.Text = "\uf4fc";
                    menu_item.IconImageSource = new FontImageSource() { Glyph = "\uf4fc", FontFamily = "FontAwesomeSolid", Size = 18, Color = Color.White };
                }
                viewModel.Blocks = new ObservableCollection<Block>();
                NeedToRefreshDirectSale = false;
                viewModel.CreateFilterXml();
                await viewModel.LoadTotalDirectSale2();

                if (viewModel.Blocks != null && viewModel.Blocks.Count != 0)
                {
                    viewModel.Block = viewModel.Blocks[0];
                    if (viewModel.Block.Floors.Count != 0)
                    {
                        var floor = viewModel.Block.Floors[0];
                        floor.iShow = true;
                        await viewModel.LoadUnitByFloor(floor.bsd_floorid);
                        AddToolTip();
                        SetRealTimeData();
                    }
                    BindableLayout.SetItemsSource(stackBlocks, viewModel.Blocks);
                    var rd = stackBlocks.Children[0] as RadBorder;
                    var lb = rd.Content as Label;
                    VisualStateManager.GoToState(rd, "Selected");
                    VisualStateManager.GoToState(lb, "Selected");
                    gridStatus.IsVisible = true;
                    line_blue.IsVisible = true;
                    lb_khong_co_du_lieu.IsVisible = false;
                }
                else
                {
                    BindableLayout.SetItemsSource(stackBlocks, viewModel.Blocks);
                    viewModel.Block = new Block();
                    gridStatus.IsVisible = false;
                    line_blue.IsVisible = false;
                    lb_khong_co_du_lieu.IsVisible = true;
                }    
                LoadingHelper.Hide();
            }
            catch(Exception ex)
            {

            }
        }
    }
    public class QueuesControl : BsdListView
    {
        private DataTemplate dataTemplate;
        private QueuesControlViewModel viewModel;
        public QueuesControl(Guid unit_id)
        {
            this.BindingContext = viewModel = new QueuesControlViewModel(unit_id);
            this.SetBinding(BsdListView.ItemsSourceProperty, "Data");
            this.Margin = new Thickness(-10, 0);
            this.BackgroundColor = Color.White;
            Init();
        }
        private async void Init()
        {
            await viewModel.LoadData();
            CreateItemTemplate();
        }

        public void CreateItemTemplate()
        {
            if (dataTemplate == null)
            {
                dataTemplate = new DataTemplate(() =>
                {
                    Grid grid = new Grid
                    {
                        RowDefinitions = {
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                        new RowDefinition{Height = GridLength.Auto },
                    }
                    ,
                        ColumnDefinitions =
                    {
                        new ColumnDefinition{ Width = GridLength.Auto},
                        new ColumnDefinition{ Width = new GridLength(1,GridUnitType.Star)},
                    },
                        Margin = new Thickness(0, 1, 0, 0),
                        Padding = 10,
                        BackgroundColor = Color.White
                    };

                    TapGestureRecognizer tapped = new TapGestureRecognizer();
                    tapped.Tapped += Tapped_Tapped;
                    tapped.SetBinding(TapGestureRecognizer.CommandParameterProperty, new Binding("."));
                    grid.GestureRecognizers.Add(tapped);

                    //status
                    RadBorder radBorder = new RadBorder { CornerRadius = 5, VerticalOptions = LayoutOptions.Start };
                    radBorder.SetBinding(RadBorder.BackgroundColorProperty, "statuscode_color");
                    Label label = new Label();
                    label.SetBinding(Label.TextProperty, "statuscode_format");
                    label.FontSize = 14;
                    label.FontAttributes = FontAttributes.Bold;
                    label.TextColor = Color.White;
                    label.Margin = 5;
                    radBorder.Content = label;
                    grid.Children.Add(radBorder);
                    Grid.SetColumn(radBorder, 0);
                    Grid.SetRow(radBorder, 0);

                    //ten
                    Label labelName = new Label { FontSize = 15, TextColor = (Color)Application.Current.Resources["NavigationPrimary"], FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

                    labelName.SetBinding(Label.TextProperty, new MultiBinding
                    {
                        Bindings = new Collection<BindingBase>
                        {
                            new Binding(){Source = Language.ma},
                            new Binding("bsd_queuenumber"),
                            new Binding("name")
                        },
                        StringFormat = "({0}: {1}) {2}"
                    });

                    grid.Children.Add(labelName);
                    Grid.SetColumn(labelName, 1);
                    Grid.SetRow(labelName, 0);

                    //khach hang
                    FieldListViewItem fieldKH = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.khach_hang, TextColor = Color.FromHex("#444444") };
                    fieldKH.SetBinding(FieldListViewItem.TextProperty, "customername");
                    grid.Children.Add(fieldKH);
                    Grid.SetColumn(fieldKH, 0);
                    Grid.SetRow(fieldKH, 1);
                    Grid.SetColumnSpan(fieldKH, 2);

                    // thoi gian het han
                    FieldListViewItem fieldDate = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.thoi_gian_het_han, TextColor = Color.FromHex("#444444") };
                    fieldDate.SetBinding(FieldListViewItem.TextProperty, new Binding("bsd_queuingexpired") { StringFormat = "{0:dd/MM/yyyy - HH:mm}" });
                    grid.Children.Add(fieldDate);
                    Grid.SetColumn(fieldDate, 0);
                    Grid.SetRow(fieldDate, 3);
                    Grid.SetColumnSpan(fieldDate, 2);

                    // phi_giu_cho_da_thanh_toan
                    FieldListViewItem fieldPaid = new FieldListViewItem { TitleTextColor = Color.FromHex("#444444"), Title = Language.phi_giu_cho_da_thanh_toan, TextColor = Color.FromHex("#444444") };
                    fieldPaid.SetBinding(FieldListViewItem.TextProperty, "bsd_queuingfeepaid_format");
                    grid.Children.Add(fieldPaid);
                    Grid.SetColumn(fieldPaid, 0);
                    Grid.SetRow(fieldPaid, 4);
                    Grid.SetColumnSpan(fieldPaid, 2);

                    return new ViewCell { View = grid };
                });
            }
            this.ItemTemplate = dataTemplate;
        }

        private void Tapped_Tapped(object sender, EventArgs e)
        {
            var item = (QueueListModel)((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item == null) return;
            LoadingHelper.Show();
            QueuesDetialPage queuesDetialPage = new QueuesDetialPage(item.opportunityid);
            queuesDetialPage.OnCompleted = async (isSuccess) =>
            {
                if (isSuccess)
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
    }
    public class QueuesControlViewModel : ListViewBaseViewModel2<QueueListModel>
    {
        public QueuesControlViewModel(Guid unit_id)
        {
            PreLoadData = new Command(() =>
            {
                EntityName = "opportunities";
                FetchXml = $@"<fetch version='1.0' count='5' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='opportunity'>
                        <attribute name='name'/>
                        <attribute name='statuscode' />
                        <attribute name='bsd_project' />
                        <attribute name='opportunityid' />
                        <attribute name='bsd_queuingfeepaid' />
                        <attribute name='bsd_collectedqueuingfee' />
                        <attribute name='bsd_queuingexpired' />
                        <attribute name='bsd_queuenumber' />
                        <attribute name='bsd_queueforproject' />
                        <order attribute='statuscode' descending='false' />
                        <filter type='and'>
                            <condition attribute='bsd_units' operator='eq' value='{unit_id}'/>
                            <condition attribute='{UserLogged.UserAttribute}' operator='eq' value='{UserLogged.Id}'/>
                            <condition attribute='statuscode' operator='in'>
                                <value>100000002</value>
                                <value>100000000</value>
                            </condition>
                        </filter>
                        <link-entity name='bsd_project' from='bsd_projectid' to='bsd_project' visible='false' link-type='outer' alias='a_edc3f143ba81e911a83b000d3a07be23'>
                            <attribute name='bsd_name' alias='project_name'/>
                        </link-entity>
                        <link-entity name='account' from='accountid' to='parentaccountid' visible='false' link-type='outer' alias='a_87ea9a00777ee911a83b000d3a07fbb4'>
                            <attribute name='name' alias='account_name'/>
                        </link-entity>
                        <link-entity name='contact' from='contactid' to='parentcontactid' visible='false' link-type='outer' alias='a_8eea9a00777ee911a83b000d3a07fbb4'>
                            <attribute name='bsd_fullname' alias='contact_name'/>
                        </link-entity>
                      </entity>
                    </fetch>";
            });
        }
    }
}