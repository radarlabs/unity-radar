using System.Threading.Tasks;
using RadarSDK;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace RadarSDKBridge
{
    /// <summary>
    /// Provides a wrapper around the Radar SDK's core functionality, including methods for 
    /// initializing the SDK, setting user ID and metadata, and managing tracking operations.
    /// Supports error callbacks for centralized error handling through RadarErrorHandler.
    /// </summary>
    public static class RadarServiceWrapper
    {
        public static Action<string> OnError;
        public static Queue<System.Action> _mainThreadActions = new Queue<System.Action>();



        #region Methods

        public static void Initialize()
        {
            string publishableKey = Debug.isDebugBuild ? RadarSDKManager.TestPublishableKey : RadarSDKManager.LivePublishableKey;
            Radar.Initialize(publishableKey, fraud: true);
            LogManager.Instance.Log("RadarServiceWrapper.Initialize() Complete");
        }


        public static bool SetUserId(string userId)
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }
                Radar.SetUserId(userId);
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
            try
            {
                if (!Radar.Initialized) { Initialize(); }
                return Radar.GetUserId();
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error getting user ID: {e.Message}");
                return null;
            }
        }


        public static bool SetMetadata(MetadataContainer metadata)
        {
            try
            {
                if (!Radar.Initialized) { Initialize(); }
                Radar.SetMetadata(metadata);
                return true;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"Error setting metadata: {e.Message}");
                return false;
            }
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


        public static void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            LogManager.Instance.Log($" SetVerifiedReceiver");
            try
            {
                if (!Radar.Initialized) { Initialize(); }
                LogManager.Instance.Log(" -> onTokenUpdated -> " + onTokenUpdated.ToString());
                Radar.SetVerifiedReceiver(onTokenUpdated);
            }
            catch (Exception e)
            {
                LogManager.Instance.Log($"Error setting verified receiver: {e.Message}", LogType.Error);
                OnError?.Invoke($"Error setting verified receiver: {e.Message}");
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

                    EnqueueMainThreadAction(() =>
                    {
                        tcs.SetResult(location); // Set the result on the main thread
                    });
                }
                else
                {
                    LogManager.Instance.Log("Failed to get location", LogType.Error);

                    EnqueueMainThreadAction(() =>
                    {
                        tcs.SetResult(null); // Set the result on the main thread
                    });
                }
            });

            return tcs.Task;
        }

        #endregion

        public static void SetErrorCallback(Action<string> errorCallback)
        {
            OnError = errorCallback;
        }


        // Helper method to run actions on the main thread
        private static void EnqueueMainThreadAction(System.Action action)
        {
            lock (_mainThreadActions)
            {
                _mainThreadActions.Enqueue(action);
            }
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