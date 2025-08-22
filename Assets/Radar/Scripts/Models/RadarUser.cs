namespace RadarSDK
{
    /// <summary>
    /// Represents the current user state.
    /// Check out the <a href="https://radar.com/documentation">documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarUser
    {
        /// <summary>
        /// The Radar ID of the user.
        /// </summary>
        public string _id;

        /// <summary>
        /// The unique ID of the user, provided when you identified the user. May be `null` if the user has not been identified.
        /// </summary>
        public string userId;

        /// <summary>
        /// The device ID of the user.
        /// </summary>
        public string deviceId;

        /// <summary>
        /// The user's current location.
        /// </summary>
        public RadarLocation location;

        /// <summary>
        /// A boolean indicating whether the user is stopped.
        /// </summary>
        public bool stopped;

        /// <summary>
        /// A boolean indicating whether the user was last updated in the foreground.
        /// </summary>
        public bool foreground;

        /// <summary>
        /// The user's current country. May be `null` if the country is not available or if Regions is not enabled.
        /// </summary>
        public RadarRegion country;

        /// <summary>
        /// The user's current state. May be `null` if the state is not available or if Regions is not enabled. See <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
        /// </summary>
        public RadarRegion state;

        /// <summary>
        /// The source of the user's current location.
        /// </summary>
        public string source;

        /// <summary>
        /// A boolean indicating whether the user has been "Marked as Debug" in the dashboard.
        /// </summary>
        public bool debug;

        /// <summary>
        /// The user's current fraud state. May be `null` if fraud detection is not enabled.
        /// </summary>
        public RadarFraud fraud;

        // The commented out fields are not currently in use.
        // Prefer to leave them commented out due to case sensitivity.

        /// <summary>
        /// The optional description of the user.
        /// </summary>
        //public string description;
    }
}