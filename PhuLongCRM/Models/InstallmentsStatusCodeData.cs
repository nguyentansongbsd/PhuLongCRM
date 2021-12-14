using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class InstallmentsStatusCodeData
    {
        public static List<StatusCodeModel> InstallmentsStatusData()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("1","Nháp","#06CF79"), // nháp
                new StatusCodeModel("100000000","Chưa thanh toán","#03ACF5"),  // chưa thnah toán
                new StatusCodeModel("100000001","Đã thanh toán","#FDC206"),  // đã thah toán
                new StatusCodeModel("2","Inactive","#FA7901"), // vô hiệu lực
                new StatusCodeModel("0","","#f1f1f1")
            };
        }

        public static StatusCodeModel GetInstallmentsStatusCodeById(string id)
        {
            return InstallmentsStatusData().SingleOrDefault(x => x.Id == id);
        }
    }
}
