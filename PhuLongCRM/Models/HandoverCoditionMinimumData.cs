using System;
using System.Collections.Generic;
using System.Linq;
using PhuLongCRM.Resources;

namespace PhuLongCRM.Models
{
    public class HandoverCoditionMinimumData
    {
        public static OptionSet GetHandoverCoditionMinimum(string Id)
        {
            return HandoverCoditionMinimums().SingleOrDefault(x => x.Val == Id);
        }
        public static OptionSet GetMapIdProject_Handover(string Id)
        {
            return MapIdProject_Handover().SingleOrDefault(x => x.Val == Id);
        }
        public static List<OptionSet> HandoverCoditionMinimums()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000001",Language.handover_bare_shell_type ),//"Bare-shell" //handover_bare_shell_type
                new OptionSet("100000002",Language.handover_basic_finished_type ),//"Basic Finished" //handover_basic_finished_type
                new OptionSet("100000000",Language.handover_fully_finished_type ),//"Fully Finished" //handover_fully_finished_type
                new OptionSet("100000003",Language.handover_fully_finished_and_bare_shell_type ),//"Fully Finished the outside and Bare-shell the inside" //handover_fully_finished_and_bare_shell_type
                new OptionSet("100000005",Language.handover_add_on_option_type ),//"Add On Option" //handover_add_on_option_type
            };
        }
        public static List<OptionSet> MapIdProject_Handover()
        {
            return new List<OptionSet>()
            {
                new OptionSet("100000001","861450000" ),//"Bare-shell" //handover_bare_shell_type
                new OptionSet("100000002","861450001" ),//"Basic Finished" //handover_basic_finished_type
                new OptionSet("100000000","861450002"),//"Fully Finished" //handover_fully_finished_type
                new OptionSet("100000003","861450003" ),//"Fully Finished the outside and Bare-shell the inside" //handover_fully_finished_and_bare_shell_type
                new OptionSet("100000005","861450004" ),//"Add On Option" //handover_add_on_option_type
            };
        }
    }
}
