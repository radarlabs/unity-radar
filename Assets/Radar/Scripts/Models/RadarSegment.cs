using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a segment.
    /// </summary>
    [System.Serializable]
    public class RadarSegment
    {
        [SerializeField] private string description;
        [SerializeField] private string externalId;

        public string Description { get => description; set => description = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
    }
}
