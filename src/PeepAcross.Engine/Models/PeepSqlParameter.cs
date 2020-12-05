
namespace PeepAcross.Engine.Models
{
    class PeepSqlParameter: PeepParameter
    {
        // SQL server name
        public string SqlServer { get; set; }

        // SQL server port
        public int SqlServerPort { get; set; }

        // SQL server hosted database name
        public string SqlDatabase { get; set; }

        // SQL server query
        public string SqlQuery { get; set; }

        // AAD tenant Id for MI
        public string AADTenantId { get; set; }

        // AAD client Id for MI
        public string AADClientId { get; set; }

        // AAD client secret value for MI
        public string AADClientSecretKey { get; set; }

        // SQL userID 
        public string SqlUserID { get; set; }

        // Associated password for sql connection
        public string SqlUserPassword { get; set; }

        public PeepSqlParameter(string sqlServer, int sqlServerPort, string sqlDatabase, string sqlQuery,
            string sqlUserID, string sqlPassword)
            : this(sqlServer, sqlServerPort, sqlDatabase, sqlQuery)
        {
            SqlUserID = sqlUserID;
            SqlUserPassword = sqlPassword;
        }

        public PeepSqlParameter(string sqlServer, int sqlServerPort, string sqlDatabase, string sqlQuery, 
            string aadTenantId, string aadClientId, string aadClientSecretKey)
            :this(sqlServer, sqlServerPort, sqlDatabase, sqlQuery)
        {
            AADTenantId = aadTenantId;
            AADClientId = aadClientId;
            AADClientSecretKey = aadClientSecretKey;
        }

        public PeepSqlParameter(string sqlServer, int sqlServerPort, string sqlDatabase, string sqlQuery)
        {
            SqlServer = sqlServer;
            SqlServerPort = sqlServerPort;
            SqlDatabase = sqlDatabase;
            SqlQuery = sqlQuery;
        }
    }
}
