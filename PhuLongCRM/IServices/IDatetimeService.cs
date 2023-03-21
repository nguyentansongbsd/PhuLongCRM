using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.IServices
{
    public interface IDatetimeService
    {
        string TimeAgo(DateTime date);
    }
}
