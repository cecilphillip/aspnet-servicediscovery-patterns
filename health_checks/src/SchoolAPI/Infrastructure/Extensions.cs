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

namespace SchoolAPI.Infrastructure
{
    public static class Extensions
    {
        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app)
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
                    ID = consulConfig.Value.ServiceID,
                    Name = consulConfig.Value.ServiceName,
                    Address = $"{uri.Scheme}://{uri.Host}",
                    Port = uri.Port,
                    Tags = new[] { "Students", "Courses", "School" },
                    Check =new AgentServiceCheck()
                    {
                        HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/api/health/status",
                        Timeout = TimeSpan.FromSeconds(3) ,
                        Interval = TimeSpan.FromSeconds(10)
                    }
                };

                var registrationTask = consulClient.Agent.ServiceDeregister(registration.ID)
                    .Then(() => consulClient.Agent.ServiceRegister(registration));
                   
                registrationTask.Wait();
            }
            catch (Exception x)
            {
                logger.LogCritical(x.ToString());
            }

            return app;
        }

        // Adapted from: http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx
        public static Task Then(this Task first, Func<Task> next)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (next == null) throw new ArgumentNullException(nameof(next));

            var tcs = new TaskCompletionSource<object>();
            first.ContinueWith(t1 =>
            {
                if (first.IsFaulted) tcs.TrySetException(first.Exception?.InnerExceptions);
                else if (first.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var nextTask = next();
                        if (nextTask == null) tcs.TrySetCanceled();
                        else
                            nextTask.ContinueWith(t2 =>
                            {
                                if (nextTask.IsFaulted) tcs.TrySetException(nextTask.Exception.InnerExceptions);
                                else if (nextTask.IsCanceled) tcs.TrySetCanceled();
                                else tcs.TrySetResult(null);
                            }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }
    }
}
