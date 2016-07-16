using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using SchoolClient.Models;

namespace SchoolClient
{
    public class Config
    {
        public string BaseUrl { get; set; }
        public string StudentResource { get; set; }
        public string CoursesResource { get; set; }
    }

    public class ApiClient
    {
        const string API_CONFIG_SECTION = "school-api";

        private readonly List<Config> _serverConfigs;       
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _apiClient;
        private readonly RetryPolicy _serverRetryPolicy;
        private int _currentConfigIndex = 0;

        public ApiClient(IConfigurationRoot configuration)
        {
            _configuration = configuration;

            _apiClient = new HttpClient();
            _serverConfigs = new List<Config>();
            configuration.GetSection(API_CONFIG_SECTION).Bind(_serverConfigs);
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //TODO: Validate server configs

            var retries = _serverConfigs.Count() * 2;
            _serverRetryPolicy = Policy.Handle<HttpRequestException>()
               .RetryAsync(retries, (exception, retryCount) =>
               {
                   ChooseNextServer(retryCount);
               });
        }

        private void ChooseNextServer(int retryCount)
        {
            if (retryCount % 2 == 0)
            {
                Console.WriteLine("trying next server... \n");
                _currentConfigIndex++;
            }
        }

        public virtual Task<IEnumerable<Student>> GetStudents()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
                {
                    var config = _serverConfigs[_currentConfigIndex];
                    var response = await _apiClient.GetAsync(config.BaseUrl + config.StudentResource).ConfigureAwait(false);
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<IEnumerable<Student>>(content);
                });
        }

        public virtual Task<IEnumerable<Course>> GetCourses()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var config = _serverConfigs[_currentConfigIndex];
                var response = await _apiClient.GetAsync(config.BaseUrl + config.CoursesResource).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Course>>(content);
            });
        }
    }
}
