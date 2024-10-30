#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace RadarSDK.Android
{
    /// <summary>
    /// Adapter for the Radar SDK on Android.
    /// Bridges with .aar file.
    /// </summary>
    public class AndroidAdapter : IRadarPlatformAdapter
    {
        private AndroidJavaObject _instance;



        public void Initialize(string publishableKey)
        {
            LogManager.Instance.Log("AndroidAdapter.Initialize() " + publishableKey, LogType.Attention);

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
            using (var radarClass = new AndroidJavaClass("io.radar.sdk.Radar"))
            {
                _instance = radarClass.GetStatic<AndroidJavaObject>("INSTANCE");
            }

            var locationServicesProvider2 = new AndroidJavaClass("io.radar.sdk.Radar$RadarLocationServicesProvider");
            var locationServicesProvider = locationServicesProvider2.GetStatic<AndroidJavaObject>("GOOGLE");
            object[] @params = { context, publishableKey, null, locationServicesProvider, Radar.Settings.Fraud };
            _instance.CallStatic("initialize", @params);

            LogManager.Instance.Log("AndroidAdapter.Initialize() Completed", LogType.Attention);
        }


        public void SetUserID(string userId)
        {
            LogManager.Instance.Log("AndroidAdapter.SetUserID() " + userId, LogType.Attention);
            object[] parameters = new object[1];
            parameters[0] = userId;
            _instance.CallStatic("setUserId", parameters);
            LogManager.Instance.Log("AndroidAdapter.SetUserID() Completed", LogType.Attention);
        }


        public string GetUserID()
        {
            string userId = _instance.CallStatic<string>("getUserId");
            LogManager.Instance.Log("AndroidAdapter.GetUserID() -> " + userId, LogType.Attention);
            return userId;
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            // Serialize to JSON using Unity's JsonUtility
            string jsonString = JsonUtility.ToJson(metadata);
            LogManager.Instance.Log("AndroidAdapter.SetMetadata() " + jsonString, LogType.Attention);

            AndroidJavaObject jsonObject = new AndroidJavaObject("org.json.JSONObject", jsonString);

            // Call the setMetadata method in Radar.kt
            _instance.CallStatic("setMetadata", jsonObject);
            LogManager.Instance.Log("AndroidAdapter.SetMetadata() Completed", LogType.Attention);
        }


        public void GetLocation(Action<Location> onLocationReceived)
        {
            LogManager.Instance.Log("AndroidAdapter.GetLocation()", LogType.Attention);
            if (_instance == null)
            {
                LogManager.Instance.Log("Radar SDK is not initialized", LogType.Error);
                return;
            }
            // Create a proxy for the RadarLocationCallback
            AndroidJavaProxy callbackProxy = new RadarLocationCallbackProxy(onLocationReceived);
            _instance.CallStatic("getLocation", callbackProxy);
            LogManager.Instance.Log("AndroidAdapter.GetLocation() Completed", LogType.Attention);
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            LogManager.Instance.Log("AndroidAdapter.TrackVerifiedAsync()");
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();
            // Create an instance of the RadarTrackVerifiedCallbackProxy, passing the callback
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                // Set the result for the task
                taskCompletionSource.SetResult((status, locationData));
            });
            // Call the trackVerified method on the Radar SDK
            _instance.CallStatic("trackVerified", beacons, callbackProxy);
            LogManager.Instance.Log("AndroidAdapter.TrackVerifiedAsync()  Complete");
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            LogManager.Instance.Log("AndroidAdapter.StartTrackingVerifiedAsync() " + interval + "; " + beacons);
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            // Create a callback proxy to handle the completion
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                taskCompletionSource.SetResult((status, locationData));
            });
            // Call startTrackingVerified on the Radar SDK
            _instance.CallStatic("startTrackingVerified", interval, beacons);
            LogManager.Instance.Log("AndroidAdapter.StartTrackingVerifiedAsync() Complete");
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            LogManager.Instance.Log("AndroidAdapter.StopTrackingAsync()");
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            // Call stopTracking on the Radar SDK
            _instance.CallStatic("stopTracking");
            // Assume stop tracking does not return location data, just a status
            taskCompletionSource.SetResult((RadarStatus.SUCCESS, null)); // Replace with appropriate status if needed
            LogManager.Instance.Log("AndroidAdapter.StopTrackingAsync()  Complete");
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            LogManager.Instance.Log("AndroidAdapter.GetVerifiedLocationTokenAsync()");

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

            LogManager.Instance.Log("AndroidAdapter.GetVerifiedLocationTokenAsync() Complete");

            return taskCompletionSource.Task;
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            LogManager.Instance.Log("AndroidAdapter.SetVerifiedReceiver()");

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Create a custom Java receiver instance with the callback
                var receiver = new AndroidJavaObject("io.radar.sdk.CustomVerifiedReceiver", new CustomVerifiedReceiverCallback(onTokenUpdated));

                // Call the setVerifiedReceiver method on the Radar SDK
                _instance.CallStatic("setVerifiedReceiver", receiver);
            }

            LogManager.Instance.Log("AndroidAdapter.SetVerifiedReceiver() Complete");
        }
    }


    public class RadarVerifiedLocationCallbackProxy : AndroidJavaProxy
    {
        private readonly Action<RadarStatus, VerifiedLocationData?> _onComplete;

        public RadarVerifiedLocationCallbackProxy(Action<RadarStatus, VerifiedLocationData?> onComplete)
            : base("io.radar.sdk.Radar$RadarTrackVerifiedCallback")
        {
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
                LogManager.Instance.Log("Location is null", LogType.Error);
            }
        }
    }


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
