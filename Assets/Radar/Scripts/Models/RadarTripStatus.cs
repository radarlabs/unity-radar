using System;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents trip status.
    /// </summary>
    [System.Serializable]
    public enum RadarTripStatus
    {
        Unknown,
        Started,
        Approaching,
        Arrived,
        Expired,
        Completed,
        Canceled
    }
}
