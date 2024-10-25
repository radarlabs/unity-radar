using System.Threading.Tasks;
using RadarSDK;
using System.Collections;
using UnityEngine;

namespace RadarSDKBridge
{
    public static class RadarSDKManager
    {
        public const string TEMP_UNIQUE_USER_ID = "TEST_uniqueUserId_001";

        public static string UserId
        {
            get { return radarSettings != null ? radarSettings.userId : TEMP_UNIQUE_USER_ID; }
        }

        public static MetadataContainer Metadata
        {
            get { return radarSettings != null ? radarSettings.metadata : null; }
        }

        public static int TrackingInterval
        {
            get { return radarSettings != null ? radarSettings.trackingInterval : 60; }
        }

        public static bool UseBeacons
        {
            get { return radarSettings != null ? radarSettings.useBeacons : true; }
        }


        private static RadarSettingsData radarSettings;


        public static void Initialize()
        {
            LogManager.Instance.Log("RadarSDKManager Initialize()", LogType.Log);
            radarSettings = Resources.Load<RadarSettingsData>("Settings/RadarSettings");
            RadarErrorHandler.InitializeErrorHandling();
        }

        // Coroutine wrapper for asynchronous methods
        public static IEnumerator TrackUser()
        {
            var task = TrackUserAsync(radarSettings.userId);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }

        public static IEnumerator StartTracking()
        {
            var task = StartTrackingAsync(radarSettings.trackingInterval, radarSettings.useBeacons);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }

        public static IEnumerator StopTracking()
        {
            var task = StopTrackingAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }

        // Example of async versions of the methods
        public static async Task TrackUserAsync(string userId)
        {
            await RadarServiceWrapper.TrackVerified(userId);
        }

        public static async Task StartTrackingAsync(int interval, bool useBeacons)
        {
            await RadarServiceWrapper.StartTrackingVerified(interval, useBeacons);
        }

        public static async Task StopTrackingAsync()
        {
            await RadarServiceWrapper.StopTracking();
        }
    }
}