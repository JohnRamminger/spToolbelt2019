using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib.Objects
{
    public class spiDocument
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public string ServerRelativePath { get; set; }
        public int VersionCount { get; set; }
        public int PathLength { get; set; }
        public spiListItem Item { get; set; }
    }
}
