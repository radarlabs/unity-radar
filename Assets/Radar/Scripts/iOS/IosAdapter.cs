#if UNITY_IOS
using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RadarSDK.iOS
{
    /// <summary>
    /// Adapter for the Radar SDK on iOS.
    /// Bridges with a .xcframework file.
    /// </summary>
    public class IosAdapter : IRadarPlatformAdapter
    {
        [DllImport("__Internal")]
        private static extern void Radar_initializeWithPublishableKey(string publishableKey);

        [DllImport("__Internal")]
        private static extern void Radar_setUserId(string userId);

        [DllImport("__Internal")]
        private static extern void Radar_setMetadata(string metadataJson);

        [DllImport("__Internal")]
        private static extern void Radar_trackVerified(bool beacons);

        [DllImport("__Internal")]
        private static extern void Radar_startTrackingVerified(double interval, bool beacons);

        [DllImport("__Internal")]
        private static extern void Radar_stopTrackingVerified();

        [DllImport("__Internal")]
        private static extern string Radar_getUserId(); // Added function to get user ID

        [DllImport("__Internal")]
        private static extern void Radar_setVerifiedReceiver(IntPtr callback); // Assuming you need a callback for receiving tokens

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
            // Here we need to implement the logic to retrieve the verified location token
            // This should bridge to an iOS method that returns location data.
            throw new NotImplementedException("This method needs to be implemented.");
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

        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            LogManager.Instance.Log("IosAdapter SetVerifiedReceiver", LogType.Attention);
            if (onTokenUpdated == null)
            {
                LogManager.Instance.Log("No callback function provided for receiving verified tokens.", LogType.Error);
                Debug.LogError("No callback function provided for receiving verified tokens.");
                return;
            }
            LogManager.Instance.Log("IosAdapter SetVerifiedReceiver 2", LogType.Attention);
            // Assuming that the native iOS SDK accepts a function pointer as a callback for verified tokens
            IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(onTokenUpdated);
            LogManager.Instance.Log("IosAdapter SetVerifiedReceiver 3", LogType.Attention);
            Radar_setVerifiedReceiver(callbackPtr);
            LogManager.Instance.Log("IosAdapter SetVerifiedReceiver ---end", LogType.Attention);
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

        public async Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            LogManager.Instance.Log("IosAdapter TrackVerifiedAsync " + beacons, LogType.Attention);
            Radar_trackVerified(beacons);
            LogManager.Instance.Log("IosAdapter TrackVerifiedAsync2", LogType.Attention);
            await Task.Delay(10); // Mocking asynchronous behavior for demonstration
            LogManager.Instance.Log("IosAdapter TrackVerifiedAsync ---end", LogType.Attention);
            return (RadarStatus.SUCCESS, null); // Placeholder for actual location data
        }
    }
}
#endif
