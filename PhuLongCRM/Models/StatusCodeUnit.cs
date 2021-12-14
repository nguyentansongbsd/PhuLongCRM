using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class StatusCodeUnit
    {
        public static StatusCodeModel GetStatusCodeById(string statusCodeId)
        {
            return StatusCodes().SingleOrDefault(x => x.Id == statusCodeId);
        }

        public static List<StatusCodeModel> StatusCodes()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("0","Nháp","#333333"),
                new StatusCodeModel("1","Chuẩn bị","#F1C50D"),
                new StatusCodeModel("100000000","Sẵn sàng","#2FCC71"),
                new StatusCodeModel("100000007","Booking","#00CED1"),
                new StatusCodeModel("100000004","Giữ chỗ","#04A8F4"),
                new StatusCodeModel("100000006","Đặt cọc","#14A184"),
                new StatusCodeModel("100000005","Đồng ý chuyển cọc","#8F44AD"),
                new StatusCodeModel("100000003","Đã đủ tiền cọc","#e67e22"),
                new StatusCodeModel("100000010","Option","#808080"),
                new StatusCodeModel("100000001","Thanh toán đợt 1","#808080"),
                new StatusCodeModel("100000009","Signed D.A","#A0DB8E"),
                new StatusCodeModel("100000008","Qualified","#6897BB"),
                new StatusCodeModel("100000002","Đã bán","#BF3A2B"),
            };
        }
    }

    public class StatusCodeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Background { get; set; }
        public StatusCodeModel(string id,string name,string background)
        {
            Id = id;
            Name = name;
            Background = background;
        }
    }
}
