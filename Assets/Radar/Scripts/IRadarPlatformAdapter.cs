using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RadarSDK
{
    public interface IRadarPlatformAdapter
    {
        event Action<RadarVerifiedLocationToken> TokenUpdated;
        event Action<string> Log;
        event Action<RadarStatus> Error;
        
        string UserId { /*get;*/ set; }
        Dictionary<string, object> Metadata { /*get;*/ set; }

        void Initialize(string publishableKey);
        void RequestLocationPermissions();

        Task<(RadarStatus status, RadarLocation location, bool stopped)> GetLocation();
        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationToken();
        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerified(bool beacons = false, RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium);
        Task<(RadarStatus Status, RadarLocation Location, IEnumerable<RadarEvent> Events, RadarUser User)> TrackOnce(RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium, bool beacons = false);
        void StartTrackingVerified(int interval, bool beacons);
        void StopTrackingVerified();
    }
}