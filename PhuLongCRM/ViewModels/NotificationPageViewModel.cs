﻿using Firebase.Database;
using Firebase.Database.Query;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class NotificationPageViewModel : BaseViewModel
    {
        FirebaseClient firebase = new Firebase.Database.FirebaseClient("https://phulonguat-default-rtdb.firebaseio.com/");
        public ObservableCollection<NotificaModel> Notifications { get; set; } = new ObservableCollection<NotificaModel>();

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }
        public ICommand RefreshCommand => new Command(async () =>
        {
            IsRefreshing = true;
            await RefreshDashboard();
            IsRefreshing = false;
        });
        public NotificationPageViewModel()
        {
        }
        public async Task LoadData()
        {
            var Items = (await firebase
                                  .Child("Notifications")
                                  .OnceAsync<NotificaModel>()).Select(item => new NotificaModel
                                  {
                                      Key = item.Key,
                                      Id = item.Object.Id,
                                      Title = item.Object.Title,
                                      Body = item.Object.Body,
                                      ProjectId = item.Object.ProjectId,
                                      NotificationType = item.Object.NotificationType,
                                      IsRead = item.Object.IsRead,
                                      CreatedDate = item.Object.CreatedDate,
                                      QueueId = item.Object.QueueId,
                                      QuoteId = item.Object.QuoteId,
                                      ReservationId = item.Object.ReservationId,
                                      ContractId = item.Object.ContractId
                                  }).OrderByDescending(x => x.CreatedDate);
            foreach (var item in Items)
            {
                if (item.IsRead)
                {
                    item.BackgroundColor = "#F2F2F2";
                }
                else
                {
                    item.BackgroundColor = "#ffffff";
                }
                this.Notifications.Add(item);
            }
        }
        public async Task UpdateStatus(string key, NotificaModel data)
        {
            await firebase.Child("Notifications").Child(key).PutAsync(new NotificaModel()
            {
                Id = data.Id,
                Title = data.Title,
                Body = data.Body,
                IsRead = true,
                NotificationType = data.NotificationType,
                CreatedDate = data.CreatedDate,
                ProjectId = data.ProjectId,
                QueueId = data.QueueId,
                QuoteId = data.QueueId,
                ReservationId = data.ReservationId,
                ContractId = data.ContractId
            });
        }
        public async Task DeleteNotification(string key)
        {
            await firebase.Child("Notifications").Child(key).DeleteAsync();
        }
        public async Task RefreshDashboard()
        {
            Notifications.Clear();
            await Task.WhenAll(
                LoadData()
                );
        }
    }
}
