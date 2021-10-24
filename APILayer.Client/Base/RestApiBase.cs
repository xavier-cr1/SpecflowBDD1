﻿using APILayer.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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

        public RestApiBase(IConfigurationRoot configurationRoot, ISpecFlowOutputHelper specFlowOutputHelper)
        {
            this.ConfigurationRoot = configurationRoot;
            this._specFlowOutputHelper = specFlowOutputHelper;
        }

        /// <returns>The <see cref="Task{TResult}"/></returns>
        protected async Task<SwaggerResponse<T>> CreateGenericSwaggerResponse<T>(HttpResponseMessage response) where T : class
        {
            // try deserializer json schema validator?
            var status = ((int)response.StatusCode).ToString();
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            T result = JsonConvert.DeserializeObject<T>(responseData);
            return new SwaggerResponse<T>(status, result);
        }

        protected async Task<SwaggerResponse> CreateSwaggerResponse(HttpResponseMessage response)
        {
            var status = ((int)response.StatusCode).ToString();

            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new SwaggerResponse(status, responseData);
        }

        /// <summary>
        /// Get request for http client.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.RelativeOrAbsolute));

            var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Post request for http client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="item">The item.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T item)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.RelativeOrAbsolute));
            requestMessage.Content = content;

            var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Post request for http client without content.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <returns>The <see cref="Task{TResult}"/></returns>
        public async Task<HttpResponseMessage> PostAsync(string url, string authorizationToken = null)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.RelativeOrAbsolute));

            var response = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None).ConfigureAwait(false);

            return response;
        }
    }
}
