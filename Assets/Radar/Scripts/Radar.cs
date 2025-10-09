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

        public static event Action<RadarVerifiedLocationToken> TokenUpdated;
        public static event Action<string> Log;
        public static event Action<RadarStatus> Error;

        private static IRadarPlatformAdapter _platformAdapter;
        private static Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> _cachedTrackVerifiedTask;
        public static bool Initialized { get; private set; }
        #endregion


        public static void Initialize(string publishableKey)
        {
            if (Initialized)
            {
                return;
            }

            CreatePlatformAdapter();
            _platformAdapter.Initialize(publishableKey);
            _platformAdapter.TokenUpdated += token => TokenUpdated?.Invoke(token);
            _platformAdapter.Log += log => Log?.Invoke(log);
            _platformAdapter.Error += error => Error?.Invoke(error);
            Initialized = true;
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
#elif UNITY_ANDROID
            _platformAdapter = new Android.AndroidAdapter();
#elif UNITY_IOS
            _platformAdapter = new iOS.IosAdapter();
#else
            _platformAdapter = new ProxyPlatform.ProxyAdapter();
#endif
        }

        public static void StartTrackingVerified(int interval, bool beacons)
            => _platformAdapter.StartTrackingVerified(interval, beacons);

        public static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(
            bool beacons = false,
            RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            // todo: do we need to cache the call?
            if (_cachedTrackVerifiedTask != null)
            {
                return await _cachedTrackVerifiedTask;
            }
            _cachedTrackVerifiedTask = _platformAdapter.TrackVerified(beacons, desiredAccuracy);
            var completedTask = await _cachedTrackVerifiedTask;
            _cachedTrackVerifiedTask = null;

            return completedTask;
        }

        public static void StopTrackingVerified()
            => _platformAdapter.StopTrackingVerified();

        public static async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
            => await _platformAdapter.GetVerifiedLocationToken();

        public static async Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
            => await _platformAdapter.GetLocation();
    }
}