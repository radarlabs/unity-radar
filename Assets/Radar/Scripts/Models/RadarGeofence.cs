using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a geofence.
    /// </summary>
    [System.Serializable]
    public class RadarGeofence
    {
        [SerializeField] private string _id;
        [SerializeField] private string description;
        [SerializeField] private string tag;
        [SerializeField] private string externalId;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarOperatingHours operatingHours;
        [SerializeField] private RadarGeofenceGeometry geometry;

        public string Id { get => _id; set => _id = value; }
        public string Description { get => description; set => description = value; }
        public string Tag { get => tag; set => tag = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarOperatingHours OperatingHours { get => operatingHours; set => operatingHours = value; }
        public RadarGeofenceGeometry Geometry { get => geometry; set => geometry = value; }
    }
}
