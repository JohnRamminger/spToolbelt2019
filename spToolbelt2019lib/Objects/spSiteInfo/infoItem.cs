using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib
{
    public class infoItem
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public List<infoPermItem> permissions { get; set; }
    }
}
