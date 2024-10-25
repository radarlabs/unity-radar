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
        private static extern void Radar_trackVerifiedWithCompletionHandler(int requestId, CompletionResponseDict response);

        [DllImport("__Internal")]
        private static extern void Radar_getVerifiedLocationTokenWithCompletionHandler(int requestId, CompletionResponseDict response);


        private static event Action<(int requestId, string statusStr, string jsonStr)> OnResponse;

        /// <summary>
        /// Server response function
        /// </summary>
        [MonoPInvokeCallback(typeof(CompletionResponseDict))]
        private static void TrackVerifiedResponseCallback(int requestId, string statusStr, string jsonStr)
        {
            OnResponse?.Invoke((requestId, statusStr, jsonStr));
        }

        private readonly TaskCompletionSource<(RadarStatus, VerifiedLocationData?)> _currentTcs;
        public Task<(RadarStatus, VerifiedLocationData?)> CompletionTask => _currentTcs.Task;
        private readonly int _id;
        private readonly RadarRequestType _requestType;


        public IosTrackVerifiedHandler(RadarRequestType requestType)
        {
            _id = GetHashCode();
            _requestType = requestType;

            _currentTcs = new TaskCompletionSource<(RadarStatus, VerifiedLocationData?)>();

            OnResponse += ResponseReceiveCallback;

            // Send the appropriate request based on the request type
            if (_requestType == RadarRequestType.TrackVerified)
            {
                Radar_trackVerifiedWithCompletionHandler(_id, TrackVerifiedResponseCallback);
            }
            else if (_requestType == RadarRequestType.GetVerifiedLocationToken)
            {
                Radar_getVerifiedLocationTokenWithCompletionHandler(_id, TrackVerifiedResponseCallback);
            }
        }

        private void ResponseReceiveCallback((int requestId, string statusStr, string jsonStr) r)
        {
            if (r.requestId != _id)
            {
                return;
            }

            OnResponse -= ResponseReceiveCallback;

            if (_currentTcs == null || _currentTcs.Task.IsCompleted)
            {
                return;
            }

            var status = Utils.StatusStringToEnum(r.statusStr);
            if (status == RadarStatus.SUCCESS)
            {
                _currentTcs.TrySetResult((status, Utils.GetTrackDataFromJson(r.jsonStr)));
            }
            else
            {
                _currentTcs.TrySetResult((status, null));
            }
        }
    }
}

#endif