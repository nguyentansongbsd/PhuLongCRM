using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhuLongCRM.Models
{
    public class AccountLocalization
    {
        public static List<OptionSet> LocalizationOptions;

        public static void Localizations()
        {
            LocalizationOptions = new List<OptionSet>()
            {
                new OptionSet("100000000", "Trong nước"),
                new OptionSet("100000001", "Nước ngoài")
            };
        }
        public static OptionSet GetLocalizationById(string Id)
        {
            Localizations();
            if (Id != string.Empty)
            {
                OptionSet optionSet = LocalizationOptions.Single(x => x.Val == Id);
                return optionSet;
            }
            return null;
        }
    }
}
