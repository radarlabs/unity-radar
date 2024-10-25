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
            Debug.Log("AndroidAdapter.Initialize(publishableKey) " + publishableKey);
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
            Debug.Log("AndroidAdapter.SetUserID(userId) " + userId);
            object[] parameters = new object[1];
            parameters[0] = userId;
            _instance.CallStatic("setUserId", parameters);
        }


        public string GetUserID()
        {
            string userId = _instance.CallStatic<string>("getUserId");
            Debug.Log("AndroidAdapter.GetUserID() -> " + userId);
            return userId;
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            // Serialize to JSON using Unity's JsonUtility
            string jsonString = JsonUtility.ToJson(metadata);
            AndroidJavaObject jsonObject = new AndroidJavaObject("org.json.JSONObject", jsonString);

            // Call the setMetadata method in Radar.kt
            _instance.CallStatic("setMetadata", jsonObject);
            Debug.Log("AndroidAdapter.SetMetadata() -> " + jsonString);
        }


        public void GetLocation(Action<Location> onLocationReceived)
        {
            if (_instance == null)
            {
                Debug.LogError("Radar SDK is not initialized");
                return;
            }
            // Create a proxy for the RadarLocationCallback
            AndroidJavaProxy callbackProxy = new RadarLocationCallbackProxy(onLocationReceived);
            _instance.CallStatic("getLocation", callbackProxy);
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



        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            Debug.Log("AndroidAdapter.SetVerifiedReceiver() called");

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


    public class RadarLocationCallbackProxy : AndroidJavaProxy
    {
        private Action<Location> _onLocationReceived;

        public RadarLocationCallbackProxy(Action<Location> onLocationReceived)
            : base("io.radar.sdk.Radar$RadarLocationCallback")
        {
            _onLocationReceived = onLocationReceived;
        }

        public void onComplete(AndroidJavaObject status, AndroidJavaObject location, bool stopped)
        {
            if (location != null)
            {
                double latitude = location.Call<double>("getLatitude");
                double longitude = location.Call<double>("getLongitude");
                // Create the Location struct and assign the coordinates
                var radarLocation = new Location
                {
                    type = "Point",
                    coordinates = new double[] { longitude, latitude }
                };
                // Invoke the callback to pass the location data to Unity
                _onLocationReceived?.Invoke(radarLocation);
            }
            else
            {
                Debug.LogError("Location is null");
            }
        }
    }


    public class CustomVerifiedReceiverCallback : AndroidJavaProxy
    {
        private readonly Action<RadarVerifiedLocationToken> _onTokenUpdated;

        public CustomVerifiedReceiverCallback(Action<RadarVerifiedLocationToken> onTokenUpdated)
            : base("io.radar.sdk.CustomVerifiedReceiver$OnTokenUpdatedListener")
        {
            if (onTokenUpdated == null)
            {
                Debug.LogWarning("shiiiiiiiiiiiiiiit CustomVerifiedReceiverCallback onTokenUpdated shit");
            }
            _onTokenUpdated = onTokenUpdated;
        }

        public void onTokenUpdated(AndroidJavaObject context, AndroidJavaObject token)
        {
            // Retrieve the Date object for expiresAt
            var expiresAtDate = token.Call<AndroidJavaObject>("getExpiresAt");

            Debug.Log("CustomVerifiedReceiverCallback onTokenUpdated 1");
            // Convert the Date object to a long (Unix timestamp)
            long expiresAt = expiresAtDate.Call<long>("getTime");
            int expiresIn = token.Call<int>("getExpiresIn");
            // Convert the Java token object to a C# object
            Debug.Log("CustomVerifiedReceiverCallback onTokenUpdated 2");
            var verifiedLocationToken = new RadarVerifiedLocationToken
            {
                Passed = token.Call<bool>("getPassed"),
                Token = token.Call<string>("getToken"),
                ExpiresAt = expiresAt, // Use the converted Unix timestamp
                ExpiresIn = expiresIn // Retrieve as int
            };
            Debug.Log("CustomVerifiedReceiverCallback onTokenUpdated 3");
            if (_onTokenUpdated == null)
            {
                Debug.LogWarning("CustomVerifiedReceiverCallback onTokenUpdated shit");
            }
            _onTokenUpdated?.Invoke(verifiedLocationToken);
            Debug.Log("CustomVerifiedReceiverCallback onTokenUpdated 4");
        }
    }
}
#endif
