using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib
{
    public static class DataAccessHelpers
    {
        public static string GetConnectionString(string cConnectionStringName = "Default")
        {
            return ConfigurationManager.ConnectionStrings[cConnectionStringName].ConnectionString;
        }
    }
}
