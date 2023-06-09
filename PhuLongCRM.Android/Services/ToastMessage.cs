using System;
using app = Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using PhuLongCRM.IServices;
using PhuLongCRM.Droid.Services;
using Java.Lang;
using Java.Util;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace PhuLongCRM.Droid.Services
{
    public class ToastMessage : IToastMessage
    {
        public void LongAlert(string message)
        {
            ShowToast(message, ToastLength.Long);
        }

        public void ShortAlert(string message)
        {
            ShowToast(message, ToastLength.Short);
        }

        public void ShowToast(string Message, ToastLength toastLength)
        {
            Context context = app.Application.Context;
            Toast toast = Toast.MakeText(context, Message, (ToastLength)ToastDuration.Long);
            var view = (context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater).Inflate(Resource.Layout.ToastLayout, null);

            TextView txtMessage = view.FindViewById<TextView>(Resource.Id.txtMessage);
            txtMessage.Text = Message;
            toast.View = view;
            toast.Show();
           
        }
    }
    public static class ToastDuration 
    {
        public const int Short = 20000;
        public const int Long = 500;
    }

}
