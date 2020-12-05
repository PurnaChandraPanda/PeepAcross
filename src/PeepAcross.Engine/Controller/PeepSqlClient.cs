using Microsoft.IdentityModel.Clients.ActiveDirectory;
using PeepAcross.Engine.Models;
using PeepAcross.Engine.Util;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Controller
{
    class PeepSqlClient : IPeepClient
    {
        public async Task Connect(PeepParameter peepParameter)
        {
            PeepSqlParameter sqlParameter = peepParameter as PeepSqlParameter;
            string sqlConnectionString = string.Empty;
            string accessToken = string.Empty;

            try
            {
                if (sqlParameter.AADTenantId is not null)
                {
                    string aadInstanceId = $"https://login.windows.net/{sqlParameter.AADTenantId}";
                    string resourceId = "https://database.windows.net/";
                    sqlConnectionString = $"Data Source=tcp:{sqlParameter.SqlServer},{sqlParameter.SqlServerPort};Initial Catalog={sqlParameter.SqlDatabase};Persist Security Info=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False";

                    // Get the AAD access token
                    accessToken = await GetAccessToken(
                                                    sqlParameter.AADClientId,
                                                    sqlParameter.AADClientSecretKey,
                                                    aadInstanceId,
                                                    resourceId
                                                );

                    // Prepare for the Sql operation
                    await SqlCall(sqlConnectionString, sqlParameter.SqlQuery, accessToken);
                }
                else if (sqlParameter.SqlUserID is not null)
                {
                    sqlConnectionString = $"Data Source=tcp:{sqlParameter.SqlServer},{sqlParameter.SqlServerPort};Initial Catalog={sqlParameter.SqlDatabase};Persist Security Info=False;User ID={sqlParameter.SqlUserID};Password={sqlParameter.SqlUserPassword};Connect Timeout=30;Encrypt=True;TrustServerCertificate=False";
                    await SqlCall(sqlConnectionString, sqlParameter.SqlQuery);
                }

            }
            catch (Exception ex)
            {
                await Logger.ErrorMessage(ex.ToString());
            }
            finally
            {
                if (sqlParameter != null)
                {
                    sqlParameter = null;
                }
                if (sqlConnectionString != null)
                {
                    sqlConnectionString = null;
                }
            }
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecretKey, string aadInstanceId, string resourceId)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstanceId);
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecretKey);

            // Start time calculation
            DateTime startTime = DateTime.Now;
            await Logger.Message("Time ", $"{String.Format("{0:hh:mm:ss.fff}", startTime)}");
            
            // Acquire token from AAD
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(resourceId, clientCredential);

            // End time calculation
            DateTime endTime = DateTime.Now;
            await Logger.Message("Got token at ", $"{String.Format("{0:hh:mm:ss.fff}", endTime)}");

            // Caculate elapsed time
            await Logger.Message("Total time to get token in milliseconds ", $"{(endTime - startTime).TotalMilliseconds}");
            
            // Return the acquired access token
            return authenticationResult.AccessToken;
        }

        private async Task SqlCall(string sqlConnectionString, string sqlQuery, string accessToken = "")
        {
            using (var conn = new SqlConnection(sqlConnectionString))
            {
                // Set SqlConnection with the access token
                if (!string.IsNullOrEmpty(accessToken))
                {
                    conn.AccessToken = accessToken;

                    // Display the access token
                    await Logger.Message("This is your token: ", $"{accessToken}");
                }

                // Start time calculation
                DateTime startTime = DateTime.Now;
                await Logger.Message("Starting to open connection at ",$"{String.Format("{0:hh:mm:ss.fff}", startTime)}");

                // Sql connection open
                conn.Open();

                // End time calculation
                DateTime endTime = DateTime.Now;
                await Logger.Message("Got connection at ", $"{String.Format("{0:hh:mm:ss.fff}", endTime)}");

                // Display the delta time
                await Logger.Message("Total time to establish connection in milliseconds ", $"{(endTime - startTime).TotalMilliseconds}");

                // Update start time for sql query execution and display
                startTime = DateTime.Now;               
                await Logger.Message($"Starting to run query at ", $"{String.Format("{0:hh:mm:ss.fff}", startTime)}");

                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    var result = await cmd.ExecuteScalarAsync(CancellationToken.None);
                    await Logger.Message(valueMessage: result.ToString());
                }

                // Update end time for the sql query execution and display
                endTime = DateTime.Now;
                await Logger.Message("Completing running query at ", $"{String.Format("{0:hh:mm:ss.fff}", endTime)}");

                // Display the delta time taken in query execution
                await Logger.Message("Total time to execute query in milliseconds ", $"{(endTime - startTime).TotalMilliseconds}");
            }
        }
    }
}
