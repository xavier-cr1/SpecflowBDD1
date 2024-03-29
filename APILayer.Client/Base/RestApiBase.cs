﻿using APILayer.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;

namespace APILayer.Client
{
    public abstract class RestApiBase
    {
        protected readonly string JsonMediaType = "application/json";
        protected readonly IConfigurationRoot ConfigurationRoot;
        protected readonly ISpecFlowOutputHelper _specFlowOutputHelper;
        private readonly HttpClient httpClient;

        public RestApiBase(IConfigurationRoot configurationRoot, ISpecFlowOutputHelper specFlowOutputHelper, HttpClient httpClient)
        {
            this.ConfigurationRoot = configurationRoot;
            this._specFlowOutputHelper = specFlowOutputHelper;
            this.httpClient = httpClient;
        }

        /// <returns>The <see cref="Task{TResult}"/></returns>
        protected async Task<SwaggerResponse<T>> CreateGenericSwaggerResponse<T>(HttpResponseMessage response) where T : class
        {
            // try deserializer json schema validator?
            try
            {
                var status = ((int)response.StatusCode).ToString();
                var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                T result = JsonConvert.DeserializeObject<T>(responseData);
                return new SwaggerResponse<T>(status, result);
            }
            catch(Exception ex)
            {
                this._specFlowOutputHelper.WriteLine($"Failed to deserialize into type swagger response: {ex.Message}");
                throw;
            }
        }

        protected async Task<SwaggerResponse> CreateSwaggerResponse(HttpResponseMessage response)
        {
            try
            {
                var status = ((int)response.StatusCode).ToString();

                var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new SwaggerResponse(status, responseData);
            }
            catch (Exception ex)
            {
                this._specFlowOutputHelper.WriteLine($"Failed to return a swagger response : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get request for http client.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="userAutorization">The user autorization if needed.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        protected async Task<HttpResponseMessage> GetAsync(string url, string userAutorization = "")
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.RelativeOrAbsolute));

                if(!string.IsNullOrEmpty(userAutorization))
                {
                    this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", userAutorization);
                }

                this._specFlowOutputHelper.WriteLine($"Sending GET request to url: {url}");
                var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

                return response;
            }
            catch (TimeoutException timeoutEx)
            {
                this._specFlowOutputHelper.WriteLine($"Timeout exception: {timeoutEx.Message} when trying to request a POST in url: {url}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                this._specFlowOutputHelper.WriteLine($"Unhandled exception: {ex.Message} when trying to request a POST in url: {url}");
                throw;
            }
        }

        /// <summary>
        /// Post request for http client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="item">The item.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T item)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, this.JsonMediaType);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.RelativeOrAbsolute))
                {
                    Content = content
                };

                this._specFlowOutputHelper.WriteLine($"Sending POST request to url: {url}");
                var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

                return response;
            }
            catch (TimeoutException timeoutEx)
            {
                this._specFlowOutputHelper.WriteLine($"Timeout exception: {timeoutEx.Message} when trying to request a POST in url: {url}");
                return new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                this._specFlowOutputHelper.WriteLine($"Unhandled exception: {ex.Message} when trying to request a POST in url: {url}");
                throw;
            }
        }

        /// <summary>
        /// Post request for http client without content.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        protected async Task<HttpResponseMessage> PostAsync(string url)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.RelativeOrAbsolute));

            var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

            return response;
        }
    }
}
