
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections;
using System.Collections.Generic;

namespace PeepAcross.Engine.Models
{
    class PeepHttpParameter: PeepParameter
    {
        public string ServiceUri { get; set; }
        public string MethodKind { get; set; }
        public IDictionary<string, string> Headers { get; set; } = null;
        public string MessageBody { get; set; }
        public bool BypassServerCertValidation { get; set; } = false;
        public string[] ClientCertificateDetail { get; set; }
        public int LoadTestCount { get; set; } = 1;

        public PeepHttpParameter(string serviceUri = "", string methodKind = "")
            : this(serviceUri, methodKind, null)
        {
            
        }
        public PeepHttpParameter(string serviceUri, string methodKind, IDictionary<string, string> headers)
            :this(serviceUri, methodKind, headers, false)
        {

        }

        public PeepHttpParameter(string serviceUri, string methodKind, IDictionary<string, string> headers, bool bypassServerCertValidation)
            :this(serviceUri, methodKind, headers, bypassServerCertValidation, null)
        {
            
        }

        public PeepHttpParameter(string serviceUri, string methodKind, IDictionary<string, string> headers,
                                bool bypassServerCertValidation, string[] clientCertificateDetail)
        {
            ServiceUri = serviceUri;
            MethodKind = methodKind;
            Headers = headers;
            BypassServerCertValidation = bypassServerCertValidation;
            ClientCertificateDetail = clientCertificateDetail;
        }

            //public PeepHttpParameter(string serviceUri, string methodKind, string messageBodyPath)
            //    : this(serviceUri, methodKind, messageBodyPath, false, null, 1)
            //{

            //}

            //public PeepHttpParameter(string serviceUri, string methodKind, bool bypassServerCertValidation,
            //            string messageBodyPath, string clientCertificatePath, int loadTestCount)
            //{
            //    ServiceUri = serviceUri;
            //    MethodKind = methodKind;
            //    MessageBodyPath = messageBodyPath;
            //    BypassServerCertValidation = bypassServerCertValidation;
            //    ClientCertificatePath = clientCertificatePath;
            //    LoadTestCount = loadTestCount;
            //}
        }
}
