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
        private Action<RadarVerifiedLocationToken> _onTokenUpdatedCallback;



        public void GetLocation(Action<Location> onLocationReceived)
        {
            Location mockLocation = new Location
            {
                coordinates = new double[] { _mockLocation.x, _mockLocation.y }
            };

            onLocationReceived?.Invoke(mockLocation);
        }


        public string GetUserID()
        {
            Debug.Log("ProxyAdapter.GetUserID()");
            return _mockUserId;
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync()
        {
            Debug.Log("ProxyAdapter.GetVerifiedLocationTokenAsync()");

            // Simulate an asynchronous token retrieval
            return Task.Run(() =>
            {
                Task.Delay(1000).Wait(); // Simulate delay

                var data = new VerifiedLocationData
                {
                    Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                    user = new User { _id = _mockUserId, fraud = new Fraud { bypassed = true } }
                };

                return (RadarStatus.SUCCESS, new VerifiedLocationData?(data));
            });
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
            Debug.Log("ProxyAdapter.SetMetadata() with: " + jsonString);
        }


        public void SetUserID(string userId)
        {
            _mockUserId = userId;
            Debug.Log($"ProxyAdapter.SetUserID() with: {userId}");
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            Debug.Log("ProxyAdapter.SetVerifiedReceiver()");
            _onTokenUpdatedCallback = onTokenUpdated;

            // Simulate receiving a token update
            Task.Run(async () =>
            {
                await Task.Delay(2000); // Simulate delay
                var token = new RadarVerifiedLocationToken
                {
                    Passed = true,
                    Token = "mock_verified_token",
                    ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 60000, // 1 minute from now
                    ExpiresIn = 60
                };

                _onTokenUpdatedCallback?.Invoke(token);
            });
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            Debug.Log($"ProxyAdapter.StartTrackingVerifiedAsync() with interval: {interval}, beacons: {beacons}");

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
            Debug.Log("ProxyAdapter.StopTrackingAsync()");

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
