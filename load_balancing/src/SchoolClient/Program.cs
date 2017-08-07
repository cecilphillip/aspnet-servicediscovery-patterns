using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SchoolClient
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static ServiceClient _apiClient;
        public static void Main(string[] args)
        {
            LoadConfig();

            var logger = new LoggerFactory().AddConsole().CreateLogger<ServiceClient>();
            _apiClient = new ServiceClient(_configuration, logger);

            using (_apiClient)
            {
                try
                {
                    ListStudents();
                    ListCourses();
                }
                catch (Exception)
                {
                    logger.LogError("Unable to request resource");
                }
                Console.ReadLine();
            }
        }

        private static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
        }

        private static void ListStudents()
        {
            var students = _apiClient.GetStudents();
            Console.WriteLine($"{students.Count()}");
        }

        private static void ListCourses()
        {
            var courses = _apiClient.GetCourses();
            Console.WriteLine($"{courses.Count()}");
        }
    }
}
