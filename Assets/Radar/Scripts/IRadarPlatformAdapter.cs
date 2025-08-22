using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RadarSDK
{
    public interface IRadarPlatformAdapter
    {
        string UserId { get; set; }
        Dictionary<string, object> Metadata { /*get;*/ set; }

        void Initialize(string publishableKey);

        void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated);
        Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation();
        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken();
        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(bool beacons = false, RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium);
        void StartTrackingVerified(int interval, bool beacons);
        void StopTrackingVerified();
    }
}