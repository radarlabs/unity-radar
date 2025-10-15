using System;
using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    [System.Serializable]
    public class RadarEvent
    {
        [SerializeField] private string _id;
        [SerializeField] private string createdAt;
        [SerializeField] private string actualCreatedAt;
        [SerializeField] private bool live;
        [SerializeField] private RadarEventType type;
        [SerializeField] private string conversionName;
        [SerializeField] private RadarGeofence geofence;
        [SerializeField] private RadarPlace place;
        [SerializeField] private RadarRegion region;
        [SerializeField] private RadarBeacon beacon;
        [SerializeField] private RadarTrip trip;
        [SerializeField] private RadarPlace[] alternatePlaces;
        [SerializeField] private RadarPlace verifiedPlace;
        [SerializeField] private RadarEventVerification verification;
        [SerializeField] private RadarEventConfidence confidence;
        [SerializeField] private float duration;
        [SerializeField] private RadarLocation location;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarFraud fraud;
        [SerializeField] private bool replayed;

        public string Id { get => _id; set => _id = value; }
        public DateTime? CreatedAt
        {
            get => DateTime.TryParse(createdAt, out DateTime result) ? result : (DateTime?)null;
            set => createdAt = value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public DateTime? ActualCreatedAt
        {
            get => DateTime.TryParse(actualCreatedAt, out DateTime result) ? result : (DateTime?)null;
            set => actualCreatedAt = value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public bool Live { get => live; set => live = value; }
        public RadarEventType Type { get => type; set => type = value; }
        public string ConversionName { get => conversionName; set => conversionName = value; }
        public RadarGeofence Geofence { get => geofence; set => geofence = value; }
        public RadarPlace Place { get => place; set => place = value; }
        public RadarRegion Region { get => region; set => region = value; }
        public RadarBeacon Beacon { get => beacon; set => beacon = value; }
        public RadarTrip Trip { get => trip; set => trip = value; }
        public IEnumerable<RadarPlace> AlternatePlaces { get => alternatePlaces; set => alternatePlaces = value as RadarPlace[]; }
        public RadarPlace VerifiedPlace { get => verifiedPlace; set => verifiedPlace = value; }
        public RadarEventVerification Verification { get => verification; set => verification = value; }
        public RadarEventConfidence Confidence { get => confidence; set => confidence = value; }
        public float Duration { get => duration; set => duration = value; }
        public RadarLocation Location { get => location; set => location = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarFraud Fraud { get => fraud; set => fraud = value; }
        public bool Replayed { get => replayed; set => replayed = value; }
    }
}
