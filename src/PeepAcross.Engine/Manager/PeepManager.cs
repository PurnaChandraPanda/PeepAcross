using PeepAcross.Engine.Controller;
using PeepAcross.Engine.Models;
using PeepAcross.Engine.Util;
using System;
using System.Threading.Tasks;

namespace PeepAcross.Engine.Manager
{
    public class PeepManager: IPeepManager
    {
        private static PeepParameter _peepParameter;
        private static string _clientType;
        private static IPeepClient _client;

        public async Task Build()
        {
            switch (_clientType)
            {
                case "sql":
                    _client = new PeepSqlClient();
                    break;

                case "httpclient":
                    _client = new PeepHttpClient();
                    break;

                default:
                    break;
            }

            var controller = new PeepController(_client);
            await controller.Invoke(_peepParameter);
        }

        public async Task<bool> ParseArguments(string[] args)
        {
            bool parseFlag = false;

            if (args.Length == 0)
            {
                await Logger.Message(message: await ArgumentParser.OnUsage());
                return parseFlag;
            }

            var parsedResult = await ArgumentParser.Parse(args);

            // read the key-value pair and type
            var dictionaryResult = parsedResult.Item2;
            _clientType = parsedResult.Item1;

            if (dictionaryResult != null && dictionaryResult.Count > 0)
            {
                // Set the response as parameters are parsed
                switch (_clientType)
                {
                    case "sql":
                        if (dictionaryResult.ContainsKey("aadTenantId"))
                        {
                            _peepParameter = new PeepSqlParameter(
                                                    sqlServer: dictionaryResult["sqlServer"],
                                                    sqlServerPort: Int32.Parse(dictionaryResult["sqlServerPort"]),
                                                    sqlDatabase: dictionaryResult["sqlDatabase"],
                                                    sqlQuery: dictionaryResult["sqlQuery"],
                                                    aadTenantId: dictionaryResult["aadTenantId"],
                                                    aadClientId: dictionaryResult["aadClientId"],
                                                    aadClientSecretKey: dictionaryResult["aadClientSecretKey"]
                                                 );
                        }
                        else if (dictionaryResult.ContainsKey("sqlUserID"))
                        {
                            _peepParameter = new PeepSqlParameter(
                                                    sqlServer: dictionaryResult["sqlServer"],
                                                    sqlServerPort: Int32.Parse(dictionaryResult["sqlServerPort"]),
                                                    sqlDatabase: dictionaryResult["sqlDatabase"],
                                                    sqlQuery: dictionaryResult["sqlQuery"],
                                                    sqlUserID: dictionaryResult["sqlUserID"],
                                                    sqlPassword: dictionaryResult["sqlUserPassword"]
                                                 );
                        }
                        break;

                    case "httpclient":
                        var peepHttpParameter = new PeepHttpParameter(
                                                 dictionaryResult["serviceUri"],
                                                 dictionaryResult["methodKind"]
                                                );

                        string dictionaryValue;

                        if (dictionaryResult.TryGetValue("headers", out dictionaryValue))
                        {
                            peepHttpParameter.Headers = await HttpHelper.GetHeaders(dictionaryValue);
                        }
                        if (dictionaryResult.TryGetValue("bypassServerCertValidation", out dictionaryValue))
                        {
                            peepHttpParameter.BypassServerCertValidation = Boolean.Parse(dictionaryValue);
                        }
                        if (dictionaryResult.TryGetValue("clientCertificate", out dictionaryValue))
                        {
                            peepHttpParameter.ClientCertificateDetail = HttpHelper.GetClientCertificateDetails(dictionaryValue);
                        }
                        if (dictionaryResult.TryGetValue("body", out dictionaryValue))
                        {
                            peepHttpParameter.MessageBody = await HttpHelper.GetMessageBody(dictionaryValue);
                        }
                        if (dictionaryResult.TryGetValue("loadTest", out dictionaryValue))
                        {
                            peepHttpParameter.LoadTestCount = Int32.Parse(dictionaryValue);
                        }

                        // Set the global PeepParameter
                        _peepParameter = peepHttpParameter;

                        break;

                    default:
                        break;
                }
            }

            parseFlag = true;
            return parseFlag;
        }
    }
}
