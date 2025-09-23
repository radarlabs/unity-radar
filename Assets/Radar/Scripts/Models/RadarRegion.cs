using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a region.
    /// Check out the <a href="https://radar.com/documentation/regions">regions documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public struct RadarRegion
    {
        [SerializeField] private string _id;
        [SerializeField] private string name;
        [SerializeField] private string code;
        [SerializeField] private string type;
        [SerializeField] private string flag;
        [SerializeField] private bool allowed;
        [SerializeField] private bool passed;
        [SerializeField] private bool inExclusionZone;
        [SerializeField] private bool inBufferZone;
        [SerializeField] private double distanceToBorder;

        /// <summary>
        /// The Radar ID of the region.
        /// </summary>
        public string Id { get => _id; set => _id = value; }

        /// <summary>
        /// The name of the region.
        /// </summary>
        public string Name { get => name; set => name = value; }

        /// <summary>
        /// The unique code for the region.
        /// </summary>
        public string Code { get => code; set => code = value; }

        /// <summary>
        /// The type of the region.
        /// </summary>
        public string Type { get => type; set => type = value; }

        /// <summary>
        /// The optional flag of the region.
        /// </summary>
        public string Flag { get => flag; set => flag = value; }

        /// <summary>
        /// A boolean indicating whether the jurisdiction is allowed. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool Allowed { get => allowed; set => allowed = value; }

        /// <summary>
        /// A boolean indicating whether all jurisdiction checks for the region have passed. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool Passed { get => passed; set => passed = value; }

        /// <summary>
        /// A boolean indicating whether the user is in an exclusion zone for the jurisdiction. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool InExclusionZone { get => inExclusionZone; set => inExclusionZone = value; }

        /// <summary>
        /// A boolean indicating whether the user is too close to the border for the jurisdiction. May be `false` if Fraud is not enabled.
        /// </summary>
        public bool InBufferZone { get => inBufferZone; set => inBufferZone = value; }

        /// <summary>
        /// The distance in meters to the border of the jurisdiction. May be 0 if Fraud is not enabled.
        /// </summary>
        public double DistanceToBorder { get => distanceToBorder; set => distanceToBorder = value; }
    }
}