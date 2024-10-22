using System;
using System.Threading.Tasks;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// The main class used to interact with the Radar SDK.
    /// Check out the <a href="https://radar.com/documentation/sdk">SDK documentation</a> for more information.
    /// </summary>
    public static class Radar
    {
        private static IRadarPlatformAdapter _platformAdapter;
        private static Task<(RadarStatus Status, VerifiedLocationData? Data)> _cachedTrackVerifiedTask;
        public static bool Initialized { get; private set; }
        public static ClientSettings Settings { get; private set; }

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
            Debug.Log($"Radar: '{nameof(ProxyPlatform.ProxyAdapter)}' was created");
#elif UNITY_ANDROID
            _platformAdapter = new Android.AndroidAdapter();
            Debug.Log($"Radar: '{nameof(Android.AndroidAdapter)}' was created");
#elif UNITY_IOS
            _platformAdapter = new iOS.IosAdapter();
            Debug.Log($"Radar: '{nameof(iOS.IosAdapter)}' was created");
#else
            Debug.LogError($"Radar: '{Application.platform}' is not supported, defaulting to {nameof(ProxyPlatform.ProxyAdapter)}");
            _platformAdapter = new ProxyPlatform.ProxyAdapter();
            Debug.Log($"Radar: Fallback! '{nameof(ProxyPlatform.ProxyAdapter)}' was created");
#endif
        }


        /// <summary>
        /// Initializes the Radar SDK. Call this method from the main thread before calling any other Radar methods.
        /// Check out <a href="https://radar.com/documentation/sdk/android#foreground-tracking">Android</a>, 
        /// and <a href="https://radar.com/documentation/sdk/ios#foreground-tracking">iOS</a> for more details.
        /// </summary>
        /// <param name="publishableKey">The publishable key for the Radar SDK.</param>
        /// <param name="fraud">A boolean indicating whether fraud detection is enabled.</param>
        public static void Initialize(string publishableKey, bool fraud = false)
        {
            Debug.Log("Radar.Initialize " + publishableKey + "  [][]  " + Initialized);
            if (Initialized)
            {
                return;
            }

            Settings = new ClientSettings(fraud: fraud);
            CreatePlatformAdapter();
            Debug.Log($"Radar.CreatePlatformAdapter");
            _platformAdapter.Initialize(publishableKey);
            Initialized = true;
            Debug.Log($"Radar.Initialized(fraud: '{fraud}')");
        }

        /// <summary>
        /// Identifies the user. Until you identify the user, Radar will automatically identify the user by `deviceId`.
        /// Check out <a href="https://radar.com/documentation/sdk/android#identify-user">Android</a>, 
        /// and <a href="https://radar.com/documentation/sdk/ios#identify-user">iOS</a> for more details.
        /// </summary>
        /// <param name="userId">The user ID to be set for the Radar SDK.</param>
        public static void SetUserId(string userId)
        {
            CheckInitializedOrThrow();
            _platformAdapter.SetUserID(userId);
            Debug.Log("Radar.SetUserId: " + userId);
            Console.WriteLine($"Radar.SetUserId({userId})");
        }

        public static string GetUserId()
        {
            CheckInitializedOrThrow();
            string userId = _platformAdapter.GetUserID();
            Console.WriteLine($"Radar.GetUserId({userId})");
            return userId;
        }

        public static void SetMetadata(MetadataContainer metadata)
        {
            CheckInitializedOrThrow();
            _platformAdapter.SetMetadata(metadata);
            Console.WriteLine($"Radar.SetMetadata({metadata})");
        }

        public static async Task StartTrackingVerified(int interval, bool beacons)
        {
            Debug.Log("Radar.StartTrackingVerified(interval, beacons)   " + interval + " | " + beacons);
            CheckInitializedOrThrow();

            var startTask = StartTrackingVerified_Internal(interval, beacons);
            var timeOut = DefaultOnTimeOut<bool>(11); // Timeout in 11 seconds if there's an issue
            await Task.WhenAny(startTask, timeOut);
        }

        private static Task StartTrackingVerified_Internal(int interval, bool beacons)
        {
            return _platformAdapter.StartTrackingVerifiedAsync(interval, beacons);
        }

        public static async Task StopTracking()
        {
            Debug.Log("Radar.StopTracking()");
            CheckInitializedOrThrow();

            var stopTask = StopTracking_Internal();
            var timeOut = DefaultOnTimeOut<bool>(11); // Timeout in 11 seconds if there's an issue
            await Task.WhenAny(stopTask, timeOut);
        }

        private static Task StopTracking_Internal()
        {
            return _platformAdapter.StopTrackingAsync();
        }


        public static Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationToken()
        {
            return _platformAdapter.GetVerifiedLocationTokenAsync();
        }


        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            _platformAdapter.SetVerifiedReceiver(onTokenUpdated);
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
        /// <returns>A Task that returns a tuple containing the RadarStatus and VerifiedLocationData.</returns>
        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> TrackVerified()
        {
            if (_cachedTrackVerifiedTask != null)
            {
                return await _cachedTrackVerifiedTask;
            }
            var track = TrackVerified_Internal();
            var timeOut = DefaultOnTimeOut<(RadarStatus Status, VerifiedLocationData? Data)>(11);
            _cachedTrackVerifiedTask = Task.WhenAny(track, timeOut).ContinueWith(t => t.Result.Result);
            var completedTask = await _cachedTrackVerifiedTask;
            _cachedTrackVerifiedTask = null;

            return completedTask;
        }

        private static async Task<T> DefaultOnTimeOut<T>(int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return default;
        }

        private static Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerified_Internal(
            bool beacons = false)
        {
            CheckInitializedOrThrow();
            return _platformAdapter.TrackVerifiedAsync(beacons);
        }

        private static void CheckInitializedOrThrow()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException(
                    $"Radar: Was not initialized, must first call the '{nameof(Initialize)}' method");
            }
        }
    }
}