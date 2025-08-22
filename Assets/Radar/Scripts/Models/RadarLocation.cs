namespace RadarSDK
{
    /// <summary>
    /// Represents a location with coordinates.
    /// Check out the <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public class RadarLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}