using System;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents address confidence level.
    /// </summary>
    [System.Serializable]
    public enum RadarAddressConfidence
    {
        None,
        Exact,
        Interpolated,
        Fallback
    }
}
