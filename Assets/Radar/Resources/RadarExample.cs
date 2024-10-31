
using UnityEngine;
using RadarSDKBridge;
using System.Threading.Tasks;
using System.Collections;

namespace RadarSDK
{
    /// <summary>
    /// Example script that shows usage of each method
    /// </summary>
    public class RadarExample : MonoBehaviour
    {
        private void Start()
        {
            InitializeRadar();
        }


        private void InitializeRadar()
        {
            StartCoroutine(RadarSDKManager.Initialize());
        }


        private async Task InitializeRadarAsync()
        {
            await RadarSDKManager.InitializeAsync();
        }


        public void TrackVerified()
        {
            StartCoroutine(RadarSDKManager.TrackVerified());
        }


        private async Task TrackVerifiedAsync()
        {
            await RadarSDKManager.TrackVerifiedAsync(RadarSDKManager.UserId);
        }


        public void StartTrackingVerified()
        {
            StartCoroutine(RadarSDKManager.StartTrackingVerified());
        }


        private async Task StartTrackingVerifiedAsync()
        {
            await RadarSDKManager.StartTrackingVerifiedAsync(RadarSDKManager.TrackingInterval, RadarSDKManager.UseBeacons);
        }


        public void StopTracking()
        {
            StartCoroutine(RadarSDKManager.StopTracking());
        }


        private async Task StopTrackingAsync()
        {
            await RadarSDKManager.StopTrackingAsync();
        }


        public void GetVerifiedLocationToken()
        {
            StartCoroutine(GetVerifiedLocationTokenCoroutine());
        }


        private IEnumerator GetVerifiedLocationTokenCoroutine()
        {
            var task = RadarSDKManager.GetVerifiedLocationTokenAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result.HasValue)
            {
                Debug.Log($"Verified Location Token received: Status = {task.Result.Value.Status}");
            }
            else
            {
                Debug.LogError("Failed to retrieve verified location token.");
            }
        }
        

        private async Task GetVerifiedLocationTokenAsync()
        {
            var result = await RadarSDKManager.GetVerifiedLocationTokenAsync();
            if (result.HasValue)
            {
                Debug.Log($"Verified Location Token received: Status = {result.Value.Status}");
            }
            else
            {
                Debug.LogError("Failed to retrieve verified location token.");
            }
        }


        public void GetLocation()
        {
            StartCoroutine(GetLocationCoroutine());
        }


        private IEnumerator GetLocationCoroutine()
        {
            var task = RadarSDKManager.GetLocationAsync();
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result != null)
            {
                Debug.Log($"Location: Latitude = {task.Result.Value.latitude}, Longitude = {task.Result.Value.longitude}");
            }
            else
            {
                Debug.LogError("Failed to retrieve location.");
            }
        }


        private async Task GetLocationAsync()
        {
            var location = await RadarSDKManager.GetLocationAsync();
            if (location != null)
            {
                Debug.Log($"Location: Latitude = {location.Value.latitude}, Longitude = {location.Value.longitude}");
            }
            else
            {
                Debug.LogError("Failed to retrieve location.");
            }
        }


        public void SetUserId(string userId)
        {
            RadarSDKManager.SetUserId(userId);
            Debug.Log($"User ID set to: {userId}");
        }


        private async Task SetUserIdAsync(string userId)
        {
            await RadarSDKManager.SetUserIdAsync(userId);
            Debug.Log($"User ID set to: {userId}");
        }


        public void SetMetadata(MetadataContainer metadata)
        {
            RadarSDKManager.SetMetadata(metadata);
            Debug.Log("Metadata set.");
        }


        private async Task SetMetadataAsync(MetadataContainer metadata)
        {
            await RadarSDKManager.SetMetadataAsync(metadata);
            Debug.Log("Metadata set.");
        }


        public void SetVerifiedReceiver()
        {
            RadarSDKManager.SetVerifiedReceiver(OnVerifiedLocationTokenReceived);
        }


        private void OnVerifiedLocationTokenReceived(RadarVerifiedLocationToken token)
        {
            Debug.Log($"Verified location token updated: {token}");
        }
    }

}