using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a chain.
    /// </summary>
    [System.Serializable]
    public class RadarChain
    {
        [SerializeField] private string slug;
        [SerializeField] private string name;
        [SerializeField] private string externalId;
        [SerializeField] private JSONObject metadata;

        public string Slug { get => slug; set => slug = value; }
        public string Name { get => name; set => name = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
    }
}
