using PhuLongCRM.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class OptionSetFilter : OptionSet
    {
        public string CustomerCode { get; set; }
        public string SDT { get; set; }
        public string SDT_format
        {
            get
            {
                if (SDT != null && SDT.Contains("-"))
                {
                    return SDT.Split('-')[1].StartsWith("84") ? SDT.Replace("84", "+84-") : SDT;
                }
                else if (SDT != null && SDT.Contains("+84"))
                {
                    return SDT.Replace("+84", "+84-");
                }
                else if (SDT != null && SDT.StartsWith("84"))
                {
                    return "+84-" + SDT.Substring(2); /* SDT.Replace("84", "+84-");*/
                }
                else
                {
                    return SDT;
                }
            }
        }
        public string CMND { get; set; }
        public string CCCD { get; set; }
        public string HC { get; set; }
        public string SoGPKD { get; set; }
        public string SoID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CCCD))
                    return CCCD;
                else if (!string.IsNullOrWhiteSpace(CMND))
                    return CMND;
                else if (!string.IsNullOrWhiteSpace(HC))
                    return HC;
                else if (!string.IsNullOrWhiteSpace(SoGPKD))
                    return SoGPKD;
                else
                    return null;
            }
        }
        public string TitleID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CCCD))
                    return Language.so_the_can_cuoc;
                else if (!string.IsNullOrWhiteSpace(CMND))
                    return Language.so_cmnd;
                else if (!string.IsNullOrWhiteSpace(HC))
                    return Language.so_ho_chieu;
                else if (!string.IsNullOrWhiteSpace(SoGPKD))
                    return Language.so_gpkd;
                else
                    return Language.so_the_can_cuoc;
            }
        }
    }
}
