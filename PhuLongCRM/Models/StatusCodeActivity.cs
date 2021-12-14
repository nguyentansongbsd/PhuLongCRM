using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class StatusCodeActivity
    {
        public static StatusCodeModel GetStatusCodeById(string statusCodeId)
        {
            return StatusCodes().SingleOrDefault(x => x.Id == statusCodeId);
        }

        public static List<StatusCodeModel> StatusCodes()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("1","Hoàn Thành","#03ACF5"),
                new StatusCodeModel("0","Đang Mở","#06CF79"),
                new StatusCodeModel("2","Đã hủy","#333333"),
                new StatusCodeModel("3","Scheduled","#04A388"),
            };
        }
    }
}
