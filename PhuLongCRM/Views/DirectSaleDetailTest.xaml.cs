﻿using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectSaleDetailTest : ContentPage
    {
        public static bool? NeedToRefreshDirectSale = null;
        private bool RefreshDirectSale { get; set; }
        public Action<int> OnCompleted;
        private DirectSaleDetailTestViewModel viewModel;
        public DirectSaleDetailTest(DirectSaleSearchModel filter)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new DirectSaleDetailTestViewModel();
            viewModel.Filter = filter;
            Init();
        }
        public async void Init()
        {
            await viewModel.LoadTotalDirectSale();
            if (viewModel.DirectSaleResult != null && viewModel.DirectSaleResult.Count != 0)
            {
                var rd = stackBlocks.Children[0] as RadBorder;
                var lb = rd.Content as Label;
                VisualStateManager.GoToState(rd, "Selected");
                VisualStateManager.GoToState(lb, "Selected");
                NumberUnitInBlock(viewModel.DirectSaleResult[0]);
               await viewModel.LoadFloor();
            }
            else
            {
                OnCompleted?.Invoke(2);
                return;
            }

            //if (viewModel.Block.Floors.Count != 0)
            //{

            //    OnCompleted?.Invoke(0);
            //}
            //else
            //{
            //    OnCompleted?.Invoke(1);
            //}
            OnCompleted?.Invoke(0);
        }
        public async void Block_Tapped(object sender, EventArgs e)
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
                var item = (DirectSaleModel)(blockChoosed.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                NumberUnitInBlock(item);
            }
            LoadingHelper.Hide();
        }
        public void NumberUnitInBlock(DirectSaleModel block)
        {
            if (block != null)
            {
                var arrStatus = block.stringQty.Split(',');
                viewModel.Block = block;
                
            }
        }
        private void NumberUnit_Tapped(object sender, EventArgs e)
        {
            var item = (int)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            if (item == 1)
                PopupHover.ShowHover(Language.chuan_bi);
            else if (item == 2)
                PopupHover.ShowHover(Language.san_sang);
            else if (item == 3)
                PopupHover.ShowHover(Language.dat_cho);
            else if (item == 4)
                PopupHover.ShowHover(Language.giu_cho);
            else if (item == 5)
                PopupHover.ShowHover(Language.dat_coc);
            else if (item == 6)
                PopupHover.ShowHover(Language.dong_y_chuyen_coc);
            else if (item == 7)
                PopupHover.ShowHover(Language.da_du_tien_coc);
            else if (item == 8)
                PopupHover.ShowHover(Language.hoan_tat_dat_coc);
            else if (item == 9)
                PopupHover.ShowHover(Language.thanh_toan_dot_1);
            else if (item == 10)
                PopupHover.ShowHover(Language.da_ky_ttdc_hddc);
            else if (item == 11)
                PopupHover.ShowHover(Language.du_dieu_dien);
            else if (item == 12)
                PopupHover.ShowHover(Language.da_ban);
        }

        private async void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            await viewModel.LoadFloor();
        }

        private async void CollectionView_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            await viewModel.LoadFloor();
        }
    }
}