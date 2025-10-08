using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Provides utility methods for converting JSON data into RadarVerifiedLocationToken objects and
    /// mapping status strings to RadarStatus enums.
    /// </summary>
    public static class Utils
    {
        public static RadarStatus StatusStringToEnum(string enumStr)
        {
            return System.Enum.TryParse(enumStr, true, out RadarStatus status) ? status : RadarStatus.ERROR_UNKNOWN;
        }
    }
}