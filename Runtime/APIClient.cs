using Cysharp.Threading.Tasks;
using UAPIModule.SharedTypes;

namespace UAPIModule
{
    public static class APIClient
    {
        private static readonly RequestSender requestSender;

        static APIClient()
        {
            requestSender = RequestSender.Instance;
        }

        public static async UniTask<NetworkResponse> SendRequest(APIRequestConfig config, RequestScreenConfig screenConfig)
        {
            return await requestSender.SendRequest(config, screenConfig);
        }

        public static async UniTask<NetworkResponse<K>> SendRequest<K>(APIRequestConfig config, RequestScreenConfig screenConfig) where K : class
        {
            return await requestSender.SendRequest<K>(config, screenConfig);
        }
    }
}
