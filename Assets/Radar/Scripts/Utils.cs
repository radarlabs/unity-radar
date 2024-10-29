using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Provides utility methods for converting JSON data into VerifiedLocationData objects and 
    /// mapping status strings to RadarStatus enums.
    /// </summary>
    public static class Utils
    {
        public static VerifiedLocationData GetTrackDataFromJson(string jsonStr)
        {
            return JsonUtility.FromJson<VerifiedLocationData>(jsonStr);
        }

        public static RadarStatus StatusStringToEnum(string enumStr)
        {
            return (RadarStatus)System.Enum.Parse(typeof(RadarStatus), enumStr, ignoreCase: true);
        }
    }
}