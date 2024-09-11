# UAPI
# Description
UAPI is a C# library developed by UModules aimed at simplifying API development and integration. It provides a set of tools and utilities to streamline the process of creating, managing, and consuming APIs, making it easier for developers to build robust and scalable applications.

# Features
* **Easy Integration:** Simplifies the integration of APIs into your applications.
* **Comprehensive Documentation:** Detailed documentation to help you get started quickly.
* **Modular Architecture:** Highly modular design allows for easy customization and extension.
* **Robust Testing:** Includes a suite of tests to ensure reliability and stability.

## Installation
### Step 1: Install UniTask
UAPI depends on UniTask, which needs to be installed before adding UAPI. Follow these steps:
1. Open Unity and go to Window -> Package Manager.
2. Press the + button and select Add package from git URL....
3. Enter the following URL and press Add:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Step 2: Install UAPI
**Option 1:** Install via Git URL
1. After installing UniTask, open Window -> Package Manager in Unity.
2. Press the + button and choose Add package from git URL....
3. Enter the following URL to install UAPI:
```
https://github.com/UModules/UAPI.gitupm
```

**Option 2:** Install via Manifest File
Alternatively, you can install UAPI by editing your manifest.json file located in the Packages folder of your Unity project.
Add the following line to your dependencies:
```
"com.umodules.uapi": "https://github.com/UModules/UAPI.git#upm"
```

**Option 3:** Manual Installation
1. Clone the UAPI repo or download the latest release from GitHub.
2. Add the UAPI folder to your Unity project manually or import the .unitypackage file.

# Usage
### Sample Usage
This example demonstrates how to create an API manager and send a request:
```C#
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
```

# Documentation
For detailed documentation, please refer to the [UAPI Documentation](https://github.com/UModules/UAPI/wiki).

# License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/UModules/UAPI/wiki/LICENSE) file for details.
