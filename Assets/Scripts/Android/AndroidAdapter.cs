#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace RadarSDK.Android
{
    public class AndroidAdapter : IRadarPlatformAdapter
    {
        private AndroidJavaObject _instance;


        public void Initialize(string publishableKey)
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
            using (var radarClass = new AndroidJavaClass("io.radar.sdk.Radar"))
            {
                _instance = radarClass.GetStatic<AndroidJavaObject>("INSTANCE");
            }
            // Correct enum conversion and method calls
            var locationServicesProvider2 = new AndroidJavaClass("io.radar.sdk.Radar$RadarLocationServicesProvider");
            var locationServicesProvider = locationServicesProvider2.GetStatic<AndroidJavaObject>("GOOGLE");

            object[] @params = { context, publishableKey, null, locationServicesProvider, Radar.Settings.Fraud };
            _instance.CallStatic("initialize", @params);
        }


        public void SetUserID(string userId)
        {
            object[] parameters = new object[1];
            parameters[0] = userId;
            _instance.CallStatic("setUserId", parameters);
            Debug.Log("AndroidAdapter.SetUserID " + userId);
        }


        public string GetUserID()
        {
            string userId = _instance.CallStatic<string>("getUserId");
            Debug.Log("AndroidAdapter.GetUserID " + userId);
            return userId;
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            // Serialize to JSON using Unity's JsonUtility
            string jsonString = JsonUtility.ToJson(metadata);
            AndroidJavaObject jsonObject = new AndroidJavaObject("org.json.JSONObject", jsonString);

            // Call the setMetadata method in Radar.kt
            _instance.CallStatic("setMetadata", jsonObject);
            Debug.Log("AndroidAdapter.SetMetadata called with: " + jsonString);
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            Debug.Log("AndroidAdapter.TrackVerifiedAsync()");
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();
            // Create an instance of the RadarTrackVerifiedCallbackProxy, passing the callback
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                // Set the result for the task
                taskCompletionSource.SetResult((status, locationData));
            });
            // Call the trackVerified method on the Radar SDK
            _instance.CallStatic("trackVerified", beacons, callbackProxy);
            Debug.Log("AndroidAdapter.TrackVerifiedAsync()  ---end");
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            Debug.Log("AndroidAdapter.StartTrackingVerifiedAsync(interval, beacons)   " + interval + " | " + beacons);
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            // Create a callback proxy to handle the completion
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                taskCompletionSource.SetResult((status, locationData));
            });
            Debug.Log("AndroidAdapter.StartTrackingVerifiedAsync(interval, beacons) [before calling]");
            // Call startTrackingVerified on the Radar SDK
            _instance.CallStatic("startTrackingVerified", interval, beacons);
            Debug.Log("AndroidAdapter.StartTrackingVerifiedAsync(interval, beacons)  ---end");
            return taskCompletionSource.Task;
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            Debug.Log("AndroidAdapter.StopTrackingAsync()");
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            // Call stopTracking on the Radar SDK
            _instance.CallStatic("stopTracking");
            Debug.Log("AndroidAdapter.StopTrackingAsync() [before calling]");
            // Assume stop tracking does not return location data, just a status
            taskCompletionSource.SetResult((RadarStatus.SUCCESS, null)); // Replace with appropriate status if needed
            Debug.Log("AndroidAdapter.StopTrackingAsync()  ---end");
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            Debug.Log("AndroidAdapter.GetVerifiedLocationTokenAsync()");

            // Create a TaskCompletionSource to return a Task
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            // Create an instance of the RadarVerifiedLocationCallbackProxy, passing the callback
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                // Set the result for the task
                taskCompletionSource.SetResult((status, locationData));
            });

            // Call the getVerifiedLocationToken method on the Radar SDK
            _instance.CallStatic("getVerifiedLocationToken", callbackProxy);

            Debug.Log("AndroidAdapter.GetVerifiedLocationTokenAsync() ---end");

            // Return the Task
            return taskCompletionSource.Task;
        }


        // public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        // {
        //     Debug.Log("AndroidAdapter.SetVerifiedReceiver() called");

        //     // Create a proxy for the RadarVerifiedReceiver
        //     var receiverProxy = new RadarVerifiedReceiverProxy(onTokenUpdated);

        //     // Call the setVerifiedReceiver method on the Radar SDK
        //     //! Error Unity AndroidJavaException: java.lang.IllegalArgumentException: io.radar.sdk.RadarVerifiedReceiver is not an interface
        //     _instance.CallStatic("setVerifiedReceiver", receiverProxy);


        //     Debug.Log("AndroidAdapter.SetVerifiedReceiver() ---end");
        // }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            Debug.Log("AndroidAdapter.SetVerifiedReceiver() called");

            // Get the current Unity Android activity
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Create a custom Java receiver instance with the callback
                var receiver = new AndroidJavaObject("io.radar.sdk.CustomVerifiedReceiver", new CustomVerifiedReceiverCallback(onTokenUpdated));

                // Call the setVerifiedReceiver method on the Radar SDK
                _instance.CallStatic("setVerifiedReceiver", receiver);
            }

            Debug.Log("AndroidAdapter.SetVerifiedReceiver() ---end");
        }
    }


    public class RadarVerifiedLocationCallbackProxy : AndroidJavaProxy
    {
        private readonly Action<RadarStatus, VerifiedLocationData?> _onComplete;

        public RadarVerifiedLocationCallbackProxy(Action<RadarStatus, VerifiedLocationData?> onComplete)
            : base("io.radar.sdk.Radar$RadarTrackVerifiedCallback")
        {
            Debug.Log("TrackVerifiedAsyncc  RadarTrackVerifiedCallbackProxy 1");
            _onComplete = onComplete;
        }

        // This method overrides the onComplete callback in Java
        public void onComplete(AndroidJavaObject status, AndroidJavaObject token)
        {
            // Convert Java objects to C# types
            RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
            VerifiedLocationData? locationData = null;
            if (token != null)
            {
                locationData = new VerifiedLocationData();
            }
            // Call the action passed in the constructor
            _onComplete?.Invoke(radarStatus, locationData);
        }
    }


    // public class RadarVerifiedReceiverProxy : AndroidJavaProxy
    // {
    //     private readonly Action<RadarVerifiedLocationToken> _onTokenUpdated;

    //     public RadarVerifiedReceiverProxy(Action<RadarVerifiedLocationToken> onTokenUpdated)
    //         : base("io.radar.sdk.RadarVerifiedReceiver")
    //     {
    //         Debug.Log("RadarVerifiedReceiverProxy Constructor");
    //         _onTokenUpdated = onTokenUpdated;
    //     }

    //     public void onTokenUpdated(AndroidJavaObject context, AndroidJavaObject token)
    //     {
    //         Debug.Log("RadarVerifiedReceiverProxy onTokenUpdated");
    //         // Convert the Java token object to a C# object
    //         var verifiedLocationToken = new RadarVerifiedLocationToken
    //         {
    //             Passed = token.Call<bool>("getPassed"),
    //             Token = token.Call<string>("getToken"),
    //             ExpiresAt = token.Call<long>("getExpiresAt"),
    //             ExpiresIn = token.Call<long>("getExpiresIn")
    //         };
    //         Debug.Log("_onTokenUpdated?.Invoke(verifiedLocationToken); " + (_onTokenUpdated == null));

    //         _onTokenUpdated?.Invoke(verifiedLocationToken);
    //     }
    // }


    // public class OnTokenUpdatedCallbackProxy : AndroidJavaProxy
    // {
    //     private readonly Action<string> _onTokenUpdated;

    //     public OnTokenUpdatedCallbackProxy(Action<string> onTokenUpdated)
    //         : base("io.radar.sdk.CustomRadarVerifiedReceiver$OnTokenUpdatedCallback")
    //     {
    //         _onTokenUpdated = onTokenUpdated;
    //     }

    //     // This method will be called by the Android SDK when the token is updated
    //     public void onTokenUpdated(AndroidJavaObject context, AndroidJavaObject token)
    //     {
    //         Debug.Log("OnTokenUpdatedCallbackProxy.onTokenUpdated called");

    //         // Extract the token data from the Android object and pass it to Unity
    //         string tokenData = token.Call<string>("getToken"); // Assuming token is a string
    //         _onTokenUpdated?.Invoke(tokenData);
    //     }
    // }


    public class CustomVerifiedReceiverCallback : AndroidJavaProxy
    {
        private readonly Action<RadarVerifiedLocationToken> _onTokenUpdated;

        public CustomVerifiedReceiverCallback(Action<RadarVerifiedLocationToken> onTokenUpdated)
            : base("io.radar.sdk.CustomVerifiedReceiver$OnTokenUpdatedListener")
        {
            _onTokenUpdated = onTokenUpdated;
        }

        public void onTokenUpdated(AndroidJavaObject context, AndroidJavaObject token)
        {
            // Retrieve the Date object for expiresAt
            var expiresAtDate = token.Call<AndroidJavaObject>("getExpiresAt");

            Debug.Log("CustomVerifiedReceiverCallback onTokenUpdated");
            // Convert the Date object to a long (Unix timestamp)
            long expiresAt = expiresAtDate.Call<long>("getTime");
            int expiresIn = token.Call<int>("getExpiresIn");
            // Convert the Java token object to a C# object
            var verifiedLocationToken = new RadarVerifiedLocationToken
            {
                Passed = token.Call<bool>("getPassed"),
                Token = token.Call<string>("getToken"),
                ExpiresAt = expiresAt, // Use the converted Unix timestamp
                ExpiresIn = expiresIn // Retrieve as int
            };

            _onTokenUpdated?.Invoke(verifiedLocationToken);
        }
    }
}
#endif
