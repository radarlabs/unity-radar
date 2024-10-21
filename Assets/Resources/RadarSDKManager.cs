using System.Threading.Tasks;
using RadarSDK;
using System.Collections;
using UnityEngine;

namespace RadarSDKBridge
{
    public static class RadarSDKManager
    {
        private static RadarSettingsData radarSettings;


        public static void Initialize()
        {
            radarSettings = Resources.Load<RadarSettingsData>("RadarSettings");

            RadarServiceWrapper.Initialize();

            if (radarSettings != null)
            {
                if (!string.IsNullOrEmpty(radarSettings.userId))
                {
                    Radar.SetUserId(radarSettings.userId);
                }
                if (radarSettings.metadata != null)
                {
                    Radar.SetMetadata(radarSettings.metadata);
                }
            }
            else
            {
                Debug.LogWarning("RadarSettings asset not found.");
            }
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

        // Async versions of the methods
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