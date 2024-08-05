using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UAPIModule.Abstraction;
using UAPIModule.SharedTypes;
using UnityEngine;

namespace UAPIModule
{
    public static class APIClient
    {
        private static readonly Dictionary<string, RequestSender> requestSenders = new();

        public static void CreateRequest(string key, INetworkScreen networkScreen)
        {
            TryCreate(key, networkScreen, out _);
        }

        public static async UniTask<NetworkResponse> SendRequest(string key, APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig)
        {
            if (requestSenders.TryGetValue(key, out var requestSender))
            {
                return await requestSender.SendRequest(config, feedbackConfig, sendConfig);
            }
            throw new KeyNotFoundException($"No request sender found with key '{key}'.");
        }

        public static async UniTask<NetworkResponse> CreateAndSendRequest(string key, INetworkScreen networkScreen, APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig)
        {
            if (!requestSenders.TryGetValue(key, out var requestSender) && !TryCreate(key, networkScreen, out requestSender))
            {
                return null;
            }
            return await requestSender.SendRequest(config, feedbackConfig, sendConfig);
        }

        public static async UniTask<NetworkResponse<K>> SendRequest<K>(string key, APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig) where K : class
        {
            if (requestSenders.TryGetValue(key, out var requestSender))
            {
                return await requestSender.SendRequest<K>(config, feedbackConfig, sendConfig);
            }
            throw new KeyNotFoundException($"No request sender found with key '{key}'.");
        }

        public static async UniTask<NetworkResponse<K>> CreateAndSendRequest<K>(string key, INetworkScreen networkScreen, APIConfigData config, RequestFeedbackConfig feedbackConfig, RequestSendConfig sendConfig) where K : class
        {
            if (!requestSenders.TryGetValue(key, out var requestSender) && !TryCreate(key, networkScreen, out requestSender))
            {
                return null;
            }
            return await requestSender.SendRequest<K>(config, feedbackConfig, sendConfig);
        }

        private static bool TryCreate(string key, INetworkScreen networkScreen, out RequestSender requestSender)
        {
            if (requestSenders.ContainsKey(key))
            {
                Debug.LogError($"RequestSender with key '{key}' already exists.");
                requestSender = null;
                return false;
            }

            requestSender = new RequestSender(networkScreen);
            requestSenders.Add(key, requestSender);
            return true;
        }
    }
}
