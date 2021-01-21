using System.Collections.Generic;

namespace spToolbelt2019lib
{
    public class infoSite
    {
        public string Id { get; set; }
        public string ParentWebId { get; set; }
        public string ParentWeb { get; set; }
        public string SiteTitle { get; set; }
        public string SiteUrl { get; set; }
        public List<infoList> Lists { get; set; }
        public List<string> Features { get; set; }
        public bool HasUniquePermissions { get; set; }

        public bool IsSiteCollection { get; set; }
        public List<string> CodeFiles { get; set; }

        public List<infoPage> PageFiles { get; set; }

        public List<infoPermissionsGroup> SiteGroups { get; set; }
        public List<infoPermissionsGroup> Groups { get; set; }

        public List<infoPermItem> Permissions { get; set; }



    }
}
