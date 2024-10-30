using System.Threading.Tasks;
using RadarSDK;
using System.Collections;
using UnityEngine;
using System;

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

        private static RadarSettingsData radarSettings;


        public const string TEMP_UNIQUE_USER_ID = "TEST_uniqueUserId_001";

        public static string UserId
        {
            get { return radarSettings != null ? radarSettings.userId : TEMP_UNIQUE_USER_ID; }
        }

        public static bool AddUserIdExtension
        {
            get { return radarSettings != null ? radarSettings.addUserIdExtension : true;}
        }

        public static bool IsDebuggingEnabled
        {
            get { return radarSettings != null ? radarSettings.enableDebugging : true; }
        }

        public static string OriginalTestPublishableKey
        {
            get
            {
                return radarSettings.testPublishableKey;
            }
        }

        public static string TestPublishableKey
        {
            get
            {
                // Check if PlayerPrefs has an override key saved
                if (PlayerPrefs.HasKey("OverrideTestPublishableKey"))
                {
                    return PlayerPrefs.GetString("OverrideTestPublishableKey");
                }
                // Return the default key from RadarSettingsData
                return radarSettings.testPublishableKey;
            }
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

        #endregion


        // Method to set the override key in PlayerPrefs
        public static void SaveOverrideTestPublishableKey(string newKey)
        {
            PlayerPrefs.SetString("OverrideTestPublishableKey", newKey);
            PlayerPrefs.Save();
        }


        #region Static Methods
        // Static methods calls to RadarServiceWrapper.cs

        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            RadarServiceWrapper.SetVerifiedReceiver(onTokenUpdated);
        }


        public static void SetUserId(string userId)
        {
            RadarServiceWrapper.SetUserId(userId);
        }


        public static string GetUserId()
        {
            return RadarServiceWrapper.GetUserId();
        }


        public static void SetMetadata(MetadataContainer metadata)
        {
            RadarServiceWrapper.SetMetadata(metadata);
        }

        #endregion


        #region Coroutine Wrappers
        // Coroutine wrappers for asynchronous methods

        public static IEnumerator Initialize()
        {
            Debug.Log("RadarSDKManager Initialize()");
            radarSettings = Resources.Load<RadarSettingsData>("Settings/RadarSettings");
            LogManager.Instance.SetLogConsole(radarSettings.enableDebugging);
            RadarErrorHandler.InitializeErrorHandling();
            var task = InitializeAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }
            Debug.Log("RadarSDKManager Initialize() Complete");
        }


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


        public static IEnumerator GetVerifiedLocationToken()
        {
            var task = GetVerifiedLocationTokenAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }


        public static IEnumerator GetLocation()
        {
            var task = GetLocationAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }


        #endregion


        #region Async Methods
        // Async versions of the methods

        public static async Task InitializeAsync()
        {
            Debug.Log("RadarSDKManager InitializeAsync()");
            await Task.Run(() => RadarServiceWrapper.Initialize());
            Debug.Log("RadarSDKManager InitializeAsync() Complete");
        }


        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> TrackVerifiedAsync(string userId)
        {
            return await RadarServiceWrapper.TrackVerified(userId);
        }


        public static async Task StartTrackingVerifiedAsync(int interval, bool useBeacons)
        {
            await RadarServiceWrapper.StartTrackingVerified(interval, useBeacons);
        }


        public static async Task StopTrackingAsync()
        {
            await RadarServiceWrapper.StopTracking();
        }


        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> GetVerifiedLocationTokenAsync()
        {
            return await RadarServiceWrapper.GetVerifiedLocationToken();
        }


        public static async Task<Location?> GetLocationAsync()
        {
            return await RadarServiceWrapper.GetLocation();
        }


        public static async Task SetUserIdAsync(string userId)
        {
            await Task.Run(() => RadarServiceWrapper.SetUserId(userId));
        }


        public static async Task<string> GetUserIdAsync()
        {
            return await Task.Run(() => RadarServiceWrapper.GetUserId());
        }


        public static async Task SetMetadataAsync(MetadataContainer metadata)
        {
            await Task.Run(() => RadarServiceWrapper.SetMetadata(metadata));
        }

        #endregion
    }
}