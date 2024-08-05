using UAPIModule.SharedTypes;
using UnityEngine;

namespace UAPIModule.Assets
{
    [CreateAssetMenu(fileName = nameof(APIConfig), menuName = "UAPIModule/" + nameof(APIConfig), order = 1)]
    public class APIConfig : ScriptableObject
    {
        [field: SerializeField]
        public BaseURLConfig BaseURLConfig { get; private set; }
        [SerializeField, Tooltip("The base URL configuration for the API request.")]
        private BaseURLConfig baseURLConfig;

        [field: SerializeField]
        public string Endpoint { get; private set; }
        [SerializeField, Tooltip("The endpoint of the API request.")]
        private string endpoint;

        [field: SerializeField]
        public HTTPRequestMethod MethodType { get; private set; }
        [SerializeField, Tooltip("The HTTP method type (GET, POST, PUT, etc.) for the API request.")]
        private HTTPRequestMethod methodType;

        [field: SerializeField, Tooltip("The headers for the API request.")]
        public HttpRequestParams Headers { get; private set; }
        [SerializeField, Tooltip("The headers for the API request.")]
        private HttpRequestParams headers;

        [field: SerializeField]
        public bool NeedsAuthHeader { get; private set; }
        [SerializeField, Tooltip("Indicates whether the API request needs an authorization header.")] 
        private bool needsAuthHeader;

        [field: SerializeField]
        public int Timeout { get; private set; } = 10000;
        [SerializeField, Tooltip("The timeout duration for the API request in milliseconds.")] 
        private int timeout = 1000;

        [field: SerializeField]
        public bool UseBearerPrefix { get; private set; } = true;
        [SerializeField, Tooltip("Indicates whether to use the 'Bearer' prefix in the authorization header.")] 
        private bool useBearerPrefix;

        public APIConfigData Get() =>
            new(BaseURLConfig, Endpoint, MethodType, headers, needsAuthHeader, timeout, useBearerPrefix);
    }
}
