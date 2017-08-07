using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private const string API_CONFIG_SECTION = "school-api";

        private readonly List<Config> _serverConfigs;
        private readonly HttpClient _apiClient;
        private readonly RetryPolicy _serverRetryPolicy;
        private int _currentConfigIndex;
        private ILogger<ApiClient> _logger;

        public ApiClient(IConfigurationRoot configuration, ILogger<ApiClient> logger)
        {
            _apiClient = new HttpClient();
            _logger = logger;

            _serverConfigs = new List<Config>();
            configuration.GetSection(API_CONFIG_SECTION).Bind(_serverConfigs);

            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var retries = _serverConfigs.Count() * 2 - 1;
            _logger.LogInformation($"Retry count set to {retries}");
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
                _logger.LogWarning("Trying Next Server... \n");
                _currentConfigIndex++;

                if (_currentConfigIndex > _serverConfigs.Count - 1)
                    _currentConfigIndex = 0;
            }
        }

        public virtual Task<IEnumerable<Student>> GetStudents()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
                {
                    var config = _serverConfigs[_currentConfigIndex];
                    var requestPath = $"{config.BaseUrl}{config.StudentResource}";

                    _logger.LogInformation($"Making request to {requestPath}");

                    var response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                    var students = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<IEnumerable<Student>>(students);
                });
        }

        public virtual Task<IEnumerable<Course>> GetCourses()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var config = _serverConfigs[_currentConfigIndex];
                var requestPath = $"{config.BaseUrl}{config.CoursesResource}";

                _logger.LogInformation($"Making request to {requestPath}");

                var response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                var courses = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<IEnumerable<Course>>(courses);
            });
        }
    }
}
