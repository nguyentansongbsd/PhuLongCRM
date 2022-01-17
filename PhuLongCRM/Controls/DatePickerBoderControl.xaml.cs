using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.Controls
{
    public partial class DatePickerBoderControl : Grid
    {
        public event EventHandler Date_Selected;

        public static readonly BindableProperty DateProperty = BindableProperty.Create(nameof(Date), typeof(DateTime?), typeof(DatePickerBoderControl), null, BindingMode.TwoWay,propertyChanged: HadValueChanged);
        public DateTime? Date { get { return (DateTime?)GetValue(DateProperty); } set { SetValue(DateProperty, value); } }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(DatePickerBoderControl), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty ShowEntryProperty = BindableProperty.Create(nameof(ShowEntry), typeof(bool), typeof(DatePickerBoderControl), true, BindingMode.TwoWay);
        public bool ShowEntry { get => (bool)GetValue(ShowEntryProperty); set => SetValue(ShowEntryProperty, value); }

        public static readonly BindableProperty FormatDateProperty = BindableProperty.Create(nameof(FormatDate), typeof(string), typeof(DatePickerBoderControl), "dd/MM/yyyy", BindingMode.TwoWay);
        public string FormatDate { get => (string)GetValue(FormatDateProperty); set => SetValue(FormatDateProperty, value); }

        public DatePickerBoderControl()
        {
            InitializeComponent();
            datePicker.SetBinding(DatePickerBorder.DateProperty, new Binding("Date") { Source = this });
            datePicker.SetBinding(DatePickerBorder.FormatProperty, new Binding("FormatDate") { Source = this });
            entry.SetBinding(MainEntry.PlaceholderProperty, new Binding("Placeholder") { Source = this });
            entry.SetBinding(MainEntry.IsVisibleProperty, new Binding("ShowEntry") { Source = this });
            ShowEntry = Date.HasValue ? false : true;
            btnClear.IsVisible = !ShowEntry;
        }
        private static void HadValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null) return;
            DatePickerBoderControl control = (DatePickerBoderControl)bindable;
            if (oldValue == null && newValue != null && control.ShowEntry == true)
            {
                control.ShowEntry = false;
                control.btnClear.IsVisible = !control.ShowEntry;
            }
        }

        private void datePicker_OnChangeState(object sender, EventArgs e)
        {
            ShowEntry = false;
            if (!Date.HasValue)
            {
                this.Date = DateTime.Now;
            }
            btnClear.IsVisible = !ShowEntry;
        }

        private void ClearDate_Tapped(object sender, EventArgs e)
        {
            this.Date = null;
            this.ShowEntry = true;
            btnClear.IsVisible = !ShowEntry;
        }

        private void datePicker_DateSelected(System.Object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            this.Date_Selected?.Invoke(sender, EventArgs.Empty);
        }
    }
}
