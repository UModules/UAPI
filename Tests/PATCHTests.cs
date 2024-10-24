﻿using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UAPIModule.SharedTypes;
using UnityEngine.TestTools;

namespace UAPIModule.Test
{
    [TestFixture]
    public class PATCHTests
    {
        [UnityTest]
        public IEnumerator SendRequest_RequestAndReceivePatchCallbackFromAPI()
        {
            return UniTask.ToCoroutine(async () =>
            {
                // Arrange
                var baseURL = "https://httpbin.org";
                var endpoint = "/patch";
                var headers = new Dictionary<string, string>();
                var bodies = new Dictionary<string, object>
                {
                    { "name", "updated-item" }
                };
                var screenConfig = RequestScreenConfig.GetNoScreen();

                // Act
                var config = APIRequestConfig.GetWithoutToken(
                    baseURL: baseURL,
                    endpoint: endpoint,
                    methodType: HTTPRequestMethod.PATCH,
                    headers: headers,
                    bodies: bodies,
                    timeout: 10000
                );

                var receivedResponse = await APIClient.SendRequest<PatchResponse>(config, screenConfig);

                // Assert
                Assert.NotNull(receivedResponse, "The response passed to the callback is null.");
                Assert.IsTrue(receivedResponse.isSuccessful, "The response indicates the request was not successful.");
                Assert.AreEqual(200, receivedResponse.statusCode, "The status code is incorrect.");
                Assert.IsNull(receivedResponse.errorMessage, "The error message should be null.");
                Assert.NotNull(receivedResponse.data, "The response data should not be null.");
            });
        }

        [Serializable]
        public class PatchResponse
        {
            public Json json;

            [Serializable]
            public class Json
            {
                public string name;
            }
        }
    }
}
