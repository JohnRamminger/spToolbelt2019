using Dapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace spToolbelt2019lib
{
    public class slItemInfo
    {

        [Key]
        public string ItemGV { get; set; }
        public string ListItemGV { get; set; }
        public string LastScan { get; set; }
        public string LastModified { get; set; }
        public string LastModifiedUser { get; set; }
        public int VersionCount { get; set; }
        public string VersionInfo { get; set; }

        public string ListGV { get; set; }
        public string SiteGV { get; set; }
        public string ItemData { get; set; }
        public int ItemLevel { get; set; }
        public int ItemSize { get; set; }
        public string ItemType { get; set; }
        public string ItemUrl { get; set; }
        public int UniquePermissions { get; set; }
        public string Permissions { get; set; }

        public string ListTitle { get; set; }
        public string ParentFolderUrl { get; set; }
        public string Title { get; set; }
    }
}


	