using System;
using System.Collections.Generic;
using System.Linq;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Models
{
    public class QuoteStatusCodeData
    {
        public static List<StatusCodeModel> QuoteStatusData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("100000000",Language.dat_coc,"#ffc43d"), // Reservation
                new StatusCodeModel("100000001",Language.da_thanh_ly,"#F43927"), // Terminated
                new StatusCodeModel("100000002",Language.dang_cho_huy_bo_tien_gui,"#808080"),//Pending Cancel Deposit
                new StatusCodeModel("100000003",Language.tu_choi,"#808080"), // Reject
                new StatusCodeModel("100000004",Language.da_ky_rf,"#808080"),//Signed RF
                new StatusCodeModel("100000005",Language.da_het_han_ky_rf,"#808080"), // Expired of signing RF
                new StatusCodeModel("100000006",Language.chuyen_coc,"#808080"),//Collected
                new StatusCodeModel("100000007",Language.bao_gia,"#FF8F4F"), //Quotation
                new StatusCodeModel("100000008",Language.bao_gia_het_han,"#808080"), // Expired Quotation
                new StatusCodeModel("100000009",Language.het_han,"#B3B3B3"), // ~ Het han
                new StatusCodeModel("100000010",Language.da_ky_phieu_coc,"#808080"),//Đã ký phiếu cọc
                new StatusCodeModel("100000012",Language.nhap,"#808080"),

                new StatusCodeModel("1",Language.dang_xu_ly,"#808080"),//In Progress
                new StatusCodeModel("2",Language.dang_xu_ly,"#808080"),//In Progress
                new StatusCodeModel("3",Language.tt_du_tien_coc,"#808080"),//Deposited
                new StatusCodeModel("4",Language.thanh_cong,"#8bce3d"), // Won
                new StatusCodeModel("5",Language.mat_khach_hang,"#808080"), // Lost
                new StatusCodeModel("6",Language.da_huy,"#808080"), // Canceled
                new StatusCodeModel("7",Language.da_sua_doi,"#808080"), // Revised

                new StatusCodeModel("861450001",Language.da_trinh,"#808080"),//Submitted
                new StatusCodeModel("861450002",Language.da_duyet,"#808080"),//Approved
                new StatusCodeModel("861450000",Language.thay_doi_thong_tin,"#808080"),//Change information
            };
        }

        public static StatusCodeModel GetQuoteStatusCodeById(string id)
        {
            return QuoteStatusData().SingleOrDefault(x => x.Id == id);
        }
    }
}
