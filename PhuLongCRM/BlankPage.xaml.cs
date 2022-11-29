using Firebase.Database;
using Newtonsoft.Json;
using PhuLongCRM.Config;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Firebase.Database.Query;
using PhuLongCRM.ViewModels;
using System.Globalization;
using System.IO;
using ZXing.Aztec.Internal;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using Telerik.Windows.Documents.Fixed.Model;
using PhuLongCRM.IServices;

namespace PhuLongCRM
{
    public partial class BlankPage : ContentPage
    {
        private int bufferSize = 4095;
        private HttpClient _client;
        string url = "https://diaocphulong.sharepoint.com/sites/PhuLong-UAT/_layouts/15/download.aspx?UniqueId=ce918e10-82f2-4995-9b98-e91e14fd1880&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvZGlhb2NwaHVsb25nLnNoYXJlcG9pbnQuY29tQDg3YmJkYjA4LTQ4YmEtNGRiZi05YzUzLTkyY2VhZTE2YzM1MyIsImlzcyI6IjAwMDAwMDAzLTAwMDAtMGZmMS1jZTAwLTAwMDAwMDAwMDAwMCIsIm5iZiI6IjE2Njg3NTgzNzEiLCJleHAiOiIxNjY4NzYxOTcxIiwiZW5kcG9pbnR1cmwiOiJkeDVPcHlmblhMNlR3T2hCbWd0OE0xbk9oOUM4dGNiQ0VIQ3l4OE5XSVY0PSIsImVuZHBvaW50dXJsTGVuZ3RoIjoiMTQxIiwiaXNsb29wYmFjayI6IlRydWUiLCJjaWQiOiJOR1ZrTUdNNVltSXRPVEkzTWkwME4yTmxMVGcwWkRrdE5UUXhNelUwWldJMFkyUTQiLCJ2ZXIiOiJoYXNoZWRwcm9vZnRva2VuIiwic2l0ZWlkIjoiTnpSbU9HWmhaRGd0TVROak1TMDBNamhsTFdGa1pHVXRNakk1TkRNMFl6WmhNemRtIiwiYXBwX2Rpc3BsYXluYW1lIjoiQXp1cmUgQXBwIENSTSBCU0QiLCJuYW1laWQiOiJhNzU0NGE1OC1iN2JiLTQ1NTMtOTU0OC1kNTZkMWNmYmVjNTVAODdiYmRiMDgtNDhiYS00ZGJmLTljNTMtOTJjZWFlMTZjMzUzIiwicm9sZXMiOiJhbGxzaXRlcy5yZWFkIGFsbHNpdGVzLndyaXRlIiwidHQiOiIxIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbCwiaXBhZGRyIjoiMjAuMTkwLjE0NC4xNjkifQ.Q2NRVjdPU1ordkdwTzM1d0tnOVdPUXlPTklLZTF0RlpwVXU1VEp2L0tYMD0&ApiVersion=2.0";

        IDownLoaderService download = DependencyService.Get<IDownLoaderService>();

        public BlankPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            _client = new HttpClient();
            download.OnFileDownloaded += Download_OnFileDownloaded;
            Init();
        }

        private void Download_OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            if (e.FileSaved)
            {
                DisplayAlert("download", "thanh cong", "ok");
            }
            else
            {
                DisplayAlert("download", "Loi", "ok");
            }
        }

        public async void Init()
        {
            //webview.Uri = "https://diaocphulong.sharepoint.com/sites/PhuLong-UAT/_layouts/15/download.aspx?UniqueId=ce918e10-82f2-4995-9b98-e91e14fd1880&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvZGlhb2NwaHVsb25nLnNoYXJlcG9pbnQuY29tQDg3YmJkYjA4LTQ4YmEtNGRiZi05YzUzLTkyY2VhZTE2YzM1MyIsImlzcyI6IjAwMDAwMDAzLTAwMDAtMGZmMS1jZTAwLTAwMDAwMDAwMDAwMCIsIm5iZiI6IjE2Njg3NTgzNzEiLCJleHAiOiIxNjY4NzYxOTcxIiwiZW5kcG9pbnR1cmwiOiJkeDVPcHlmblhMNlR3T2hCbWd0OE0xbk9oOUM4dGNiQ0VIQ3l4OE5XSVY0PSIsImVuZHBvaW50dXJsTGVuZ3RoIjoiMTQxIiwiaXNsb29wYmFjayI6IlRydWUiLCJjaWQiOiJOR1ZrTUdNNVltSXRPVEkzTWkwME4yTmxMVGcwWkRrdE5UUXhNelUwWldJMFkyUTQiLCJ2ZXIiOiJoYXNoZWRwcm9vZnRva2VuIiwic2l0ZWlkIjoiTnpSbU9HWmhaRGd0TVROak1TMDBNamhsTFdGa1pHVXRNakk1TkRNMFl6WmhNemRtIiwiYXBwX2Rpc3BsYXluYW1lIjoiQXp1cmUgQXBwIENSTSBCU0QiLCJuYW1laWQiOiJhNzU0NGE1OC1iN2JiLTQ1NTMtOTU0OC1kNTZkMWNmYmVjNTVAODdiYmRiMDgtNDhiYS00ZGJmLTljNTMtOTJjZWFlMTZjMzUzIiwicm9sZXMiOiJhbGxzaXRlcy5yZWFkIGFsbHNpdGVzLndyaXRlIiwidHQiOiIxIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbCwiaXBhZGRyIjoiMjAuMTkwLjE0NC4xNjkifQ.Q2NRVjdPU1ordkdwTzM1d0tnOVdPUXlPTklLZTF0RlpwVXU1VEp2L0tYMD0&ApiVersion=2.0";
            //CultureInfo ci = new CultureInfo("en-us");
            //double a = 0.7500000000;
            //double e = 1.0000000000;
            //string b = string.Format("{0:#0.##,##}", a);
            //string c = string.Format("{0:#0.##,##}", e);

            try
            {

                //Device.OpenUri(new Uri("ms-word:ofe|u|https://calibre-ebook.com/downloads/demos/demo.docx"));

                //string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"song");

                //await DownloadApkAsync();

                

            }
            catch(Exception ex)
            {

            }
            


        }
        private async Task DownloadApkAsync()
        {
            var downloadedFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

            var success = await DownloadFileAsync("https://calibre-ebook.com/downloads/demos/demo.docx", downloadedFilePath);

            if (success)
            {
                Console.WriteLine($"File downloaded to: {downloadedFilePath}");
            }
            else
            {
                Console.WriteLine("Download failed");
            }
        }

        private async Task<bool> DownloadFileAsync(string fileUrl, string downloadedFilePath)
        {
            try
            {
                var client = new HttpClient();

                var downloadStream = await client.GetStreamAsync(fileUrl);

                var fileStream = System.IO.File.Create(downloadedFilePath);

                await downloadStream.CopyToAsync(fileStream);

                return true;
            }
            catch (Exception ex)
            {
                //TODO handle exception
                return false;
            }
        }
        public RadFixedDocument Document { get; set; }
        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted && await Permissions.CheckStatusAsync<Permissions.StorageWrite>() != PermissionStatus.Granted)
            {
                var readPermision = await PermissionHelper.RequestPermission<Permissions.StorageRead>("Thư Viện", "PhuLongCRM cần quyền truy cập vào thư viện", PermissionStatus.Granted);
                var writePermision = await PermissionHelper.RequestPermission<Permissions.StorageWrite>("Thư Viện", "PhuLongCRM cần quyền truy cập vào thư viện", PermissionStatus.Granted);
            }
            if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() == PermissionStatus.Granted && await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted)
            {
                download.DownloadFile("https://www.africau.edu/images/default/sample.pdf", "PhuLongDownLoad"); //http://www.dada-data.net/uploads/image/hausmann_abcd.jpg
            }
            



            //try
            //{
            //    var fileName = "sample.pdf";
            //    var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //    var filePath = Path.Combine(localFolder, fileName);
            //    using (Stream output = System.IO.File.OpenWrite(filePath))
            //    {
            //        new Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.PdfFormatProvider().Export(this.Document, output);

            //        await Application.Current.MainPage.DisplayAlert("Document is saved to local application data: ", filePath, "OK");
            //    }


            //    var downloadManager = CrossDownloadManager.Current;
            //    var file = downloadManager.CreateDownloadFile(url);
            //    downloadManager.Start(file);

                

            //    if (file.Status == Plugin.DownloadManager.Abstractions.DownloadFileStatus.INITIALIZED)
            //    {
            //        //CrossDownloadManager.Current.PathNameForDownloadedFile = new System.Func<IDownloadFile, string>(file =>
            //        //{

            //        //    string fileName = (new NSUrl(file.Url, false)).LastPathComponent;
            //        //    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            //        //});
            //    }
            //    if (file.Status == DownloadFileStatus.COMPLETED)
            //    {

            //    }

            //    //var response = await _client.GetAsync("https://calibre-ebook.com/downloads/demos/demo.docx", HttpCompletionOption.ResponseHeadersRead);

            //    //if (!response.IsSuccessStatusCode)
            //    //{
            //    //    throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
            //    //}

            //    //// Step 2 : Filename
            //    //var fileName = response.Content.Headers?.ContentDisposition?.FileName ?? "tmp.zip";

            //    //// Step 3 : Get total of data
            //    //var totalData = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
            //    ////var canSendProgress = totalData != -1L && progress != null;

            //    //// Step 4 : Get total of data
            //    //string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //    //string libFolder = Path.Combine(docFolder, "..", "Library");
            //    //var filePath = Path.Combine(libFolder, fileName);

            //    //using (var fileStream = OpenStream(filePath))
            //    //{
            //    //    using (var stream = await response.Content.ReadAsStreamAsync())
            //    //    {
            //    //        var totalRead = 0L;
            //    //        var buffer = new byte[bufferSize];
            //    //        var isMoreDataToRead = true;

            //    //        do
            //    //        {
            //    //            //token.ThrowIfCancellationRequested();

            //    //            var read = await stream.ReadAsync(buffer, 0, buffer.Length);

            //    //            if (read == 0)
            //    //            {
            //    //                isMoreDataToRead = false;
            //    //            }
            //    //            else
            //    //            {
            //    //                // Write data on disk.
            //    //                await fileStream.WriteAsync(buffer, 0, read);

            //    //                totalRead += read;

            //    //                //if (canSendProgress)
            //    //                //{
            //    //                //    progress.Report((totalRead * 1d) / (totalData * 1d) * 100);
            //    //                //}
            //    //            }
            //    //        } while (isMoreDataToRead);
            //    //    }
            //    //}
            //}
            //catch(Exception ex)
            //{

            //}

            //await DownloadApkAsync();

            
        }
        private Stream OpenStream(string path)
        {
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize);
        }
    }
    public class CustomWebView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(propertyName: "Uri",
                returnType: typeof(string),
                declaringType: typeof(CustomWebView),
                defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

    }
}
