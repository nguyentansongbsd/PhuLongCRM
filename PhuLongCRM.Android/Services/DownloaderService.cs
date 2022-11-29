using System;
using System.IO;
using System.Net;
using PhuLongCRM.Droid.Services;
using PhuLongCRM.IServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(DownloaderService))]
namespace PhuLongCRM.Droid.Services
{
    public class DownloaderService : IDownLoaderService
    {
        public event EventHandler<DownloadEventArgs> OnFileDownloaded;

        public void DownloadFile(string url,string folder)
        {
            //var context = Android.App.Application.Context;
            //var pathToNewFolder = Path.Combine(context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).ToString());
            
            try
            {
                string pathToNewFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, folder);
                Directory.CreateDirectory(pathToNewFolder);
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, Path.GetFileName(url));
                webClient.DownloadFileAsync(new Uri(url),pathToNewFile);
            }
            catch (Exception ex)
            {
                if (OnFileDownloaded != null)
                {
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
                }
            }
        }

        private void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
            }
            else
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(true));
            }
        }
    }
}
