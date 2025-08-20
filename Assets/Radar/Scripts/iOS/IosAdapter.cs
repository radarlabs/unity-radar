#if UNITY_IOS
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace RadarSDK.iOS
{
    /// <summary>
    /// Adapter for the Radar SDK on iOS.
    /// Bridges with .xcframework file.
    /// </summary>
    public class IosAdapter : IRadarPlatformAdapter
    {
        #region Variables
        private delegate void RadarTokenUpdatedCallback(IntPtr token, bool passed, long expiresAt, int expiresIn);
        private delegate void RadarLocationCallback(double latitude, double longitude, int callbackId);


        // Store the callback action to invoke later
        private static Action<RadarVerifiedLocationToken> _onTokenUpdated;


        [DllImport("__Internal")]
        private static extern void Radar_initializeWithPublishableKey(string publishableKey);

        [DllImport("__Internal")]
        private static extern void Radar_setUserId(string userId);
        [DllImport("__Internal")]
        private static extern string Radar_getUserId();

        [DllImport("__Internal")]
        private static extern void Radar_setMetadata(string metadataJson);

        [DllImport("__Internal")]
        private static extern void Radar_startTrackingVerified(double interval, bool beacons);

        [DllImport("__Internal")]
        private static extern void Radar_stopTrackingVerified();

        [DllImport("__Internal")]
        private static extern void Radar_setVerifiedDelegate(RadarTokenUpdatedCallback callback);

        [DllImport("__Internal")]
        private static extern void Radar_getLocation(RadarLocationCallback callback);

        private static Dictionary<int, Action<Location>> locationCallbacks = new Dictionary<int, Action<Location>>();
        private static int currentCallbackId = 0;

        // P/Invoke for the native getLocation method with the callbackId
        [DllImport("__Internal")]
        private static extern void Radar_getLocation(RadarLocationCallback callback, int callbackId);
        #endregion



        public void Initialize(string publishableKey)
        {
            if (string.IsNullOrEmpty(publishableKey))
            {
                LogManager.Instance.Log("Publishable key is missing. Initialization failed.", LogType.Error);
                return;
            }
            Radar_initializeWithPublishableKey(publishableKey);
        }


        public string GetUserID()
        {
            string userId = Radar_getUserId();
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID not set or unavailable.", LogType.Warning);
                return null;
            }
            return userId;
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> GetVerifiedLocationTokenAsync()
        {
            return new IosTrackVerifiedHandler(RadarRequestType.GetVerifiedLocationToken).CompletionTask;
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            string metadataJson = JsonUtility.ToJson(metadata);
            Radar_setMetadata(metadataJson);
        }


        public void SetUserID(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID is null or empty. Skipping SetUserID.", LogType.Warning);
                return;
            }
            Radar_setUserId(userId);
        }


        public void OnTokenUpdated(string token) // This function will be called by UnitySendMessage from iOS
        {
            LogManager.Instance.Log("Received verified token: " + token, LogType.Attention);
            // Handle the token as needed
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            _onTokenUpdated = onTokenUpdated;

            // Set up the delegate to be called when a new token is available
            Radar_setVerifiedDelegate(OnTokenUpdated);
        }

        // This function will be called by the native code
        [AOT.MonoPInvokeCallback(typeof(RadarTokenUpdatedCallback))]
        private static void OnTokenUpdated(IntPtr tokenPtr, bool passed, long expiresAt, int expiresIn)
        {
            // Convert the IntPtr to a string (token)
            string token = Marshal.PtrToStringAnsi(tokenPtr);

            // Create a new RadarVerifiedLocationToken object and populate it with the received data
            var verifiedLocationToken = new RadarVerifiedLocationToken
            {
                Passed = passed,
                Token = token,
                ExpiresAt = expiresAt, // Unix timestamp (milliseconds)
                ExpiresIn = expiresIn
            };

            // Call the C# callback action
            _onTokenUpdated?.Invoke(verifiedLocationToken);
        }


        public async Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            Radar_startTrackingVerified(interval, beacons);
            return (RadarStatus.SUCCESS, null); // Placeholder for actual verified location data
        }


        public async Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> StopTrackingAsync()
        {
            Radar_stopTrackingVerified();
            return (RadarStatus.SUCCESS, null);
        }


        // public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> TrackVerifiedAsync(bool beacons = false)
        // {
        //     return new IosTrackVerifiedHandler(RadarRequestType.TrackVerified).CompletionTask;
        // }

        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> TrackVerifiedAsync(
            bool beacons = false,
            string desiredAccuracy = "MEDIUM")
        {
            return new IosTrackVerifiedHandler(
                RadarRequestType.TrackVerified,
                desiredAccuracy
            ).CompletionTask;
        }


        public void GetLocation(Action<Location> onLocationReceived)
        {
            // Increment the callback ID and store the callback in the dictionary
            int callbackId = currentCallbackId++;
            locationCallbacks[callbackId] = onLocationReceived;

            // Call the native method with the static callback and the callbackId
            Radar_getLocation(OnLocationUpdated, callbackId);
        }

        // Static method that will be called by native code when location is received
        [AOT.MonoPInvokeCallback(typeof(RadarLocationCallback))]
        private static void OnLocationUpdated(double latitude, double longitude, int callbackId)
        {
            // Check if valid coordinates were received
            if (locationCallbacks.TryGetValue(callbackId, out var callback))
            {
                if (Location.IsValidLocation(latitude, longitude))
                {
                    // Create a Location struct to pass the location back to Unity
                    var location = new Location
                    {
                        type = "Point",
                        coordinates = new double[] { longitude, latitude }
                    };

                    // Invoke the C# callback with the location
                    callback?.Invoke(location);
                }
                else
                {
                    // Handle location failure
                    LogManager.Instance.Log("Failed to get location", LogType.Error);
                    callback?.Invoke(default);
                }
                // Remove the callback from the dictionary once it's invoked
                locationCallbacks.Remove(callbackId);
            }
        }
    }
}
#endif
