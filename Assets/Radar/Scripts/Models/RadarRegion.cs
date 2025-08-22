namespace RadarSDK
{
    /// <summary>
    /// Represents a region.
    /// Check out the <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarRegion
    {
        /// <summary>
        /// The Radar ID of the region.
        /// </summary>
        public string _id;

        /// <summary>
        /// The name of the region.
        /// </summary>
        public string name;

        /// <summary>
        /// The unique code for the region.
        /// </summary>
        public string code;

        /// <summary>
        /// The type of the region.
        /// </summary>
        public string type;

        /// <summary>
        /// The optional flag of the region.
        /// </summary>
        public string flag;

        /// <summary>
        /// A boolean indicating whether the jurisdiction is allowed. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool allowed;

        /// <summary>
        /// A boolean indicating whether all jurisdiction checks for the region have passed. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool passed;

        /// <summary>
        /// A boolean indicating whether the user is in an exclusion zone for the jurisdiction. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool inExclusionZone;

        /// <summary>
        /// A boolean indicating whether the user is too close to the border for the jurisdiction. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool inBufferZone;

        /// <summary>
        /// The distance in meters to the border of the jurisdiction. May be 0 if Fraud is not enabled.
        /// </summary>
        public double distanceToBorder;
    }
}