using UAPIModule.Assets;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule.Sample
{
    public class APITest : MonoBehaviour
    {
        private const string API_KEY = "TEST";

        public APIConfig apiConfig;

        private void Awake()
        {
            APIClient.CreateRequest(API_KEY, NetworkLoadingHandlerCreator.CreateAndGet());
        }

        private async void Start()
        {
            RequestFeedbackConfig feedbackConfig = RequestFeedbackConfig.InitializationFeedback;

            NetworkResponse response = await APIClient.SendRequest(API_KEY, apiConfig.Get(), feedbackConfig, null);
            if (response.isSuccessful)
            {
                Debug.Log(response.ToString());
            }
            else
            {
                Debug.LogError($"Request failed: {response.errorMessage}");
            }
        }
    }
}
