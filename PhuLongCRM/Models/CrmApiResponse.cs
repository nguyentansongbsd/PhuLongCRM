using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class CrmApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public ErrorResponse ErrorResponse { get; set; }

        public string GetErrorMessage()
        {
            return ErrorResponse?.error?.message?.ToString() ?? "Lỗi, Vui lòng thực hiện lại thao tác.";
        }
    }
}
