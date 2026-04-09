using System.Net;

namespace Vialidad.Routing
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | (SecurityProtocolType)12288; // Tls12 + Tls13
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;
            WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            //var routingCalculator = new RoutingCalculator();
            //routingCalculator.CalculatePendingRoutes("driving");

            var normalizer = new RoutingNormalizer();
            normalizer.NormalizeDb();
        }
    }
}
