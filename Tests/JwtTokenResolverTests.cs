using NUnit.Framework;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule.Tests
{
    public class JwtTokenResolverTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteAll();
            JwtTokenResolver.SetUseBearerPrefix(true); // Reset to default state
        }

        [Test]
        public void AccessToken_SetAndGet_ReturnsCorrectToken()
        {
            // Arrange
            var token = "testAccessToken";

            // Act
            JwtTokenResolver.SetAccessToken(token);
            var retrievedToken = JwtTokenResolver.AccessToken;

            // Assert
            Assert.AreEqual(token, retrievedToken);
        }

        [Test]
        public void RefreshToken_SetAndGet_ReturnsCorrectToken()
        {
            // Arrange
            var token = "testRefreshToken";

            // Act
            JwtTokenResolver.SetRefreshToken(token);
            var retrievedToken = JwtTokenResolver.RefreshToken;

            // Assert
            Assert.AreEqual(token, retrievedToken);
        }

        [Test]
        public void AccessTokenHeader_UseBearerPrefix_ReturnsBearerToken()
        {
            // Arrange
            var token = "testAccessToken";
            JwtTokenResolver.SetAccessToken(token);

            // Act
            var authHeader = JwtTokenResolver.AccessTokenHeader;

            // Assert
            Assert.AreEqual(JwtTokenResolver.AUTHORIZATION_HEADER_KEY, authHeader.Key);
            Assert.AreEqual("Bearer " + token, authHeader.Value);
        }

        [Test]
        public void AccessTokenHeader_DoNotUseBearerPrefix_ReturnsTokenWithoutBearer()
        {
            // Arrange
            var token = "testAccessToken";
            JwtTokenResolver.SetAccessToken(token);
            JwtTokenResolver.SetUseBearerPrefix(false);

            // Act
            var authHeader = JwtTokenResolver.AccessTokenHeader;

            // Assert
            Assert.AreEqual(JwtTokenResolver.AUTHORIZATION_HEADER_KEY, authHeader.Key);
            Assert.AreEqual(token, authHeader.Value);
        }

        [Test]
        public void RemoveTokens_RemovesBothAccessTokenAndRefreshToken()
        {
            // Arrange
            JwtTokenResolver.SetAccessToken("accessToken");
            JwtTokenResolver.SetRefreshToken("refreshToken");

            // Act
            JwtTokenResolver.RemoveTokens();

            // Assert
            Assert.IsEmpty(JwtTokenResolver.AccessToken);
            Assert.IsEmpty(JwtTokenResolver.RefreshToken);
        }

        [Test]
        public void RemoveAccessToken_RemovesOnlyAccessToken()
        {
            // Arrange
            JwtTokenResolver.SetAccessToken("accessToken");
            JwtTokenResolver.SetRefreshToken("refreshToken");

            // Act
            JwtTokenResolver.RemoveAccessToken();

            // Assert
            Assert.IsEmpty(JwtTokenResolver.AccessToken);
            Assert.AreEqual("refreshToken", JwtTokenResolver.RefreshToken);
        }
    }
}
