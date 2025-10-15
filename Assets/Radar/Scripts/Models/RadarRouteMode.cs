using System;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents route modes.
    /// </summary>
    [System.Serializable]
    [Flags]
    public enum RadarRouteMode
    {
        Foot,
        Bike,
        Car,
        Truck,
        Motorbike
    }
}
