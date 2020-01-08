using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib.Objects
{
    class spiFolder
    {
        public string Name { get; set; }
        public string ServerRelativePath { get; set; }
        public List<spiFolder> Folders { get; set; }
        public List<spiDocument> Files { get; set; }
        public int ItemCount { get; set; }

    }
}
