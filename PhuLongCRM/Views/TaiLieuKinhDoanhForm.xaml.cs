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
            await viewModel.loadAllSalesLiteratureIten();
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
        
        public async Task<string> downloadFile_salesliteratureitem(Guid salesliteratureitemid)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='salesliteratureitem'>
                                    <attribute name='filename' />
                                    <attribute name='documentbody' />
                                    <order attribute='modifiedon' descending='false' />
                                    <filter type='and'>
                                        <condition attribute='salesliteratureitemid' operator='eq' value='{" + salesliteratureitemid + @"}' />
                                    </filter>
                                  </entity>
                                </fetch>";
            var result = await CrmHelper.RetrieveMultiple<RetrieveMultipleApiResponse<SalesLiteratureItemListModel>>("salesliteratureitems", fetch);
            if(result == null)
            {
                return null;
            }

            var data = result.value.FirstOrDefault();

            var fileName = data.filename;
            if (data.documentbody == null)
            {
                return null;
            }
            var body = data.documentbody;

            byte[] arr = Convert.FromBase64String(body);
            MemoryStream stream = new MemoryStream(arr);

            await Xamarin.Forms.DependencyService.Get<IPDFSaveAndOpen>().SaveAndView(Guid.NewGuid() + ".pdf", "application / pdf", stream, PDFOpenContext.InApp);

            return "a";
            //return DependencyService.Get<IFileService>().SaveFile(fileName, arr, "Download/PhuLong");
        }

        private async void DownloadFileButton_Cliked(object sender, System.EventArgs e)
        {
            var readPermision = await PermissionHelper.RequestPermission<Permissions.StorageRead>("", "", PermissionStatus.Granted);
            var writePermision = await PermissionHelper.RequestPermission<Permissions.StorageWrite>("", "", PermissionStatus.Granted);

            var item = (SalesLiteratureItemListModel)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            await this.downloadFile_salesliteratureitem(item.salesliteratureitemid);

            //await Navigation.PushAsync(new ViewPDFFilePage(filepath));




            //if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted && await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
            //{
            //    viewModel.IsBusy = true;
            //    var item = (SalesLiteratureItemListModel)((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;

            //    var res = await this.downloadFile_salesliteratureitem(item.salesliteratureitemid);
            //    if (res)
            //    {
            //        item.status = 1;
            //        viewModel.list_DownLoad.Add(item);
            //        popup_dowload_file.ItemSource = viewModel.list_DownLoad;
            //        popup_dowload_file.focus();
            //    }
            //    else
            //    {
            //        await DisplayAlert("", "Lỗi. Vui lòng thử lại", "Ok");
            //    }
            //    popup_dowload_file.isTapable = true;
            //    viewModel.IsBusy = false;
            //}
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
    }
}
