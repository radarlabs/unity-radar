using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a location with coordinates.
    /// Check out the <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public class RadarLocation
    {
        [SerializeField] private double[] coordinates = new double[2]; // [longitude, latitude]

        public double Latitude { get => coordinates[1]; set => coordinates[1] = value; }
        public double Longitude { get => coordinates[0]; set => coordinates[0] = value; }
    }
}