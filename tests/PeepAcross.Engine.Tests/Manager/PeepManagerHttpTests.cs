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
    public class PeepManagerHttpTests
    {        
        private readonly string _serviceUri1;
        private readonly string _serviceUri2;
        private readonly string _bypassServerCertValidation;
        private readonly string _clientCertificateFile;
        private readonly string _clientCertificateStore;
        private readonly string _headersRaw;
        private readonly string _headersFile;
        private readonly string _bodyRaw;
        private readonly string _bodyFile;
        private readonly string _loadTest;
        private readonly IPeepManager _peepManager;

        public PeepManagerHttpTests()
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .Build();

            _serviceUri1 = config.GetSection("httpclient:serviceUri:one").Value;
            _serviceUri2 = config.GetSection("httpclient:serviceUri:two").Value;
            _bypassServerCertValidation = config.GetSection("httpclient:bypassServerCertValidation").Value;
            _clientCertificateFile = $"{config.GetSection("httpclient:clientCertificate:file:certificatePfxOrPemOrCrt").Value},{config.GetSection("httpclient:clientCertificate:file:certificatePassword").Value}";
            _clientCertificateStore = $"{config.GetSection("httpclient:clientCertificate:store:certificateThumbprint").Value},{config.GetSection("httpclient:clientCertificate:store:certificateStore").Value}";
            _headersRaw = config.GetSection("httpclient:headers:rawJson").Value;
            _headersFile = config.GetSection("httpclient:headers:fileJson").Value;
            _bodyRaw = config.GetSection("httpclient:body:rawJson").Value;
            _bodyFile = config.GetSection("httpclient:body:fileJson").Value;
            _loadTest = config.GetSection("httpclient:loadTest").Value;
            _peepManager = new PeepManager();
        }

        private Task<string[]> GetHttpsPlainArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri1,
                                            "-methodKind", "get", "-bypassServerCertValidation", _bypassServerCertValidation };
            return Task.FromResult(arguments);
        }
        private Task<string[]> GetHttpsClientCertFileArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri1,
                                            "-methodKind", "get", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-clientCertificate", _clientCertificateFile};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetHttpsClientCertStoreArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri1,
                                            "-methodKind", "get", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-clientCertificate", _clientCertificateStore};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetLocalHeaderRawArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri2,
                                            "-methodKind", "get", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-headers", _headersRaw};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetLocalHeaderFileArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri2,
                                            "-methodKind", "get", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-headers", _headersRaw};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetLocalBodyRawArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri2,
                                            "-methodKind", "post", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-body", _bodyRaw};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetLocalBodyFileArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri2,
                                            "-methodKind", "post", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-body", _bodyFile};
            return Task.FromResult(arguments);
        }

        private Task<string[]> GetLocalBodyRawLoadArguments()
        {
            string[] arguments = new string[] {
                                    "-httpclient", "-serviceUri", _serviceUri2,
                                            "-methodKind", "post", "-bypassServerCertValidation", _bypassServerCertValidation,
                                            "-body", _bodyRaw, "-loadTest", _loadTest};
            return Task.FromResult(arguments);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientTest()
        {
            string[] arguments = await GetHttpsPlainArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientCertFromFileTest()
        {
            string[] arguments = await GetHttpsClientCertFileArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientCertFromStoreTest()
        {
            string[] arguments = await GetHttpsClientCertStoreArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientHeadersRawTest()
        {
            string[] arguments = await GetLocalHeaderRawArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientHeadersFileTest()
        {
            string[] arguments = await GetLocalHeaderFileArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientPostBodyRawTest()
        {
            string[] arguments = await GetLocalBodyRawArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientPostBodyFileTest()
        {
            string[] arguments = await GetLocalBodyFileArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod]
        public async Task ParseArgumentsHttpclientPostBodyRawLoadTest()
        {
            string[] arguments = await GetLocalBodyRawLoadArguments();
            var result = await _peepManager.ParseArguments(arguments);
            Assert.AreEqual(expected: true, result);
        }

        [TestMethod()]
        public async Task BuildHttpclientTest()
        {
            string[] arguments = await GetHttpsPlainArguments();
            var result = await _peepManager.ParseArguments(arguments);
            if (result)
            {
                await _peepManager.Build();
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task BuildHttpclientPostBodyRawLoadTest()
        {
            string[] arguments = await GetLocalBodyRawLoadArguments();
            var result = await _peepManager.ParseArguments(arguments);
            if (result)
            {
                await _peepManager.Build();
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public async Task BuildHttpclientHeadersFileTest()
        {
            string[] arguments = await GetLocalHeaderFileArguments();
            var result = await _peepManager.ParseArguments(arguments);
            if (result)
            {
                await _peepManager.Build();
                Assert.IsTrue(true);
            }
        }
    }
}
