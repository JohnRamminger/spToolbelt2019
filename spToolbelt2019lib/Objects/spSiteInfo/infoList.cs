using Microsoft.SharePoint;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace spToolbelt2019lib
{ 
    public class infoList
    {
        public string ListTitle { get; set; }
        public bool UniquePermissions { get; set; }
        public int ListItemCount { get; set; }
        public List<infoItem> items { get; set; }
        public List<infoEventReciever> EventRecievers { get; set; }
        public List<infoField> fields { get; set; }
        public string Id { get; set; }
        public string BaseTemplate { get; set; }
        public List<string> workflows { get; set; }
        public List<infoPermItem> Permissions { get; set; }
        public List<string> ContentTypes { get; set; }
        public bool IsPageLibrary { get; set; }

    }
}
