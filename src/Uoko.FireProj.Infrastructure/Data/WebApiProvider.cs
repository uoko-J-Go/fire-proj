using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Uoko.FireProj.Infrastructure.Data
{
    /// <summary>
    /// WebAPI访问支持
    /// </summary>
    public class WebApiProvider
    {
        private HttpClient _client;
        public WebApiProvider()
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            _client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
        }
        public WebApiProvider(string baseAddress)
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            _client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress), Timeout = TimeSpan.FromSeconds(60) };
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T dto)
        {
            return await _client.PostAsJsonAsync(requestUrl, dto).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            return await _client.GetAsync(requestUrl).ConfigureAwait(false);
        }

        public TResponse Post<TRequest, TResponse>(string requestUrl, TRequest dto)
        {
            var result = PostAsync(requestUrl, dto).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<TResponse>().Result;
        }
        public TResponse Get<TResponse>(string requestUrl)
        {
            var result = GetAsync(requestUrl).Result;
            result.EnsureSuccessStatusCode();
            return result.Content.ReadAsAsync<TResponse>().Result;
        }
    }
}