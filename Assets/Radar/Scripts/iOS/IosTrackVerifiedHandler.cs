#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;

namespace RadarSDK.iOS
{
    internal enum RadarRequestType
    {
        TrackVerified,
        GetVerifiedLocationToken
    }

    /// <summary>
    /// Handles the radar track verification callback from the Radar sdk.
    /// </summary>
    internal sealed class IosTrackVerifiedHandler
    {
        private delegate void CompletionResponseDict(int requestId, string statusStr, string jsonStr);

        [DllImport("__Internal")]
        private static extern void Radar_trackVerifiedWithCompletionHandler(
            int requestId,
            CompletionResponseDict response,
            string desiredAccuracy
        );

        private static event Action<(int requestId, string statusStr, string jsonStr)> OnResponse;

        [MonoPInvokeCallback(typeof(CompletionResponseDict))]
        private static void TrackVerifiedResponseCallback(int requestId, string statusStr, string jsonStr)
        {
            OnResponse?.Invoke((requestId, statusStr, jsonStr));
        }

        private readonly TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)> _currentTcs;
        public Task<(RadarStatus, RadarVerifiedLocationToken)> CompletionTask => _currentTcs.Task;
        private readonly int _id;

        public IosTrackVerifiedHandler(RadarRequestType requestType, RadarTrackingOptionsDesiredAccuracy desiredAccuracy = RadarTrackingOptionsDesiredAccuracy.Medium)
        {
            _id = GetHashCode();
            _currentTcs = new TaskCompletionSource<(RadarStatus, RadarVerifiedLocationToken)>();

            OnResponse += ResponseReceiveCallback;

            // Call the native function with the additional desiredAccuracy parameter
            Radar_trackVerifiedWithCompletionHandler(_id, TrackVerifiedResponseCallback, desiredAccuracy.ToString().ToUpper());
        }

        private void ResponseReceiveCallback((int requestId, string statusStr, string jsonStr) response)
        {
            if (response.requestId != _id) return;

            var status = Utils.StatusStringToEnum(response.statusStr);

            if (status == RadarStatus.SUCCESS && !string.IsNullOrEmpty(response.jsonStr))
            {
                var locationData = Utils.GetTrackDataFromJson(response.jsonStr);
                _currentTcs.TrySetResult((status, locationData));
            }
            else
            {
                _currentTcs.TrySetResult((status, null));
            }

            OnResponse -= ResponseReceiveCallback;
        }
    }
}

#endif