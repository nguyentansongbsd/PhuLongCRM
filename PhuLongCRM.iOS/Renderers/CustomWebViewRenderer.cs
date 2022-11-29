using System;
using System.IO;
using System.Net;
using Foundation;
using PhuLongCRM;
using PhuLongCRM.iOS.Renderers;
using PhuLongCRM.iOS.Services;
using QuickLook;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using static CoreFoundation.DispatchSource;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace PhuLongCRM.iOS.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }
            if (e.OldElement != null)
            {
                // Cleanup
            }
            if (e.NewElement != null)
            {
                

                string url = "https://diaocphulong.sharepoint.com/sites/PhuLong-UAT/_layouts/15/download.aspx?UniqueId=ce918e10-82f2-4995-9b98-e91e14fd1880&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvZGlhb2NwaHVsb25nLnNoYXJlcG9pbnQuY29tQDg3YmJkYjA4LTQ4YmEtNGRiZi05YzUzLTkyY2VhZTE2YzM1MyIsImlzcyI6IjAwMDAwMDAzLTAwMDAtMGZmMS1jZTAwLTAwMDAwMDAwMDAwMCIsIm5iZiI6IjE2Njg3NTgzNzEiLCJleHAiOiIxNjY4NzYxOTcxIiwiZW5kcG9pbnR1cmwiOiJkeDVPcHlmblhMNlR3T2hCbWd0OE0xbk9oOUM4dGNiQ0VIQ3l4OE5XSVY0PSIsImVuZHBvaW50dXJsTGVuZ3RoIjoiMTQxIiwiaXNsb29wYmFjayI6IlRydWUiLCJjaWQiOiJOR1ZrTUdNNVltSXRPVEkzTWkwME4yTmxMVGcwWkRrdE5UUXhNelUwWldJMFkyUTQiLCJ2ZXIiOiJoYXNoZWRwcm9vZnRva2VuIiwic2l0ZWlkIjoiTnpSbU9HWmhaRGd0TVROak1TMDBNamhsTFdGa1pHVXRNakk1TkRNMFl6WmhNemRtIiwiYXBwX2Rpc3BsYXluYW1lIjoiQXp1cmUgQXBwIENSTSBCU0QiLCJuYW1laWQiOiJhNzU0NGE1OC1iN2JiLTQ1NTMtOTU0OC1kNTZkMWNmYmVjNTVAODdiYmRiMDgtNDhiYS00ZGJmLTljNTMtOTJjZWFlMTZjMzUzIiwicm9sZXMiOiJhbGxzaXRlcy5yZWFkIGFsbHNpdGVzLndyaXRlIiwidHQiOiIxIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbCwiaXBhZGRyIjoiMjAuMTkwLjE0NC4xNjkifQ.Q2NRVjdPU1ordkdwTzM1d0tnOVdPUXlPTklLZTF0RlpwVXU1VEp2L0tYMD0&ApiVersion=2.0";
                //var customWebView = Element as CustomWebView;
                //string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", WebUtility.UrlEncode(url)));
                //Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                //Control.ScalesPageToFit = true;

                
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), url);

                //Create a file and write the stream into it.
                MemoryStream stream = new MemoryStream();
                
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
                QLPreviewItem item = new QLPreviewItemBundle(url, filePath);
                qlPreview.DataSource = new PreviewControllerDS(item);

                currentController.PresentViewController(qlPreview, true, null);

            }
        }
    }
}
