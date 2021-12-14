using PhuLongCRM.Helper;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class LichLamViec : ContentPage
    {
        public LichLamViec()
        {
            InitializeComponent();
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            LoadingHelper.Show();
            string item = e.Item as string;         
            if (item.Contains("tháng"))
            {
                LoadingHelper.Show();
                LichLamViecTheoThang lichLamViecTheoThang = new LichLamViecTheoThang();
                lichLamViecTheoThang.OnComplete = async (OnComplete) =>
                {
                    if (OnComplete == true)
                    {
                        await Navigation.PushAsync(lichLamViecTheoThang);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        await DisplayAlert("Thông Báo", "Không tìm thấy lịch làm việc", "Đóng");
                    }
                };
            } else if (item.Contains("tuần"))
            {
                LoadingHelper.Show();
                LichLamViecTheoTuan lichLamViecTheoTuan = new LichLamViecTheoTuan();
                lichLamViecTheoTuan.OnComplete = async (OnComplete) =>
                {
                    if (OnComplete == true)
                    {
                        await Navigation.PushAsync(lichLamViecTheoTuan);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        await DisplayAlert("Thông Báo", "Không tìm thấy lịch làm việc", "Đóng");
                    }
                };               
            }else if (item.Contains("ngày"))
            {
                LoadingHelper.Show();
                LichLamViecTheoNgay lichLamViecTheoNgay = new LichLamViecTheoNgay();
                lichLamViecTheoNgay.OnComplete = async (OnComplete) =>
                {
                    if (OnComplete == true)
                    {
                        await Navigation.PushAsync(lichLamViecTheoNgay);
                        LoadingHelper.Hide();
                    }
                    else
                    {
                        LoadingHelper.Hide();
                        await DisplayAlert("Thông Báo", "Không tìm thấy lịch làm việc", "Đóng");
                    }
                };              
            }         
        }
    }
}
