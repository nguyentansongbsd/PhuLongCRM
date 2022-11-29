using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;
using QuickLook;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DownloaderService))]
namespace PhuLongCRM.iOS.Services
{
    public class DownloaderService : IDownLoaderService
    {
        public event EventHandler<DownloadEventArgs> OnFileDownloaded;
        string pathToNewFile = string.Empty;

        public void DownloadFile(string url, string folder)
        {
            string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            Directory.CreateDirectory(pathToNewFolder);

            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, Path.GetFileName(url));
                webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
            }
            catch (Exception ex)
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
            }
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
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

                
                //MemoryStream stream = new MemoryStream(data);
                //FileStream fileStream = File.Open(pathToNewFile, FileMode.Create);
                //stream.Position = 0;
                //stream.CopyTo(fileStream);
                //fileStream.Flush();
                //fileStream.Close();

                UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (currentController.PresentedViewController != null)
                    currentController = currentController.PresentedViewController;
                UIView currentView = currentController.View;

                QLPreviewController qlPreview = new QLPreviewController();
                QLPreviewItem item = new QLPreviewItemBundle("sample.pdf", pathToNewFile);
                qlPreview.DataSource = new PreviewControllerDS(item);

                currentController.PresentViewController(qlPreview, true, null);
            }
        }
    }
}
