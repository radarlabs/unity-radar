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
        private delegate void RadarLocationCallback(string status, double latitude, double longitude, bool stopped, int callbackId);


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

        private static Dictionary<int, TaskCompletionSource<(RadarStatus, RadarLocation, bool)>> locationCallbacks = new Dictionary<int, TaskCompletionSource<(RadarStatus, RadarLocation, bool)>>();
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

        public string UserId
        {
            get => Radar_getUserId();
            set => Radar_setUserId(value);
        }

        public Dictionary<string, object> Metadata
        {
            set => Radar_setMetadata(JsonUtility.ToJson(value));
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
        {
            return new IosTrackVerifiedHandler(RadarRequestType.GetVerifiedLocationToken).CompletionTask;
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


        public void StartTrackingVerified(int interval, bool beacons)
        {
            Radar_startTrackingVerified(interval, beacons);
        }


        public void StopTrackingVerified()
        {
            Radar_stopTrackingVerified();
        }


        // public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerifiedAsync(bool beacons = false)
        // {
        //     return new IosTrackVerifiedHandler(RadarRequestType.TrackVerified).CompletionTask;
        // }

        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(
            bool beacons = false,
            RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            return new IosTrackVerifiedHandler(
                RadarRequestType.TrackVerified,
                desiredAccuracy
            ).CompletionTask;
        }


        public Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
        {
            // Increment the callback ID and store the callback in the dictionary
            int callbackId = currentCallbackId++;
            var tsc = new TaskCompletionSource<(RadarStatus, RadarLocation, bool)>();
            locationCallbacks[callbackId] = tsc;

            // Call the native method with the static callback and the callbackId
            Radar_getLocation(OnLocationUpdated, callbackId);

            return tsc.Task;
        }

        // Static method that will be called by native code when location is received
        [AOT.MonoPInvokeCallback(typeof(RadarLocationCallback))]
        private static void OnLocationUpdated(string statusStr, double latitude, double longitude, bool stopped, int callbackId)
        {
            // Check if valid coordinates were received
            if (locationCallbacks.TryGetValue(callbackId, out var tsc))
            {
                var status = Utils.StatusStringToEnum(statusStr);
                RadarLocation location = null;
                if (status == RadarStatus.SUCCESS)
                {
                    location = new RadarLocation
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    };
                }
                tsc.TrySetResult((status, location, stopped));
                locationCallbacks.Remove(callbackId);
            }
        }
    }
}
#endif
