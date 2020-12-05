using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeepAcross.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Tests
{
    [TestClass()]
    public class EntryPointSqlTests
    {
        private readonly string _sqlServer;
        private readonly string _sqlServerPort;
        private readonly string _sqlDatabase;
        private readonly string _sqlQuery;
        private readonly string _aadTenantId;
        private readonly string _aadClientId;
        private readonly string _aadClientSecretKey;
        private readonly string _sqlUserID;
        private readonly string _sqlUserPassword;

        public EntryPointSqlTests()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .Build();

            _sqlServer = config.GetSection("sql:sqlServer").Value;
            _sqlServerPort = config.GetSection("sql:sqlServerPort").Value;
            _sqlDatabase = config.GetSection("sql:sqlDatabase").Value;
            _sqlQuery = config.GetSection("sql:sqlQuery").Value;
            _aadTenantId = config.GetSection("sql:aad:aadTenantId").Value;
            _aadClientId = config.GetSection("sql:aad:aadClientId").Value;
            _aadClientSecretKey = config.GetSection("sql:aad:aadClientSecretKey").Value;
            _sqlUserID = config.GetSection("sql:pwd:sqlUserID").Value;
            _sqlUserPassword = config.GetSection("sql:pwd:sqlUserPassword").Value;
        }

        private Task<string[]> FakeMIArguments()
        {
            string[] arguments = new string[] {
                                    "-sql", "-sqlServer", "xxxx.database.windows.net",
                                            "-sqlServerPort", "1433", "-sqlDatabase", "db1231sf",
                                            "-sqlQuery", "SELECT 1", "-aadTenantId", Guid.NewGuid().ToString(),
                                            "-aadClientId", Guid.NewGuid().ToString(), "-aadClientSecretKey", "secret-key" };
            return Task.FromResult(arguments);
        }

        private Task<string[]> FakePwdArguments()
        {
            string[] arguments = new string[] {
                                    "-sql", "-sqlServer", "xxxx.database.windows.net",
                                            "-sqlServerPort", "1433", "-sqlDatabase", "db1231sf",
                                            "-sqlQuery", "SELECT 1", "-sqlUserID", "userid", "-sqlUserPassword", "password" };
            return Task.FromResult(arguments);
        }

        private Task<string[]> GoodMIArguments()
        {
            string[] arguments = new string[] {
                                    "-sql", "-sqlServer", _sqlServer,
                                            "-sqlServerPort", _sqlServerPort, "-sqlDatabase", _sqlDatabase,
                                            "-sqlQuery", _sqlQuery, "-aadTenantId", _aadTenantId,
                                            "-aadClientId", _aadClientId, "-aadClientSecretKey", _aadClientSecretKey };
            return Task.FromResult(arguments);
        }

        private Task<string[]> GoodPwdArguments()
        {
            string[] arguments = new string[] {
                                    "-sql", "-sqlServer", _sqlServer,
                                            "-sqlServerPort", _sqlServerPort, "-sqlDatabase", _sqlDatabase,
                                            "-sqlQuery", _sqlQuery,
                                            "-sqlUserID", _sqlUserID, "-sqlUserPassword", _sqlUserPassword };
            return Task.FromResult(arguments);
        }

        [TestMethod()]
        public async Task RunSqlMITest()
        {
            string[] arguments = await GoodMIArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunSqlUserPasswordTest()
        {
            string[] arguments = await GoodPwdArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }
    }
}