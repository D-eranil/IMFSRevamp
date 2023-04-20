using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFS.Web.Api.WebModels
{
    public class TopNavItem
    {
        public string Name { get; set; }

        public string Route { get; set; }

        public bool DynamicLink { get; set; }

        public string IconClass { get; set; }

        public string ItemState { get; set; }

        public bool HasChildren { get; set; }

        public string IncludeRoles { get; set; }
        public List<TopNavItem> Children { get; set; }
    }
}





