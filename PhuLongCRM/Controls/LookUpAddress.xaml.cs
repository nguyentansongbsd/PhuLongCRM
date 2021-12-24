using PhuLongCRM.Models;
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
    public partial class LookUpAddress : Grid
    {
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpAddress), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(AddressModel), typeof(LookUpAddress), null, BindingMode.TwoWay);

        public AddressModel SelectedItem { get => (AddressModel)GetValue(SelectedItemProperty); set { SetValue(SelectedItemProperty, value); } }
        public BottomModal BottomModal { get; set; }
        public Grid CenterModal { get; set; }
        private LookUpViewAddress LookUpViewAddress { get; set; }
        public LookUpAddress()
        {
            InitializeComponent();
            this.Entry.BindingContext = this;
            this.Entry.SetBinding(EntryNoneBorder.PlaceholderProperty, "Placeholder");
            this.Entry.SetBinding(EntryNoneBorder.TextProperty, "SelectedItem.address");
        }

        public void Clear_Clicked(object sender, EventArgs e)
        {
            this.SelectedItem = null;
        }
        public void HideClearButton()
        {
            BtnClear.IsVisible = false;
        }
        public async void OpenLookUp_Tapped(object sender, EventArgs e)
        {
            await OpenModal();
        }

        public async Task OpenModal()
        {
            if (LookUpViewAddress == null)
            {
                //LookUpViewAddress.BottomModal = BottomModal;
                LookUpViewAddress = new LookUpViewAddress();
                LookUpViewAddress.SetBinding(LookUpViewAddress.PlaceholderProperty, "Placeholder");
                LookUpViewAddress.SetBinding(LookUpViewAddress.SelectedItemProperty, "SelectedItem");
                LookUpViewAddress.BottomModal = BottomModal;
            }
            if (CenterModal != null)
                CenterModal.Children.Add(LookUpViewAddress);
            CenterModal.IsVisible = true;
            await LookUpViewAddress.ShowAddress();
        }
    }
}