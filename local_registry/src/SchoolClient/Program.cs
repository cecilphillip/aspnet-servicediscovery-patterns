using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SchoolClient
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static ApiClient _apiClient;
        public static void Main(string[] args)
        {
            LoadConfig();
            _apiClient = new ApiClient(_configuration);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;

            ListStudents().Wait();
            ListCourses().Wait();

            Console.ResetColor();
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
            Console.WriteLine($"{students.Count()}");
        }

        private static async Task ListCourses()
        {
            var courses = await _apiClient.GetCourses();
            Console.WriteLine($"{courses.Count()}");
        }
    }
}
