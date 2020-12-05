
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

        private Task<HttpClient> GetHttpClient(bool bypassServerCertValidation = false, string[] clientCertificateDetail = null)
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
                    if (clientCertificateDetail[0].IndexOf(".") != -1)
                    {
                        clientCertificate = new X509Certificate2(clientCertificateDetail[0], clientCertificateDetail[1]);
                    }
                    else
                    {
                        X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                        store.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection xResults = store.Certificates.Find(
                                                                X509FindType.FindByThumbprint,
                                                                "C58C1042CC448BE67EB69A279245D02C2CC0B4D3",
                                                                false);
                        clientCertificate = xResults[0];
                    }

                    httpClientHandler.ClientCertificates.Add(clientCertificate);
                }

                actualHttpClient = new HttpClient(httpClientHandler);

                return Task.FromResult(actualHttpClient);
            }
            finally
            {
                if (httpClientHandler is not null)
                {
                    httpClientHandler = null;
                }
            }
        }
    }
}
