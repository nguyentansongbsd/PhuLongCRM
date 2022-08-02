using System;
using System.Collections.Generic;
using System.Text;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PhuLongCRM.Helper
{
    public class HoverHelper
    {
        public static RadBorder radBorder;
        static double checkWidth;
        static double checkHeight;
        static double Width;
        static double Height;
        static double minWidth;
        static double minHeight;
        static double maxWidth;
        static double maxHeight;
        private static void BuildHover(string content)
        {
            if(radBorder == null)
            {
                Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) * 99 / 100;
                Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density) * 90 / 100;
                checkWidth = (Width / 3) / 2;
                checkHeight = Height / 20;
                minWidth = 0;
                minHeight = checkHeight;
                maxWidth = Width - (Width / 3);
                maxHeight = Height - minHeight;
            }    

            radBorder = new RadBorder();
            radBorder.BorderColor = Color.FromHex("#70111111");
            radBorder.BackgroundColor = Color.FromHex("#70111111");
            radBorder.BorderThickness = 1;
            radBorder.CornerRadius = 5;
            Label label = new Label();
            label.Text = content;
            label.TextColor = Color.White;
            label.FontSize = 14;
            label.VerticalOptions = LayoutOptions.Center;
            label.HorizontalOptions = LayoutOptions.Center;
            label.BackgroundColor = Color.Transparent;
            radBorder.Content = label;
        }
        public static void ShowHover(float x, float y, string contentHover, AbsoluteLayout absoluteLayout)
        {
            if (contentHover == null || string.IsNullOrWhiteSpace(contentHover))
            {
                radBorder.IsVisible = false;
                return;
            }
            if (radBorder == null)
            {
                BuildHover(contentHover);
                if (x <= checkWidth)
                    x = float.Parse(minWidth.ToString());
                else if (x >= maxWidth)
                    x = float.Parse(maxWidth.ToString());
                else
                    x = x - float.Parse(checkWidth.ToString());

                if (y <= checkHeight)
                    y = float.Parse(minHeight.ToString());
                else if (y >= (maxHeight))
                    y = float.Parse(maxHeight.ToString());
                else
                    y = y - float.Parse((checkHeight).ToString());

                AbsoluteLayout.SetLayoutBounds(radBorder, new Rectangle(x, y, 0.3, 0.05));
                AbsoluteLayout.SetLayoutFlags(radBorder, AbsoluteLayoutFlags.SizeProportional);
                absoluteLayout.Children.Add(radBorder);
            }
            else
            {
                radBorder.IsVisible = !radBorder.IsVisible;
            }    
        }
    }
}
