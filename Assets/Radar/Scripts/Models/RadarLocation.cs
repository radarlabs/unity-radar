using System;
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
        [SerializeField] private double? accuracy;
        [SerializeField] private double? altitude;
        [SerializeField] private double? course;
        [SerializeField] private bool? isFromMockProvider;
        [SerializeField] private double? speed;
        [SerializeField] private string? timestamp;
        [SerializeField] private double? verticalAccuracy;

        public double? Accuracy { get => accuracy; set => accuracy = value; }
        public double? Altitude { get => altitude; set => altitude = value; }
        public double? Course { get => course; set => course = value; }
        public bool? IsFromMockProvider { get => isFromMockProvider; set => isFromMockProvider = value; }
        public double Latitude { get => coordinates[1]; set => coordinates[1] = value; }
        public double Longitude { get => coordinates[0]; set => coordinates[0] = value; }
        public double? Speed { get => speed; set => speed = value; }
        public DateTimeOffset? Timestamp
        {
            get => DateTimeOffset.TryParse(timestamp, out DateTimeOffset result) ? result : null;
            set => timestamp = value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public double? VerticalAccuracy { get => verticalAccuracy; set => verticalAccuracy = value; }
    }
}