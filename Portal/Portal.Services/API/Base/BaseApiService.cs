using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Portal.Shared.Interfaces;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
namespace Portal.Services.API.Base
{
    public abstract class BaseApiService
    {
        #region Prop
        private readonly IPortal _portal;
        private readonly string _httpClientName;
        protected IHttpClientFactory HttpClientFactory { get; }
        protected ILocalStorageService LocalStorageService { get; init; }
        protected ILogger Logger { get; init; }
        #endregion

        protected BaseApiService(IPortal portal, IHttpClientFactory httpClientFactory, string httpClientName,
            ILocalStorageService localStorageService, ILogger<BaseApiService> logger)
        {
            HttpClientFactory = httpClientFactory;
            _portal = portal;
            _httpClientName = httpClientName;
            LocalStorageService = localStorageService;
            Logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string uri) where T : new()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequestAsync<T>(request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(string uri, object value, HttpContent? content = null)
        {
            content ??= new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = content
            };

            return await SendRequestAsync(request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task PutAsync(string uri, object value, HttpContent? content = null)
        {
            content ??= new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = content
            };

            await SendRequestAsync(request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            await SendRequestAsync(request);
        }

        #region helper methods
        protected async Task SetAuthToken(HttpRequestMessage request)
        {
            // Add jwt auth header if user is logged in and request is to an api url
            var isApiUrl = request.RequestUri != null && !request.RequestUri.IsAbsoluteUri;
            if (!isApiUrl)
                return;

            var authToken = await LocalStorageService.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(authToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        private async Task<T> SendRequestAsync<T>(HttpRequestMessage request) where T : new()
        {
            using var response = await SendRequestAsync(request);
            if (response is not { IsSuccessStatusCode: true })
                return new T();

            return await response.Content.ReadFromJsonAsync<T>() ?? new T();
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            try
            {
                _portal.IsLoading = true;

                if (request == null)
                {
                    throw new NullReferenceException(nameof(request));
                }

                await SetAuthToken(request);

                request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());

#if DEBUG
                Logger.LogWarning("{requestMethod}{requestUri}", request.Method, request.RequestUri);
#endif

                var client = HttpClientFactory.CreateClient(_httpClientName);
                var response = await client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new ApiException(response.StatusCode);
                    case HttpStatusCode.Forbidden:
                        var content = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(content))
                        {
                            content = $"You don't have permission to access [{request.RequestUri?.AbsolutePath}]";
                        }

                        throw new ApiException(response.StatusCode, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(errorContent))
                {
                    errorContent = "An unknown error has occurred!";
                }

                throw new ApiException(response.StatusCode, $"{response.StatusCode} : {errorContent}");
            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("Failed to fetch", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ApiException(HttpStatusCode.BadRequest, "Unable to connect to the Aura server.");
                }

                throw;
            }
            finally
            {
                _portal.IsLoading = false;
            }
        }

#endregion
    }
}
