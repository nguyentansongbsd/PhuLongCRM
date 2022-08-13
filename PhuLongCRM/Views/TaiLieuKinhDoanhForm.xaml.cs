using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using PhuLongCRM.Controls;
using Xamarin.Essentials;
using PhuLongCRM.IServices;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaiLieuKinhDoanhForm : ContentPage
    {
        public Action<bool> CheckTaiLieuKinhDoanh;
        
        public TaiLieuKinhDoanhFormViewModel viewModel;

        public TaiLieuKinhDoanhForm(Guid literatureid)
        {
            InitializeComponent();
            BindingContext = viewModel = new TaiLieuKinhDoanhFormViewModel();
            viewModel.salesliteratureid = literatureid;
            Init();          
        }

        private async void Init()
        {
            await viewModel.loadData();
            await viewModel.loadUnit();
            await viewModel.loadDoiThuCanhTranh();
            viewModel.IsBusy = false;

            if (viewModel.TaiLieuKinhDoanh != null)
            {
                CheckTaiLieuKinhDoanh(true);
            }
            else
            {
                CheckTaiLieuKinhDoanh(false);
            }
        }
        
        private async void PdfFile_Tapped(object sender, System.EventArgs e)
        {
            try
            {
                var readPermision = await PermissionHelper.RequestPermission<Permissions.StorageRead>("Thư Viện", "PhuLongCRM cần quyền truy cập vào thư viện", PermissionStatus.Granted);
                var writePermision = await PermissionHelper.RequestPermission<Permissions.StorageWrite>("Thư Viện", "PhuLongCRM cần quyền truy cập vào thư viện", PermissionStatus.Granted);

                if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted && await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
                {
                    LoadingHelper.Show();
                    var item = (SalesLiteratureItemListModel)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
                    byte[] arr = Convert.FromBase64String(item.documentbody);
                    await DependencyService.Get<IPDFSaveAndOpen>().SaveAndView(item.filename, arr);
                    LoadingHelper.Hide();
                }
            }
            catch(Exception ex)
            {
                LoadingHelper.Hide();
            }
            
        }

        void ListViewFileDownloaded_Tapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            SalesLiteratureItemListModel item = e.Item as SalesLiteratureItemListModel;
            this.popup_dowload_file.SelectedItem = null;
            //DisplayAlert("", item.filename, "OK");
            if (item.status == 1)
            {
                try
                {
                    DependencyService.Get<IFileService>().OpenFile(item.filename);
                }
                catch
                {
                    DisplayAlert("", "Ứng dụng không được hỗ trợ. Không thể mở file", "OK");
                }
            }
            else
            {
                
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (this.popup_dowload_file.IsVisible)
            {
                this.popup_dowload_file.unFocus();
                return true;
            }

            return base.OnBackButtonPressed();
        }

        private async void ShowMoreThongTinUnit_Clicked(object sender,EventArgs e)
        {
            viewModel.IsBusy = true;
            viewModel.PageThongTinUnit++;
            await viewModel.loadUnit();
            viewModel.IsBusy = false;
        }

        private async void ShowMoreDuAnCanhTranh_Clicked(object sender,EventArgs e)
        {
            viewModel.IsBusy = true;
            viewModel.PageDuAnCanhTranh++;
            await viewModel.loadDoiThuCanhTranh();
            viewModel.IsBusy = false;
        }

        private async void ShowMoreTaiLieu_Clicked(object sender, EventArgs e)
        {
            viewModel.IsBusy = true;
            viewModel.PageTaiLieu++;
            await viewModel.loadAllSalesLiteratureIten();
            viewModel.IsBusy = false;
        }

        private async void ShowFileDownLoad_Tapped(object sender,EventArgs e)
        {
            viewModel.IsBusy = true;
            if (viewModel.list_DownLoad.Count ==0)
            {
                await DisplayAlert("", "Chưa có file nào đươc tải", "Ok");
            }
            else
            {
                popup_dowload_file.focus();
                popup_dowload_file.isTapable = true;
            }

            viewModel.IsBusy = false;
        }

        private async void TabControl_IndexTab(object sender, LookUpChangeEvent e)
        {
            if (e.Item != null)
            {
                if ((int)e.Item == 0)
                {
                    ThongTin.IsVisible = true;
                    ThongTinTaiLieu.IsVisible = false;
                }
                else if ((int)e.Item == 1)
                {
                    if (viewModel.list_salesliteratureitem != null && viewModel.list_salesliteratureitem.Count <= 0)
                    {
                        LoadingHelper.Show();
                        await viewModel.loadAllSalesLiteratureIten();
                        LoadingHelper.Hide();
                    }
                    ThongTin.IsVisible = false;
                    ThongTinTaiLieu.IsVisible = true;
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            viewModel.IsBusy = true;
            if (viewModel.list_DownLoad.Count == 0)
            {
                await DisplayAlert("", "Chưa có file nào đươc tải", "Ok");
            }
            else
            {
                popup_dowload_file.focus();
                popup_dowload_file.isTapable = true;
            }
            viewModel.IsBusy = false;
        }

        private async void GoToUnit_Tapped(object sender, EventArgs e)
        {
            var id = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            LoadingHelper.Show();
            UnitInfo unit = new UnitInfo(id);
            unit.OnCompleted = async (isSuccess) => {
                if (isSuccess)
                {
                    await Navigation.PushAsync(unit);
                    LoadingHelper.Hide();
                }
                else
                {
                    LoadingHelper.Hide();
                    ToastMessageHelper.ShortMessage(Language.khong_tim_thay_thong_tin_vui_long_thu_lai);
                }
            };
        }
    }
}
