using PhuLongCRM.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ListTaiLieuKinhDoanhModel 
    {
        public Guid salesliteratureid { get; set; }
        public string name { get; set; }
        public string subjecttitle { get; set; }
        public int literaturetypecode { get; set; }
        public string literaturetypecode_format
        {
            get
            {
                switch (literaturetypecode)
                {
                    case 0:
                        return "Presentation";
                    case 1:
                        return "Product Sheet";
                    case 2:
                        return "Policies And Procedures";
                    case 3:
                        return "Sales Literature";
                    case 4:
                        return "Spreadsheets";
                    case 5:
                        return "News";
                    case 6:
                        return "Bulletins";
                    case 7:
                        return "Price Sheets";
                    case 8:
                        return "Manuals";
                    case 9:
                        return "Company Background";
                    case 100001:
                        return "Marketing Collateral";
                    default:
                        return " ";
                }
            }
        }
        public DateTime createdon { get; set; }
        public string createdon_format
        {
            get
            {
                return this.createdon.ToString("dd/MM/yyyy");
            }
        }
    }
}

