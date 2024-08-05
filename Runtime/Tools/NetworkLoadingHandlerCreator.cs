using UAPIModule.Abstraction;
using UnityEngine;

namespace UAPIModule.Tools
{
    internal class SimpleLoadingHandler : MonoBehaviour, INetworkLoadingHandler
    {
        public void ShowLoading()
        {
            Debug.Log("Loading started...");
            // Implement loading UI show logic
        }

        public void HideLoading()
        {
            Debug.Log("Loading ended.");
            // Implement loading UI hide logic
        }
    }

    public static class NetworkLoadingHandlerCreator
    {
        public static INetworkLoadingHandler CreateAndGet() =>
            new GameObject("SimpleLoadingHandler").AddComponent<SimpleLoadingHandler>();
    }
}
