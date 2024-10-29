using System.Threading.Tasks;
using RadarSDK;
using System.Collections;
using UnityEngine;

namespace RadarSDKBridge
{
    /// <summary>
    /// Manages the initialization and configuration of the Radar SDK. 
    /// Loads user-configurable settings from the RadarSettings asset and initializes 
    /// the Radar SDK with the appropriate settings, such as user ID and tracking options.
    /// </summary>
    public static class RadarSDKManager
    {
        #region Variables
        public const string TEMP_UNIQUE_USER_ID = "TEST_uniqueUserId_001";

        public static string UserId
        {
            get { return radarSettings != null ? radarSettings.userId : TEMP_UNIQUE_USER_ID; }
        }

        public static bool AddUserIdExtension
        {
            get { return radarSettings.addUserIdExtension; }
        }

        public static bool IsDebuggingEnabled
        {
            get { return radarSettings.enableDebugging; }
        }

        public static string TestPublishableKey
        {
            get { return radarSettings.testPublishableKey; }
        }

        public static string LivePublishableKey
        {
            get { return radarSettings.livePublishableKey; }
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

        #endregion


        public static void Initialize()
        {
            LogManager.Instance.Log("RadarSDKManager Initialize()", LogType.Log);
            radarSettings = Resources.Load<RadarSettingsData>("Settings/RadarSettings");
            RadarErrorHandler.InitializeErrorHandling();
        }

        // Coroutine wrapper for asynchronous methods
        public static IEnumerator TrackVerified()
        {
            var task = TrackVerifiedAsync(radarSettings.userId);
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }


        public static IEnumerator StartTrackingVerified()
        {
            var task = StartTrackingVerifiedAsync(radarSettings.trackingInterval, radarSettings.useBeacons);
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
        public static async Task TrackVerifiedAsync(string userId)
        {
            await RadarServiceWrapper.TrackVerified(userId);
        }


        public static async Task StartTrackingVerifiedAsync(int interval, bool useBeacons)
        {
            await RadarServiceWrapper.StartTrackingVerified(interval, useBeacons);
        }


        public static async Task StopTrackingAsync()
        {
            await RadarServiceWrapper.StopTracking();
        }
    }
}