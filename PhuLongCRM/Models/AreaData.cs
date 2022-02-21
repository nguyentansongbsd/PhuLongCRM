﻿using PhuLongCRM.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class AreaData
    {
        public static List<OptionSet> Areas()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000000","Miền bắc"),
                new OptionSet("100000001","Miền trung"),
                new OptionSet("100000002","Miền nam"),
                new OptionSet("100000003",Language.tat_ca),
            };
        }

        public static OptionSet GetAreaById(string Id)
        {
            return Areas().SingleOrDefault(x => x.Val == Id);
        }
    }
}
