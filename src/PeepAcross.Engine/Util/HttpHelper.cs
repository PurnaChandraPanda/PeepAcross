using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Util
{
    class HttpHelper
    {
        public static HttpMethod ConvertToHttpMethod(string methodKind)
        {
            return new HttpMethod(methodKind);
        }

        public static string[] GetClientCertificateDetails(string clientCertificateDetail)
        {
            string[] clientCertificateResult = new string[2];
            int separatorIndex = clientCertificateDetail.IndexOf(',');
            clientCertificateResult[0] = clientCertificateDetail.Substring(0, separatorIndex);            
            clientCertificateResult[1] = clientCertificateDetail.Substring(separatorIndex+1, clientCertificateDetail.Length-separatorIndex-1);
            
            return clientCertificateResult;
        }

        public static async Task<IDictionary<string, string>> GetHeaders(string headersDetail)
        {
            IDictionary<string, string> headers;
            Stream headerStream = null;

            try
            {
                if (headersDetail.EndsWith(".json"))
                {
                    headerStream = await GetJsonStream(path: headersDetail);
                }
                else
                {
                    headerStream = await GetJsonStream(rawJson: headersDetail);
                }

                headers = await JsonSerializer.DeserializeAsync<IDictionary<string, string>>(headerStream);
                
                return headers;
            }
            finally
            {
                // Clear header stream
                if (headerStream != null)
                {
                    await headerStream.FlushAsync();
                }
            }
        }

        public static async Task<string> GetMessageBody(string body)
        {
            string responseBody = string.Empty;

            if (body.EndsWith(".json"))
            {
                //return await File.ReadAllTextAsync(body);
                responseBody = await Task.FromResult(File.ReadAllText(body));
            }
            else
            {
                responseBody = await FormatJson(body);
            }

            return responseBody;
        }

        private static async Task<Stream> GetJsonStream(string path = "", string rawJson = "")
        {
            Stream stream;
            string modifiedJson = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var fileStream = File.OpenRead(path);
                    stream = fileStream;
                }
                else
                {
                    modifiedJson = await FormatJson(rawJson);
                    var byteArray = Encoding.UTF8.GetBytes(modifiedJson);
                    var memoryStream = new MemoryStream(byteArray);
                    stream = memoryStream;
                }

                return await Task.FromResult(stream);
            }
            finally
            {
                if (modifiedJson is not null)
                {
                    modifiedJson = null;
                }
            }
        }

        private static Task<string> FormatJson(string rawJsonString)
        {
            string modifiedJsonString = rawJsonString.Replace(":", "\":\"")
                                                .Replace(",", "\",\"")
                                                .Replace("{", "{\"")
                                                .Replace("}", "\"}");
            return Task.FromResult(modifiedJsonString);
        }
    }
}
