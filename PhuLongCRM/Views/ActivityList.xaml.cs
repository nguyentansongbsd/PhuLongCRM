using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityList : ContentPage
    {
        public static bool? NeedToRefreshPhoneCall = null;
        public static bool? NeedToRefreshMeet = null;
        public static bool? NeedToRefreshTask = null;
        public Action<bool> OnCompleted;
        public ActivityListViewModel viewModel;
        public ActivityList()
        {
            InitializeComponent();
            BindingContext = viewModel = new ActivityListViewModel();
            NeedToRefreshPhoneCall = false;
            NeedToRefreshMeet = false;
            NeedToRefreshTask = false;
            Init();
        }

        public async void Init()
        {
            viewModel.EntityName = "tasks";
            viewModel.entity = "task";
            VisualStateManager.GoToState(radBorderTask, "Active");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "Active");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            await viewModel.LoadData();
            if (viewModel.Data.Count > 0)
            {
                OnCompleted?.Invoke(true);
            }
            else
            {
                OnCompleted?.Invoke(false);
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (viewModel.Data != null && NeedToRefreshPhoneCall == true && viewModel.entity == "phonecall")
            {
                LoadingHelper.Show();
                viewModel.EntityName = "phonecalls";
                viewModel.entity = "phonecall";
                await viewModel.LoadOnRefreshCommandAsync();
                ActivityPopup.Refresh();
                NeedToRefreshPhoneCall = false;
                LoadingHelper.Hide();
            }
            if (viewModel.Data != null && NeedToRefreshMeet == true && viewModel.entity == "appointment")
            {
                LoadingHelper.Show();
                viewModel.EntityName = "appointments";
                viewModel.entity = "appointment";
                await viewModel.LoadOnRefreshCommandAsync();
                ActivityPopup.Refresh();
                NeedToRefreshMeet = false;
                LoadingHelper.Hide();
            }

            if (viewModel.Data != null && NeedToRefreshTask == true && viewModel.entity == "task")
            {
                LoadingHelper.Show();
                viewModel.EntityName = "tasks";
                viewModel.entity = "task";
                await viewModel.LoadOnRefreshCommandAsync();
                ActivityPopup.Refresh();
                NeedToRefreshTask = false;
                LoadingHelper.Hide();
            }
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var item = e.Item as HoatDongListModel;
                ActivityPopup.ShowActivityPopup(item.activityid,item.activitytypecode);
            }
        }

        private async void SearchBar_SearchButtonPressed(System.Object sender, System.EventArgs e)
        {
            LoadingHelper.Show();
            await viewModel.LoadOnRefreshCommandAsync();
            LoadingHelper.Hide();
        }

        private void SearchBar_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.Keyword))
            {
                SearchBar_SearchButtonPressed(null, EventArgs.Empty);
            }
        }

        private async void NewActivity_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string[] options = new string[] { "Tạo Công Việc", "Tạo Cuộc Họp", "Tạo Cuộc Gọi" };
            string asw = await DisplayActionSheet("Tuỳ chọn", "Hủy", null, options);
            if (asw == "Tạo Công Việc")
            {
                await Navigation.PushAsync(new TaskForm());
            }
            else if (asw == "Tạo Cuộc Gọi")
            {
                await Navigation.PushAsync(new PhoneCallForm());
            }
            else if (asw == "Tạo Cuộc Họp")
            {
                await Navigation.PushAsync(new MeetingForm());
            }
            LoadingHelper.Hide();
        }

        private async void Task_Tapped(object sender,EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "Active");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "Active");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            if (viewModel.entity != "task")
            {
                viewModel.EntityName = "tasks";
                viewModel.entity = "task";
                await viewModel.LoadOnRefreshCommandAsync();
               // listView.ItemsSource = viewModel.Data;
            }
            LoadingHelper.Hide();
        }

        private async void Meeting_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "InActive");
            VisualStateManager.GoToState(radBorderMeeting, "Active");
            VisualStateManager.GoToState(radBorderPhoneCall, "InActive");
            VisualStateManager.GoToState(lblTask, "InActive");
            VisualStateManager.GoToState(lblMeeting, "Active");
            VisualStateManager.GoToState(lblPhoneCall, "InActive");

            if (viewModel.entity != "appointment")
            {
                viewModel.EntityName = "appointments";
                viewModel.entity = "appointment";
                await viewModel.LoadOnRefreshCommandAsync();

                if(viewModel.Data != null && viewModel.Data.Count > 0)
                {
                    List<HoatDongListModel> list = new List<HoatDongListModel>();
                    foreach (var item in viewModel.Data)
                    {
                        var meet = list.FirstOrDefault(x => x.activityid == item.activityid);
                        if (meet != null)
                        {
                            if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                            {
                                string new_customer = ", " + item.callto_contact_name;
                                meet.customer += new_customer;
                            }
                            if (!string.IsNullOrWhiteSpace(item.callto_account_name))
                            {
                                string new_customer = ", " + item.callto_account_name;
                                meet.customer += new_customer;
                            }
                            if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                            {
                                string new_customer = ", " + item.callto_lead_name;
                                meet.customer += new_customer;
                            }
                        }
                        else
                        {
                            item.scheduledstart = item.scheduledstart.ToLocalTime();
                            item.scheduledend = item.scheduledend.ToLocalTime();
                            if (!string.IsNullOrWhiteSpace(item.callto_contact_name))
                            {
                                item.customer = item.callto_contact_name;
                            }
                            if (!string.IsNullOrWhiteSpace(item.callto_account_name))
                            {
                                item.customer = item.callto_account_name;
                            }
                            if (!string.IsNullOrWhiteSpace(item.callto_lead_name))
                            {
                                item.customer = item.callto_lead_name;
                            }
                            list.Add(item);
                        }
                    }
                    viewModel.Data.Clear();
                    foreach (var item in list)
                    {
                        viewModel.Data.Add(item);
                    }
                    //= new Xamarin.Forms.Extended.InfiniteScrollCollection<HoatDongListModel>(list);
                }
            }
            
            LoadingHelper.Hide();
        }

        private async void PhoneCall_Tapped(object sender,EventArgs e)
        {
            LoadingHelper.Show();
            VisualStateManager.GoToState(radBorderTask, "InActive");
            VisualStateManager.GoToState(radBorderMeeting, "InActive");
            VisualStateManager.GoToState(radBorderPhoneCall, "Active");
            VisualStateManager.GoToState(lblTask, "InActive");
            VisualStateManager.GoToState(lblMeeting, "InActive");
            VisualStateManager.GoToState(lblPhoneCall, "Active");

            if (viewModel.entity != "phonecall")
            {
                viewModel.EntityName = "phonecalls";
                viewModel.entity = "phonecall";
                await viewModel.LoadOnRefreshCommandAsync();
            }

            LoadingHelper.Hide();
        }       
    }
}