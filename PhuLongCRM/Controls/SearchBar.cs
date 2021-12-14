using System;
using Xamarin.Forms;

namespace PhuLongCRM.Controls
{
    public class SearchBar : Xamarin.Forms.SearchBar
    {
        public SearchBar()
        {
            Placeholder = "Tìm kiếm";
            FontSize = 15;
            TextColor = Color.FromHex("#444444");
            FontFamily = "Segoe";
            BackgroundColor = Color.White;
        }
    }
}
