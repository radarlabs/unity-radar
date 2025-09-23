#if UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;

namespace RadarSDK.Android
{
    /// <summary>
    /// Handles the radar track verification callback from the Radar sdk.
    /// </summary>
    internal class AndroidTrackVerifiedHandler : AndroidJavaProxy
    {
        /// Initializes a new instance of the <see cref="AndroidTrackVerifiedHandler"/> class.
        public AndroidTrackVerifiedHandler() : base("io.radar.sdk.Radar$RadarTrackVerifiedCallback")
        {
            _currentTcs = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();
        }

        public Task<(RadarStatus, RadarVerifiedLocationToken)> CompletionTask => _currentTcs.Task;
        private readonly TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)> _currentTcs;

        /// <summary>
        /// Handles the completion of the radar track verification process.
        /// This method is called by Radar sdk.
        /// </summary>
        /// <param name="status">The status of the radar tracking.</param>
        /// <param name="results">The token data returned from radar tracking.</param>
        // ReSharper disable once InconsistentNaming
        public void onComplete(AndroidJavaObject status, AndroidJavaObject results)
        {
            // Convert the status to an enum
            string enumName = status.Call<string>("name");
            RadarStatus radarStatus = Utils.StatusStringToEnum(enumName);

            if (radarStatus == RadarStatus.SUCCESS && results != null)
            {
                // Convert the results to JSON and deserialize to RadarVerifiedLocationToken
                AndroidJavaObject json = results.Call<AndroidJavaObject>("toJson");
                string jsonString = json.Call<string>("toString");

                // Use Utils to parse JSON into RadarVerifiedLocationToken
                var locationData = Utils.GetTrackDataFromJson(jsonString);

                _currentTcs.TrySetResult((radarStatus, locationData));
            }
            else
            {
                _currentTcs.TrySetResult((radarStatus, null));
            }
        }
    }
}
#endif