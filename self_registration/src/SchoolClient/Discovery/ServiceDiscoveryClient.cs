using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Consul;
using Newtonsoft.Json.Linq;

namespace SchoolClient
{
    public interface IServiceDiscoveryClient
    {
        string CatalogProvider { get; }
        Task DiscoverAsync();
        Task<string> Invoke(ServiceDiscoveryAttribute serviceAttr);
    }

    public class ServiceDiscoveryClient : IServiceDiscoveryClient
    {
        public string CatalogProvider { get; } = "Consul";
        private readonly ConsulClient _consulClient;
        private readonly HttpClient _innerClient = new HttpClient();
        private List<ServiceWrapper<AgentService>> _discoveredServices;

        public ServiceDiscoveryClient(string address)
        {
            _discoveredServices = new List<ServiceWrapper<AgentService>>();
            _consulClient = new ConsulClient(c =>
           {
               var uri = new Uri(address);
               c.Address = uri;
           });
        }

        public async Task DiscoverAsync()
        {
            var services = await _consulClient.Agent.Services();
            foreach (var service in services.Response)
            {
                var pathInfo = await DiscoverPathsFromSwagger(service.Value);
                _discoveredServices.Add(new ServiceWrapper<AgentService>
                {
                    Service = service.Value,
                    Meta = pathInfo
                });
            }
        }

        private async Task<IEnumerable<ServiceMeta>> DiscoverPathsFromSwagger(AgentService service)
        {
            var swaggerUrl = $"{service.GetServiceUrl()}swagger/v1/swagger.json";
            var resp = await _innerClient.GetAsync(swaggerUrl);
            var content = await resp.Content.ReadAsStringAsync();
            var swaggerJson = JObject.Parse(content);

            //Get paths
            var paths = swaggerJson["paths"] as JObject;
            var pathInfo = paths.Properties().Select(p =>
             {
                 IEnumerable<JProperty> enumerable = (p.Value as JObject).Properties();
                 var verbs = enumerable.Select(pv => pv.Name);
                 return new ServiceMeta(p.Name, verbs);
             }).ToList();

            return pathInfo;
        }

        public async Task<string> Invoke(ServiceDiscoveryAttribute serviceAttr)
        {
            if (_discoveredServices.Any())
            {
                var service = _discoveredServices.Where(svc =>
                                     svc.Meta.Any(meta => meta.Path.Equals(serviceAttr.Endpoint, StringComparison.OrdinalIgnoreCase)
                                                 && meta.Verbs.Contains(serviceAttr.HttpVerb, StringComparer.OrdinalIgnoreCase)));

                var serviceUrl = service.First().Service.GetServiceUrl();
                if (serviceAttr.HttpVerb == "GET")
                {
                    var resp = await _innerClient.GetAsync(new Uri(serviceUrl, serviceAttr.Endpoint));
                    var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return content;
                }
            }
            return null;
        }
    }
}