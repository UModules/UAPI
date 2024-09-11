# UAPI
# Description
UAPI is a C# library developed by [Usef Farahmand](https://github.com/UsefFarahmand) aimed at simplifying API development and integration. It provides a set of tools and utilities to streamline the process of creating, managing, and consuming APIs, making it easier for developers to build robust and scalable applications.

# Features
* **Easy Integration:** Simplifies the integration of APIs into your applications.
* **Comprehensive Documentation:** Detailed documentation to help you get started quickly.
* **Modular Architecture:** Highly modular design allows for easy customization and extension.
* **Robust Testing:** Includes a suite of tests to ensure reliability and stability.

## Installation
### Step 1: Install UniTask
UAPI depends on `UniTask`, which needs to be installed before adding `UAPI`. Follow these steps:
1. Open Unity and go to Window -> Package Manager.
2. Press the `+` button and select **Add package from git URL...**
3. Enter the following URL and press **Add**:
```
https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
```

### Step 2: Install UAPI
**Option 1:** Install via Git URL
1. After installing UniTask, open Window -> Package Manager in Unity.
2. Press the `+` button and choose **Add package from git URL...**
3. Enter the following URL to install UAPI:
```
https://github.com/UModules/UAPI.gitupm
```

**Option 2:** Install via Manifest File
Alternatively, you can install `UAPI` by editing your `manifest.json` file located in the Packages folder of your Unity project.
Add the following line to your dependencies:
```
"com.umodules.uapi": "https://github.com/UModules/UAPI.git#upm"
```

**Option 3:** Manual Installation
1. Clone the UAPI repo or download the latest release from GitHub.
2. Add the UAPI folder to your Unity project manually or import the .unitypackage file.

# Usage
### Step 1: Create an `APIConfig` Scriptable Object
1. In Unity, go to **Assets -> Create -> UAPIModule -> APIConfig** to create a new `APIConfig` Scriptable Object.
2. Fill in the variables within the `APIConfig` asset in the Inspector, which include:
    * **Base URL Configuration:** The base URL for the API.
    * **Endpoint:** The specific endpoint of the API.
    * **Method Type:** The HTTP method (GET, POST, etc.).
    * **Headers:** Optional headers for the API request.
    * **Needs Auth Header:** Whether authorization is required.
    * **Timeout:** The timeout duration (in milliseconds).
    * **Use Bearer Prefix:** Whether to use the 'Bearer' prefix in the authorization header.
### Step 2: Create a Scene and Set Up the APITest Component
1. Create a new Unity scene:
    * In Unity, go to File -> New Scene and save the scene with a relevant name (e.g., "APITestScene").
2. Create an empty GameObject:
    * Right-click in the Hierarchy window and select Create Empty to create an empty GameObject.
    * Name it something like "API Test Object."
3. Create the `APITest` script:
    * With the new GameObject selected, go to the Inspector window.
    * Click **Add Component** and search for `APITest`. Attach this script to the empty GameObject.
4. Serialize the APIConfig object:
    * In the Inspector of the GameObject with the `APITest` script, you’ll see a field for the `APIConfig` object.
    * Drag and drop the `APIConfig` asset you created earlier into this field to serialize it.
### Step 3: Play the Project
1. **Save the scene** if you haven’t already.
2. Press Play in Unity to run the project.
3. The `APITest` script will automatically execute and send a request based on the serialized `APIConfig`. You can monitor the console for any debug messages such as the API response or error messages.

## Sample Usage Code
```C#
using UAPIModule.Assets;
using UAPIModule.SharedTypes;
using UAPIModule.Tools;
using UnityEngine;

namespace UAPIModule.Tests
{
    public class APITest : MonoBehaviour
    {
        private const string API_KEY = "TEST";

        public APIConfig apiConfig;

        private void Awake()
        {
            APIClient.CreateRequest(API_KEY, NetworkLoadingHandlerCreator.CreateAndGet());
        }

        private async void Start()
        {
            RequestFeedbackConfig feedbackConfig = RequestFeedbackConfig.InitializationFeedback;

            NetworkResponse response = await APIClient.SendRequest(API_KEY, apiConfig.Get(), feedbackConfig, null);
            if (response.isSuccessful)
            {
                Debug.Log(response.ToString());
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
