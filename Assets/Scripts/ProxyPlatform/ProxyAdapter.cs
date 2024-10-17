using System;
using System.Threading.Tasks;
using UnityEngine;

namespace RadarSDK.ProxyPlatform
{
    public class ProxyAdapter : IRadarPlatformAdapter
    {
        private Vector2 _mockLocation = new Vector2(40.7128f, -74.0060f);
        private string _mockUserId = "proxy_user_id";
        private MetadataContainer _mockMetadata;

        public string GetUserID()
        {
            Debug.Log("ProxyAdapter.GetUserID called");
            return _mockUserId;
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void Initialize(string _)
        {
            Debug.Log("Radar Proxy Adapter Initialized");
        }

        public void SetMetadata(MetadataContainer metadata)
        {
            // Store the metadata locally for mocking purposes
            _mockMetadata = metadata;

            // Serialize to JSON using Unity's JsonUtility
            string jsonString = JsonUtility.ToJson(metadata);
            Debug.Log("ProxyAdapter.SetMetadata called with: " + jsonString);
        }

        public void SetUserID(string userId)
        {
            _mockUserId = userId;
            Debug.Log($"ProxyAdapter.SetUserID called with: {userId}");
        }

        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            throw new NotImplementedException();
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            Debug.Log($"ProxyAdapter.StartTrackingVerifiedAsync called with interval: {interval}, beacons: {beacons}");

            // Simulate tracking verified response with mock location data
            var data = new VerifiedLocationData
            {
                Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                user = new User { _id = _mockUserId, fraud = new Fraud { bypassed = true } }
            };

            // Returning success status and mock data
            return Task.FromResult((RadarStatus.SUCCESS, new VerifiedLocationData?(data)));
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync()
        {
            Debug.Log("ProxyAdapter.StopTrackingAsync called");

            // Simulate stopping tracking
            return Task.FromResult<(RadarStatus, VerifiedLocationData?)>((RadarStatus.SUCCESS, null));
        }

        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool _ = false)
        {
            var data = new VerifiedLocationData
            {
                Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                user = new User { _id = "proxy_id", fraud = new Fraud { bypassed = true } }
            };

            return Task.FromResult((RadarStatus.SUCCESS, new VerifiedLocationData?(data)));
        }
    }
}
