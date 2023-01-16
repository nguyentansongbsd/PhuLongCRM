using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabNewsControl : Grid
    {
        public static readonly BindableProperty ListTabProperty = BindableProperty.Create(nameof(ListTab), typeof(string), typeof(TabNewsControl), null, propertyChanged: ListTabChanged);
        private static void ListTabChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == null) return;
            TabNewsControl control = (TabNewsControl)bindable;
            control.SetUpTab();
        }
        public string ListTab
        {
            get { return (string)GetValue(ListTabProperty); }
            set { SetValue(ListTabProperty, value); }
        }

        public event EventHandler<LookUpChangeEvent> IndexTab;

        //ngôn ngữ
        private static System.Globalization.CultureInfo resourceCulture;

        ResourceManager resourceManager = new ResourceManager("PhuLongCRM.Resources.Language", typeof(Resources.Language).Assembly);

        private List<string> ListTabName {get;set;}

        public TabNewsControl()
        {
            InitializeComponent();
        }
        private void SetUpTab()
        {
            if (!string.IsNullOrWhiteSpace(ListTab))
            {
                var list = ListTab.Split(',').ToList();
                if(list != null && list.Count > 0)
                {
                    ListTabName = new List<string>();
                    foreach(var item in list)
                    {
                        ListTabName.Add(GetStringByKey(item));
                    }    
                }    
                if (ListTabName != null && ListTabName.Count > 0)
                {
                    for (int i = 0; i < ListTabName.Count; i++)
                    {
                        if(i!=0)
                        {
                            this.ColumnDefinitions.Add(new ColumnDefinition() { Width = 2 });
                            BoxView boxView = new BoxView();
                            boxView.WidthRequest = 2;
                            boxView.HeightRequest = 20;
                            boxView.BackgroundColor = Color.FromHex("#0F646B");
                            boxView.HorizontalOptions = LayoutOptions.Center;
                            boxView.VerticalOptions = LayoutOptions.Center;
                            Grid.SetColumn(boxView, this.Children.Count);
                            Grid.SetRow(boxView, 0);
                            this.Children.Add(boxView);
                        }    

                        var tab = CreateTabs(ListTabName[i],i.ToString());
                        this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        if (i == 0)
                        {
                            Grid.SetColumn(tab, 0);
                            Grid.SetRow(tab, 0);
                        }
                        else
                        {
                            Grid.SetColumn(tab, this.Children.Count);
                            Grid.SetRow(tab, 0);
                        }
                        this.Children.Add(tab);
                        TapGestureRecognizer tapped = new TapGestureRecognizer();
                        tapped.Tapped += Tapped_Tapped;
                        tab.GestureRecognizers.Add(tapped);
                    }
                    Tab_Tapped(this.Children[0] as Label);
                }
            }
        }
        private void Tapped_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(sender as Label);
        }

        public Label CreateTabs(string NameTab, string ClassId)
        {
            var format = new FormattedString();
            format.Spans.Add(new Span { Text = NameTab });
            Label label = new Label { ClassId = ClassId, FontSize = 14, TextColor = Color.FromHex("#0F646B"), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FormattedText = format, LineBreakMode = LineBreakMode.TailTruncation };
            return label;
        }

        private void Tab_Tapped(Label label)
        {
            if (label != null && ListTabName != null)
            {
                for (int i = 0; i < this.Children.Count; i++) //-1 do có boxview ở cuối
                {
                    var children = this.Children[i] as Label;
                    if (children != null)
                    {
                        if (children.ClassId == label.ClassId)
                        {
                            var format = new FormattedString();
                            format.Spans.Add(new Span { Text = "\uf058 ", FontFamily = "FontAwesomeRegular" });
                            format.Spans.Add(new Span { Text = ListTabName[int.Parse(children.ClassId)] });
                            children.FormattedText = format;
                            children.FontAttributes = FontAttributes.Bold;
                            EventHandler<LookUpChangeEvent> eventHandler = IndexTab;
                            eventHandler?.Invoke((object)this, new LookUpChangeEvent { Item = i });
                        }
                        else
                        {
                            var format = new FormattedString();
                            format.Spans.Add(new Span { Text = ListTabName[int.Parse(children.ClassId)] });
                            children.FormattedText = format;
                            children.FontAttributes = FontAttributes.None;
                        }
                    }
                }
            }
        }
        private string GetStringByKey(string key)
        {
            var value = resourceManager.GetString(key, new CultureInfo(UserLogged.Language));
            if (!string.IsNullOrWhiteSpace(value))
                return value;
            else
                return key;
        }
    }
}