using System;
using System.Collections.Generic;
using System.Text;

namespace IMFS.Web.Models.Misc
{
    public class LookupModel
    {
        public LookupModel() { }
        public LookupModel(string label, string value)
        {
            this.label = label;
            this.value = value;
        }

        public LookupModel(string label, string value, bool selected = false)
        {
            this.label = label;
            this.value = value;
            this.selected = selected;
        }

        public string label { get; set; }
        public string value { get; set; }
        public bool selected { get; set; }
        public string styleClass { get; set; }
        public string tooltip { get; set; }
    }
}
