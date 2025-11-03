using System;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a coordinate with latitude and longitude.
    /// </summary>
    [System.Serializable]
    public class RadarCoordinate
    {
        [SerializeField] private double latitude;
        [SerializeField] private double longitude;

        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
    }
}
