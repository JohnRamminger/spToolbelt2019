using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019Lib.Objects
{
    public class spFieldValue
    {

        public spFieldValue(string cFieldName,string cFieldType,string cFieldValue)
        {
            FieldName = cFieldName;
            FieldType = cFieldType;
            FIeldValue = cFieldValue;

        }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FIeldValue { get; set; }
    }
}
