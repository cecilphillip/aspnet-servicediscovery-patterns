using System;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace SchoolAPI.Infrastructure
{
    public static class Extensions
    {
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            try
            {
                var features = app.Properties["server.Features"] as FeatureCollection;
                var addresses = features.Get<IServerAddressesFeature>();
                var address = addresses.Addresses.First();

                var uri = new Uri(address);
                var registration = new AgentServiceRegistration()
                {
                    ID = $"{consulConfig.Value.ServiceID}-{uri.Port}",
                    Name = consulConfig.Value.ServiceName,
                    Address = $"{uri.Scheme}://{uri.Host}",
                    Port = uri.Port,
                    Tags = new[] { "Students", "Courses", "School" }
                };

                logger.LogInformation("Registering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() => {
                    logger.LogInformation("Deregistering from Consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();                    
                });
            }
            catch (Exception x)
            {
                logger.LogCritical(x.ToString());
            }

            return app;
        }      
    }
}
