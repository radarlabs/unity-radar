
using UnityEngine;
using RadarSDKBridge;
using System.Threading.Tasks;
using System.Collections;

namespace RadarSDK
{
    /// <summary>
    /// Example script that shows usage of each method
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


        private async Task StartTrackingVerifiedAsync()
        {
            await Radar.StartTrackingVerified(RadarSDKManager.TrackingInterval, RadarSDKManager.UseBeacons);
        }


        private async Task StopTrackingAsync()
        {
            await Radar.StopTracking();
        }


        private void GetVerifiedLocationToken()
        {
            StartCoroutine(GetVerifiedLocationTokenCoroutine());
        }

        private IEnumerator GetVerifiedLocationTokenCoroutine()
        {
            var task = Radar.GetVerifiedLocationToken();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsCompleted && task.Result.Data != null)
            {
                Debug.Log($"Verified Location Token received: Status = {task.Result.Status}");
            }
            else
            {
                Debug.LogError("Failed to retrieve verified location token.");
            }
        }


        private async Task GetVerifiedLocationTokenAsync()
        {
            var result = await Radar.GetVerifiedLocationToken();
            if (result.Data != null)
            {
                Debug.Log($"Verified Location Token received: Status = {result.Status}");
            }
            else
            {
                Debug.LogError("Failed to retrieve verified location token.");
            }
        }

        private Task<Location?> GetLocation()
        {
            var tcs = new TaskCompletionSource<Location?>();

            Radar.GetLocation(location =>
            {
                if (location.coordinates != null)
                {
                    LogManager.Instance.Log($"Location received: Latitude = {location.latitude}, Longitude = {location.longitude}", LogType.Warning);
                }
                else
                {
                    LogManager.Instance.Log("Failed to get location", LogType.Error);
                }
            });

            return tcs.Task;
        }


        private void SetUserId(string userId)
        {
            Radar.SetUserId(userId);
        }


        private void SetMetadata(MetadataContainer metadata)
        {
            Radar.SetMetadata(metadata);
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