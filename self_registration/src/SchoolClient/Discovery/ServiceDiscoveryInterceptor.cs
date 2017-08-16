using System.Linq;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace SchoolClient
{
    public class ServiceDiscoveryInterceptor : IInterceptor
    {
        private readonly IServiceDiscoveryClient _discoveryClient;
        public ServiceDiscoveryInterceptor(IServiceDiscoveryClient discoveryClient)
        {
            _discoveryClient = discoveryClient;
            _discoveryClient.DiscoverAsync().GetAwaiter().GetResult();
        }
        public void Intercept(IInvocation invocation)
        {
            var serviceAttr = invocation.Method
                            .GetCustomAttributes(false)
                            .OfType<ServiceDiscoveryAttribute>()
                            .SingleOrDefault();

            if (serviceAttr != null)
            {
                //TODO: fix async/Task handling
                var result = _discoveryClient.Invoke(serviceAttr).GetAwaiter().GetResult();

                var returnedValue =  JsonConvert.DeserializeObject(result, invocation.Method.ReturnType);
                invocation.ReturnValue = returnedValue;
            }
        }
    }
}