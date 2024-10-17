using UnityEngine;

namespace RadarSDK
{
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