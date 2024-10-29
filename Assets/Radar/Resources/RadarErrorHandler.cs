using RadarSDKBridge;

namespace RadarSDK
{
    /// <summary>
    /// Handles global error management for the Radar SDK. 
    /// Initializes a centralized error callback to catch and log errors throughout the SDK's usage.
    /// </summary>
    public static class RadarErrorHandler
    {
        public static void InitializeErrorHandling()
        {
            // Set global error callback to handle errors
            RadarServiceWrapper.SetErrorCallback(HandleError);
        }

        private static void HandleError(string errorMessage)
        {
            // Log the error for debugging purposes
            LogManager.Instance.Log($"Showing error to user: {errorMessage}", LogType.Error);
        }
    }
}