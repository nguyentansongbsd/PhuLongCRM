﻿using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.Converters
{
    public class Base64ToImageSourceConverter : IValueConverter
    {
        ImageSource image;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()) && value is string)
            {
                image = null;
                if (value.ToString().StartsWith("https://"))
                {
                    return image = value.ToString();
                }
                else
                {
                    if (IsBase64String(value.ToString()))
                    {
                        byte[] bytes = System.Convert.FromBase64String(value.ToString());
                        image = ImageSource.FromStream(() => new MemoryStream(bytes));
                        return image;
                    }
                    else
                    {
                        return $"https://ui-avatars.com/api/?background=2196F3&rounded=false&color=ffffff&size=150&length=2&name={value.ToString()}";
                    }
                }
            }
            else
            {
                string name = string.IsNullOrWhiteSpace(UserLogged.ContactName) ? UserLogged.User : UserLogged.ContactName;
                return $"https://ui-avatars.com/api/?background=2196F3&rounded=false&color=ffffff&size=150&length=2&name={name}";
            }
            
        }

        public bool IsBase64String(string base64)
        {
            base64 = base64.Trim();
            return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
