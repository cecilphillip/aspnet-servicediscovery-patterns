using System;
using System.IO;
using System.Threading.Tasks;
using Consul;

namespace SchoolClient
{

    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceDiscoveryAttribute : Attribute
    {
        public string Endpoint { get; set; }
        public string HttpVerb { get; set; }
    }

    public static class AgentServiceExtensions
    {
        public static Uri GetServiceUrl(this AgentService service) =>
               new Uri($"{service.Address}:{service.Port}");

    }
}