using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib.Objects
{
    class spiList
    {
        public string Title { get; set; }
        public string InternalName { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public List<spiListItem> Items { get; set; }
        public spiFolder RootFolder { get; set; }
        public int ItemCount { get; set; }
        public int BaseTemplateType { get; set; }
        public bool UniquePerms { get; set; }
        public string ServerRelativePath { get; set; }

        public string Permissions { get; set; }

    }
}
