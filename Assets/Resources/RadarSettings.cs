
using UnityEngine;
using RadarSDKBridge;

namespace RadarSDK
{
    public class RadarSettings : MonoBehaviour
    {
        private void Start()
        {
            InitializeRadar();
            RadarErrorHandler.InitializeErrorHandling();
        }

        private void InitializeRadar()
        {
            RadarSDKManager.Initialize();
        }

        public void StartUserTracking()
        {
            StartCoroutine(RadarSDKManager.TrackUser());
        }

        public void StartTracking()
        {
            StartCoroutine(RadarSDKManager.StartTracking());
        }

        public void StopTracking()
        {
            StartCoroutine(RadarSDKManager.StopTracking());
        }
    }
}