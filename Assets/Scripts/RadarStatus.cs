namespace RadarSDK
{
    /// <summary>
    /// The status types for a request.
    /// Checkout <a href="https://radar.com/documentation/sdk/android#foreground-tracking">Android</a>, 
    /// and <a href="https://radar.com/documentation/sdk/ios#foreground-tracking"> iOS</a>
    /// foreground tracking for more information.
    /// </summary>
    public enum RadarStatus
    {
        /// Success
        SUCCESS,
        /// SDK not initialized
        ERROR_PUBLISHABLE_KEY,
        /// Location permissions not granted
        ERROR_PERMISSIONS,
        /// Location services error or timeout (20 seconds)
        ERROR_LOCATION,
        /// Beacon ranging error or timeout (5 seconds)
        ERROR_BLUETOOTH,
        /// Network error or timeout (10 seconds)
        ERROR_NETWORK,
        /// Bad request (missing or invalid params)
        ERROR_BAD_REQUEST,
        /// Unauthorized (invalid API key)
        ERROR_UNAUTHORIZED,
        /// Payment required (organization disabled or usage exceeded)
        ERROR_PAYMENT_REQUIRED,
        /// Forbidden (insufficient permissions or no beta access)
        ERROR_FORBIDDEN,
        /// Not found
        ERROR_NOT_FOUND,
        /// Too many requests (rate limit exceeded)
        ERROR_RATE_LIMIT,
        /// Internal server error
        ERROR_SERVER,
        /// Unknown error
        ERROR_UNKNOWN
    }
}