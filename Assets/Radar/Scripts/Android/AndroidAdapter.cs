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

        public event Action<RadarVerifiedLocationToken> TokenUpdated;
        public event Action<string> Log;
        public event Action<RadarStatus> Error;


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
            AndroidJavaObject receiver = new AndroidJavaObject(
                "io.radar.sdk.CustomReceiver",
                new CustomReceiverCallback(
                    message => Log?.Invoke(message),
                    status => Error?.Invoke(status)
                )
            );

            // Set the receiver in the Radar SDK
            _instance.CallStatic("setReceiver", receiver);
        }

        public string UserId
        {
            get => _instance.CallStatic<string>("getUserId");
            set => _instance.CallStatic("setUserId", new object[] { value });
        }

        public Dictionary<string, object> Metadata
        {
            // get
            // {
            //     var metadataJson = _instance.CallStatic<string>("getMetadata");
            //     return JsonUtility.FromJson<Dictionary<string, object>>(metadataJson);
            // }
            set
            {
                // Serialize the metadata to JSON format
                string jsonString = JsonUtility.ToJson(value);
                AndroidJavaObject jsonObject = new AndroidJavaObject("org.json.JSONObject", jsonString);
                _instance.CallStatic("setMetadata", jsonObject);
            }
        }


        public void RequestLocationPermissions()
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }


        public Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
        {

            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarLocation, bool)>();

            // Create a callback proxy to handle the completion
            var callbackProxy = new RadarLocationCallbackProxy((status, location, stopped) =>
            {
                // Set the result for the task
                taskCompletionSource.SetResult((status, location, stopped));
            });

            // Call getLocation on the Radar SDK
            _instance.CallStatic("getLocation", callbackProxy);

            return taskCompletionSource.Task;
        }


        public async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(
            bool beacons = false,
            RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            // Instantiate AndroidTrackVerifiedHandler to handle the callback
            var handler = new AndroidTrackVerifiedHandler();

            // Find the RadarTrackingOptionsDesiredAccuracy enum value in the Kotlin SDK
            AndroidJavaClass trackingOptionsClass = new AndroidJavaClass("io.radar.sdk.RadarTrackingOptions$RadarTrackingOptionsDesiredAccuracy");
            AndroidJavaObject desiredAccuracyEnum = trackingOptionsClass.CallStatic<AndroidJavaObject>("valueOf", desiredAccuracy.ToString().ToUpper());

            // Call the trackVerified method on the Radar SDK, passing in the parameters and handler
            _instance.CallStatic("trackVerified", beacons, desiredAccuracyEnum, handler);

            // Await the handler's task completion, which will contain the track verification result
            var result = await handler.CompletionTask;

            // Debug log to confirm received data
            // var json = JsonUtility.ToJson(result);
            return result;
        }



        public void StartTrackingVerified(int interval, bool beacons)
        {
            _instance.CallStatic("startTrackingVerified", interval, beacons);
        }


        public void StopTrackingVerified()
        {
            _instance.CallStatic("stopTrackingVerified");
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
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
                    Passed = token.Call<bool>("getPassed"),
                    Token = token.Call<string>("getToken"),
                    ExpiresIn = token.Call<int>("getExpiresIn")
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
        private Action<RadarStatus, RadarLocation, bool> _onLocationReceived;

        public RadarLocationCallbackProxy(Action<RadarStatus, RadarLocation, bool> onLocationReceived)
            : base("io.radar.sdk.Radar$RadarLocationCallback")
        {
            _onLocationReceived = onLocationReceived;
        }

        public void onComplete(AndroidJavaObject status, AndroidJavaObject location, bool stopped)
        {
            RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
            var radarLocation = location != null ? new RadarLocation()
            {
                Latitude = location.Call<double>("getLatitude"),
                Longitude = location.Call<double>("getLongitude"),
            } : null;
            _onLocationReceived?.Invoke(radarStatus, radarLocation, stopped);
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
                ExpiresAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(expiresAt),
                ExpiresIn = expiresIn
            };
            _onTokenUpdated?.Invoke(verifiedLocationToken);
        }
    }

    public class CustomReceiverCallback : AndroidJavaProxy
    {
        private readonly Action<string> _onLog;
        private readonly Action<RadarStatus> _onError;
        
        public CustomReceiverCallback(Action<string> onLog, Action<RadarStatus> onError)
            : base("io.radar.sdk.CustomReceiver$Listener")
        {
            _onLog = onLog;
            _onError = onError;
        }

        public void onLog(AndroidJavaObject context, string message)
        {
            _onLog?.Invoke(message);
        }

        public void onError(AndroidJavaObject context, AndroidJavaObject status)
        {
            _onError?.Invoke((RadarStatus)status.Call<int>("ordinal"));
        }
    }
}
#endif
