using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CentralPG.Extensions;

    public static class HttpClientExtension
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Ann Error occoured: {response.ReasonPhrase}");
            }
            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(dataAsString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

