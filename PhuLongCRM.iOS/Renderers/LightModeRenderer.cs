using System;
using PhuLongCRM.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(LightModeRenderer))]
namespace PhuLongCRM.iOS.Renderers
{
	public class LightModeRenderer : PageRenderer
	{
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            //OverrideUserInterfaceStyle = UIKit.UIUserInterfaceStyle.Light;
        }
    }
}

