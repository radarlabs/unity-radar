using UnityEngine;
using RadarSDKBridge;

namespace RadarSDK
{
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
            Debug.LogError($"Radar SDK Error: {errorMessage}");

            // Optionally, show a user-friendly message in the game UI
            ShowErrorMessageToUser(errorMessage);
        }

        private static void ShowErrorMessageToUser(string errorMessage)
        {
            // Assume you have a UI Text element to display errors
            // Example: ErrorManager.ShowError(errorMessage);
            Debug.Log($"Showing error to user: {errorMessage}");
        }
    }
}