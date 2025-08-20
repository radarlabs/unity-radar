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
            if (string.IsNullOrEmpty(publishableKey))
            {
                LogManager.Instance.Log("Publishable key is missing. Initialization failed.", LogType.Error);
                return;
            }
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
        }


        public void SetUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID is null or empty. Skipping SetUserID.", LogType.Warning);
                return;
            }
            object[] parameters = new object[1];
            parameters[0] = userId;
            _instance.CallStatic("setUserId", parameters);
        }


        public string GetUserID()
        {
            string userId = _instance.CallStatic<string>("getUserId");
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID not set or unavailable.", LogType.Warning);
                return null;
            }
            return userId;
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            // Serialize to JSON using Unity's JsonUtility
            string jsonString = JsonUtility.ToJson(metadata);

            AndroidJavaObject jsonObject = new AndroidJavaObject("org.json.JSONObject", jsonString);

            // Call the setMetadata method in Radar.kt
            _instance.CallStatic("setMetadata", jsonObject);
        }


        public void GetLocation(Action<Location> onLocationReceived)
        {
            if (_instance == null)
            {
                LogManager.Instance.Log("Radar SDK is not initialized", LogType.Error);
                return;
            }
            // Create a proxy for the RadarLocationCallback
            AndroidJavaProxy callbackProxy = new RadarLocationCallbackProxy(onLocationReceived);
            _instance.CallStatic("getLocation", callbackProxy);
        }


        public async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerifiedAsync(
            bool beacons = false,
            string desiredAccuracy = "MEDIUM")
        {
            // Instantiate AndroidTrackVerifiedHandler to handle the callback
            var handler = new AndroidTrackVerifiedHandler();

            // Find the RadarTrackingOptionsDesiredAccuracy enum value in the Kotlin SDK
            AndroidJavaClass trackingOptionsClass = new AndroidJavaClass("io.radar.sdk.RadarTrackingOptions$RadarTrackingOptionsDesiredAccuracy");
            AndroidJavaObject desiredAccuracyEnum = trackingOptionsClass.CallStatic<AndroidJavaObject>("valueOf", desiredAccuracy.ToUpper());

            // Call the trackVerified method on the Radar SDK, passing in the parameters and handler
            _instance.CallStatic("trackVerified", beacons, desiredAccuracyEnum, handler);

            // Await the handler's task completion, which will contain the track verification result
            var result = await handler.CompletionTask;

            // Debug log to confirm received data
            // var json = JsonUtility.ToJson(result);
            return result;
        }



        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            // Create a callback proxy to handle the completion
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                taskCompletionSource.SetResult((status, locationData));
            });
            // Call startTrackingVerified on the Radar SDK
            _instance.CallStatic("startTrackingVerified", interval, beacons);
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> StopTrackingAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            // Call stopTracking on the Radar SDK
            _instance.CallStatic("stopTracking");
            // Assume stop tracking does not return location data, just a status
            taskCompletionSource.SetResult((RadarStatus.SUCCESS, null)); // Replace with appropriate status if needed
            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationTokenAsync()
        {
            // Create a TaskCompletionSource to return a Task
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            // Create an instance of the RadarVerifiedLocationCallbackProxy, passing the callback
            var callbackProxy = new RadarVerifiedLocationCallbackProxy((status, locationData) =>
            {
                // Set the result for the task
                taskCompletionSource.SetResult((status, locationData));
            });

            // Call the getVerifiedLocationToken method on the Radar SDK
            _instance.CallStatic("getVerifiedLocationToken", callbackProxy);

            return taskCompletionSource.Task;
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Create an instance of CustomVerifiedReceiver with the callback
                AndroidJavaObject receiver = new AndroidJavaObject(
                    "io.radar.sdk.CustomVerifiedReceiver",
                    new CustomVerifiedReceiverCallback(onTokenUpdated)
                );

                // Set the receiver in the Radar SDK
                _instance.CallStatic("setVerifiedReceiver", receiver);
            }
        }
    }



    public class RadarVerifiedLocationCallbackProxy : AndroidJavaProxy
    {
        private readonly Action<RadarStatus, RadarVerifiedLocationToken> _onComplete;

        public RadarVerifiedLocationCallbackProxy(Action<RadarStatus, RadarVerifiedLocationToken> onComplete)
            : base("io.radar.sdk.Radar$RadarTrackVerifiedCallback")
        {
            _onComplete = onComplete;
        }

        // This method overrides the onComplete callback in Java
        public void onComplete(AndroidJavaObject status, AndroidJavaObject token)
        {
            // Convert Java objects to C# types
            RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
            RadarVerifiedLocationToken verifiedLocationToken = null;

            if (token != null)
            {
                // Retrieve and convert token data from Java to C#

                verifiedLocationToken = new RadarVerifiedLocationToken
                {
                    passed = token.Call<bool>("getPassed"),
                    token = token.Call<string>("getToken"),
                    expiresIn = token.Call<int>("getExpiresIn")
                };

                // Convert the RadarVerifiedLocationToken to JSON format
                var json = JsonUtility.ToJson(verifiedLocationToken);
            }
            else
            {
                LogManager.Instance.Log("Received a null token from the Radar SDK", LogType.Warning);
            }

            // Invoke the callback with the status and token data
            _onComplete?.Invoke(radarStatus, verifiedLocationToken);
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
