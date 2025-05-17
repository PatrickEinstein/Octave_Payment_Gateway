

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CentralPG.Data;
using CentralPG.Infrasturcture.Interfaces.Utilities;

namespace CentralPG.Infrastructure.Sevices.Utilities
{
    public class ApiCaller : IApiCaller
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<ApiCaller> logger;
        private readonly DataBaseContext dataBaseContext;

        public ApiCaller(HttpClient httpClient, IConfiguration configuration, ILogger<ApiCaller> logger, DataBaseContext dataBaseContext)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            this.logger = logger;
            this.dataBaseContext = dataBaseContext;
        }
        public async Task<string> POST(HttpContent payload, string apiUrl, string token, IDictionary<string, string> headers)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using HttpClient client = new HttpClient(clientHandler);



                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }

                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }

                HttpResponseMessage response = await client.PostAsync(apiUrl, payload);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error from {apiUrl}: {e.Message}  {e.InnerException.Message} ");
                logger.LogInformation($"Request error from {apiUrl}: {e.Message} <---->  {e.InnerException.Message} ");
                return $"Request error: {e.Message}";
            }
        }
        public async Task<string> GET(string apiUrl, string token, IDictionary<string, string> headers)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using HttpClient client = new HttpClient(clientHandler);



                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                logger.LogInformation($"Request error from {apiUrl}: {e.Message} ");
                return $"Request error: {e.Message}";
            }
        }

        public async Task<string> PUT(HttpContent payload, string apiUrl, string token, IDictionary<string, string> headers)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using HttpClient client = new HttpClient(clientHandler);

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }
                HttpResponseMessage response = await client.PutAsync(apiUrl, payload);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error from {apiUrl}: {e.Message}  {e.InnerException.Message} ");
                logger.LogInformation($"Request error from {apiUrl}: {e.Message} <---->  {e.InnerException.Message} ");
                return $"Request error: {e.Message}";
            }
        }
        public async Task<string> DELETE(string apiUrl, string token, IDictionary<string, string> headers)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using HttpClient client = new HttpClient(clientHandler);



                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                foreach (var tm in headers)
                {
                    client.DefaultRequestHeaders.Add(tm.Key, tm.Value);
                }
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error from {apiUrl}: {e.Message}  {e.InnerException.Message} ");
                logger.LogInformation($"Request error from {apiUrl}: {e.Message} <---->  {e.InnerException.Message} ");
                return $"Request error: {e.Message}";
            }
        }

    }

}
