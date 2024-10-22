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

        Task<(RadarStatus Status, VerifiedLocationData? Data)> GetVerifiedLocationTokenAsync();

        Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false);

        Task<(RadarStatus Status, VerifiedLocationData? Data)> StartTrackingVerifiedAsync(int interval, bool beacons);

        Task<(RadarStatus Status, VerifiedLocationData? Data)> StopTrackingAsync();

    }
}