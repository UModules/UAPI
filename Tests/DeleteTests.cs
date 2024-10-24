using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.TestTools; // Required for IEnumerators and coroutines
using UAPIModule.SharedTypes;
using System.Collections;

namespace UAPIModule.Test
{
    [TestFixture]
    public class DeleteTests
    {
        // Non-generic test
        [UnityTest] // Use UnityTest with IEnumerator to run async code in a coroutine
        public IEnumerator SendRequest_RequestAndReceiveCallbackFromAPI()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var baseURL = "https://httpbin.org";
                var endpoint = "/delete";
                var headers = new Dictionary<string, string>();
                var bodies = new Dictionary<string, object>();
                var screenConfig = RequestScreenConfig.GetNoScreen();

                // Act
                var config = APIRequestConfig.GetWithoutToken(
                    baseURL: baseURL,
                    endpoint: endpoint,
                    methodType: HTTPRequestMethod.DELETE,
                    headers: headers,
                    bodies: bodies,
                    timeout: 10000
                );

                NetworkResponse receivedResponse = await APIClient.SendRequest(config, screenConfig);

                // Assert
                Assert.NotNull(receivedResponse, "The response is null.");
                Assert.IsTrue(receivedResponse.isSuccessful, "The request was not successful.");
                Assert.AreEqual(200, receivedResponse.statusCode, "The status code is incorrect.");
                Assert.IsNull(receivedResponse.errorMessage, "The error message should be null.");
            });
        }

        // Generic test
        [UnityTest] // Use UnityTest with IEnumerator to run async code in a coroutine
        public IEnumerator SendRequest_RequestAndReceiveGenericCallbackFromAPI()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var baseURL = "https://httpbin.org";
                var endpoint = "/delete";
                var headers = new Dictionary<string, string>();
                var bodies = new Dictionary<string, object>();
                var screenConfig = RequestScreenConfig.GetNoScreen();

                // Act
                var config = APIRequestConfig.GetWithoutToken(
                    baseURL: baseURL,
                    endpoint: endpoint,
                    methodType: HTTPRequestMethod.DELETE,
                    headers: headers,
                    bodies: bodies,
                    timeout: 10000
                );

                NetworkResponse<DeleteResponse> receivedResponse = await APIClient.SendRequest<DeleteResponse>(config, screenConfig);

                // Assert
                Assert.NotNull(receivedResponse, "The response is null.");
                Assert.IsTrue(receivedResponse.isSuccessful, "The request was not successful.");
                Assert.AreEqual(200, receivedResponse.statusCode, "The status code is incorrect.");
                Assert.IsNull(receivedResponse.errorMessage, "The error message should be null.");
                Assert.NotNull(receivedResponse.data, "The response data should not be null.");
            });
        }

        [Serializable]
        public class DeleteResponse
        {
            public string origin;
        }
    }
}
