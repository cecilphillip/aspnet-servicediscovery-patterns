using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            Console.ReadLine();
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
            Console.WriteLine($"Student Count: {students.Count()}\n");
        }

        private static async Task ListCourses()
        {
            var courses = await _apiClient.GetCourses();
            Console.WriteLine($"Coursee Count: {courses.Count()}\n");
        }
    }
}
