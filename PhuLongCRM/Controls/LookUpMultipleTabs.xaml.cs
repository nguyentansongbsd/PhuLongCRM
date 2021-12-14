using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using PhuLongCRM.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LookUpMultipleTabs : Grid
    {
        public event EventHandler<LookUpChangeEvent> SelectedItemChange;
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(LookUpMultipleTabs), null, BindingMode.TwoWay);
        public object SelectedItem { get => (object)GetValue(SelectedItemProperty); set { SetValue(SelectedItemProperty, value); } }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpMultipleTabs), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
        public bool ShowLead { get; set; } = false;
        public bool LoadNewLead { get; set; } = false;
        public bool ShowContact { get; set; } = false;
        public bool ShowAccount { get; set; } = false;

        private RadBorder TabsLead;

        private RadBorder TabsContact;

        private RadBorder TabsAccount;

        private ListViewMultiTabs ListLead;

        private ListViewMultiTabs ListContact;

        private ListViewMultiTabs ListAccount;

        public CenterModal CenterModal { get; set; }

        private Grid gridMain;

        private Grid gridTabs;
        public bool PreOpenOneTime { get; set; } = true;
        private int numberTab { get; set; } = 0;

        public static string CodeAccount = "3";

        public static string CodeContac = "2";

        public static string CodeLead = "1";
        public LookUpMultipleTabs()
        {
            InitializeComponent();
            this.Entry.BindingContext = this;
            this.Entry.SetBinding(EntryNoneBorder.PlaceholderProperty, "Placeholder");
            this.Entry.SetBinding(EntryNoneBorder.TextProperty, "SelectedItem.Label");
            this.BtnClear.SetBinding(Button.IsVisibleProperty, new Binding("SelectedItem") { Source = this, Converter = new Converters.NullToHideConverter() });
        }

        public void Clear_Clicked(object sender, EventArgs e)
        {
            this.SelectedItem = null;
            SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
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
            if (this.CenterModal == null) return;

            if (ShowLead == false && ShowContact == false && ShowAccount == false) return;

            if (gridTabs == null && gridMain == null)
            {
                SetUpModal();
            }    

            CenterModal.Title = Placeholder;
            CenterModal.Body = gridMain;
            await CenterModal.Show();
        }    

        private void SetUpModal()
        {
            gridTabs = new Grid();
            gridMain = new Grid();
            gridMain.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            gridMain.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            if (ShowLead == true)
            {
                TabsLead = CreateTabs("KH Tiềm Năng");
                gridTabs.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                gridTabs.Children.Add(TabsLead);
                Grid.SetColumn(TabsLead, numberTab);
                Grid.SetRow(TabsLead, 0);
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Lead_Tapped;
                TabsLead.GestureRecognizers.Add(tap);

                if (numberTab == 0)
                {
                    SetUpLead();
                    gridMain.Children.Add(ListLead);
                    Grid.SetColumn(ListLead, 0);
                    Grid.SetRow(ListLead, 1);

                    ListLead.IsVisible = true;
                    var lb = TabsLead.Content as Label;
                    VisualStateManager.GoToState(TabsLead, "Selected");
                    VisualStateManager.GoToState(lb, "Selected");
                }
                numberTab++;

            }
            if (ShowContact == true)
            {
                TabsContact = CreateTabs("KH Cá Nhân");
                gridTabs.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                gridTabs.Children.Add(TabsContact);
                Grid.SetColumn(TabsContact, numberTab);
                Grid.SetRow(TabsContact, 0);
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Contact_Tapped;
                TabsContact.GestureRecognizers.Add(tap);

                if (numberTab == 0)
                {
                    SetUpContact();
                    gridMain.Children.Add(ListContact);
                    Grid.SetColumn(ListContact, 0);
                    Grid.SetRow(ListContact, 1);

                    ListContact.IsVisible = true;
                    var lb = TabsContact.Content as Label;
                    VisualStateManager.GoToState(TabsContact, "Selected");
                    VisualStateManager.GoToState(lb, "Selected");
                }
                numberTab++;
            }
            if (ShowAccount == true)
            {
                TabsAccount = CreateTabs("KH Doanh Nghiệp");
                gridTabs.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                gridTabs.Children.Add(TabsAccount);
                Grid.SetColumn(TabsAccount, numberTab);
                Grid.SetRow(TabsAccount, 0);
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Account_Tapped;
                TabsAccount.GestureRecognizers.Add(tap);

                if (numberTab == 0)
                {
                    SetUpAccount();
                    gridMain.Children.Add(ListAccount);
                    Grid.SetColumn(ListAccount, 0);
                    Grid.SetRow(ListAccount, 1);

                    ListAccount.IsVisible = true;
                    var lb = TabsAccount.Content as Label;
                    VisualStateManager.GoToState(TabsAccount, "Selected");
                    VisualStateManager.GoToState(lb, "Selected");
                }
                numberTab++;
            }

            BoxView boxView = new BoxView();
            boxView.HeightRequest = 1;
            boxView.BackgroundColor = Color.FromHex("F1F1F1");
            boxView.VerticalOptions = LayoutOptions.EndAndExpand;
            gridTabs.Children.Add(boxView);
            Grid.SetColumn(boxView, 0);
            Grid.SetRow(boxView, 0);
            Grid.SetColumnSpan(boxView, numberTab);
            if(numberTab > 1)
            {
                gridMain.Children.Add(gridTabs);
                Grid.SetColumn(gridTabs, 0);
                Grid.SetRow(gridTabs, 0);
            }             
        }

        private void Account_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(TabsAccount);
            if(ListAccount == null)
            {
                SetUpAccount();
                gridMain.Children.Add(ListAccount);
                Grid.SetColumn(ListAccount, 0);
                Grid.SetRow(ListAccount, 1);
            }    

            ListAccount.IsVisible = true;
            if (ListLead != null)
                ListLead.IsVisible = false;
            if(ListContact != null)
                ListContact.IsVisible = false;
        }

        private void Contact_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(TabsContact);
            if(ListContact == null)
            {
                SetUpContact();
                gridMain.Children.Add(ListContact);
                Grid.SetColumn(ListContact, 0);
                Grid.SetRow(ListContact, 1);
            }    

            ListContact.IsVisible = true;
            if (ListLead != null)
                ListLead.IsVisible = false;
            if (ListAccount != null)
                ListAccount.IsVisible = false;
        }

        private void Lead_Tapped(object sender, EventArgs e)
        {
            Tab_Tapped(TabsLead);
            if(ListLead == null)
            {
                SetUpLead();
                gridMain.Children.Add(ListLead);
                Grid.SetColumn(ListLead, 0);
                Grid.SetRow(ListLead, 1);
            }    

            ListLead.IsVisible = true;
            if (ListContact != null)
                ListContact.IsVisible = false;
            if (ListAccount != null)
                ListAccount.IsVisible = false;
        }
        public RadBorder CreateTabs(string NameTab)

        {
            RadBorder rd = new RadBorder();
            rd.Style = (Style)Application.Current.Resources["rabBorder_Tab"];
            Label lb = new Label();
            lb.Style = (Style)Application.Current.Resources["Lb_Tab"];
            lb.Text = NameTab;
            lb.LineBreakMode = LineBreakMode.TailTruncation;
            rd.Content = lb;
            return rd;
        }
      
        private void Tab_Tapped(RadBorder radBorder)
        {
            if (radBorder != null && gridTabs != null)
            {
                for (int i = 0; i < gridTabs.Children.Count-1; i++)
                {
                    if(gridTabs.Children[i] == radBorder)
                    {
                        var rd = gridTabs.Children[i] as RadBorder;
                        var lb = rd.Content as Label;
                        VisualStateManager.GoToState(rd, "Selected");
                        VisualStateManager.GoToState(lb, "Selected");
                    }
                    else
                    {
                        var rd = gridTabs.Children[i] as RadBorder;
                        var lb = rd.Content as Label;
                        VisualStateManager.GoToState(rd, "Normal");
                        VisualStateManager.GoToState(lb, "Normal");
                    }
                }
            }
        }
        private void SetUpContact()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='contactid' alias='Val' />
                    <attribute name='fullname' alias='Label' />
                    <order attribute='fullname' descending='false' />                   
                    <filter type='and'>
                        <condition attribute='bsd_employee' operator='eq' uitype='bsd_employee' value='" + UserLogged.Id + @"' />
                    </filter>
                  </entity>
                </fetch>";
            string entity = "contacts";

            ListContact = new ListViewMultiTabs(fetch,entity);
            ListContact.ItemTapped = async (item) =>
            {
                if (item != null)
                {
                    item.Title = CodeContac;
                    SelectedItem = item;
                    SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
                    await CenterModal.Hide();
                }
            };
        }

        private void SetUpLead()
        {
            string loadNewLead = null;
            if(LoadNewLead)
            {
                loadNewLead = @" <condition attribute='statecode' operator='in'>
                                    <value>0</value>
                                  </condition>";
            }

            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='lead'>
                                <attribute name='lastname' alias='Label' />
                                <attribute name='leadid' alias='Val' />
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                    "+ loadNewLead + @"
                                    <condition attribute='bsd_employee' operator='eq' uitype='bsd_employee' value='" + UserLogged.Id + @"' />
                                </filter>
                              </entity>
                            </fetch>";

            string entity = "leads";

            ListLead = new ListViewMultiTabs(fetch, entity);
            ListLead.ItemTapped = async (item) =>
            {
                if (item != null)
                {
                    item.Title = CodeLead;
                    SelectedItem = item;
                    SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
                    await CenterModal.Hide();
                }
            };
        }

        private void SetUpAccount()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='account'>
                                <attribute name='name' alias='Label'/>
                                <attribute name='accountid' alias='Val'/>
                                <order attribute='createdon' descending='true' />
                                <filter type='and'>
                                    <condition attribute='bsd_employee' operator='eq' uitype='bsd_employee' value='" + UserLogged.Id + @"' />
                                </filter>
                              </entity>
                            </fetch>";
   
            string entity = "accounts";

            ListAccount = new ListViewMultiTabs(fetch, entity);
            ListAccount.ItemTapped = async (item) =>
            {
                if (item != null)
                {
                    item.Title = CodeAccount;
                    SelectedItem = item;
                    SelectedItemChange?.Invoke(this, new LookUpChangeEvent());
                    await CenterModal.Hide();
                }
            };
        }
    }
}