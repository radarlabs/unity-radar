namespace RadarSDK
{
    /// <summary>
    /// Represents fraud detection signals for location verification.
    /// Note that these values should not be trusted unless you called `trackVerified()` instead of `trackOnce()`.
    /// Check out the <a href="https://radar.com/documentation/fraud">fraud documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarFraud
    {
        /// <summary>
        /// A boolean indicating whether the user passed fraud detection checks. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool passed;

        /// <summary>
        /// A boolean indicating whether fraud detection checks were bypassed for the user for testing. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool bypassed;

        /// <summary>
        /// A boolean indicating whether the request was made with SSL pinning configured successfully. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool verified;

        /// <summary>
        /// A boolean indicating whether the user's IP address is a known proxy. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool proxy;

        /// <summary>
        /// A boolean indicating whether the user's location is being mocked, such as in a simulator or using a location spoofing app.
        /// May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool mocked;

        /// <summary>
        /// A boolean indicating whether the user's device has been compromised according to the Play Integrity API. May be `false` if fraud detection is not enabled.
        /// Check out the <a href="https://developer.android.com/google/play/integrity/overview">integrity overview</a> for more information.
        /// </summary>
        public bool compromised;

        /// <summary>
        /// A boolean indicating whether the user moved too far too fast. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool jumped;

        /// <summary>
        /// A boolean indicating whether the user is screen sharing. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool sharing;

        /// <summary>
        /// A boolean indicating whether the user's location is not accurate enough. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool inaccurate;

        /// <summary>
        /// A boolean indicating whether the user has been manually blocked. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool blocked;
    }
}