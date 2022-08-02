using TouchTracking;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using TouchTracking.Forms;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;

namespace PhuLongCRM.Controls
{
    public class AbsoluteHover : AbsoluteLayout
    {
        RadBorder radBorder;
        double checkWidth;
        double checkHeight;
        double Width;
        double Height;
        double minWidth;
        double minHeight;
        double maxWidth;
        double maxHeight;
        public AbsoluteHover()
        {
            TouchEffect touchEffect = new TouchEffect();
            touchEffect.TouchAction += TouchEffect_TouchAction;
            this.Effects.Add(touchEffect);
            Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) * 99 / 100;
            Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density) * 90 / 100;
            checkWidth = (Width / 3) / 2;
            checkHeight = Height / 20;
            minWidth = 0;
            minHeight = checkHeight;
            maxWidth = Width - (Width / 3);
            maxHeight = Height - minHeight;
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            if (args != null)
            {
                float x = args.Location.X;
                var y = args.Location.Y;

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
                this.Children.Add(radBorder);
            }
        }

        private void BuildHover(string content)
        {
            radBorder = new RadBorder();
            radBorder.BorderColor = Color.FromHex("#40111111");
            radBorder.BackgroundColor = Color.FromHex("#40111111");
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
        public void ShowHover(float x, float y, string contentHover, AbsoluteLayout absoluteLayout)
        {
            if (contentHover == null || string.IsNullOrWhiteSpace(contentHover))
            {
                radBorder.IsVisible = false;
                return;
            }

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
    }
}
