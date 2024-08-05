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

    public abstract class RequestSenderBase
    {
        protected static readonly HttpClient httpClient;
        internal readonly RequestLogger requestLogger = new();
        protected readonly INetworkLoadingHandler loadingHandler;

        static RequestSenderBase()
        {
            httpClient = new HttpClient();
        }

        protected RequestSenderBase(INetworkLoadingHandler loadingHandler)
        {
            this.loadingHandler = loadingHandler ?? throw new ArgumentNullException(nameof(loadingHandler));
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

            // Show loading if needed
            if (feedbackConfig.ShowLoading)
            {
                loadingHandler?.ShowLoading();
            }

            try
            {
                return await httpClient.SendAsync(requestMessage, cancellationToken);
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
                // Hide loading if needed
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

        protected async UniTask<bool> RefreshAccessToken()
        {
            try
            {
                string refreshToken = JwtTokenResolver.RefreshToken;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    Debug.LogError("Refresh token is null or empty");
                    return false;
                }

                var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "YOUR_REFRESH_TOKEN_URL");
                var refreshHeader = JwtTokenResolver.RefreshTokenHeader;
                refreshRequest.Headers.Add(refreshHeader.Key, refreshHeader.Value);

                var response = await httpClient.SendAsync(refreshRequest);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonUtility.FromJson<TokenResponse>(responseBody);
                    JwtTokenResolver.SetAccessToken(tokenResponse.AccessToken);
                    JwtTokenResolver.SetRefreshToken(tokenResponse.RefreshToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error refreshing token: {ex.Message}");
            }

            return false;
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
