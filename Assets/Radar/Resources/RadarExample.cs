
using UnityEngine;
using RadarSDKBridge;

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
            RadarErrorHandler.InitializeErrorHandling();
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