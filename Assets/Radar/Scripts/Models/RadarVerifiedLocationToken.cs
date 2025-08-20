namespace RadarSDK
{
    /// <summary>
    /// Represents a user's verified location.
    /// Check out the <a href="https://radar.com/documentation/fraud">fraud documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarVerifiedLocationToken
    {
        public Location? Location { get; set; }
        /// <summary>
        /// The user.
        /// </summary>
        public User user;

        /// <summary>
        /// A boolean indicating whether the user passed all jurisdiction and fraud detection checks.
        /// </summary>
        public bool passed;

        // The commented out fields are not currently in use.
        // Prefer to leave them commented out due to case sensitivity.

        /// <summary>
        /// An array of events.
        /// </summary>
        public List<Event> events { get; set; }

        /// <summary>
        /// A signed JSON Web Token (JWT) containing the user and array of events. Verify the token server-side using your secret key.
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// The number of seconds until the token expires.
        /// </summary>
        public int expiresIn { get; set; }
    }
}