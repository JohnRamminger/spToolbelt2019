using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace spToolbelt2019lib
{
    public static class SQLDataAccess
    {

        public static List<T> SLLoadData<T>(string cSql, Dictionary<string, object> parameters, string cConnectionName = "SQLite")
        {
            string cConnectStr = DataAccessHelpers.GetConnectionString(cConnectionName);
            try
            {
                using (IDbConnection conn = new SQLiteConnection(cConnectStr))
                {
                    var rows = conn.Query<T>(cSql, parameters.ToDictionary());
                    return rows.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SQLDataAccess.LoadData - " + ex.Message + " - " + cConnectStr, ex);
            }
        }

        public static void SLSaveData<T>(string cSql, Dictionary<string, object> parameters, string cConnectionName = "SQLite")
        {
            string cConnectStr = DataAccessHelpers.GetConnectionString(cConnectionName);

            try
            {
                using (IDbConnection conn = new SQLiteConnection(cConnectStr))
                {
                    conn.Execute(cSql, parameters.ToDictionary());
                }

            }
            catch (Exception ex)
            {
                throw new Exception("SQLDataAccess.SaveData - " + ex.Message + " - " + cConnectStr, ex);
            }
        }



        public static List<T> LoadData<T>(string cSql, Dictionary<string, object> parameters, string cConnectionName = "Default")
        {
            string cConnectStr = DataAccessHelpers.GetConnectionString(cConnectionName);
            try
            {
                using (IDbConnection conn = new SqlConnection(cConnectStr))
                {
                    var rows = conn.Query<T>(cSql, parameters.ToDictionary());
                    return rows.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SQLDataAccess.LoadData - " + ex.Message + " - " + cConnectStr, ex);
            }
        }

        public static void SaveData<T>(string cSql, Dictionary<string, object> parameters, string cConnectionName = "Default")
        {
            string cConnectStr = DataAccessHelpers.GetConnectionString(cConnectionName);

            try
            {
                using (IDbConnection conn = new SqlConnection(cConnectStr))
                {
                    conn.Execute(cSql, parameters.ToDictionary());
                }

            }
            catch (Exception ex)
            {
                throw new Exception("SQLDataAccess.SaveData - " + ex.Message + " - " + cConnectStr, ex);
            }
        }


        public static DynamicParameters ToDictionary(this Dictionary<string, object> parameters)
        {
            if (parameters == null) return null;
            DynamicParameters dp = new DynamicParameters();
            parameters.ToList().ForEach(x => dp.Add(x.Key, x.Value));
            return dp;
        }

    }
}
