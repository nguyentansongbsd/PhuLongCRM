//using System;
//using System.IO;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using PhuLongCRM.iOS.Services;
//using PhuLongCRM.IServices;
//using QuickLook;
//using UIKit;

using System;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using UIKit;
using QuickLook;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;

[assembly: Dependency(typeof(PDFSaveAndOpen))]

namespace PhuLongCRM.iOS.Services
{
	public class PDFSaveAndOpen : IPDFSaveAndOpen
	{
        public async Task SaveAndView(string fileName, string contentType, MemoryStream stream, PDFOpenContext context)
        {
            //Get the root path in iOS device.
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, fileName);

            //Create a file and write the stream into it.
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
        }
    }
}

