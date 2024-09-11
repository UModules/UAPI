using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UAPIModule.Abstraction;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule
{
    internal class RequestSender
    {
        protected static readonly HttpClient httpClient = new();
        internal readonly RequestLogger requestLogger = new();
        protected readonly INetworkScreen loadingHandler;

        public RequestSender(INetworkScreen loadingHandler)
        {
            this.loadingHandler = loadingHandler ?? throw new ArgumentNullException(nameof(loadingHandler));
        }

        public async UniTask<NetworkResponse<T>> SendRequest<T>(APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig) where T : class
        {
            if (httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var cancellationTokenSource = new CancellationTokenSource(config.Timeout);

            try
            {
                var response = await SendRequestInternal(config, feedbackConfig, sendConfig, cancellationTokenSource.Token);
                string responseBody = await response.Content.ReadAsStringAsync();

                var networkResponse = new NetworkResponse<T>
                {
                    isSuccessful = response.IsSuccessStatusCode,
                    statusCode = (long)response.StatusCode,
                    errorMessage = response.IsSuccessStatusCode ? null : response.ReasonPhrase,
                    data = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(responseBody) : null
                };

                if (!networkResponse.isSuccessful)
                {
                    networkResponse.errorMessage = responseBody;
                }

                requestLogger.LogResponse(networkResponse, config.BaseURLConfig.BaseURL + config.Endpoint);
                ShowResponseMessage(networkResponse);

                return networkResponse;
            }
            catch (Exception e)
            {
                HandleCustomError(e, config);
                return null;
            }
        }

        public async UniTask<NetworkResponse> SendRequest(APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig)
        {
            if (httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var cancellationTokenSource = new CancellationTokenSource(config.Timeout);

            try
            {
                var response = await SendRequestInternal(config, feedbackConfig, sendConfig, cancellationTokenSource.Token);
                string responseBody = await response.Content.ReadAsStringAsync();

                var networkResponse = new NetworkResponse
                {
                    isSuccessful = response.IsSuccessStatusCode,
                    statusCode = (long)response.StatusCode,
                    errorMessage = response.IsSuccessStatusCode ? null : response.ReasonPhrase,
                };

                if (!networkResponse.isSuccessful)
                {
                    networkResponse.errorMessage = responseBody;
                }

                requestLogger.LogResponse(networkResponse, config.BaseURLConfig.BaseURL + config.Endpoint);
                ShowResponseMessage(networkResponse);

                return networkResponse;
            }
            catch (Exception e)
            {
                HandleCustomError(e, config);
                return null;
            }
        }

        protected async UniTask<HttpResponseMessage> SendRequestInternal(APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig, CancellationToken cancellationToken)
        {
            string url = config.BaseURLConfig != null ? config.BaseURLConfig.BaseURL + config.Endpoint : config.Endpoint;
            if (sendConfig.HasPathSuffix)
            {
                url = UrlUtility.Join(url, sendConfig.PathSuffix);
            }

            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(config.MethodType.ToString()), url);
            AddHeaders(requestMessage, config, sendConfig);

            if ((config.MethodType == HTTPRequestMethod.POST || config.MethodType == HTTPRequestMethod.PUT) && sendConfig.HasBody())
            {
                SetRequestBody(requestMessage, config, sendConfig);
            }

            requestLogger.LogRequest(url);

            if (feedbackConfig.ShowLoading)
            {
                loadingHandler?.ShowLoading();
            }

            try
            {
                var response = await httpClient.SendAsync(requestMessage, cancellationToken);
                return response;
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TimeoutException("Request timed out");
                }
                throw;
            }
            finally
            {
                if (feedbackConfig.ShowLoading)
                {
                    loadingHandler?.HideLoading();
                }
            }
        }

        protected void AddHeaders(HttpRequestMessage requestMessage, APIConfigData config, RequestSendConfig sendConfig)
        {
            if (config.Headers != null)
            {
                foreach (var header in config.Headers.Parameters)
                {
                    if (!header.key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        requestMessage.Headers.Add(header.key, header.value);
                    }
                }
            }

            if (sendConfig.RequestHeaders != null)
            {
                foreach (var header in sendConfig.RequestHeaders)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (config.NeedsAuthHeader)
            {
                string authToken = sendConfig.BearerToken ?? JwtTokenResolver.AccessToken;
                if (string.IsNullOrEmpty(authToken))
                {
                    Debug.LogError("Auth token is null or empty");
                    throw new InvalidOperationException("Auth token is null or empty");
                }
                var authHeaderValue = config.UseBearerPrefix ? $"Bearer {authToken}" : authToken;
                requestMessage.Headers.Add(JwtTokenResolver.AUTHORIZATION_HEADER_KEY, authHeaderValue);
            }
        }

        protected void SetRequestBody(HttpRequestMessage requestMessage, APIConfigData config, RequestSendConfig sendConfig)
        {
            if (!string.IsNullOrEmpty(sendConfig.RequestBodyString))
            {
                requestMessage.Content = new StringContent(sendConfig.RequestBodyString);
            }
            else
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(sendConfig.RequestBody));
            }

            var contentTypeHeader = config.Headers.Parameters.FirstOrDefault(h => h.key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
            if (contentTypeHeader != null)
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentTypeHeader.value);
            }
        }

        protected void ShowResponseMessage(NetworkResponse response)
        {
            loadingHandler?.ShowMessage(response);
        }

        protected void HandleCustomError(Exception exception, APIConfigData config)
        {
            string errorMessage;
            long statusCode;

            if (exception is TimeoutException)
            {
                errorMessage = "The request timed out.";
                statusCode = (long)HTTPResponseCodes.REQUEST_TIMEOUT_408;
            }
            else if (exception is HttpRequestException httpRequestException)
            {
                errorMessage = GetErrorMessage(httpRequestException);
                statusCode = (long)HTTPResponseCodes.SERVER_ERROR_500;
            }
            else
            {
                errorMessage = exception.Message;
                statusCode = (long)HTTPResponseCodes.SERVER_ERROR_500;
            }

            var errorResponse = new NetworkResponse
            {
                isSuccessful = false,
                statusCode = statusCode,
                errorMessage = errorMessage
            };

            requestLogger.LogResponse(errorResponse, config.BaseURLConfig.BaseURL + config.Endpoint);
            ShowResponseMessage(errorResponse);
        }

        private string GetErrorMessage(HttpRequestException e)
        {
            string errorResponseBody = e.Message;
            if (e.Data.Contains("ResponseBody"))
            {
                errorResponseBody = e.Data["ResponseBody"].ToString();
            }
            return errorResponseBody;
        }
    }
}
