using System.Threading.Tasks;
using System;

namespace RadarSDK
{
    public interface IRadarPlatformAdapter
    {
        void Initialize(string publishableKey);

        void SetUserID(string userId);

        string GetUserID();

        void SetMetadata(MetadataContainer metadata);

        void SetVerifiedReceiver(Action<RadarVerifiedLocationToken> onTokenUpdated);

        void GetLocation(Action<Location> onLocationReceived);

        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> GetVerifiedLocationTokenAsync();

        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> TrackVerifiedAsync(bool beacons = false, string desiredAccuracy = "MEDIUM");

        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> StartTrackingVerifiedAsync(int interval, bool beacons);

        Task<(RadarStatus Status, RadarVerifiedLocationToken Data)> StopTrackingAsync();
    }
}