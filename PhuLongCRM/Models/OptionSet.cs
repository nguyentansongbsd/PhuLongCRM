using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class OptionSet : BaseViewModel
    {
        public string Val { get; set; }

        public string _label;
        public string Label { get => _label; set { _label = value; OnPropertyChanged(nameof(Label)); } }

        private bool _selected;
        public bool Selected { get=>_selected; set { _selected = value;OnPropertyChanged(nameof(Selected)); } }
        public bool IsMultiple { get; set; }

        public OptionSet()
        {

        }

        public OptionSet(string val, string label, bool selected = false)
        {
            Val = val;
            Label = label;
            Selected = selected;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is OptionSet)) return false;
            var optionSet = (OptionSet)obj;
            return this.Val == optionSet.Val;
        }

        public override int GetHashCode()
        {
            return this.Val.GetHashCode();
        }
    }
}
