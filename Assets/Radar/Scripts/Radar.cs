using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RadarSDK
{
    /// <summary>
    /// The main class used to interact with the Radar SDK.
    /// Check out the <a href="https://radar.com/documentation/sdk">SDK documentation</a> for more information.
    /// </summary>
    public static class Radar
    {
        #region Variables

        // The number of seconds to wait before an error is thrown if there's an issue
        public const int TIMEOUT_INTERVAL = 10;

        public static Action<string> OnError;
        public static Queue<System.Action> _mainThreadActions = new Queue<System.Action>();

        private static IRadarPlatformAdapter _platformAdapter;
        private static Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> _cachedTrackVerifiedTask;
        public static bool Initialized { get; private set; }
        public static ClientSettings Settings { get; private set; }
        #endregion


        /// <summary>
        /// Initializes the Radar SDK. Call this method from the main thread before calling any other Radar methods.
        /// Check out <a href="https://radar.com/documentation/sdk/android#foreground-tracking">Android</a>,
        /// and <a href="https://radar.com/documentation/sdk/ios#foreground-tracking">iOS</a> for more details.
        /// </summary>
        /// <param name="publishableKey">The publishable key for the Radar SDK.</param>
        /// <param name="fraud">A boolean indicating whether fraud detection is enabled.</param>
        public static void Initialize(string publishableKey, bool fraud = true)
        {
            if (Initialized)
            {
                return;
            }

            Settings = new ClientSettings(fraud: fraud);
            CreatePlatformAdapter();
            _platformAdapter.Initialize(publishableKey);
            Initialized = true;
            LogManager.Instance.Log($"Radar Initialization Completed");
        }

        public static void RequestLocationPermissions() 
        {
            _platformAdapter.RequestLocationPermissions();
        }

        public static string UserId
        {
            get => _platformAdapter.UserId;
            set => _platformAdapter.UserId = value;
        }

        public static Dictionary<string, object> Metadata
        {
            // get => _platformAdapter.Metadata;
            set => _platformAdapter.Metadata = value;
        }

        private static void CreatePlatformAdapter()
        {
#if UNITY_EDITOR
            _platformAdapter = new ProxyPlatform.ProxyAdapter();
            LogManager.Instance.Log($"Radar: '{nameof(ProxyPlatform.ProxyAdapter)}' was created");
#elif UNITY_ANDROID
            _platformAdapter = new Android.AndroidAdapter();
            LogManager.Instance.Log($"Radar: '{nameof(Android.AndroidAdapter)}' was created");
#elif UNITY_IOS
            _platformAdapter = new iOS.IosAdapter();
            LogManager.Instance.Log($"Radar: '{nameof(iOS.IosAdapter)}' was created");
#else
            _platformAdapter = new ProxyPlatform.ProxyAdapter();
            LogManager.Instance.Log($"Radar: Fallback! '{nameof(ProxyPlatform.ProxyAdapter)}' was created");
#endif
        }

        public static void StartTrackingVerified(int interval, bool beacons)
            => _platformAdapter.StartTrackingVerified(interval, beacons);

        /// <summary>
        /// Tracks the user's location with device integrity information for location verification use cases.
        /// Check out the <a href="https://radar.com/documentation/fraud">fraud documentation</a> for more information.
        /// </summary>
        /// <remarks>
        /// Note: You must configure SSL pinning before calling this method.
        /// </remarks>
        /// <param name="token">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="beacons">A boolean indicating whether to range beacons.</param>
        /// <returns>A Task that returns a tuple containing the RadarStatus and RadarVerifiedLocationToken.</returns>
        public static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(
            bool beacons = false,
            RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            try
            {
                if (_cachedTrackVerifiedTask != null)
                {
                    return await _cachedTrackVerifiedTask;
                }
                var track = _platformAdapter.TrackVerified(beacons, desiredAccuracy);
                var timeOut = DefaultOnTimeOut<(RadarStatus Status, RadarVerifiedLocationToken Data)>(TIMEOUT_INTERVAL); // Timeout if there's an issue
                _cachedTrackVerifiedTask = Task.WhenAny(track, timeOut).ContinueWith(t => t.Result.Result);
                var completedTask = await _cachedTrackVerifiedTask;
                _cachedTrackVerifiedTask = null;

                return completedTask;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during TrackVerified: {e.Message}");
                return (RadarStatus.ERROR_UNKNOWN, null);
            }
        }


        public static void StopTrackingVerified()
            => _platformAdapter.StopTrackingVerified();


        public static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
            => await _platformAdapter.GetVerifiedLocationToken();


        public static async Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
            => await _platformAdapter.GetLocation();


        // todo: delete
        private static void EnqueueMainThreadAction(System.Action action)
        {
            lock (_mainThreadActions)
            {
                _mainThreadActions.Enqueue(action);
            }
        }



        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
            => _platformAdapter.SetVerifiedReceiver(onTokenUpdated);


        private static async Task<T> DefaultOnTimeOut<T>(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return default;
        }



        public static void SetErrorCallback(Action<string> errorCallback)
        {
            OnError = errorCallback;
        }
    }
}