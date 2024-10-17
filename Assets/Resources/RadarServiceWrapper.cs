using System.Threading.Tasks;
using RadarSDK;
using UnityEngine;
using System;

namespace RadarSDKBridge
{
    public static class RadarServiceWrapper
    {
        public static void Initialize()
        {
            const string TEST_PUBLISHABLE_KEY = "prj_test_pk_0eac9fa8b8a4abcfcb40be269396e21fdca21b53";
            Debug.Log($"{nameof(RadarServiceWrapper)}.{nameof(Initialize)}({(Debug.isDebugBuild ? "TEST_KEY" : "LIVE_KEY")})");
            Radar.Initialize(TEST_PUBLISHABLE_KEY, fraud: true);
        }

        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> TrackVerified(string uniqueUserId)
        {
            if (!Radar.Initialized)
            {
                Initialize();
            }
            TryUpdateUserId(uniqueUserId);
            return await Radar.TrackVerified();
        }


        public static async Task StartTrackingVerified(int interval, bool beacons)
        {
            if (!Radar.Initialized)
            {
                Initialize();
            }

            // Call the platform adapter asynchronously
            await Radar.StartTrackingVerified(interval, beacons);
        }


        public static async Task StopTracking()
        {
            if (!Radar.Initialized)
            {
                Initialize();
            }

            // Call the platform adapter asynchronously
            await Radar.StopTracking();
        }


        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> GetVerifiedLocationToken()
        {
            if (!Radar.Initialized)
            {
                Initialize();
            }

            // Call the platform adapter asynchronously
            return await Radar.GetVerifiedLocationToken();
        }


        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            if (!Radar.Initialized)
            {
                Initialize();
            }

            // Set the receiver for token updates
            Radar.SetVerifiedReceiver(onTokenUpdated);
        }


        public static bool IsFraud(Fraud fraud)
        {
            // The fraud object contains several flags like mocked, compromised, proxy, etc.
            if (fraud.bypassed)
            {
                return false;
            }
            // Return true if any fraud flags indicate tampering
            return !(fraud is
            {
                verified: true,
                proxy: false,
                mocked: false,
                compromised: false,
                jumped: false,
                sharing: false,
                inaccurate: false,
                blocked: false
            });
        }


        /// <summary>
        /// This function ensures that the `SetUserId` call is only made if the `uniqueUserId` is changed.
        /// </summary>
        private static void TryUpdateUserId(string uniqueUserId)
        {
            const string PLAYER_PREFS_KEY = "RadarSKD-LastSetUserId";
            if (isInPlayerPrefs(uniqueUserId))
            {
                return;
            }
            // Radar.SetUserId(uniqueUserId);
            PlayerPrefs.SetString(PLAYER_PREFS_KEY, uniqueUserId);
            PlayerPrefs.Save();
            return;

            bool isInPlayerPrefs(string userId)
            {
                bool isInPlayerPrefs = PlayerPrefs.HasKey(PLAYER_PREFS_KEY);
                if (isInPlayerPrefs)
                {
                    return false;
                }
                return PlayerPrefs.GetString(PLAYER_PREFS_KEY) == userId;
            }
        }
    }
}