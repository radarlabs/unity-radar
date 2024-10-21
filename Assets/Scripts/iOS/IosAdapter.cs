#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

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

        public string GetUserID()
        {
            throw new NotImplementedException();
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void Initialize(string publishableKey)
        {
            Debug.Log("IosAdapter.Initialize() " + publishableKey);
            Radar_initializeWithPublishableKey(publishableKey);
            Debug.Log("IosAdapter.Initialize() --- end");
        }

        public void SetMetadata(MetadataContainer metadata)
        {
            string metadataJson = JsonUtility.ToJson(metadata);
            Debug.Log("IosAdapter.SetMetadata() " + metadataJson);
            Radar_setMetadata(metadataJson);
        }

        public void SetUserID(string userId)
        {
            Debug.Log("IosAdapter.SetUserID() " + userId);
            Radar_setUserId(userId);
        }

        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            throw new NotImplementedException();
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            Debug.Log($"IosAdapter.StartTrackingVerified() interval: {interval}, beacons: {beacons}");
            Radar_startTrackingVerified(interval, beacons);
            return Task.FromResult<(RadarStatus Status, VerifiedLocationData? Data)>((RadarStatus.SUCCESS, null));
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            Debug.Log("IosAdapter.StopTracking()");
            Radar_stopTrackingVerified();
            return Task.FromResult<(RadarStatus Status, VerifiedLocationData? Data)>((RadarStatus.SUCCESS, null));
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            Debug.Log("IosAdapter.TrackVerified()");
            Radar_trackVerified(beacons);
            return Task.FromResult<(RadarStatus Status, VerifiedLocationData? Data)>((RadarStatus.SUCCESS, null));
        }
    }
}
#endif
