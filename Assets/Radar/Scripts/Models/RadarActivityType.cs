using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents the activity type detected for the user.
    /// </summary>
    [System.Serializable]
    public enum RadarActivityType
    {
        Unknown = 0,
        Stationary = 1,
        Foot = 2,
        Run = 3,
        Bike = 4,
        Car = 5
    }
}
