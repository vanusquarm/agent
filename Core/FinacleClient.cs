using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Configuration; // Make sure to add this namespace

namespace Finacle.Core
{
    public  class FinacleClient
    {
        protected  readonly HttpClient _httpClient;
        private  readonly string _baseUrl;

        public FinacleClient(string _baseUrl)
        {
            // Read base URL from web.config (app.config for console applications)

            // Initialize HttpClient
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };

            // Set default request headers
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

       

        // Optional: Dispose of HttpClient when no longer needed (though it's better to use it for the lifetime of the app)
        public  void Dispose()
        {
            _httpClient?.Dispose();
        }
    }


}

