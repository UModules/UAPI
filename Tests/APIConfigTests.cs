using NUnit.Framework;
using System.Collections.Generic;
using UAPIModule.Assets;
using UAPIModule.SharedTypes;
using UnityEngine;

namespace UAPIModule.Tests
{
    public class APIConfigTests
    {
        [Test]
        public void CreateConfigData_ValidValues_ReturnsCorrectConfigData()
        {
            // Arrange
            var baseURLConfig = ScriptableObject.CreateInstance<BaseURLConfig>();
            baseURLConfig.GetType().GetProperty("BaseURL").SetValue(baseURLConfig, "https://example.com");

            var headers = ScriptableObject.CreateInstance<HttpRequestParams>();
            headers.GetType().GetProperty("Parameters").SetValue(headers, new List<HttpRequestParams.KeyValueItem>
            {
                new() { key = "Content-Type", value = "application/json" }
            });

            var apiConfig = ScriptableObject.CreateInstance<APIConfig>();
            apiConfig.GetType().GetProperty("BaseURLConfig").SetValue(apiConfig, baseURLConfig);
            apiConfig.GetType().GetProperty("Endpoint").SetValue(apiConfig, "/test");
            apiConfig.GetType().GetProperty("MethodType").SetValue(apiConfig, HTTPRequestMethod.GET);
            apiConfig.GetType().GetProperty("Headers").SetValue(apiConfig, headers);
            apiConfig.GetType().GetProperty("NeedsAuthHeader").SetValue(apiConfig, true);
            apiConfig.GetType().GetProperty("Timeout").SetValue(apiConfig, 15000);
            apiConfig.GetType().GetProperty("UseBearerPrefix").SetValue(apiConfig, false);

            // Act
            var configData = apiConfig.CreateConfigData();

            // Assert
            Assert.AreEqual(baseURLConfig, configData.BaseURLConfig);
            Assert.AreEqual("/test", configData.Endpoint);
            Assert.AreEqual(HTTPRequestMethod.GET, configData.MethodType);
            Assert.AreEqual(headers, configData.Headers);
            Assert.AreEqual(true, configData.NeedsAuthHeader);
            Assert.AreEqual(15000, configData.Timeout);
            Assert.AreEqual(false, configData.UseBearerPrefix);
        }
    }
}
