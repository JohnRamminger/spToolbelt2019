using Dapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace spToolbelt2019lib
{
    public class ItemInfo
    {

        [Key]
        public int ItemID { get; set; }
        public string ListItemID { get; set; }
        public DateTime LastScan { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedUser { get; set; }
        public int VersionCount { get; set; }
        public string VersionInfo { get; set; }

        public string ListID { get; set; }
        public string SiteID { get; set; }
        public string ItemData { get; set; }
        public int ItemLevel { get; set; }
        public int ItemSize { get; set; }
        public string ItemType { get; set; }
        public string ItemUrl { get; set; }
        public bool UniquePermissions { get; set; }
        public string Permissions { get; set; }

        public string ListTitle { get; set; }
        public string ParentFolderUrl { get; set; }
        public string Title { get; set; }
    }
}


	