# UAPI
# Description
UAPI is a C# library developed by UModules aimed at simplifying API development and integration. It provides a set of tools and utilities to streamline the process of creating, managing, and consuming APIs, making it easier for developers to build robust and scalable applications.

# Features
* **Easy Integration:** Simplifies the integration of APIs into your applications.
* **Comprehensive Documentation:** Detailed documentation to help you get started quickly.
* **Modular Architecture:** Highly modular design allows for easy customization and extension.
* **Robust Testing:** Includes a suite of tests to ensure reliability and stability.

## Installation
### Git URL
UAPI supports Unity Package Manager. To install the project as a Git package, follow these steps:
In Unity, open Window -> Package Manager.
Press the + button, choose "Add package from git URL...".
Enter "https://github.com/UModules/UAPI.gitupm" and press Add.

### Unity Package
Alternatively, you can add the code directly to the project:
1. Clone the repo or download the latest release.
2. Add the UAPI folder to your Unity project or import the .unitypackage.

### Manifest File
You can also install via git URL by adding this entry to your manifest.json:

`"com.umodules.uapi": "https://github.com/UModules/UAPI.git#upm"`

# Usage
### Sample Usage
This example demonstrates how to create an API manager and send a request:
namespace UAPIModule.Tests
{
    public class APIManager : RequestSender<string>
    {
        public APIManager(INetworkLoadingHandler loadingHandler) : base(loadingHandler) { }
    }

    public class APITest : MonoBehaviour
    {
        public APIConfig apiConfig;
        private INetworkLoadingHandler loadingHandler;

        private void Awake()
        {
            loadingHandler = NetworkLoadingHandlerCreator.CreateAndGet();
        }

        private async void Start()
        {
            APIManager apiManager = new APIManager(loadingHandler);
            RequestFeedbackConfig feedbackConfig = RequestFeedbackConfig.InitializationFeedback;

            NetworkResponse<string> response = await apiManager.SendRequest(apiConfig, feedbackConfig, null);
            if (response.isSuccessful)
            {
                Debug.Log(response.data);
            }
            else
            {
                Debug.LogError($"Request failed: {response.errorMessage}");
            }
        }
    }
}

# Documentation
For detailed documentation, please refer to the [UAPI Documentation](https://github.com/UModules/UAPI/wiki).

# License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/UModules/UAPI/wiki/LICENSE) file for details.
