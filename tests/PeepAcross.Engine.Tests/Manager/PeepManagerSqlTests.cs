using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeepAcross.Engine.Manager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Manager.Tests
{
    [TestClass()]
    public class PeepManagerSqlTests
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
        private readonly IPeepManager _peepManager;

        public PeepManagerSqlTests()
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
            _peepManager = new PeepManager();
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
        public async Task ParseArgumentsSqlMITest()
        {
            string[] arguments = await FakeMIArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod()]
        public async Task ParseArgumentsSqlRawUserPasswordTest()
        {
            string[] arguments = await FakePwdArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod()]
        public async Task BuildSqlMITest()
        {
            string[] arguments = await GoodMIArguments();
            var result = await _peepManager.ParseArguments(arguments);

            if (result)
            {
                await _peepManager.Build();
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public async Task BuildSqlUserPasswordTest()
        {
            string[] arguments = await GoodPwdArguments();
            var result = await _peepManager.ParseArguments(arguments);

            if (result)
            {
                await _peepManager.Build();
                Assert.IsTrue(true);
            }
        }
    }
}
