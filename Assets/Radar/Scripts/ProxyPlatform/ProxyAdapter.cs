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
            return _mockUserId;
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> GetVerifiedLocationTokenAsync()
        {
            // Simulate an asynchronous token retrieval
            return Task.Run(() =>
            {
                Task.Delay(1000).Wait(); // Simulate delay

                var data = new RadarVerifiedLocationToken
                {
                    Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                    user = new User { _id = _mockUserId, fraud = new Fraud { bypassed = true } }
                };

                return (RadarStatus.SUCCESS, new RadarVerifiedLocationToken?(data));
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
        }


        public void SetUserID(string userId)
        {
            _mockUserId = userId;
        }


        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
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


        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> StartTrackingVerifiedAsync(int interval, bool beacons)
        {
            // Simulate tracking verified response with mock location data
            var data = new RadarVerifiedLocationToken
            {
                Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                user = new User { _id = _mockUserId, fraud = new Fraud { bypassed = true } }
            };

            // Returning success status and mock data
            return Task.FromResult((RadarStatus.SUCCESS, new RadarVerifiedLocationToken?(data)));
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> StopTrackingAsync()
        {
            // Simulate stop tracking
            return Task.FromResult<(RadarStatus, RadarVerifiedLocationToken?)>((RadarStatus.SUCCESS, null));
        }


        public Task<(RadarStatus Status, RadarVerifiedLocationToken? Data)> TrackVerifiedAsync(bool _ = false, string desiredAccuracy = "MEDIUM")
        {
            var data = new RadarVerifiedLocationToken
            {
                Location = new Location { coordinates = new double[] { _mockLocation.x, _mockLocation.y } },
                user = new User { _id = "proxy_id", fraud = new Fraud { bypassed = true } }
            };

            return Task.FromResult((RadarStatus.SUCCESS, new RadarVerifiedLocationToken?(data)));
        }
    }
}
