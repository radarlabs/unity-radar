using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents event verification status.
    /// </summary>
    [System.Serializable]
    public enum RadarEventVerification
    {
        Accept = 1,
        Unverify = 0,
        Reject = -1
    }
}
