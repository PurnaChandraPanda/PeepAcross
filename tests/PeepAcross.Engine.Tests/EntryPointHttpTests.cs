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
    public class EntryPointHttpTests
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

        public EntryPointHttpTests()
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

        [TestMethod()]
        public async Task RunHttpsPlainTest()
        {
            string[] arguments = await GetHttpsPlainArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunClientCertFileTest()
        {
            string[] arguments = await GetHttpsClientCertFileArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunClientCertStoreTest()
        {
            string[] arguments = await GetHttpsClientCertStoreArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunHeaderRawTest()
        {
            string[] arguments = await GetLocalHeaderRawArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunHeaderFileTest()
        {
            string[] arguments = await GetLocalHeaderFileArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunBodyRawTest()
        {
            string[] arguments = await GetLocalBodyRawArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunBodyFileTest()
        {
            string[] arguments = await GetLocalBodyFileArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public async Task RunBodyRawLoadTest()
        {
            string[] arguments = await GetLocalBodyRawLoadArguments();
            await EntryPoint.Run(arguments);
            Assert.IsTrue(true);
        }
    }
}
