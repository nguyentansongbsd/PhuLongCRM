using Android;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Webkit;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Xamarin.Forms;
using PhuLongCRM.IServices;
using PhuLongCRM.Droid.Services;

[assembly: Dependency(typeof(PDFSaveAndOpen))]
namespace PhuLongCRM.Droid.Services
{
    public class PDFSaveAndOpen : IPDFSaveAndOpen
    {
        [Obsolete]
        public async Task SaveAndView(string fileName, string contentType, MemoryStream stream, PDFOpenContext context)
        {
            string exception = string.Empty;
            string root = null;

            if (ContextCompat.CheckSelfPermission(Forms.Context, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions((Activity)Forms.Context, new String[] { Manifest.Permission.WriteExternalStorage }, 1);
            }

            string filePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
            string fullName = Path.Combine(filePath, fileName);

            // If the file exist on device delete it
            if (System.IO.File.Exists(fullName))
            {
                // Note: In the second run of this method, the file exists
                System.IO.File.Delete(fullName);
            }

            // Write bytes on "My documents"
            System.IO.File.WriteAllBytes(fullName, stream.ToArray());

            //if (file.Exists() && contentType != "application/html")
            //{
            //    string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            //    string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            //    Intent intent = new Intent(Intent.ActionView);
            //    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            //    Android.Net.Uri path = FileProvider.GetUriForFile(Forms.Context, Android.App.Application.Context.PackageName + ".provider", file);
            //    intent.SetDataAndType(path, mimeType);
            //    intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            //    switch (context)
            //    {
            //        case PDFOpenContext.InApp:
            //            Forms.Context.StartActivity(intent);
            //            break;
            //        case PDFOpenContext.ChooseApp:
            //            Forms.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }
    }
}