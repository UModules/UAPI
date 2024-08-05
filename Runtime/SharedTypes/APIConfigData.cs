using UAPIModule.Assets;

namespace UAPIModule.SharedTypes
{
    public struct APIConfigData
    {
        public BaseURLConfig BaseURLConfig { get; private set; }
        public string Endpoint { get; private set; }
        public HTTPRequestMethod MethodType { get; private set; }
        public HttpRequestParams Headers { get; private set; }
        public bool NeedsAuthHeader { get; private set; }
        public int Timeout { get; private set; }
        public bool UseBearerPrefix { get; private set; }

        public APIConfigData(BaseURLConfig baseURLConfig,
                             string endpoint,
                             HTTPRequestMethod methodType,
                             HttpRequestParams headers,
                             bool needsAuthHeader,
                             int timeout,
                             bool useBearerPrefix)
        {
            BaseURLConfig = baseURLConfig;
            Endpoint = endpoint;
            MethodType = methodType;
            Headers = headers;
            NeedsAuthHeader = needsAuthHeader;
            Timeout = timeout;
            UseBearerPrefix = useBearerPrefix;
        }
    }
}
