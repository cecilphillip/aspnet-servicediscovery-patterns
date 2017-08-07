using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using SchoolClient.Models;

namespace SchoolClient
{
    public class ApiClient
    {
        private readonly List<Uri> _serverUrls;
        private readonly IConfigurationRoot _configuration;
        private readonly HttpClient _apiClient;
        private readonly RetryPolicy _serverRetryPolicy;
        private int _currentConfigIndex;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(IConfigurationRoot configuration, ILogger<ApiClient> logger)
        {
            _logger = logger;
            _configuration = configuration;

            _apiClient = new HttpClient();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _serverUrls = new List<Uri>();

            var consulClient = new ConsulClient(c =>
            {
                var uri = new Uri(_configuration["consulConfig:address"]);
                c.Address = uri;
            });

            _logger.LogInformation("Discovering Services from Consul.");
            var services = consulClient.Agent.Services().Result.Response;
            foreach (var service in services)
            {
                var isSchoolApi = service.Value.Tags.Any(t => t == "School") && service.Value.Tags.Any(t => t == "Students");
                if (isSchoolApi)
                {
                    var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                    _serverUrls.Add(serviceUri);
                }
            }
            _logger.LogInformation($"{_serverUrls.Count} endpoints found.");

            var retries = _serverUrls.Count * 2 - 1;
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
                _logger.LogWarning("Trying next server... \n");
                _currentConfigIndex++;

                if (_currentConfigIndex > _serverUrls.Count - 1)
                    _currentConfigIndex = 0;
            }
        }

        public virtual Task<IEnumerable<Student>> GetStudents()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
                {
                    var serverUrl = _serverUrls[_currentConfigIndex];
                    var requestPath = $"{serverUrl}api/students";

                    _logger.LogInformation($"Making request to {requestPath}");
                    var response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonConvert.DeserializeObject<IEnumerable<Student>>(content);
                });
        }

        public virtual Task<IEnumerable<Course>> GetCourses()
        {
            return _serverRetryPolicy.ExecuteAsync(async () =>
            {
                var serverUrl = _serverUrls[_currentConfigIndex];
                var requestPath = $"{serverUrl}api/courses";

                _logger.LogInformation($"Making request to {serverUrl}");
                var response = await _apiClient.GetAsync(requestPath).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return JsonConvert.DeserializeObject<IEnumerable<Course>>(content);
            });
        }
    }
}
