using UnityEngine;

namespace RadarSDK
{
    [CreateAssetMenu(fileName = "RadarSettings", menuName = "Radar/Create Radar Settings")]
    public class RadarSettingsData : ScriptableObject
    {
        public string userId = "DefaultUserId";
        public int trackingInterval = 60; // Interval in seconds
        public bool useBeacons = true;
        public MetadataContainer metadata;
    }
}