using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PhuLongCRM.Controls
{
    public partial class PhoneEntryControl : MainEntry
    {
        public static readonly BindableProperty PhoneNumProperty = BindableProperty.Create(nameof(PhoneNum), typeof(string), typeof(PhoneEntryControl), "+84-", BindingMode.TwoWay);
        public string PhoneNum { get => (string)GetValue(PhoneNumProperty); set => SetValue(PhoneNumProperty, value); }

        public PhoneEntryControl()
        {
            InitializeComponent();
            entryPhone.SetBinding(MainEntry.TextProperty, new Binding("PhoneNum") { Source = this }) ;
            if (!string.IsNullOrWhiteSpace(PhoneNum))
            {
                PhoneNum += "+84-";
            }
        }

        void entryPhone_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            
        }
    }
}
