# UAPI UniTask 🚀
UAPI UniTask is a C# library developed by [Usef Farahmand](https://github.com/UsefFarahmand) aimed at simplifying API development and integration. It provides a set of tools and utilities to streamline the process of creating, managing, and consuming APIs, making it easier for developers to build robust and scalable applications.

✨ If you prefer using Unity's `Coroutine` system for asynchronous operations, we also offer a version of this package that integrates with Coroutines. You can find it [here](https://github.com/UModules/UAPI-Coroutine).

# Features 🌟
* **Easy Integration:** Simplifies the integration of APIs into your applications.
* **Comprehensive Documentation:** Detailed documentation to help you get started quickly.
* **Modular Architecture:** Highly modular design allows for easy customization and extension.
* **Robust Testing:** Includes a suite of tests to ensure reliability and stability.

## Installation 🛠️
### Step 1: Install `UniTask` 📦
UAPI UniTask depends on `UniTask`, which needs to be installed before adding UAPI UniTask. Follow these steps:
1. Open Unity and go to Window -> Package Manager.
2. Press the `+` button and select **Add package from git URL...**
3. Enter `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask` and press **Add**.

### Step 2: Install `UAPI UniTask`
### Git URL 🌐
1. After installing UniTask, open Window -> Package Manager in Unity.
2. Press the `+` button and choose **Add package from git URL...**
3. Enter `https://github.com/UModules/UAPI-UniTask.git` and press **Add**.

### Manifest File 📄
To install via git URL by editing the `manifest.json`, add this entry:
```"com.umodules.uapi": "https://github.com/UModules/UAPI-UniTask.git#upm"```

### Unity Package 📦
Alternatively, you can add the code directly to your project:
1. Clone the repo or download the latest release.
2. Add the UAPI UniTask folder to your Unity project or import the `.unitypackage`.

## Usage 📖
### Sample Usage 🎮
To see how UAPI UniTask works, you can explore the sample provided:
1. Open Unity and load the `Sample/Scenes/APISample.unity` scene.
2. Run the sample to see how API requests are handled asynchronously using coroutines.

### Custom Request Function 🔧
To demonstrate how to use UAPI UniTask, here's a simple function that sends a request and handles the response:
```C#
private void OnRequest()
{
   UniTask.Void(async () =>
   {
         var response = await APIClient.SendRequest(/*APIRequestConfig*/, /*RequestScreenConfig*/);
         if (response.isSuccessful)
         {
            Debug.Log($"Response: {response.ToString()}");
         }
         else
         {
            Debug.LogError("Request failed: " + response.errorMessage);
         }
   });
}
```
#### Key Classes and Configurations:
- **`APIRequestConfig`:** Configuration for creating and managing API requests. This class is responsible for defining the key properties of an API request, including the URL, HTTP method, headers, request body, timeout, and optional authentication. It also provides methods to generate request configurations with or without an authentication token. The class ensures that all necessary fields, such as the access token when authentication is required, are properly validated before making the request. It also offers methods to determine whether a request has headers or a body.
- **`RequestScreenConfig`:** Configuration for managing the display of network-related screens during API requests, including options for showing, hiding, or customizing screens based on the network state or response.
- **`Response(NetworkResponse response)`:** The callback function that handles the API response. It checks whether the response is successful, and logs the result accordingly.

# License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/UModules/UAPI-UniTask/blob/main/LICENSE) file for details.
