using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class FollowUpType
    {
        public static List<StatusCodeModel> FollowUpTypeData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000002","Giao dịch - TT Đủ Đợt 1","#FDC206"),
                new StatusCodeModel("100000003","Giao dịch - Đã ký HĐMB","#06CF79"),
                new StatusCodeModel("100000004","Giao dịch - Installments","#03ACF5"),
                new StatusCodeModel("100000006","Giao dịch - Đã thanh lý","#04A388"),
                new StatusCodeModel("100000001","Đặt cọc - TT đủ tiền cọc","#9A40AB"),
                new StatusCodeModel("100000000","Đặt cọc - Sign off RF","#FA7901"),
                new StatusCodeModel("100000005","Đặt cọc - Đã thanh lý","#808080"),
                new StatusCodeModel("100000007","Units","#D42A16"),
                new StatusCodeModel("0","","#333333"),
            };
        }

        public static StatusCodeModel GetFollowUpTypeById(string id)
        {
            return FollowUpTypeData().SingleOrDefault(x => x.Id == id);
        }
    }
}
