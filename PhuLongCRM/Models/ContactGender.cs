using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ContactGender
    {
        public static List<OptionSet> GenderOptions;

        public static void GetGenders()
        {
            GenderOptions = new List<OptionSet>()
            {
                new OptionSet("1","Nam"),
                new OptionSet("2","Nữ"),
                new OptionSet("100000000","Khác")
            };
        }
        public static OptionSet GetGenderById(string Id)
        {
            GetGenders();
            if (Id != string.Empty)
            {
                OptionSet optionSet = GenderOptions.Single(x => x.Val == Id);
                return optionSet;
            }
            return null;
        }
    }
}
