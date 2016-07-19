using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SchoolClient
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static ServiceClient _apiClient;
        public static void Main(string[] args)
        {
            LoadConfig();

            _apiClient = new ServiceClient(_configuration);

            ListStudents();
            ListCourses();

            Console.ReadLine();
            Console.ResetColor();
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
