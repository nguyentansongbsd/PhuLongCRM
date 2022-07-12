using Foundation;
using PhuLongCRM.iOS.Services;
using PhuLongCRM.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ClearCookies))]
namespace PhuLongCRM.iOS.Services
{
    public class ClearCookies : IClearCookies
    {
        public void ClearAllCookies()
        {
            NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
            foreach (var cookie in CookieStorage.Cookies)
                CookieStorage.DeleteCookie(cookie);
        }
    }
}