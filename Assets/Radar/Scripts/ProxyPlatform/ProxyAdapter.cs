using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace RadarSDK.ProxyPlatform
{
    public class ProxyAdapter : IRadarPlatformAdapter
    {
        private static RadarLocation _mockLocation = new RadarLocation { Latitude = 40.7128f, Longitude = -74.0060f };
        private static RadarUser _mockUser = new RadarUser { Id = "proxy_id", Fraud = new RadarFraud { Bypassed = true } };
        private static RadarVerifiedLocationToken _mockToken = new RadarVerifiedLocationToken
        {
            Passed = true,
            Token = "mock_verified_token",
            ExpiresAt = DateTime.Now.AddMinutes(1),
            ExpiresIn = 60
        };
        private static RadarEvent[] _mockEvents = new RadarEvent[]
        {
            new RadarEvent
            {
                Id = "mock_event_1",
                Type = RadarEventType.UserEnteredPlace,
                CreatedAt = DateTime.Now.AddMinutes(-5),
                Location = _mockLocation
            }
        };

        private string _mockUserId = "proxy_user_id";
        private Dictionary<string, object> _mockMetadata;
        private Action<RadarVerifiedLocationToken> _onTokenUpdatedCallback;

        public event Action<RadarVerifiedLocationToken> TokenUpdated;
        public event Action<string> Log;
        public event Action<RadarStatus> Error;

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

        public void RequestLocationPermissions()
        {
            Debug.Log("Radar Proxy Adapter Requesting Location Permissions");
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

        public async Task<(RadarStatus Status, RadarLocation Location, IEnumerable<RadarEvent> Events, RadarUser User)> TrackOnce(RadarTrackingOptionsDesiredAccuracy _ = RadarTrackingOptionsDesiredAccuracy.Medium, bool __ = false)
        {
            await Task.Delay(1000); // Simulate delay
            return (RadarStatus.SUCCESS, _mockLocation, _mockEvents, _mockUser);
        }
    }
}
