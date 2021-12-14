using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.Controls
{
    public partial class DatePickerBoderControl : Grid
    {
        public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime?), typeof(DatePickerControl), null, BindingMode.TwoWay,propertyChanged: HadValueChanged);
        public DateTime? Date { get { return (DateTime?)GetValue(DateProperty); } set { SetValue(DateProperty, value); } }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DatePickerControl), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty ShowEntryProperty = BindableProperty.Create(nameof(ShowEntry), typeof(bool), typeof(DatePickerControl), true, BindingMode.TwoWay);
        public bool ShowEntry { get => (bool)GetValue(ShowEntryProperty); set => SetValue(ShowEntryProperty, value); }

        public static readonly BindableProperty FormatDateProperty = BindableProperty.Create(nameof(FormatDate), typeof(string), typeof(DatePickerControl), "dd/MM/yyyy", BindingMode.TwoWay);
        public string FormatDate { get => (string)GetValue(FormatDateProperty); set => SetValue(FormatDateProperty, value); }

        public DatePickerBoderControl()
        {
            InitializeComponent();
            datePicker.SetBinding(DatePickerBorder.DateProperty, new Binding("Date") { Source = this });
            datePicker.SetBinding(DatePickerBorder.FormatProperty, new Binding("FormatDate") { Source = this });
            entry.SetBinding(MainEntry.PlaceholderProperty, new Binding("Placeholder") { Source = this });
            entry.SetBinding(MainEntry.IsVisibleProperty, new Binding("ShowEntry") { Source = this });
            ShowEntry = Date.HasValue ? false : true;
        }
        private static void HadValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null) return;
            DatePickerBoderControl control = (DatePickerBoderControl)bindable;
            if (oldValue == null && newValue != null && control.ShowEntry == true)
            {
                control.ShowEntry = false;
            }
        }
    }
}
