using System;
using System.Threading.Tasks;
using PhuLongCRM.IServices;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace PhuLongCRM.Helper
{
    public class ToastMessageHelper
    {
        public static async void Message(string message)
        {
            var messageOptions = new MessageOptions
            {
                Message = message,
                Foreground = Color.Black,
                Font = Font.SystemFontOfSize(14),
                Padding = new Thickness(10)
            };

            var options = new ToastOptions
            {
                MessageOptions = messageOptions,
                CornerRadius = new Thickness(15),
                BackgroundColor = Color.FromHex("#ffffff"),
                Duration = TimeSpan.FromMilliseconds(5000),
            };

            await App.Current.MainPage.DisplayToastAsync(options);
        }
    }
}
