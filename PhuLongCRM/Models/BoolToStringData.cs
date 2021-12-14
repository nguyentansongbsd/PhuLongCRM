using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class BoolToStringData
    {       
        public static string GetStringByBool(bool _bool)
        {
            if(_bool)
            {
                return "Có";
            }   
            else
            {
                return "Không";
            }    
        }
    }
}
