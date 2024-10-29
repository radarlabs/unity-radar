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


        public string GetUserID()
        {
            LogManager.Instance.Log("IosAdapter GetUserID()", LogType.Attention);
            // Call the native method to get the User ID
            string userId = Radar_getUserId();
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("IosAdapter - User ID not set or unavailable.", LogType.Warning);
                Debug.LogWarning("User ID not set or unavailable.");
                return null;
            }
            LogManager.Instance.Log("IosAdapter GetUserID()  ---end " + userId, LogType.Attention);
            return userId;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            LogManager.Instance.Log("IosAdapter GetVerifiedLocationTokenAsync()", LogType.Attention);
            return new IosTrackVerifiedHandler(RadarRequestType.GetVerifiedLocationToken).CompletionTask;
        }


        public void Initialize(string publishableKey)
        {
            LogManager.Instance.Log("IosAdapter Initialize()", LogType.Attention);
            if (string.IsNullOrEmpty(publishableKey))
            {
                LogManager.Instance.Log("IosAdapter - Publishable key is missing. Initialization failed.", LogType.Error);
                Debug.LogError("Publishable key is missing. Initialization failed.");
                return;
            }
            LogManager.Instance.Log("IosAdapter Radar_initializeWithPublishableKey()", LogType.Attention);
            Radar_initializeWithPublishableKey(publishableKey);
            LogManager.Instance.Log("IosAdapter Initialize()  ---end", LogType.Attention);
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            LogManager.Instance.Log("IosAdapter SetMetadata()", LogType.Attention);
            string metadataJson = JsonUtility.ToJson(metadata);
            LogManager.Instance.Log("IosAdapter SetMetadata2()", LogType.Attention);
            Radar_setMetadata(metadataJson);
            LogManager.Instance.Log("IosAdapter SetMetadata()  ---end", LogType.Attention);
        }


        public void SetUserID(string userId)
        {
            LogManager.Instance.Log("IosAdapter SetUserID() " + userId, LogType.Attention);
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID is null or empty. Skipping SetUserID.", LogType.Warning);
                Debug.LogWarning("User ID is null or empty. Skipping SetUserID.");
                return;
            }
            LogManager.Instance.Log("IosAdapter SetUserID()2 " + userId, LogType.Attention);
            Radar_setUserId(userId);
            LogManager.Instance.Log("IosAdapter SetUserID()  ---end", LogType.Attention);
        }


        public void OnTokenUpdated(string token) // This function will be called by UnitySendMessage from iOS
        {
            Debug.Log("Received verified token: " + token);
            // Handle the token as needed
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            Debug.Log("IosAdapter.SetVerifiedReceiver() called");

            _onTokenUpdated = onTokenUpdated;

            // Set up the delegate to be called when a new token is available
            Radar_setVerifiedDelegate(OnTokenUpdated);

            Debug.Log("IosAdapter.SetVerifiedReceiver() ---end");
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

            Debug.Log($"Token updated: {verifiedLocationToken.Token}, Passed: {verifiedLocationToken.Passed}, ExpiresAt: {verifiedLocationToken.ExpiresAt}, ExpiresIn: {verifiedLocationToken.ExpiresIn}");

            // Call the C# callback action
            _onTokenUpdated?.Invoke(verifiedLocationToken);
        }


        public async Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            LogManager.Instance.Log("IosAdapter StartTrackingVerifiedAsync " + interval + " " + beacons, LogType.Attention);
            Radar_startTrackingVerified(interval, beacons);
            LogManager.Instance.Log("IosAdapter StartTrackingVerifiedAsync2", LogType.Attention);
            await Task.Delay(10); // Mocking asynchronous behavior for demonstration
            LogManager.Instance.Log("IosAdapter StartTrackingVerifiedAsync ---end", LogType.Attention);
            return (RadarStatus.SUCCESS, null); // Placeholder for actual verified location data
        }


        public async Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            LogManager.Instance.Log("IosAdapter StopTrackingAsync", LogType.Attention);
            Radar_stopTrackingVerified();
            LogManager.Instance.Log("IosAdapter StopTrackingAsync2", LogType.Attention);
            await Task.Delay(10); // Mocking asynchronous behavior for demonstration
            LogManager.Instance.Log("IosAdapter StopTrackingAsync ---end", LogType.Attention);
            return (RadarStatus.SUCCESS, null);
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            LogManager.Instance.Log("IosAdapter TrackVerifiedAsync " + beacons, LogType.Attention);
            return new IosTrackVerifiedHandler(RadarRequestType.TrackVerified).CompletionTask;
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
            LogManager.Instance.Log("IosAdapter > OnLocationUpdated", LogType.Attention);
            // Check if valid coordinates were received
            if (locationCallbacks.TryGetValue(callbackId, out var callback))
            {
                LogManager.Instance.Log("IosAdapter > TryGetValue " + callbackId + "    | " + latitude + " | " + longitude, LogType.Attention);
                if (latitude != -91 && longitude != -181)
                {
                    LogManager.Instance.Log("IosAdapter > TryGetValue", LogType.Attention);
                    // Create a Location struct to pass the location back to Unity
                    var location = new Location
                    {
                        type = "Point",
                        coordinates = new double[] { longitude, latitude }
                    };

                    // Invoke the C# callback with the location
                    LogManager.Instance.Log("IosAdapter > callback?.Invoke " + (callback == null), LogType.Attention);
                    callback?.Invoke(location);
                }
                else
                {
                    // Handle location failure
                    Debug.LogError("Failed to get location");
                    callback?.Invoke(default);
                }
                LogManager.Instance.Log("IosAdapter > Remove", LogType.Attention);
                // Remove the callback from the dictionary once it's invoked
                locationCallbacks.Remove(callbackId);
            }
        }
    }
}
#endif
