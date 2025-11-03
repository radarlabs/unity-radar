using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Abstract base class for geofence geometry.
    /// </summary>
    [System.Serializable]
    public abstract class RadarGeofenceGeometry { }

    /// <summary>
    /// Represents circular geofence geometry.
    /// </summary>
    [System.Serializable]
    public class RadarCircleGeometry : RadarGeofenceGeometry
    {
        [SerializeField] private RadarCoordinate center;
        [SerializeField] private double radius;

        public RadarCoordinate Center { get => center; set => center = value; }
        public double Radius { get => radius; set => radius = value; }
    }

    /// <summary>
    /// Represents polygon geofence geometry.
    /// </summary>
    [System.Serializable]
    public class RadarPolygonGeometry : RadarGeofenceGeometry
    {
        [SerializeField] private RadarCoordinate center;
        [SerializeField] private RadarCoordinate[] coordinates;
        [SerializeField] private double radius;

        public RadarCoordinate Center { get => center; set => center = value; }
        public IEnumerable<RadarCoordinate> Coordinates { get => coordinates; set => coordinates = value as RadarCoordinate[]; }
        public double Radius { get => radius; set => radius = value; }
    }
}
