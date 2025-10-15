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

        public AndroidAdapter()
        {
            using var radarClass = new AndroidJavaClass("io.radar.sdk.Radar");
            _instance = radarClass.GetStatic<AndroidJavaObject>("INSTANCE");
        }


        public void Initialize(string publishableKey)
        {
            if (string.IsNullOrEmpty(publishableKey))
            {
                return;
            }
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");

            var locationServicesProvider2 = new AndroidJavaClass("io.radar.sdk.Radar$RadarLocationServicesProvider");
            var locationServicesProvider = locationServicesProvider2.GetStatic<AndroidJavaObject>("GOOGLE");
            object[] @params = { context, publishableKey, null, locationServicesProvider, true, null, null, activity };
            _instance.CallStatic("initialize", @params);
            _instance.CallStatic("setReceiver", new AndroidJavaObject(
                "io.radar.sdk.CustomReceiver",
                new CustomReceiverCallback(
                    message => Log?.Invoke(message),
                    status => Error?.Invoke(status)
                )
            ));
            _instance.CallStatic("setVerifiedReceiver", new AndroidJavaObject(
                "io.radar.sdk.CustomVerifiedReceiver",
                new CustomVerifiedReceiverCallback(
                    token => TokenUpdated?.Invoke(token)
                )
            ));
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

            var callbackProxy = new RadarLocationCallback(res => taskCompletionSource.SetResult(res));

            _instance.CallStatic("getLocation", callbackProxy);

            return taskCompletionSource.Task;
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(
            bool beacons = false,
            RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            var callbackProxy = new RadarTrackVerifiedCallback(res => taskCompletionSource.SetResult(res));
            AndroidJavaClass trackingOptionsClass = new AndroidJavaClass("io.radar.sdk.RadarTrackingOptions$RadarTrackingOptionsDesiredAccuracy");
            AndroidJavaObject desiredAccuracyEnum = trackingOptionsClass.CallStatic<AndroidJavaObject>("valueOf", desiredAccuracy.ToString().ToUpper());

            _instance.CallStatic("trackVerified", beacons, desiredAccuracyEnum, callbackProxy);
            return taskCompletionSource.Task;
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
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            var callbackProxy = new RadarTrackVerifiedCallback(res => taskCompletionSource.SetResult(res));

            _instance.CallStatic("getVerifiedLocationToken", callbackProxy);
            return taskCompletionSource.Task;
        }

        public Task<(RadarStatus Status, RadarLocation Location, IEnumerable<RadarEvent> Events, RadarUser User)> TrackOnce(RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium, bool beacons = false)
        {
            var taskCompletionSource = new TaskCompletionSource<(RadarStatus, RadarLocation, IEnumerable<RadarEvent>, RadarUser)>();

            var callbackProxy = new RadarTrackOnceCallback(res => taskCompletionSource.SetResult(res));
            AndroidJavaClass trackingOptionsClass = new AndroidJavaClass("io.radar.sdk.RadarTrackingOptions$RadarTrackingOptionsDesiredAccuracy");
            AndroidJavaObject desiredAccuracyEnum = trackingOptionsClass.CallStatic<AndroidJavaObject>("valueOf", desiredAccuracy.ToString().ToUpper());

            _instance.CallStatic("trackOnce", desiredAccuracyEnum, beacons, callbackProxy);
            return taskCompletionSource.Task;
        }
    }

    // Handle every native interface, cast tuple to type T
    public class RadarCallbackProxy<T> : AndroidJavaProxy
    {
        private Action<T> _onComplete;

        public RadarCallbackProxy(string interfaceName, Action<T> onComplete)
            : base(interfaceName)
        {
            _onComplete = onComplete;
        }

        public void onComplete(AndroidJavaObject obj1, AndroidJavaObject obj2)
            => OnComplete((obj1, obj2));

        public void onComplete(AndroidJavaObject obj1, AndroidJavaObject obj2, bool obj3)
            => OnComplete((obj1, obj2, obj3));

        public void onComplete(AndroidJavaObject obj1, AndroidJavaObject obj2, AndroidJavaObject[] obj3, AndroidJavaObject obj4)
            => OnComplete((obj1, obj2, obj3, obj4));

        private void OnComplete(object obj)
            => _onComplete?.Invoke((T)obj);
    }

    public class RadarTrackVerifiedCallback : RadarCallbackProxy<(AndroidJavaObject, AndroidJavaObject)>
    {
        public RadarTrackVerifiedCallback(Action<(RadarStatus, RadarVerifiedLocationToken)> onComplete)
            : base("io.radar.sdk.Radar$RadarTrackVerifiedCallback", res => {
                var (status, results) = res;
                RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
                RadarVerifiedLocationToken radarVerifiedLocationToken = null;
                if (radarStatus == RadarStatus.SUCCESS && results != null)
                {
                    // Convert the results to JSON and deserialize to RadarVerifiedLocationToken
                    AndroidJavaObject json = results.Call<AndroidJavaObject>("toJson");
                    string jsonString = json.Call<string>("toString");

                    // Use Utils to parse JSON into RadarVerifiedLocationToken
                    radarVerifiedLocationToken = JsonUtility.FromJson<RadarVerifiedLocationToken>(jsonString);

                }
                onComplete?.Invoke((radarStatus, radarVerifiedLocationToken));
            }) {}
    }

    public class RadarLocationCallback : RadarCallbackProxy<(AndroidJavaObject, AndroidJavaObject, bool)>
    {
        public RadarLocationCallback(Action<(RadarStatus, RadarLocation, bool)> onComplete)
            : base("io.radar.sdk.Radar$RadarLocationCallback", res => {
                var (status, location, stopped) = res;
                RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
                var radarLocation = location != null ? new RadarLocation()
                {
                    Latitude = location.Call<double>("getLatitude"),
                    Longitude = location.Call<double>("getLongitude"),
                } : null;
                onComplete?.Invoke((radarStatus, radarLocation, stopped));
            }) {}
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

    public class RadarTrackOnceCallback : RadarCallbackProxy<(AndroidJavaObject, AndroidJavaObject, AndroidJavaObject[], AndroidJavaObject)>
    {
        public RadarTrackOnceCallback(Action<(RadarStatus, RadarLocation, IEnumerable<RadarEvent>, RadarUser)> onComplete)
            : base("io.radar.sdk.Radar$RadarTrackCallback", res => {
                var (status, location, events, user) = res;
                RadarStatus radarStatus = (RadarStatus)status.Call<int>("ordinal");
                RadarLocation radarLocation = null;
                IEnumerable<RadarEvent> radarEvents = null;
                RadarUser radarUser = null;
                
                if (radarStatus == RadarStatus.SUCCESS)
                {
                    if (location != null)
                    {
                        radarLocation = new RadarLocation()
                        {
                            Latitude = location.Call<double>("getLatitude"),
                            Longitude = location.Call<double>("getLongitude"),
                        };
                    }
                    
                    if (events != null && events.Length > 0)
                    {
                        List<RadarEvent> eventList = new List<RadarEvent>();
                        foreach (var eventObj in events)
                        {
                            if (eventObj != null)
                            {
                                AndroidJavaObject json = eventObj.Call<AndroidJavaObject>("toJson");
                                string jsonString = json.Call<string>("toString");
                                RadarEvent radarEvent = JsonUtility.FromJson<RadarEvent>(jsonString);
                                if (radarEvent != null)
                                {
                                    eventList.Add(radarEvent);
                                }
                            }
                        }
                        radarEvents = eventList;
                    }
                    
                    if (user != null)
                    {
                        AndroidJavaObject json = user.Call<AndroidJavaObject>("toJson");
                        string jsonString = json.Call<string>("toString");
                        radarUser = JsonUtility.FromJson<RadarUser>(jsonString);
                    }
                }
                
                onComplete?.Invoke((radarStatus, radarLocation, radarEvents, radarUser));
            }) {}
    }
}
#endif
