using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingControl : Grid
    {
        public event EventHandler<int> StarChanged;
        private int index;
        public RatingControl()
        {
            InitializeComponent();
        }

        private void Rating_Tapped(object sender, EventArgs e)
        {
            var i = int.Parse(((TapGestureRecognizer)((Label)sender).GestureRecognizers[0]).CommandParameter as string);
            ChangeStar(i);
            EventHandler<int> eventHandler = StarChanged;
            eventHandler?.Invoke((object)this, index);
        }
        private void ChangeStar(int starcount)
        {
            bool ratingDown = false;
            if (starcount < 5 || starcount == 5 && star5.TextColor == Color.FromHex("FEC93D"))
            {
                if (starcount < 5 && star5.TextColor == Color.FromHex("FEC93D"))
                    ratingDown = true;
                star5.FontFamily = "FontAwesomeRegular";
                star5.TextColor = Color.Gray;
            }
            else
            {
                star5.FontFamily = "FontAwesomeSolid";
                star5.TextColor = Color.FromHex("FEC93D");
            }
            if (starcount < 4 || starcount == 4 && star4.TextColor == Color.FromHex("FEC93D") && ratingDown == false || starcount > 4 && star5.TextColor == Color.Gray)
            {
                if (starcount < 4 && star4.TextColor == Color.FromHex("FEC93D"))
                    ratingDown = true;
                else
                    ratingDown = false;
                star4.FontFamily = "FontAwesomeRegular";
                star4.TextColor = Color.Gray;
            }
            else
            {
                star4.FontFamily = "FontAwesomeSolid";
                star4.TextColor = Color.FromHex("FEC93D");
            }
            if (starcount < 3 || starcount == 3 && star3.TextColor == Color.FromHex("FEC93D") && ratingDown == false || starcount > 3 && star4.TextColor == Color.Gray)
            {
                if (starcount < 3 && star3.TextColor == Color.FromHex("FEC93D"))
                    ratingDown = true;
                else
                    ratingDown = false;
                star3.FontFamily = "FontAwesomeRegular";
                star3.TextColor = Color.Gray;
            }
            else
            {
                star3.FontFamily = "FontAwesomeSolid";
                star3.TextColor = Color.FromHex("FEC93D");
            }
            if (starcount < 2 || starcount == 2 && star2.TextColor == Color.FromHex("FEC93D") && ratingDown == false || starcount > 2 && star3.TextColor == Color.Gray)
            {
                if (starcount < 2 && star2.TextColor == Color.FromHex("FEC93D"))
                    ratingDown = true;
                else
                    ratingDown = false;
                star2.FontFamily = "FontAwesomeRegular";
                star2.TextColor = Color.Gray;
            }
            else
            {
                star2.FontFamily = "FontAwesomeSolid";
                star2.TextColor = Color.FromHex("FEC93D");
            }
            if (starcount < 1 || starcount == 1 && star1.TextColor == Color.FromHex("FEC93D") && ratingDown == false || starcount > 1 && star2.TextColor == Color.Gray)
            {
                star1.FontFamily = "FontAwesomeRegular";
                star1.TextColor = Color.Gray;
            }
            else
            {
                star1.FontFamily = "FontAwesomeSolid";
                star1.TextColor = Color.FromHex("FEC93D");
            }
            if (star1.TextColor == Color.Gray)
                index = 0;
            else
                index = starcount;
        }
        public void InitStar(int starcount)
        {
            ChangeStar(starcount);
        }
    }
}