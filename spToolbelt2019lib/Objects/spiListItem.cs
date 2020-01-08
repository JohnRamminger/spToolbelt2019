using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib.Objects
{
    public class spiListItem
    {
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Permissions { get; set; }
        public bool HasUniquePerms { get; set; }
        public string properties { get; set; }
    }
}
