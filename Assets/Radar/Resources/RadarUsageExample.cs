
using UnityEngine;
using RadarSDKBridge;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RadarSDK
{
    /// <summary>
    /// Example script that shows usage of each method
    /// todo: delete this class, RadarExample.cs should be sufficient
    /// </summary>
    public class RadarUsageExample : MonoBehaviour
    {
        private void Start()
        {
            InitializeRadar();
        }

        private void InitializeRadar()
        {
            string publishableKey = Debug.isDebugBuild ? RadarSDKManager.TestPublishableKey : RadarSDKManager.LivePublishableKey;
            Radar.Initialize(publishableKey, fraud: true);
        }

        private void StartTrackingVerified()
            => Radar.StartTrackingVerified(RadarSDKManager.TrackingInterval, RadarSDKManager.UseBeacons);

        private void StopTrackingVerified()
            => Radar.StopTrackingVerified();

        private async Task GetVerifiedLocationToken()
        {
            var (status, data) = await Radar.GetVerifiedLocationToken();
            if (status == RadarStatus.SUCCESS)
            {
                Debug.Log($"Verified Location Token received: {data.Token}, ExpiresAt: {data.ExpiresAt}, ExpiresIn: {data.ExpiresIn}");
            }
            else
            {
                Debug.LogError("Failed to retrieve verified location token.");
            }
        }

        private async Task GetLocation()
        {
            var (status, location, stopped) = await Radar.GetLocation();

            if (status == RadarStatus.SUCCESS)
            {
                LogManager.Instance.Log($"Location received: Latitude = {location.Latitude}, Longitude = {location.Longitude}", LogType.Warning);
            }
            else
            {
                LogManager.Instance.Log("Failed to get location", LogType.Error);
            }
        }

        private void SetUserId(string userId)
        {
            Radar.UserId = userId;
        }

        private void SetMetadata(Dictionary<string, object> metadata)
        {
            Radar.Metadata = metadata;
        }

        private void SetVerifiedReceiver()
        {
            Radar.SetVerifiedReceiver(OnVerifiedLocationTokenReceived);
        }

        private void OnVerifiedLocationTokenReceived(RadarVerifiedLocationToken token)
        {
            Debug.Log($"Verified location token updated: {token}");
        }
    }

}