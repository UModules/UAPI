using UAPIModule.Abstraction;
using UAPIModule.Assets;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule.Tests
{
    public class APIManager : RequestSender<string>
    {
        public APIManager(INetworkLoadingHandler loadingHandler) : base(loadingHandler) { }
    }

    public class APITest : MonoBehaviour
    {
        public APIConfig apiConfig;
        private INetworkLoadingHandler loadingHandler;

        private void Awake()
        {
            loadingHandler = NetworkLoadingHandlerCreator.CreateAndGet();
        }

        private async void Start()
        {
            APIManager apiManager = new APIManager(loadingHandler);
            RequestFeedbackConfig feedbackConfig = RequestFeedbackConfig.InitializationFeedback;

            NetworkResponse<string> response = await apiManager.SendRequest(apiConfig, feedbackConfig, null);
            if (response.isSuccessful)
            {
                Debug.Log(response.data);
            }
            else
            {
                Debug.LogError($"Request failed: {response.errorMessage}");
            }
        }
    }
}
