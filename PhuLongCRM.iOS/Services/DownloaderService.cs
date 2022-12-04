using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;
using PhuLongCRM.Views;
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
            try
            {
                string fileName = Guid.NewGuid() + ".docx";

                string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                Directory.CreateDirectory(pathToNewFolder);

                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                pathToNewFile = Path.Combine(pathToNewFolder, fileName);
                webClient.DownloadFileAsync(new Uri(url), pathToNewFile);

                UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (currentController.PresentedViewController != null)
                    currentController = currentController.PresentedViewController;
                UIView currentView = currentController.View;

                QLPreviewController qlPreview = new QLPreviewController();
                QLPreviewItem item = new QLPreviewItemBundle(fileName, pathToNewFile);
                qlPreview.DataSource = new PreviewControllerDS(item);

                currentController.PresentViewController(qlPreview, true, null);

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
                try
                {
                    //NavigationPage navigationPage = new NavigationPage();
                    //navigationPage.PushAsync(new ViewPDFFilePage("https://www.africau.edu/images/default/sample.pdf") { Title = "song" });

                    //DependencyService.Get<IPdfService>().View(pathToNewFile, "sample");
                    //await DependencyService.Get<IPdfService>().View("https://www.africau.edu/images/default/sample.pdf","song");
                    //await AppShell.Current.Navigation.PushAsync(new ViewPDFFilePage("https://www.africau.edu/images/default/sample.pdf") { Title = "song" });
                }
                catch (Exception ex)
                {

                }
                
            }
        }
    }
}
