using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SchoolClient
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static ApiClient _apiClient;
        public static void Main(string[] args)
        {
            LoadConfig();

            var generator = new ProxyGenerator();
            var proxiedClient =  generator.CreateInterfaceProxyWithoutTarget<IProxiedApiClient>(new ServiceDiscoveryInterceptor(new ServiceDiscoveryClient("http://127.0.0.1:8500")));

            var courses = proxiedClient.GetCourses();
            Console.WriteLine($"Courses Count: {courses.Count()}");
            Console.ReadLine();
        }

        private static void RegularClient()
        {
            var logger = new LoggerFactory().AddConsole().CreateLogger<ApiClient>();
            _apiClient = new ApiClient(_configuration, logger);

            try
            {
                _apiClient.Initialize().Wait();

                ListStudents().Wait();
                ListCourses().Wait();
            }
            catch (Exception)
            {
                logger.LogError("Unable to request resource");
            }

            Console.ResetColor();
        }

        private static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        private static async Task ListStudents()
        {
            var students = await _apiClient.GetStudents();
            Console.WriteLine($"Student Count: {students.Count()}");
        }

        private static async Task ListCourses()
        {
            var courses = await _apiClient.GetCourses();
            Console.WriteLine($"Courses Count: {courses.Count()}");
        }
    }
}
