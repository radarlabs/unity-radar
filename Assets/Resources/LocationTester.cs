using System;
using UnityEngine;
using System.Threading.Tasks;
using RadarSDK;
using UnityEngine.UI;


namespace RadarSDKBridge
{
    public class LocationTester : MonoBehaviour
    {
        void Start()
        {
            TestLocation();
        }

        async void TestLocation()
        {
            var result = await RadarServiceWrapper.TrackVerified("TEST_uniqueUserId_002");
            if (result.HasValue && result.Value.Status == RadarStatus.SUCCESS)
            {
                var locationData = result.Value.Data;
                Debug.Log($"Mock Location: Latitude {locationData?.Location.Value.latitude}, Longitude {locationData?.Location.Value.longitude}");
            }
            else
            {
                Debug.LogError("Failed to retrieve location data.");
            }
        }
    }
}