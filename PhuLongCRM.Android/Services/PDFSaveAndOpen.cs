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
using Android.Support.Design.Widget;

[assembly: Dependency(typeof(PDFSaveAndOpen))]
namespace PhuLongCRM.Droid.Services
{
    public class PDFSaveAndOpen : IPDFSaveAndOpen
    {
        public async Task SaveAndView(string fileName, byte[] data, string location = "Download/PDFFiles")
        {
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/" + location);
            dir.Mkdirs();

            Java.IO.File file = new Java.IO.File(dir, fileName);
            if (file.Exists()) file.Delete();

            
            try
            {
                //var filePath = Path.Combine(dir.Path, fileName);
                //using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                //{
                //    int length = data.Length;
                //    fs.Write(data, 0, length);
                //}
                MemoryStream stream = new MemoryStream(data);
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(stream.ToArray());
                outs.Flush();
                outs.Close();
            }
            catch (Exception e)
            {
                var a = e.ToString();
            }

            string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
            string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            Android.Net.Uri path = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
            intent.SetDataAndType(path, mimeType);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            Android.App.Application.Context.StartActivity(intent);
        }
    }
}