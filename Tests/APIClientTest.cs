using Cysharp.Threading.Tasks;
using System.Threading.Tasks; // Add this to use Task
using NUnit.Framework;
using UAPIModule.Abstraction;
using UAPIModule.Assets;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule.Tests
{
    public class APIClientTest
    {
        private const string RequestKey = "TestRequest";
        private APIConfigData apiConfigData;
        private RequestFeedbackConfig feedbackConfig;
        private RequestSendConfig sendConfig;
        private INetworkScreen mockNetworkScreen;

        [SetUp]
        public void SetUp()
        {
            // Create mock APIConfigData
            var baseURLConfig = ScriptableObject.CreateInstance<BaseURLConfig>();
            baseURLConfig.name = nameof(BaseURLConfig); // Assign a name for debugging
            baseURLConfig.SetBaseURL("https://jsonplaceholder.typicode.com"); // Initialize using a setter method

            // Initialize APIConfigData
            apiConfigData = new APIConfigData(
                baseURLConfig,
                "/posts", // Example endpoint
                HTTPRequestMethod.GET,
                null, // No headers for this test
                false, // No auth header
                5000, // Timeout in milliseconds
                false // No Bearer prefix
            );

            // Initialize feedback and send config
            feedbackConfig = RequestFeedbackConfig.NoFeedback; // No UI feedback during tests
            sendConfig = new RequestSendConfig(); // Empty sendConfig as we are doing GET requests

            // Mock INetworkScreen for testing
            mockNetworkScreen = NetworkLoadingHandlerCreator.CreateAndGet();

            // Create request sender
            APIClient.CreateRequest(RequestKey, mockNetworkScreen);
        }

        [Test]
        public async Task TestSendRequest()
        {
            // Test the SendRequest method of APIClient
            NetworkResponse response = await APIClient.SendRequest(RequestKey, apiConfigData, feedbackConfig, sendConfig);

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.isSuccessful, "Request should be successful.");
            Assert.AreEqual((long)HTTPResponseCodes.OK_200, response.statusCode, "Response should have status code 200.");
            Debug.Log($"Response Status: {response.statusCode}, Is Successful: {response.isSuccessful}");
        }

        [Test]
        public async Task TestCreateAndSendRequest()
        {
            // Test the CreateAndSendRequest method of APIClient
            NetworkResponse response = await APIClient.CreateAndSendRequest(RequestKey, mockNetworkScreen, apiConfigData, feedbackConfig, sendConfig);

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.isSuccessful, "Request should be successful.");
            Assert.AreEqual((long)HTTPResponseCodes.OK_200, response.statusCode, "Response should have status code 200.");
            Debug.Log($"Response Status: {response.statusCode}, Is Successful: {response.isSuccessful}");
        }

        [Test]
        public async Task TestSendTypedRequest()
        {
            // Test the generic SendRequest method with a typed response (for deserialization)
            NetworkResponse<ExampleResponse> response = await APIClient.SendRequest<ExampleResponse>(RequestKey, apiConfigData, feedbackConfig, sendConfig);

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.isSuccessful, "Request should be successful.");
            Assert.IsNotNull(response.data, "Response data should not be null.");
            Debug.Log($"Response Data: {JsonUtility.ToJson(response.data)}");
        }

        // Example response class for the typed test
        private class ExampleResponse
        {
            public int userId;
            public int id;
            public string title;
            public string body;
        }
    }
}