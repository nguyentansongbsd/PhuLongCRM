using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.Content;
using Android.Webkit;
using Java.IO;
using PhuLongCRM.Droid.Services;
using PhuLongCRM.Helper;
using PhuLongCRM.IServices;
using PhuLongCRM.Resources;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenFileService))]
namespace PhuLongCRM.Droid.Services
{
    public class OpenFileService : IOpenFileService
    {
        public async Task OpenFile(string fileName, byte[] arr = null, string url = null, string folder = "Download/PhuLongFiles")
        {
            try
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/" + folder);
                dir.Mkdirs();

                Java.IO.File file = new Java.IO.File(dir, fileName);
                if (file.Exists())
                {
                    System.IO.File.Delete(file.Path);
                }

                byte[] data = arr;
                if (arr == null && !string.IsNullOrWhiteSpace(url))
                {
                    WebClient webClient = new WebClient();
                    data = await webClient.DownloadDataTaskAsync(url);
                }
                MemoryStream stream = new MemoryStream(data);
                FileOutputStream outs = new FileOutputStream(file);
                outs.Write(stream.ToArray());
                outs.Flush();
                outs.Close();

                string extension = MimeTypeMap.GetFileExtensionFromUrl(Android.Net.Uri.FromFile(file).ToString());
                string mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                Android.Net.Uri path = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                intent.SetDataAndType(path, mimeType);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
                {
                    if (!Android.OS.Environment.IsExternalStorageManager)
                    {
                        bool accept = await Xamarin.Forms.Shell.Current.DisplayAlert("", Language.ResourceManager.GetString("phulongcrm_can_quyen_quan_ly_tat_ca_cac_tep", Language.Culture), Language.ResourceManager.GetString("cai_dat", Language.Culture), Language.ResourceManager.GetString("huy", Language.Culture));

                        if (accept)
                        {
                            try
                            {
                                Android.Net.Uri uri = Android.Net.Uri.Parse("package:" + Android.App.Application.Context.ApplicationInfo.PackageName);
                                Intent intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                                intent.SetData(uri);
                                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                                Android.App.Application.Context.StartActivity(intent);
                            }
                            catch (Exception ex)
                            {
                                Intent intent = new Intent();
                                intent.SetAction(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                                Android.App.Application.Context.StartActivity(intent);
                            }
                        }
                    }
                }
            }
            LoadingHelper.Hide();
        }

        public async Task OpenFilePdfFromUrl(string fileName, string url)
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse(url), "application/pdf");
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            intent.SetAction(Intent.ActionView);
            try
            {
                Xamarin.Essentials.Platform.CurrentActivity.StartActivity(intent);
            }
            catch (Exception ex)
            {
                await AppShell.Current.Navigation.PushAsync(new PhuLongCRM.Views.ViewPDFFilePage(url) { Title = fileName });
            }
            LoadingHelper.Hide();
        }
    }
}
