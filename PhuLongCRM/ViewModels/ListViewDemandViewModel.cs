﻿using PhuLongCRM.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ListViewDemandViewModel : ListViewBaseViewModel2<Models.OptionSet>
    {
        public List<Models.OptionSet> ItemSelecteds { get; set; } = new List<Models.OptionSet>();
        public string Key { get; set; }
        public ListViewDemandViewModel(string fetch, string entity)
        {
            PreLoadData = new Command(() =>
            {
                EntityName = entity;
                FetchXml = fetch.Replace("fetch version='1.0'", $"fetch version='1.0' count='15' page='{Page}'").Replace("%25key%25", $"%25{Key}%25"); ;
            });
        }
        public override async Task<List<Models.OptionSet>> LoadItems()
        {
            PreLoadData.Execute(null);
            var items = new List<Models.OptionSet>();

            var result = await CrmHelper.RetrieveMultiple<Models.RetrieveMultipleApiResponse<Models.OptionSet>>(EntityName, FetchXml);

            if (result != null)
            {
                var list = (List<Models.OptionSet>)result.value;

                if (ItemSelecteds != null && ItemSelecteds.Count > 0)
                {
                    var selectedSource = list.Where(x => ItemSelecteds.Any(s => s.Val == x.Val)).ToList();
                    foreach (var item in selectedSource)
                    {
                        item.Selected = true;
                    }
                }

                var count = list.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var item = list[i];
                        if (OnMapItem != null)
                        {
                            OnMapItem.Execute(item);
                        }
                        items.Add(item);
                    }

                    if (RecordPerPage.HasValue)
                    {
                        if (count < RecordPerPage)
                        {
                            OutOfData = true;
                        }
                    }
                }
                else
                {
                    OutOfData = true;
                }
            }
            else
            {
                //OutOfData = false;
                //Data.Clear();
                //_page = 1;

                //if (UseConnectFailtNotificationDefault == true)
                //{
                //    ConnectFail = async () => Helpers.ToastMessageHelper.ShortMessage("mat ket noi");
                //    ConnectFail();
                //}
                //else
                //{
                //    ConnectFail?.Invoke();
                //}
            }
            return items;
        }

        public override async Task LoadOnRefreshCommandAsync()
        {
            Page = 1;
            OutOfData = false;
            await LoadData();
            if (Data != null)
            {
                if (ItemSelecteds != null && ItemSelecteds.Count > 0)
                {
                    var selectedSource = Data.Where(x => ItemSelecteds.Any(s => s.Val == x.Val)).ToList();
                    foreach (var item in selectedSource)
                    {
                        item.Selected = true;
                    }
                }
            }
        }
    }
}
