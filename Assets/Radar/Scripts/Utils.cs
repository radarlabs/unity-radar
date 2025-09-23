using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Provides utility methods for converting JSON data into RadarVerifiedLocationToken objects and
    /// mapping status strings to RadarStatus enums.
    /// </summary>
    public static class Utils
    {
        public static RadarVerifiedLocationToken GetTrackDataFromJson(string jsonStr)
        {
            return JsonUtility.FromJson<RadarVerifiedLocationToken>(jsonStr);
        }

        public static RadarStatus StatusStringToEnum(string enumStr)
        {
            return (RadarStatus)System.Enum.Parse(typeof(RadarStatus), enumStr, ignoreCase: true);
        }
    }
}