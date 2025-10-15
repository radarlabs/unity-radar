using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents operating hours for a geofence.
    /// </summary>
    [System.Serializable]
    public class RadarOperatingHours
    {
        [SerializeField] private Dictionary<string, List<List<string>>> hours;

        public Dictionary<string, List<List<string>>> Hours { get => hours; set => hours = value; }
    }
}
