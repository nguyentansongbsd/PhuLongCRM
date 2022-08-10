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
        public async Task SaveAndView(string fileName, string contentType, MemoryStream stream, PDFOpenContext context)
        {
            string exception = string.Empty;
            string root = null;

            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions((Activity)Android.App.Application.Context, new String[] { Manifest.Permission.WriteExternalStorage }, 1);
            }

            //if (Android.OS.Environment.IsExternalStorageEmulated)
            //{
            //    root = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //}
            //else
            //    root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/" + "Download/PDFFiles");
            dir.Mkdirs();

            var filePath = Path.Combine(dir.Path, fileName);

            //using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            //{
            //    int length = data.Length;
            //    fs.Write(data, 0, length);
            //}

            //Java.IO.File myDir = new Java.IO.File(root + "/PDFFiles");
            //myDir.Mkdir();

            //Java.IO.File file = new Java.IO.File(myDir, fileName);

            //if (file.Exists()) file.Delete();

            //try
            //{
            //    FileOutputStream outs = new FileOutputStream(file);
            //    outs.Write(stream.ToArray());

            //    outs.Flush();
            //    outs.Close();
            //}
            //catch (Exception e)
            //{
            //    exception = e.ToString();
            //}

            //if (file.Exists() && contentType != "application/html")
            //{
            //    string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            //    string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            //    Intent intent = new Intent(Intent.ActionView);
            //    intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            //    Android.Net.Uri path = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".provider", file);
            //    intent.SetDataAndType(path, mimeType);
            //    intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            //    switch (context)
            //    {
            //        case PDFOpenContext.InApp:
            //            Android.App.Application.Context.StartActivity(intent);
            //            break;
            //        case PDFOpenContext.ChooseApp:
            //            Android.App.Application.Context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }
    }
}