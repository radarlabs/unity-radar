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
        private static readonly Dictionary<int, object> tasks = new();
        private static int currentTaskId = 0;
        private static IosAdapter _instance;

        #region Delegates
        private delegate void LogCallback(string message);
        private delegate void ErrorCallback(string statusStr);
        private delegate void TokenCallback(int requestId, string statusStr, string jsonStr);
        private delegate void LocationCallback(int requestId, string statusStr, string locationStr, bool stopped);
        private delegate void TokenUpdatedCallback(string jsonData);
        private delegate void TrackOnceCallback(int requestId, string statusStr, string locationStr, string eventsStr, string userStr);

        #endregion

        #region DllImports

        [DllImport("__Internal")]
        private static extern void Radar_initializeWithPublishableKey(string publishableKey);

        [DllImport("__Internal")]
        private static extern void Radar_setUserId(string userId);
        [DllImport("__Internal")]
        private static extern string Radar_getUserId();

        [DllImport("__Internal")]
        private static extern void Radar_setMetadata(string metadataJson);

        [DllImport("__Internal")]
        private static extern void Radar_getVerifiedLocationToken(int requestId, TokenCallback response);

        [DllImport("__Internal")]
        private static extern void Radar_trackVerified(
            int requestId,
            TokenCallback response,
            string desiredAccuracy
        );

        [DllImport("__Internal")]
        private static extern void Radar_startTrackingVerified(double interval, bool beacons);

        [DllImport("__Internal")]
        private static extern void Radar_stopTrackingVerified();

        [DllImport("__Internal")]
        private static extern void Radar_setDelegateCallbacks(LogCallback logCallback, ErrorCallback errorCallback, TokenUpdatedCallback tokenUpdatedCallback);

        [DllImport("__Internal")]
        private static extern void Radar_getLocation(int requestId, LocationCallback callback);

        [DllImport("__Internal")]
        private static extern void Radar_trackOnce(
            int requestId,
            TrackOnceCallback response,
            string desiredAccuracy,
            bool beacons
        );

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(LogCallback))]
        private static void OnLog(string message)
        {
            _instance.Log?.Invoke(message);
        }

        [AOT.MonoPInvokeCallback(typeof(ErrorCallback))]
        private static void OnError(string statusStr)
        {
            var status = Utils.StatusStringToEnum(statusStr);
            _instance.Error?.Invoke(status);
        }

        [AOT.MonoPInvokeCallback(typeof(TokenUpdatedCallback))]
        private static void OnTokenUpdated(string jsonData)
        {
            var verifiedLocationToken = JsonUtility.FromJson<RadarVerifiedLocationToken>(jsonData);
            _instance.TokenUpdated?.Invoke(verifiedLocationToken);
        }

        [AOT.MonoPInvokeCallback(typeof(TokenCallback))]
        private static void TokenDataCallback(int requestId, string statusStr, string jsonStr)
        {
            if (!tasks.TryGetValue(requestId, out var obj) || obj is not TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)> tcs) return;

            var status = Utils.StatusStringToEnum(statusStr);
            RadarVerifiedLocationToken token = null;
            if (status == RadarStatus.SUCCESS && !string.IsNullOrEmpty(jsonStr))
            {
                token = JsonUtility.FromJson<RadarVerifiedLocationToken>(jsonStr);
            }
            tcs.TrySetResult((status, token));
            tasks.Remove(requestId);
        }

        [AOT.MonoPInvokeCallback(typeof(LocationCallback))]
        private static void OnLocationUpdated(int requestId, string statusStr, string locationStr, bool stopped)
        {
            if (!tasks.TryGetValue(requestId, out var obj) || obj is not TaskCompletionSource<(RadarStatus, RadarLocation, bool)> tcs) return;

            var status = Utils.StatusStringToEnum(statusStr);
            RadarLocation location = null;
            if (status == RadarStatus.SUCCESS && !string.IsNullOrEmpty(locationStr))
            {
                location = JsonUtility.FromJson<RadarLocation>(locationStr);
            }
            tcs.TrySetResult((status, location, stopped));
            tasks.Remove(requestId);
        }

        [AOT.MonoPInvokeCallback(typeof(TrackOnceCallback))]
        private static void OnTrackOnceUpdated(int requestId, string statusStr, string locationStr, string eventsStr, string userStr)
        {
            if (!tasks.TryGetValue(requestId, out var obj) || obj is not TaskCompletionSource<(RadarStatus, RadarLocation, IEnumerable<RadarEvent>, RadarUser)> tcs) return;

            var status = Utils.StatusStringToEnum(statusStr);
            RadarLocation location = null;
            IEnumerable<RadarEvent> events = null;
            RadarUser user = null;
            
            if (status == RadarStatus.SUCCESS)
            {
                if (!string.IsNullOrEmpty(locationStr))
                {
                    location = JsonUtility.FromJson<RadarLocation>(locationStr);
                }
                if (!string.IsNullOrEmpty(eventsStr))
                {
                    RadarEvent[] eventArray = JsonUtility.FromJson<RadarEvent[]>(eventsStr);
                    events = eventArray;
                }
                if (!string.IsNullOrEmpty(userStr))
                {
                    user = JsonUtility.FromJson<RadarUser>(userStr);
                }
            }
            
            tcs.TrySetResult((status, location, events, user));
            tasks.Remove(requestId);
        }

        #endregion

        public event Action<RadarVerifiedLocationToken> TokenUpdated;
        public event Action<string> Log;
        public event Action<RadarStatus> Error;

        public void Initialize(string publishableKey)
        {
            if (string.IsNullOrEmpty(publishableKey))
            {
                return;
            }
            _instance = this;
            Radar_initializeWithPublishableKey(publishableKey);
            Radar_setDelegateCallbacks(OnLog, OnError, OnTokenUpdated);
        }

        public void RequestLocationPermissions()
            => Input.location.Start();

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
            lock (tasks)
            {
                var tcs = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();
                tasks[currentTaskId] = tcs;
                Radar_getVerifiedLocationToken(currentTaskId, TokenDataCallback);
                currentTaskId++;
                return tcs.Task;
            }
        }

        public void StartTrackingVerified(int interval, bool beacons)
            => Radar_startTrackingVerified(interval, beacons);

        public void StopTrackingVerified()
            => Radar_stopTrackingVerified();


        // todo: use beacons
        public Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(bool beacons = false, RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            lock (tasks)
            {
                var tcs = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();
                tasks[currentTaskId] = tcs;
                Radar_trackVerified(currentTaskId, TokenDataCallback, desiredAccuracy.ToString().ToUpper());
                currentTaskId++;
                return tcs.Task;
            }
        }

        public Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
        {
            lock (tasks)
            {
                var tcs = new TaskCompletionSource<(RadarStatus, RadarLocation, bool)>();
                tasks[currentTaskId] = tcs;
                Radar_getLocation(currentTaskId, OnLocationUpdated);
                currentTaskId++;
                return tcs.Task;
            }
        }

        public Task<(RadarStatus Status, RadarLocation Location, IEnumerable<RadarEvent> Events, RadarUser User)> TrackOnce(RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium, bool beacons = false)
        {
            lock (tasks)
            {
                var tcs = new TaskCompletionSource<(RadarStatus, RadarLocation, IEnumerable<RadarEvent>, RadarUser)>();
                tasks[currentTaskId] = tcs;
                Radar_trackOnce(currentTaskId, OnTrackOnceUpdated, desiredAccuracy.ToString().ToUpper(), beacons);
                currentTaskId++;
                return tcs.Task;
            }
        }
    }
}
#endif
