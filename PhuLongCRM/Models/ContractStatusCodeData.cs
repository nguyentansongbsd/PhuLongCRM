using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ContractStatusCodeData
    {
        public static List<StatusCodeModel> ContractStatusData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000001","TT Đủ Đợt 1","#F43927"), // thanh toán đợt 1
                new StatusCodeModel("100000003","Đang thanh tooán","#F43927"),
                new StatusCodeModel("4","Đã hủy","#8bce3d"), // đã hủy
                new StatusCodeModel("100001","Đã hoàn thành","#FF8F4F"), // hoàn thành
                new StatusCodeModel("100000004","TT Hoàn tất","#FF8F4F"), 
                new StatusCodeModel("100000007","Converted","#FF8F4F"),
                new StatusCodeModel("100000005","Bàn giao","#FF8F4F"),
                new StatusCodeModel("3","Đang xử lý","#FF8F4F"),
                new StatusCodeModel("100003","Invoiced","#FF8F4F"),
                new StatusCodeModel("1","Open","#FF8F4F"),
                new StatusCodeModel("100000000","Hoàn tất đặt cọc","#ffc43d"), 
                new StatusCodeModel("100002","Partial","#FF8F4F"),
                new StatusCodeModel("2","Pending","#FF8F4F"),
                new StatusCodeModel("100000002","Đã ký HĐMB","#FF8F4F"), // đã ký hợp đồng
                new StatusCodeModel("100000006","Đã thanh lý","#c4c4c4"), // đã thanh lý
                new StatusCodeModel("0","","#bfbfbf"),
            };
        }

        public static StatusCodeModel GetContractStatusCodeById(string id)
        {
            return ContractStatusData().SingleOrDefault(x => x.Id == id);
        }
    }
}
