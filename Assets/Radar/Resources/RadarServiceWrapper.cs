using System.Threading.Tasks;
using RadarSDK;
using UnityEngine;
using System;

namespace RadarSDKBridge
{
    public static class RadarServiceWrapper
    {
        public static Action<string> OnError;


        public static void Initialize()
        {
            const string TEST_PUBLISHABLE_KEY = "prj_test_pk_0eac9fa8b8a4abcfcb40be269396e21fdca21b53";
            Debug.Log($"{nameof(RadarServiceWrapper)}.{nameof(Initialize)}({"TEST_KEY"})");
            Radar.Initialize(TEST_PUBLISHABLE_KEY, fraud: true);
        }


        public static void SetErrorCallback(Action<string> errorCallback)
        {
            OnError = errorCallback;
        }


        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> TrackVerified(string uniqueUserId)
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }

                TryUpdateUserId(uniqueUserId);
                return await Radar.TrackVerified();
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during TrackVerified: {e.Message}");
                return null;
            }
        }


        public static async Task StartTrackingVerified(int interval, bool beacons)
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }

                await Radar.StartTrackingVerified(interval, beacons);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during StartTrackingVerified: {e.Message}");
            }
        }


        public static async Task StopTracking()
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }

                await Radar.StopTracking();
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error during StopTracking: {e.Message}");
            }
        }


        public static async Task<(RadarStatus Status, VerifiedLocationData? Data)?> GetVerifiedLocationToken()
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }

                return await Radar.GetVerifiedLocationToken();
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error getting verified location token: {e.Message}");
                return null;
            }
        }


        public static Task<Location?> GetLocation()
        {
            var tcs = new TaskCompletionSource<Location?>();

            Radar.GetLocation(location =>
            {
                if (location.coordinates != null)
                {
                    LogManager.Instance.Log($"Location received: Latitude = {location.latitude}, Longitude = {location.longitude}", LogType.Warning);
                    tcs.SetResult(location);
                }
                else
                {
                    LogManager.Instance.Log("Failed to get location", LogType.Error);
                    tcs.SetResult(null);
                }
            });
            return tcs.Task;
        }


        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }
                Radar.SetVerifiedReceiver(onTokenUpdated);
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error setting verified receiver: {e.Message}");
            }
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