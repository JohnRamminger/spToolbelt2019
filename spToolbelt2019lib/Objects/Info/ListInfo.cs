using System;
using System.Linq;

namespace spToolbelt2019lib
{
    public class ListInfo
    {
        public int ItemCount { get; set; }
        public DateTime LastItemModified { get; set; }
        public DateTime LastScan { get; set; }
        public DateTime LastUserItemModified { get; set; }
        public int ListComplexityScore { get; set; }
        public string ListID { get; set; }
        public string Permissions { get; set; }
        public DateTime PermissionsLastScan { get; set; }
        public string SiteID { get; set; }
        public string SiteUrl { get; set; }
        public string TargetAction { get; set; }
        public string TargetLocation { get; set; }

        public string TargetList { get; set; }

        public string ListType { get; set; }
        public string RootFolderUrl { get; set; }

        public Int32 Id  { get;set;}

        public string Title { get; set; }
        public bool UniquePermissions { get; set; }


    }
}
