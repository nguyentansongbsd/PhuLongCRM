using System;
using PhuLongCRM.Resources;
using Xamarin.Forms;

namespace PhuLongCRM.Controls
{
    public class SearchBar : Xamarin.Forms.SearchBar
    {
        public SearchBar()
        {
            Placeholder = Language.tim_kiem;
            FontSize = 15;
            TextColor = Color.FromHex("#444444");
            FontFamily = "Segoe";
            BackgroundColor = Color.White;
            PropertyChanged += SearchBar_PropertyChanged;
        }

        private void SearchBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           this.Placeholder = Language.tim_kiem;
        }
    }
}
