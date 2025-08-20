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

        /// <summary>
        /// Creates and initializes the platform-specific adapter for the Radar SDK.
        /// This method is called automatically at the load time of the subsystem registration.
        /// The adapter created depends on the platform the application is running on:
        /// <list type="bullet">
        /// <item><description>On the Unity Editor, a <see cref="ProxyPlatform.ProxyAdapter"/> is created.</description></item>
        /// <item><description>On Android, an <see cref="Android.AndroidAdapter"/> is created.</description></item>
        /// <item><description>On iOS, an <see cref="iOS.IosAdapter"/> is created.</description></item>
        /// <item><description>For unsupported platforms, it defaults to a <see cref="ProxyPlatform.ProxyAdapter"/>.</description></item>
        /// </list>
        /// </summary>
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

        /// <summary>
        /// Identifies the user. Until you identify the user, Radar will automatically identify the user by `deviceId`.
        /// Check out <a href="https://radar.com/documentation/sdk/android#identify-user">Android</a>,
        /// and <a href="https://radar.com/documentation/sdk/ios#identify-user">iOS</a> for more details.
        /// </summary>
        /// <param name="userId">The user ID to be set for the Radar SDK.</param>
        public static bool SetUserId(string userId)
        {
            try
            {
                CheckInitializedOrThrow();
                _platformAdapter.SetUserID(userId);
                return true;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error setting user ID: {e.Message}");
                return false;
            }
        }


        public static string GetUserId()
        {
            CheckInitializedOrThrow();
            return _platformAdapter.GetUserID();
        }


        public static bool SetMetadata(MetadataContainer metadata)
        {
            try
            {
                CheckInitializedOrThrow();
                _platformAdapter.SetMetadata(metadata);
                return true;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error setting metadata: {e.Message}");
                return false;
            }
        }


        public static async Task StartTrackingVerified(int interval, bool beacons)
        {
            try
            {
                CheckInitializedOrThrow();
                var startTask = StartTrackingVerified_Internal(interval, beacons);
                var timeOut = DefaultOnTimeOut<bool>(TIMEOUT_INTERVAL); // Timeout if there's an issue
                await Task.WhenAny(startTask, timeOut);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during StartTrackingVerified: {e.Message}");
            }
        }


        private static Task StartTrackingVerified_Internal(int interval, bool beacons)
        {
            return _platformAdapter.StartTrackingVerifiedAsync(interval, beacons);
        }

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
        public static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)?> TrackVerified()
        {
            try
            {
                CheckInitializedOrThrow();
                if (_cachedTrackVerifiedTask != null)
                {
                    return await _cachedTrackVerifiedTask;
                }
                var track = TrackVerified_Internal();
                var timeOut = DefaultOnTimeOut<(RadarStatus Status, RadarVerifiedLocationToken Data)>(TIMEOUT_INTERVAL); // Timeout if there's an issue
                _cachedTrackVerifiedTask = Task.WhenAny(track, timeOut).ContinueWith(t => t.Result.Result);
                var completedTask = await _cachedTrackVerifiedTask;
                _cachedTrackVerifiedTask = null;

                return completedTask;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during TrackVerified: {e.Message}");
                return null;
            }
        }


        private static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified_Internal(
            bool beacons = false)
        {
            CheckInitializedOrThrow();
            return await _platformAdapter.TrackVerifiedAsync(beacons);
        }


        public static async Task StopTracking()
        {
            try
            {
                CheckInitializedOrThrow();
                var stopTask = StopTracking_Internal();
                var timeOut = DefaultOnTimeOut<bool>(TIMEOUT_INTERVAL); // Timeout if there's an issue
                await Task.WhenAny(stopTask, timeOut);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during StopTracking: {e.Message}");
            }

            Task StopTracking_Internal()
            {
                return _platformAdapter.StopTrackingAsync();
            }
        }


        public static Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
        {
            try
            {
                CheckInitializedOrThrow();
                return _platformAdapter.GetVerifiedLocationTokenAsync();
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error getting verified location token: {e.Message}");
                return null;
            }
        }


        public static void GetLocation(Action<Location> onLocationReceived)
        {
            try
            {
                CheckInitializedOrThrow();
                _platformAdapter.GetLocation(onLocationReceived);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error getting verified location token: {e.Message}");
            }
        }


        private static void EnqueueMainThreadAction(System.Action action)
        {
            lock (_mainThreadActions)
            {
                _mainThreadActions.Enqueue(action);
            }
        }



        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            try
            {
                CheckInitializedOrThrow();
                _platformAdapter.SetVerifiedReceiver(onTokenUpdated);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error setting verified receiver: {e.Message}");
            }
        }


        private static async Task<T> DefaultOnTimeOut<T>(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return default;
        }



        private static void CheckInitializedOrThrow()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException($"Radar: Was not initialized, must first call the '{nameof(Initialize)}' method");
            }
        }


        public static void SetErrorCallback(Action<string> errorCallback)
        {
            OnError = errorCallback;
        }
    }
}