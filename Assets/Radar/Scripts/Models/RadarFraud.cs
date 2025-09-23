using UnityEngine;

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
        [SerializeField] private bool passed;
        [SerializeField] private bool bypassed;
        [SerializeField] private bool verified;
        [SerializeField] private bool proxy;
        [SerializeField] private bool mocked;
        [SerializeField] private bool compromised;
        [SerializeField] private bool jumped;
        [SerializeField] private bool sharing;
        [SerializeField] private bool inaccurate;
        [SerializeField] private bool blocked;

        /// <summary>
        /// A boolean indicating whether the user passed fraud detection checks. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Passed { get => passed; set => passed = value; }

        /// <summary>
        /// A boolean indicating whether fraud detection checks were bypassed for the user for testing. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Bypassed { get => bypassed; set => bypassed = value; }

        /// <summary>
        /// A boolean indicating whether the request was made with SSL pinning configured successfully. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Verified { get => verified; set => verified = value; }

        /// <summary>
        /// A boolean indicating whether the user's IP address is a known proxy. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Proxy { get => proxy; set => proxy = value; }

        /// <summary>
        /// A boolean indicating whether the user's location is being mocked, such as in a simulator or using a location spoofing app.
        /// May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Mocked { get => mocked; set => mocked = value; }

        /// <summary>
        /// A boolean indicating whether the user's device has been compromised according to the Play Integrity API. May be `false` if fraud detection is not enabled.
        /// Check out the <a href="https://developer.android.com/google/play/integrity/overview">integrity overview</a> for more information.
        /// </summary>
        public bool Compromised { get => compromised; set => compromised = value; }

        /// <summary>
        /// A boolean indicating whether the user moved too far too fast. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Jumped { get => jumped; set => jumped = value; }

        /// <summary>
        /// A boolean indicating whether the user is screen sharing. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Sharing { get => sharing; set => sharing = value; }

        /// <summary>
        /// A boolean indicating whether the user's location is not accurate enough. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Inaccurate { get => inaccurate; set => inaccurate = value; }

        /// <summary>
        /// A boolean indicating whether the user has been manually blocked. May be `false` if fraud detection is not enabled.
        /// </summary>
        public bool Blocked { get => blocked; set => blocked = value; }
    }
}