#if UNITY_IOS
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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


        public void Initialize(string publishableKey)
        {
            Radar_initializeWithPublishableKey(publishableKey);
        }

        public void SetUserID(string userId)
        {
            Radar_setUserId(userId);
        }


        public Task<(RadarStatus Status, VerifiedLocationData? Data)> TrackVerifiedAsync(bool beacons = false)
        {
            return new IosTrackVerifiedHandler().CompletionTask;
        }
    }
}
#endif