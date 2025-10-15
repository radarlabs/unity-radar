using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents the type of event.
    /// </summary>
    [System.Serializable]
    public enum RadarEventType
    {
        Unknown,
        Conversion,
        UserEnteredGeofence,
        UserExitedGeofence,
        UserEnteredPlace,
        UserExitedPlace,
        UserNearbyPlaceChain,
        UserEnteredRegionCountry,
        UserExitedRegionCountry,
        UserEnteredRegionState,
        UserExitedRegionState,
        UserEnteredRegionDMA,
        UserExitedRegionDMA,
        UserStartedTrip,
        UserUpdatedTrip,
        UserApproachingTripDestination,
        UserArrivedAtTripDestination,
        UserStoppedTrip,
        UserEnteredBeacon,
        UserExitedBeacon,
        UserEnteredRegionPostalCode,
        UserExitedRegionPostalCode,
        UserDwelledInGeofence
    }
}
