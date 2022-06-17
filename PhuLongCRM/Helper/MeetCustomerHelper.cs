using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Extended;

namespace PhuLongCRM.Helper
{
    // chỉ sử dụng cho activity meet
    public class MeetCustomerHelper
    {
        public static InfiniteScrollCollection<HoatDongListModel> MeetCustomer(InfiniteScrollCollection<HoatDongListModel> data)
        {
            if (data == null || data.Count == 0) return null;
            InfiniteScrollCollection<HoatDongListModel> list = new InfiniteScrollCollection<HoatDongListModel>();
            foreach(var item in data)
            {
                HoatDongListModel meet = list.FirstOrDefault(x => x.activityid == item.activityid);
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
            return list;
        }
    }
}
