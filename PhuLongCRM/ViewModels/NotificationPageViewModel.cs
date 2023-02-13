using Firebase.Database;
using Firebase.Database.Query;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhuLongCRM.ViewModels
{
    public class NotificationPageViewModel
    {
        FirebaseClient firebase = new Firebase.Database.FirebaseClient("https://phulonguat-default-rtdb.firebaseio.com/");
        public ObservableCollection<NotificaModel> Notifications { get; set; } = new ObservableCollection<NotificaModel>();
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
    }
}
