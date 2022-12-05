using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PhuLongCRM.Helper;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;
using QuickLook;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenFileService))]
namespace PhuLongCRM.iOS.Services
{
    public class OpenFileService : IOpenFileService
    {
        public async Task OpenFile(string fileName, byte[] arr = null, string url = null, string folder = "Download/PhuLongFiles")
        {
            //Get the root path in iOS device.
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, fileName);

            //Create a file and write the stream into it.
            byte[] data = arr;
            if (arr == null && !string.IsNullOrWhiteSpace(url))
            {
                WebClient webClient = new WebClient();
                data = await webClient.DownloadDataTaskAsync(url);
            }
            MemoryStream stream = new MemoryStream(data);
            FileStream fileStream = File.Open(filePath, FileMode.Create);
            stream.Position = 0;
            stream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();

            //Invoke the saved document for viewing
            UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (currentController.PresentedViewController != null)
                currentController = currentController.PresentedViewController;
            UIView currentView = currentController.View;

            QLPreviewController qlPreview = new QLPreviewController();
            QLPreviewItem item = new QLPreviewItemBundle(fileName, filePath);
            qlPreview.DataSource = new PreviewControllerDS(item);

            currentController.PresentViewController(qlPreview, true, null);

            LoadingHelper.Hide();
        }

        public async Task OpenFilePdfFromUrl(string fileName, string url)
        {
            await DependencyService.Get<IOpenFileService>().OpenFile(fileName,null,url);
        }
    }
}
