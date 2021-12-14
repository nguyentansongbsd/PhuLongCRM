using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ListViewMultiTabsViewModel : ListViewBaseViewModel2<Models.OptionSet>
    {
        public ListViewMultiTabsViewModel(string fetch, string entity)
        {
            PreLoadData = new Command(() =>
            {
                EntityName = entity;
                FetchXml = fetch.Replace("fetch version='1.0'", $"fetch version='1.0' count='15' page='{Page}'");
            });
        }
    }
}
