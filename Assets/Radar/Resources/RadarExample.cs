
using UnityEngine;
using RadarSDKBridge;
using System.Threading.Tasks;

namespace RadarSDK
{
    public class RadarExample : MonoBehaviour
    {
        private void Start()
        {
            InitializeRadar();
        }


        private void InitializeRadar()
        {
            RadarSDKManager.Initialize();
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

        //... TrackVerified()
    }
}