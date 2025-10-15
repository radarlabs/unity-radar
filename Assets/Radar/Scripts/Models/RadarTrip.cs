using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a trip.
    /// </summary>
    [System.Serializable]
    public class RadarTrip
    {
        [SerializeField] private string _id;
        [SerializeField] private string externalId;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private string destinationGeofenceTag;
        [SerializeField] private string destinationGeofenceExternalId;
        [SerializeField] private RadarCoordinate destinationLocation;
        [SerializeField] private RadarRouteMode? mode;
        [SerializeField] private double etaDistance;
        [SerializeField] private double etaDuration;
        [SerializeField] private RadarTripStatus status;

        public string Id { get => _id; set => _id = value; }
        public string ExternalId { get => externalId; set => externalId = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public string DestinationGeofenceTag { get => destinationGeofenceTag; set => destinationGeofenceTag = value; }
        public string DestinationGeofenceExternalId { get => destinationGeofenceExternalId; set => destinationGeofenceExternalId = value; }
        public RadarCoordinate DestinationLocation { get => destinationLocation; set => destinationLocation = value; }
        public RadarRouteMode? Mode { get => mode; set => mode = value; }
        public double EtaDistance { get => etaDistance; set => etaDistance = value; }
        public double EtaDuration { get => etaDuration; set => etaDuration = value; }
        public RadarTripStatus Status { get => status; set => status = value; }
    }
}
