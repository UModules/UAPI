using Cysharp.Threading.Tasks;
using System;
using System.Net.Http;
using System.Threading;
using UAPIModule.Abstraction;
using UAPIModule.Assets;
using UAPIModule.SharedTypes;

namespace UAPIModule
{
    public abstract class RequestSender : RequestSenderBase
    {
        protected RequestSender(INetworkLoadingHandler loadingHandler) : base(loadingHandler)
        {
        }

        public async UniTask<NetworkResponse> SendRequest(APIConfig config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig)
        {
            return await SendRequest(config.CreateConfigData(), feedbackConfig, sendConfig);
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

                return networkResponse;
            }
            catch (TimeoutException e)
            {
                return HandleError((long)HTTPResponseCodes.REQUEST_TIMEOUT_408, e.Message, config.BaseURLConfig.BaseURL + config.Endpoint);
            }
            catch (HttpRequestException e)
            {
                return HandleError((long)HTTPResponseCodes.SERVER_ERROR_500, GetErrorMessage(e), config.BaseURLConfig.BaseURL + config.Endpoint);
            }
            catch (Exception e)
            {
                return HandleError((long)HTTPResponseCodes.SERVER_ERROR_500, e.Message, config.BaseURLConfig.BaseURL + config.Endpoint);
            }
        }

        private NetworkResponse HandleError(long statusCode, string errorMessage, string url)
        {
            var errorResponse = new NetworkResponse
            {
                isSuccessful = false,
                statusCode = statusCode,
                errorMessage = errorMessage
            };

            requestLogger.LogResponse(errorResponse, url);
            return errorResponse;
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
