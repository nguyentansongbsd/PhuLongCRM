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
    public partial class CenterPopup : StackLayout
    {
        public View Content { get; set; }
        public CenterPopup()
        {
            InitializeComponent();
        }
    }
}