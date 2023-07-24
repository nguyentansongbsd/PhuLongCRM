using PhuLongCRM.Models;
using PhuLongCRM.Resources;
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
    public partial class LookUpDemand : Grid
    {
        public event EventHandler<OptionSet> SelectedItemChange;

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpMultiSelect), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty SelectedIdsProperty = BindableProperty.Create(nameof(SelectedIds), typeof(List<OptionSet>), typeof(LookUpDemand), null, BindingMode.TwoWay, null, propertyChanged: ItemSourceChange);
        public List<OptionSet> SelectedIds { get => (List<OptionSet>)GetValue(SelectedIdsProperty); set { SetValue(SelectedIdsProperty, value); } }

        public CenterModal CenterModal { get; set; }

        private ListViewDemand ListProvince;

        private ListViewDemand ListProject;

        private Grid gridMain;
        public bool ShowProvince { get; set; } = false;

        public LookUpDemand()
        {
            InitializeComponent();
        }
        public async void OpenLookUp_Tapped(object sender, EventArgs e)
        {
            await OpenModal();
        }
        public async Task OpenModal()
        {
            if (this.CenterModal == null) return;

            if (gridMain == null)
            {
                SetUpModal();
            }

            CenterModal.Title = Placeholder;
            CenterModal.Body = gridMain;
            await CenterModal.Show();
        }
        private void SetUpModal()
        {
            gridMain = new Grid();
            // gridMain.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            gridMain.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            if (ShowProvince == true)
            {
                SetUpProvince();
                gridMain.Children.Add(ListProvince);
                Grid.SetColumn(ListProvince, 0);
                Grid.SetRow(ListProvince, 0);
            }
            else
            {
                SetUpProject();
                gridMain.Children.Add(ListProject);
                Grid.SetColumn(ListProject, 0);
                Grid.SetRow(ListProject, 0);
            }

            //gridMain.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Button btnSave = new Button();
            //btnSave.HeightRequest = 35;
            //btnSave.Padding = 5;
            //btnSave.Margin = 10;
            //btnSave.CornerRadius = 10;
            //btnSave.FontSize = 16;
            //btnSave.TextColor = Color.White;
            //btnSave.TextTransform = TextTransform.None;
            //btnSave.BackgroundColor = (Color)Application.Current.Resources["NavigationPrimary"];
            //btnSave.Text = Language.luu;//"Lưu";
            //btnSave.Clicked += SaveButton_Clicked;
            //btnSave.FontAttributes = FontAttributes.Bold;

            //gridMain.Children.Add(btnSave);
            //Grid.SetColumn(btnSave, 0);
            //Grid.SetRow(btnSave, 1);
        }
        //public async void SaveButton_Clicked(object sender, EventArgs e)
        //{
        //    if (SelectedIds != null)
        //        SetData();
        //    await CenterModal.Hide();
        //}
        private static void ItemSourceChange(BindableObject bindable, object oldValue, object value)
        {
            LookUpDemand control = (LookUpDemand)bindable;
            if (control.SelectedIds != null)
                control.SetData();
        }
        private void SetData()
        {
            if (SelectedIds != null)
            {
                List<OptionSet> item = new List<OptionSet>(SelectedIds);
                SetListItem(item);
            }
        }
        public void SetListItem(List<OptionSet> selectedInSource)
        {
            BindableLayout.SetItemsSource(stacklayout, selectedInSource);
        }
        private void SetUpProvince()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='new_province'>
                                <attribute name='new_name' alias='Label' />   
                                <attribute name='new_provinceid' alias='Val' />
                                <order attribute='bsd_priority' descending='false' />
                                <filter type='and'>   
                                    <condition attribute='statecode' operator='eq' value='0' />
                                    <condition attribute='new_name' operator='like' value='%25key%25' />
                                </filter>
                              </entity>
                            </fetch>";
            string entity = "new_provinces";

            ListProvince = new ListViewDemand(fetch, entity, SelectedIds);
            ListProvince.ItemTapped = async (item) =>
            {
                if (item != null)
                {
                    if (SelectedIds == null)
                        SelectedIds = new List<OptionSet>();

                    item.Selected = !item.Selected;
                    if (item.Selected == true)
                    {
                        SelectedIds.Add(item);
                        ListProvince.viewModel.ItemSelecteds.Add(item);
                    }
                    else
                    {
                        SelectedIds.Remove(item);
                        ListProvince.viewModel.ItemSelecteds.Remove(item);
                    }
                    SelectedItemChange?.Invoke(this, item);
                    if (SelectedIds != null)
                        SetData();
                }
            };
        }
        private void SetUpProject()
        {
            string fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='bsd_project'>
                                <attribute name='bsd_name' alias='Label' />   
                                <attribute name='bsd_projectid' alias='Val' />
                                <order attribute='bsd_name' descending='false' />
                                <filter type='and'>                       
                                    <condition attribute='statecode' operator='eq' value='0' />
                                    <condition attribute='statuscode' operator='eq' value='861450002' />
                                    <condition attribute='bsd_name' operator='like' value='%25key%25' />
                                </filter>
                              </entity>
                            </fetch>";
            string entity = "bsd_projects";

            ListProject = new ListViewDemand(fetch, entity, SelectedIds);
            ListProject.ItemTapped = async (item) =>
            {
                if (item != null)
                {
                    if (SelectedIds == null)
                        SelectedIds = new List<OptionSet>();

                    item.Selected = !item.Selected;
                    if (item.Selected == true)
                    {
                        SelectedIds.Add(item);
                        ListProject.viewModel.ItemSelecteds.Add(item);
                    }
                    else
                    {
                        SelectedIds.Remove(item);
                        ListProject.viewModel.ItemSelecteds.Remove(item);
                    }
                    SelectedItemChange?.Invoke(this, item);
                    if (SelectedIds != null)
                        SetData();
                }
            };
        }
    }
}