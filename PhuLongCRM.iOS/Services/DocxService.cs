using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;
using QuickLook;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DocxService))]
namespace PhuLongCRM.iOS.Services
{
    public class DocxService : IDocxService
    {
        public async Task OpenDocxFile(string url,string fileType)
        {
            try
            {
                string fileName = Guid.NewGuid() + fileType;

                string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                Directory.CreateDirectory(pathToNewFolder);

                WebClient webClient = new WebClient();
                //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, fileName);
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

            }
            
        }
    }
}
