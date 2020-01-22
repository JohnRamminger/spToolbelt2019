using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib
{
    public class slSiteInfo
    {
        public string SiteID { get; set; }
        public string ParentSiteID { get; set; }

        public string AccessRequestEmail { get; set; }
        public string Adminsitrators { get; set; }
        public string Description { get; set; }

        public string FullControlUsers { get; set; }
        public int Depth { get; set; }
        public int FullControlUserCount { get; set; }

        public int HasSubSites { get; set; }
        public int ShowSite { get; set; }
        public int UniquePermissions { get; set; }
        public string Permissions { get; set; }
        public int UserCount { get; set; }
        public int SiteComplexityScore { get; set; }
        public int TotalSiteComplexityScore { get; set; }

        public string LastItemModified { get; set; }
        public string LastScan { get; set; }

        public string LastUserItemModified { get; set; }

        public string PermissionsLastScan { get; set; }

        public string Owners { get; set; }
        public string RequestApprovers { get; set; }

        public string SiteUrl { get; set; }
        public string Title { get; set; }
        public string TargetAction { get; set; }
        public string TargetLocation { get; set; }


    }
}
