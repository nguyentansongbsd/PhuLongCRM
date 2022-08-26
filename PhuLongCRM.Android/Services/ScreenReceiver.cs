using System;
using Android.App;
using Android.Content;

namespace PhuLongCRM.Droid.Services 
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "com.xamarin.example.TEST" })]
    public class ScreenReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(Intent.ActionUserPresent))
            {
                Intent intent1 = new Intent(context,typeof(MainActivity));
                intent1.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent1);
            }
        }
    }
}

