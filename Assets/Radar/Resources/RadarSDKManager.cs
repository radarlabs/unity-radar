using System.Threading.Tasks;
using RadarSDK;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

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
            get { return radarSettings != null ? radarSettings.addUserIdExtension : true; }
        }

        public static bool IsDebuggingEnabled
        {
            get { return radarSettings != null ? radarSettings.enableDebugging : true; }
        }

        public static string OriginalTestPublishableKey
        {
            get
            {
                if (radarSettings.testPublishableKey == String.Empty) return "prj_test_pk_0000000000000000000000000000000000000000";
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
                if (radarSettings.testPublishableKey == String.Empty) return "prj_test_pk_0000000000000000000000000000000000000000";
                return radarSettings.testPublishableKey;
            }
        }

        public static string LivePublishableKey
        {
            get
            {
                if (radarSettings.livePublishableKey == String.Empty) return "prj_live_pk_0000000000000000000000000000000000000000";
                return radarSettings.livePublishableKey;
            }
        }

        // public static Dictionary<string, object> Metadata
        // {
        //     get { return radarSettings != null ? radarSettings.metadata.ToDictionary() : null; }
        // }

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

        #region Coroutine Wrappers
        // Coroutine wrappers for asynchronous methods

        public static void Initialize()
        {
            radarSettings = Resources.Load<RadarSettingsData>("Settings/RadarSettings");
            LogManager.Instance.SetLogConsole(IsDebuggingEnabled);
            Radar.Error += status => LogManager.Instance.Log($"Error: {status}", LogType.Error);
            Radar.Log += message => LogManager.Instance.Log($"Log: {message}", LogType.Log);
            Radar.Initialize(Debug.isDebugBuild ? TestPublishableKey : LivePublishableKey, fraud: true);
        }

        #endregion


    }
}