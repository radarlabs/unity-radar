namespace RadarSDK
{
    /// <summary>
    /// Represents a location with coordinates.
    /// Check out the <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct Location
    {
        /// <summary>
        /// The type of location.
        /// </summary>
        public string type;

        /// <summary>
        /// The coordinates of the location represented as [longitude, latitude].
        /// </summary>
        public double[] coordinates;

        /// <summary>
        /// Gets the longitude of the location.
        /// </summary>
        public double longitude => coordinates != null && coordinates.Length >= 1 ? coordinates[0] : -181;

        /// <summary>
        /// Gets the latitude of the location.
        /// </summary>
        public double latitude => coordinates != null && coordinates.Length >= 2 ? coordinates[1] : -91;
    }
}