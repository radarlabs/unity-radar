using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace RadarSDK.ProxyPlatform
{
    public class ProxyAdapter : IRadarPlatformAdapter
    {
        private static RadarLocation _mockLocation = new RadarLocation { Latitude = 40.7128f, Longitude = -74.0060f };
        private static RadarUser _mockUser = new RadarUser { _id = "proxy_id", fraud = new RadarFraud { bypassed = true } };
        private static RadarVerifiedLocationToken _mockToken = new RadarVerifiedLocationToken
        {
            Passed = true,
            Token = "mock_verified_token",
            ExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 60000, // 1 minute from now
            ExpiresIn = 60
        };

        private string _mockUserId = "proxy_user_id";
        private Dictionary<string, object> _mockMetadata;
        private Action<RadarVerifiedLocationToken> _onTokenUpdatedCallback;

        public string UserId
        {
            get => _mockUserId;
            set => _mockUserId = value;
        }

        public Dictionary<string, object> Metadata
        {
            get => _mockMetadata ?? new Dictionary<string, object>();
            set => _mockMetadata = value;
        }

        public async Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation()
        {
            await Task.Delay(1000); // Simulate delay
            return (RadarStatus.SUCCESS, _mockLocation, false);
        }

        public async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken()
        {
            await Task.Delay(1000); // Simulate delay
            return (RadarStatus.SUCCESS, _mockToken);
        }

        public void Initialize(string _)
        {
            Debug.Log("Radar Proxy Adapter Initialized");
        }

        public void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated)
        {
            _onTokenUpdatedCallback = onTokenUpdated;

            // Simulate receiving a token update
            Task.Run(async () =>
            {
                await Task.Delay(2000); // Simulate delay
                _onTokenUpdatedCallback?.Invoke(_mockToken);
            });
        }

        public void StartTrackingVerified(int interval, bool beacons) { }

        public void StopTrackingVerified() { }

        public async Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(bool _ = false, RadarTrackingOptionsDesiredAccuracy __ = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            return (RadarStatus.SUCCESS, _mockToken);
        }
    }
}
