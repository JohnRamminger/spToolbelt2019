using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib
{
    public class SiteInfo
    {
        public string SiteID { get; set; }
        public string ParentSiteID { get; set; }

        public string AccessRequestEmail { get; set; }
        public string Adminsitrators { get; set; }
        public string Description { get; set; }

        public string FullControlUsers { get; set; }
        public int Depth { get; set; }
        public int FullControlUserCount { get; set; }

        public bool HasSubSites { get; set; }
        public bool ShowSite { get; set; }
        public bool UniquePermissions { get; set; }
        public string Permissions { get; set; }
        public int UserCount { get; set; }
        public int SiteComplexityScore { get; set; }
        public int TotalSiteComplexityScore { get; set; }

        public DateTime LastItemModified { get; set; }
        public DateTime LastScan { get; set; }

        public DateTime LastUserItemModified { get; set; }

        public DateTime PermissionsLastScan { get; set; }

        public string Owners { get; set; }
        public string RequestApprovers { get; set; }

        public string SiteUrl { get; set; }
        public string Title { get; set; }
        public string TargetAction { get; set; }
        public string TargetLocation { get; set; }


    }
}
