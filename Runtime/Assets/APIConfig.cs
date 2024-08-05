using UAPIModule.SharedTypes;
using UnityEngine;

namespace UAPIModule.Assets
{
    [CreateAssetMenu(fileName = nameof(APIConfig), menuName = "UAPIModule/" + nameof(APIConfig), order = 1)]
    public class APIConfig : ScriptableObject
    {
        [field: SerializeField, Tooltip("The base URL configuration for the API request.")]
        public BaseURLConfig BaseURLConfig { get; private set; }

        [field: SerializeField, Tooltip("The endpoint of the API request.")]
        public string Endpoint { get; private set; }

        [field: SerializeField, Tooltip("The HTTP method type (GET, POST, PUT, etc.) for the API request.")]
        public HTTPRequestMethod MethodType { get; private set; }

        [field: SerializeField, Tooltip("The headers for the API request.")]
        public HttpRequestParams Headers { get; private set; }

        [field: SerializeField, Tooltip("Indicates whether the API request needs an authorization header.")]
        public bool NeedsAuthHeader { get; private set; }

        [field: SerializeField, Tooltip("The timeout duration for the API request in milliseconds.")]
        public int Timeout { get; private set; } = 10000;

        [field: SerializeField, Tooltip("Indicates whether to use the 'Bearer' prefix in the authorization header.")]
        public bool UseBearerPrefix { get; private set; } = true;

        public APIConfigData CreateConfigData() =>
            new(BaseURLConfig, Endpoint, MethodType, Headers, NeedsAuthHeader, Timeout, UseBearerPrefix);
    }
}
