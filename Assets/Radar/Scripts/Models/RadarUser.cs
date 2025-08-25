using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents the current user state.
    /// Check out the <a href="https://radar.com/documentation">documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarUser
    {
        [SerializeField] private string _id;
        [SerializeField] private string userId;
        [SerializeField] private string deviceId;
        [SerializeField] private RadarLocation location;
        [SerializeField] private bool stopped;
        [SerializeField] private bool foreground;
        [SerializeField] private RadarRegion country;
        [SerializeField] private RadarRegion state;
        [SerializeField] private string source;
        [SerializeField] private bool debug;
        [SerializeField] private RadarFraud fraud;

        /// <summary>
        /// The Radar ID of the user.
        /// </summary>
        public string Id { get => _id; set => _id = value; }

        /// <summary>
        /// The unique ID of the user, provided when you identified the user. May be `null` if the user has not been identified.
        /// </summary>
        public string UserId { get => userId; set => userId = value; }

        /// <summary>
        /// The device ID of the user.
        /// </summary>
        public string DeviceId { get => deviceId; set => deviceId = value; }

        /// <summary>
        /// The user's current location.
        /// </summary>
        public RadarLocation Location { get => location; set => location = value; }

        /// <summary>
        /// A boolean indicating whether the user is stopped.
        /// </summary>
        public bool Stopped { get => stopped; set => stopped = value; }

        /// <summary>
        /// A boolean indicating whether the user was last updated in the foreground.
        /// </summary>
        public bool Foreground { get => foreground; set => foreground = value; }

        /// <summary>
        /// The user's current country. May be `null` if the country is not available or if Regions is not enabled.
        /// </summary>
        public RadarRegion Country { get => country; set => country = value; }

        /// <summary>
        /// The user's current state. May be `null` if the state is not available or if Regions is not enabled. See <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
        /// </summary>
        public RadarRegion State { get => state; set => state = value; }

        /// <summary>
        /// The source of the user's current location.
        /// </summary>
        public string Source { get => source; set => source = value; }

        /// <summary>
        /// A boolean indicating whether the user has been "Marked as Debug" in the dashboard.
        /// </summary>
        public bool Debug { get => debug; set => debug = value; }

        /// <summary>
        /// The user's current fraud state. May be `null` if fraud detection is not enabled.
        /// </summary>
        public RadarFraud Fraud { get => fraud; set => fraud = value; }

        // The commented out fields are not currently in use.
        // Prefer to leave them commented out due to case sensitivity.

        /// <summary>
        /// The optional description of the user.
        /// </summary>
        //public string description;
    }
}