
using PeepAcross.Engine.Models;
using PeepAcross.Engine.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Controller
{
    class PeepHttpClient : IPeepClient
    {
        public async Task Connect(PeepParameter peepParameter)
        {
            try
            {
                var peepHttpParameter = peepParameter as PeepHttpParameter;
                await DoConnect(peepHttpParameter.ServiceUri, 
                            peepHttpParameter.MethodKind, peepHttpParameter.Headers, 
                            peepHttpParameter.MessageBody, 
                            peepHttpParameter.BypassServerCertValidation, peepHttpParameter.ClientCertificateDetail,
                            peepHttpParameter.LoadTestCount);
            }
            catch (Exception ex)
            {
                await Logger.ErrorMessage(ex.ToString());
            }
        }

        private async Task DoConnect(string serviceUri, string methodKind, IDictionary<string, string> headers, string messageBody, bool bypassServerCertValidation, string[] clientCertificateDetail, int loadTestCount)
        {
            HttpResponseMessage httpResult = null;

            var httpMethod = HttpHelper.ConvertToHttpMethod(methodKind);

            var httpClient = await GetHttpClient(bypassServerCertValidation, clientCertificateDetail);

            try
            {
                // Reference: https://github.com/dotnet/runtime/issues/44552
                // new HttpRequestMessage inside loop is the solution, so that it would be a new message pushed for SendAsync 

                // Prepare for load test
                for (int i = 0; i < loadTestCount; i++)
                {
                    // Setup the request message
                    var message = new HttpRequestMessage
                    {
                        Method = httpMethod,
                        RequestUri = new Uri(serviceUri)
                    };

                    // Add headers to request message
                    if (headers != null && headers.Count > 0)
                    {
                        foreach (var header in headers)
                        {
                            message.Headers.Add(header.Key, header.Value);
                        }
                    }

                    // Add message body if passed
                    if (!string.IsNullOrEmpty(messageBody))
                    {
                        message.Content = new StringContent(messageBody, Encoding.UTF8, "application/json");
                    }

                    try
                    {
                        // Outbound call
                        httpResult = await httpClient.SendAsync(message);

                        // Print the response
                        if (httpResult.IsSuccessStatusCode)
                        {
                            await Logger.Message($"{i} - result: ", httpResult.ToString());
                        }
                        else
                        {
                            await Logger.ErrorMessage($"{i} - {serviceUri} - {httpResult.ToString()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Suppress and flag exception, so that can proceed for next iteration
                        await Logger.ErrorMessage(ex.ToString());
                    }
                    finally
                    {
                        // Dispose HttpRequestMessage object once a SendAsync invoked
                        if (message is not null)
                        {
                            message.Dispose();
                        }

                        // Dispose the HttpResponseMessage object
                        if (httpResult is not null)
                        {
                            httpResult.Dispose();
                        }
                    }
                }
            }
            finally
            {
                if (httpClient is not null)
                {
                    httpClient.Dispose();
                }                
            }
        }

        private async Task<HttpClient> GetHttpClient(bool bypassServerCertValidation = false, string[] clientCertificateDetail = null)
        {
            HttpClient actualHttpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            X509Certificate2 clientCertificate;

            try
            {
                if (bypassServerCertValidation)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback
                        = (message, cert, chain, errors) => true;                    
                }
                if (clientCertificateDetail != null && clientCertificateDetail.Length > 0)
                {
                    // Read client certificate from file system or certificate store
                    clientCertificate = await MapClientCertificate(clientCertificateDetail);
                    httpClientHandler.ClientCertificates.Add(clientCertificate);
                }

                actualHttpClient = new HttpClient(httpClientHandler);

                return await Task.FromResult(actualHttpClient);
            }
            finally
            {
                if (httpClientHandler is not null)
                {
                    httpClientHandler = null;
                }
            }
        }

        private Task<X509Certificate2> MapClientCertificate(string[] inClientCertificate)
        {
            X509Certificate2 outClientCertificate = null;
            X509Store store = null;
            X509Certificate2Collection xResults = null;

            try
            {
                // Read client certificate from file system as a file
                if (inClientCertificate[0].IndexOf(".") != -1)
                {
                    outClientCertificate = new X509Certificate2(inClientCertificate[0], inClientCertificate[1]);
                }
                else
                {
                    // Read client certificate from certificate store
                    var storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), inClientCertificate[1]);
                    store = new X509Store(StoreName.My, storeLocation);
                    store.Open(OpenFlags.ReadOnly);
                    xResults = store.Certificates.Find(X509FindType.FindByThumbprint, 
                                                    inClientCertificate[0],
                                                    false);
                    
                    outClientCertificate = xResults[0];
                }

                return Task.FromResult(outClientCertificate);
            }
            finally
            {
                if (store is not null)
                {
                    store.Dispose();
                }
            }
        }
    }
}
