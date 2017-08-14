using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SchoolAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            var loggingFactory = host.Services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggingFactory.CreateLogger(nameof(Program));
            logger.LogInformation($"Process ID: {Process.GetCurrentProcess().Id}");

            host.Run();
        }
    }
}
