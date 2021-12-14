using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class QuoteStatusCodeData
    {
        public static List<StatusCodeModel> QuoteStatusData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000007","Báo giá","#FF8F4F"),
                new StatusCodeModel("4","Thành công","#8bce3d"),
                new StatusCodeModel("100000001","Đã thanh lý","#F43927"), // đã thanh lý
                new StatusCodeModel("6","Đã hủy","#808080"), // đã hủy
                new StatusCodeModel("100000000","Đặt cọc","#ffc43d"), // ~đặt cọc
                new StatusCodeModel("100000009","Hết hạn","#B3B3B3"), // ~ Het han 
            };
        }

        public static StatusCodeModel GetQuoteStatusCodeById(string id)
        {
            return QuoteStatusData().SingleOrDefault(x => x.Id == id);
        }
    }
}
