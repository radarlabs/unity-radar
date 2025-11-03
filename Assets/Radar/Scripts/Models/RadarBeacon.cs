using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a beacon.
    /// </summary>
    [System.Serializable]
    public class RadarBeacon
    {
        [SerializeField] private string _id;
        [SerializeField] private string description;
        [SerializeField] private string tag;
        [SerializeField] private string externalId;
        [SerializeField] private string uuid;
        [SerializeField] private string major;
        [SerializeField] private string minor;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarCoordinate location;
        [SerializeField] private int rssi;

        public string Id { get => _id; set => _id = value; }
        public string Description { get => description; set => description = value; }
        public string Tag { get => tag; set => tag = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public string UUID { get => uuid; set => uuid = value; }
        public string Major { get => major; set => major = value; }
        public string Minor { get => minor; set => minor = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarCoordinate Location { get => location; set => location = value; }
        public int Rssi { get => rssi; set => rssi = value; }
    }
}
