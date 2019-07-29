using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019Lib.Objects
{
    public class PermissionLevel
    {
        public string Name { get; set; }
        public List<string> Members { get; set; }
        public PermissionLevel(string cName, string cMembers)
        {
            Name = cName;
            Members = new List<string>();
            string[] aMembers = cMembers.Split(';');
            foreach (string aMember in aMembers)
            {
                if (!string.IsNullOrEmpty(aMember))
                {
                    Members.Add(aMember);
                }
            }
        }

        public string GetMembers()
        {
            if (Members.Count == 0) return "";
            string cRetVal = Name + " [";
            foreach (string aMember in Members)
            {
                cRetVal += aMember + ";";
            }
            cRetVal += "] ";
            return cRetVal;
        }
    }
    public class PermissionItem
    {
        public PermissionItem(string cSiteCol, string cSite, string cPermType, string cItemType, SortedDictionary<string, PermissionLevel> oPermLevels, string cName, string cRelativePath, string cDetails)
        {
            SiteCollection = cSiteCol;
            Site = cSite;
            PermissionType = cPermType;
            PermLevels = oPermLevels;
            Name = cName;
            Details = cDetails;
            RelativePath = cRelativePath;
            ItemType = cItemType;
        }
        public PermissionItem(string cSiteCol, string cSite, string cPermType, string cItemType, string cName, string cRelativePath, string cDetails)
        {
            SiteCollection = cSiteCol;
            Site = cSite;
            PermissionType = cPermType;
            SortedDictionary<string, PermissionLevel> oLevels = new SortedDictionary<string, PermissionLevel>();
            oLevels.Add("Inherited", new PermissionLevel("Inherited", ""));
            ItemType = cItemType;

            PermLevels = oLevels;
            Name = cName;
            Details = cDetails;
            RelativePath = cRelativePath;

        }
        public string GetPermissionLevels()
        {
            string cRetVal = "";
            foreach (KeyValuePair<string, PermissionLevel> permLevel in PermLevels)
            {
                cRetVal += permLevel.Value.Name + ": (";
                foreach (string item in permLevel.Value.Members)
                {
                    cRetVal += item + "|";
                }
                cRetVal += ") ";
            }
            return cRetVal;
        }
        public string RelativePath { get; set; }
        public string Site { get; set; }
        public string SiteCollection { get; set; }
        public string PermissionType { get; set; }

        public SortedDictionary<string, PermissionLevel> PermLevels { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string ItemType { get; set; }


    }

    public class PermItem
    {
        public string LoginName { get; set; }
        public string PermItemType { get; set; }
        public string PermItemName { get; set; }

        public string PermItemLevel { get; set; }

        public List<PermItem> Members { get; set; }



    }

    public class PermItemSet
    {
        public List<PermItem> Groups { get; set; }
        public List<PermItem> Users { get; set; }



    }


}
