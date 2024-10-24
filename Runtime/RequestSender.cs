using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule
{
    internal class RequestSender : MonoBehaviour
    {
        protected readonly HttpClient httpClient = new();
        internal readonly RequestLogger requestLogger = new();

        private static RequestSender _instance;

        public static RequestSender Instance
        {
            get
            {
                if (_instance == null)
                {
                    var runnerObject = new GameObject("RequestSender");
                    _instance = runnerObject.AddComponent<RequestSender>();
                    DontDestroyOnLoad(runnerObject);
                }
                return _instance;
            }
        }

        // SendRequest for non-generic type
        public async UniTask<NetworkResponse> SendRequest(APIRequestConfig config, RequestScreenConfig screenConfig)
        {
            if (httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var cancellationTokenSource = new CancellationTokenSource(config.Timeout);

            try
            {
                var response = await SendRequestInternalAsync(config, screenConfig, cancellationTokenSource.Token);

                if (response != null)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var networkResponse = new NetworkResponse
                    {
                        isSuccessful = response.IsSuccessStatusCode,
                        statusCode = (long)response.StatusCode,
                        errorMessage = response.IsSuccessStatusCode ? null : response.ReasonPhrase
                    };

                    if (!networkResponse.isSuccessful)
                    {
                        networkResponse.errorMessage = responseBody;
                    }

                    requestLogger.LogResponse(networkResponse, config.URL);
                    ShowResponseMessage(networkResponse, screenConfig);

                    return networkResponse;
                }

                return null; // In case of failure
            }
            catch (Exception ex)
            {
                HandleCustomError(ex, config, screenConfig);
                throw;
            }
        }

        // SendRequest for generic type
        public async UniTask<NetworkResponse<K>> SendRequest<K>(APIRequestConfig config, RequestScreenConfig screenConfig) where K : class
        {
            if (httpClient == null)
            {
                throw new InvalidOperationException("HttpClient is not initialized.");
            }

            var cancellationTokenSource = new CancellationTokenSource(config.Timeout);

            try
            {
                var response = await SendRequestInternalAsync(config, screenConfig, cancellationTokenSource.Token);

                if (response != null)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var networkResponse = new NetworkResponse<K>
                    {
                        isSuccessful = response.IsSuccessStatusCode,
                        statusCode = (long)response.StatusCode,
                        errorMessage = response.IsSuccessStatusCode ? null : response.ReasonPhrase,
                        data = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<K>(responseBody) : null
                    };

                    if (!networkResponse.isSuccessful)
                    {
                        networkResponse.errorMessage = responseBody;
                    }

                    requestLogger.LogResponse(networkResponse, config.URL);
                    ShowResponseMessage(networkResponse, screenConfig);

                    return networkResponse;
                }

                return null; // In case of failure
            }
            catch (Exception ex)
            {
                HandleCustomError(ex, config, screenConfig);
                throw;
            }
        }

        // Internal method for making the HTTP request
        private async UniTask<HttpResponseMessage> SendRequestInternalAsync(APIRequestConfig config, RequestScreenConfig screenConfig, CancellationToken cancellationToken)
        {
            string url = config.URL;

            HttpRequestMessage requestMessage = new(new HttpMethod(config.MethodType.ToString()), url);
            AddHeaders(requestMessage, config);

            if ((config.MethodType == HTTPRequestMethod.POST
                || config.MethodType == HTTPRequestMethod.PUT
                || config.MethodType == HTTPRequestMethod.PATCH) && config.HasBody)
            {
                SetRequestBody(requestMessage, config);
            }

            requestLogger.LogRequest(url);
            screenConfig.TryShowScreen();

            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken);
                screenConfig.TryHideScreen();
                return response;
            }
            catch (Exception ex)
            {
                HandleCustomError(ex, config, screenConfig);
                throw;
            }
        }

        // Method for adding headers to the HTTP request
        protected void AddHeaders(HttpRequestMessage requestMessage, APIRequestConfig config)
        {
            if (config.HeadersParameters != null)
            {
                foreach (var header in config.HeadersParameters)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            if (config.NeedsAuthHeader)
            {
                string authToken = config.AccessToken;
                if (string.IsNullOrEmpty(authToken))
                {
                    Debug.LogError("Auth token is null or empty");
                    throw new InvalidOperationException("Auth token is null or empty");
                }
                requestMessage.Headers.Add("Authorization", authToken);
            }
        }

        // Method for setting the request body
        protected void SetRequestBody(HttpRequestMessage requestMessage, APIRequestConfig config)
        {
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(config.Bodies));

            var contentTypeHeader = config.HeadersParameters.FirstOrDefault(h => h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)).Value;
            if (contentTypeHeader != null)
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentTypeHeader);
            }
        }

        // Method for showing response message on the screen
        protected void ShowResponseMessage(NetworkResponse response, RequestScreenConfig screenConfig)
        {
            screenConfig.TryShowMessage(response);
        }

        // Custom error handler
        protected void HandleCustomError(Exception exception, APIRequestConfig config, RequestScreenConfig screenConfig)
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

            requestLogger.LogResponse(errorResponse, config.URL);
            ShowResponseMessage(errorResponse, screenConfig);
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
