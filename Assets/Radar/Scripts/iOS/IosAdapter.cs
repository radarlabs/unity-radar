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
            LogManager.Instance.Log("IosAdapter.GetUserID()", LogType.Attention);
            // Call the native method to get the User ID
            string userId = Radar_getUserId();
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("IosAdapter - User ID not set or unavailable.", LogType.Warning);
                Debug.LogWarning("User ID not set or unavailable.");
                return null;
            }
            LogManager.Instance.Log("IosAdapter.GetUserID()  Complete " + userId, LogType.Attention);
            return userId;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            LogManager.Instance.Log("IosAdapter.GetVerifiedLocationTokenAsync()", LogType.Attention);
            return new IosTrackVerifiedHandler(RadarRequestType.GetVerifiedLocationToken).CompletionTask;
        }


        public void Initialize(string publishableKey)
        {
            LogManager.Instance.Log("IosAdapter.Initialize()", LogType.Attention);
            if (string.IsNullOrEmpty(publishableKey))
            {
                LogManager.Instance.Log("IosAdapter - Publishable key is missing. Initialization failed.", LogType.Error);
                return;
            }
            Radar_initializeWithPublishableKey(publishableKey);
            LogManager.Instance.Log("IosAdapter.Initialize()  Complete", LogType.Attention);
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            LogManager.Instance.Log("IosAdapter.SetMetadata()", LogType.Attention);
            string metadataJson = JsonUtility.ToJson(metadata);
            LogManager.Instance.Log("IosAdapter.SetMetadata2()", LogType.Attention);
            Radar_setMetadata(metadataJson);
            LogManager.Instance.Log("IosAdapter.SetMetadata()  Complete", LogType.Attention);
        }


        public void SetUserID(string userId)
        {
            LogManager.Instance.Log("IosAdapter.SetUserID() " + userId, LogType.Attention);
            if (string.IsNullOrEmpty(userId))
            {
                LogManager.Instance.Log("User ID is null or empty. Skipping SetUserID.", LogType.Warning);
                return;
            }
            Radar_setUserId(userId);
            LogManager.Instance.Log("IosAdapter.SetUserID()  Complete", LogType.Attention);
        }


        public void OnTokenUpdated(string token) // This function will be called by UnitySendMessage from iOS
        {
            LogManager.Instance.Log("Received verified token: " + token, LogType.Attention);
            // Handle the token as needed
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            LogManager.Instance.Log("IosAdapter.SetVerifiedReceiver()", LogType.Attention);

            _onTokenUpdated = onTokenUpdated;

            // Set up the delegate to be called when a new token is available
            Radar_setVerifiedDelegate(OnTokenUpdated);

            LogManager.Instance.Log("IosAdapter.SetVerifiedReceiver() Complete", LogType.Attention);
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

            LogManager.Instance.Log($"Token updated: {verifiedLocationToken.Token}, Passed: {verifiedLocationToken.Passed}, ExpiresAt: {verifiedLocationToken.ExpiresAt}, ExpiresIn: {verifiedLocationToken.ExpiresIn}", LogType.Attention);

            // Call the C# callback action
            _onTokenUpdated?.Invoke(verifiedLocationToken);
        }


        public async Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            LogManager.Instance.Log("IosAdapter.StartTrackingVerifiedAsync " + interval + " " + beacons, LogType.Attention);
            Radar_startTrackingVerified(interval, beacons);
            await Task.Delay(10); // Mocking asynchronous behavior for demonstration
            LogManager.Instance.Log("IosAdapter.StartTrackingVerifiedAsync Complete", LogType.Attention);
            return (RadarStatus.SUCCESS, null); // Placeholder for actual verified location data
        }


        public async Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            LogManager.Instance.Log("IosAdapter.StopTrackingAsync()", LogType.Attention);
            Radar_stopTrackingVerified();
            await Task.Delay(10); // Mocking asynchronous behavior for demonstration
            LogManager.Instance.Log("IosAdapter.StopTrackingAsync() Complete", LogType.Attention);
            return (RadarStatus.SUCCESS, null);
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            LogManager.Instance.Log("IosAdapter.TrackVerifiedAsync() " + beacons, LogType.Attention);
            return new IosTrackVerifiedHandler(RadarRequestType.TrackVerified).CompletionTask;
        }


        public void GetLocation(Action<Location> onLocationReceived)
        {
            LogManager.Instance.Log("IosAdapter.GetLocation()", LogType.Attention);
            // Increment the callback ID and store the callback in the dictionary
            int callbackId = currentCallbackId++;
            locationCallbacks[callbackId] = onLocationReceived;

            // Call the native method with the static callback and the callbackId
            Radar_getLocation(OnLocationUpdated, callbackId);
            LogManager.Instance.Log("IosAdapter.GetLocation() Complete", LogType.Attention);
        }

        // Static method that will be called by native code when location is received
        [AOT.MonoPInvokeCallback(typeof(RadarLocationCallback))]
        private static void OnLocationUpdated(double latitude, double longitude, int callbackId)
        {
            LogManager.Instance.Log("IosAdapter.OnLocationUpdated()", LogType.Attention);
            // Check if valid coordinates were received
            if (locationCallbacks.TryGetValue(callbackId, out var callback))
            {
                if (latitude != -91 && longitude != -181)
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
