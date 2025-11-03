using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents the source of a location update.
    /// </summary>
    [System.Serializable]
    public enum RadarLocationSource
    {
        ForegroundLocation,
        BackgroundLocation,
        ManualLocation,
        VisitArrival,
        VisitDeparture,
        GeofenceEnter,
        GeofenceExit,
        MockLocation,
        BeaconEnter,
        BeaconExit,
        Unknown
    }
}
