using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UAPIModule.SharedTypes;
using UnityEngine.TestTools;

namespace UAPIModule.Test
{
    [TestFixture]
    public class GetTests
    {
        [UnityTest]
        public IEnumerator SendRequest_RequestAndReceiveCallbackFromAPI()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var baseURL = "https://httpbin.org";
                var endpoint = "/get";
                var headers = new Dictionary<string, string>();
                var bodies = new Dictionary<string, object>();
                var screenConfig = RequestScreenConfig.GetNoScreen();

                // Act
                var config = APIRequestConfig.GetWithoutToken(
                    baseURL: baseURL,
                    endpoint: endpoint,
                    methodType: HTTPRequestMethod.GET,
                    headers: headers,
                    bodies: bodies,
                    timeout: 10000
                );

                var receivedResponse = await APIClient.SendRequest(config, screenConfig);

                // Assert
                Assert.NotNull(receivedResponse, "The response passed to the callback is null.");
                Assert.IsTrue(receivedResponse.isSuccessful, "The response indicates the request was not successful.");
                Assert.AreEqual(200, receivedResponse.statusCode, "The status code is incorrect.");
                Assert.IsNull(receivedResponse.errorMessage, "The error message should be null.");
            });
        }

        [UnityTest]
        public IEnumerator SendRequest_RequestAndReceiveGenericCallbackFromAPI()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var baseURL = "https://httpbin.org";
                var endpoint = "/get";
                var headers = new Dictionary<string, string>();
                var bodies = new Dictionary<string, object>();
                var screenConfig = RequestScreenConfig.GetNoScreen();

                // Act
                var config = APIRequestConfig.GetWithoutToken(
                    baseURL: baseURL,
                    endpoint: endpoint,
                    methodType: HTTPRequestMethod.GET,
                    headers: headers,
                    bodies: bodies,
                    timeout: 10000
                );

                var receivedResponse = await APIClient.SendRequest<GetResponse>(config, screenConfig);

                // Assert
                Assert.NotNull(receivedResponse, "The response passed to the callback is null.");
                Assert.IsTrue(receivedResponse.isSuccessful, "The response indicates the request was not successful.");
                Assert.AreEqual(200, receivedResponse.statusCode, "The status code is incorrect.");
                Assert.IsNull(receivedResponse.errorMessage, "The error message should be null.");
                Assert.NotNull(receivedResponse.data, "The response data should not be null.");
            });
        }

        [Serializable]
        public class GetResponse
        {
            public string origin;
            public string url;
        }
    }
}
