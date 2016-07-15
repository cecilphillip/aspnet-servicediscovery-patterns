using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace SchoolClient
{
    public class Program
    {
        const string API_CONFIG_SECTION = "school-api";
        const string API_CONFIG_NAME_BASEURL = "baseUrl";
        const string API_CONFIG_NAME_STUDENTS_RESOURCE = "students";
        const string API_CONFIG_NAME_COURSES_RESOURCE = "courses";

        private static IConfigurationRoot configuration;
        private static HttpClient apiClient;
        public static void Main(string[] args)
        {
            LoadConfig();
            SetupHttpClient();
            ListStudents();
            ListCourses();

            Console.ReadLine();
            Console.ResetColor();
        }

        private static void SetupHttpClient()
        {
            apiClient = new HttpClient
            {
                BaseAddress = new Uri(configuration.GetSection(API_CONFIG_SECTION)[API_CONFIG_NAME_BASEURL])                
            };

            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            configuration = builder.Build();            
        }

        private static void ListStudents()
        {
            var response = apiClient.GetAsync(API_CONFIG_NAME_STUDENTS_RESOURCE).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);
        }

        private static void ListCourses()
        {
            var response = apiClient.GetAsync(API_CONFIG_NAME_COURSES_RESOURCE).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);
        }
    }
}
