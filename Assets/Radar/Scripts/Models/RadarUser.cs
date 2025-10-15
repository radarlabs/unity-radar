using System;
using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents the current user state.
    /// Check out the <a href="https://radar.com/documentation">documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public class RadarUser
    {
        [SerializeField] private string _id;
        [SerializeField] private string userId;
        [SerializeField] private string deviceId;
        [SerializeField] private string description;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarLocation location;
        [SerializeField] private RadarActivityType activityType;
        [SerializeField] private RadarGeofence[] geofences;
        [SerializeField] private RadarPlace place;
        [SerializeField] private RadarBeacon[] beacons;
        [SerializeField] private bool stopped;
        [SerializeField] private bool foreground;
        [SerializeField] private RadarRegion country;
        [SerializeField] private RadarRegion state;
        [SerializeField] private RadarRegion dma;
        [SerializeField] private RadarRegion postalCode;
        [SerializeField] private RadarChain[] nearbyPlaceChains;
        [SerializeField] private RadarSegment[] segments;
        [SerializeField] private RadarChain[] topChains;
        [SerializeField] private RadarLocationSource source;
        [SerializeField] private RadarTrip trip;
        [SerializeField] private RadarFraud fraud;
        [SerializeField] private bool debug;

        public string Id { get => _id; set => _id = value; }
        public string UserId { get => userId; set => userId = value; }
        public string DeviceId { get => deviceId; set => deviceId = value; }
        public string Description { get => description; set => description = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarLocation Location { get => location; set => location = value; }
        public RadarActivityType ActivityType { get => activityType; set => activityType = value; }
        public IEnumerable<RadarGeofence> Geofences { get => geofences; set => geofences = value as RadarGeofence[]; }
        public RadarPlace Place { get => place; set => place = value; }
        public IEnumerable<RadarBeacon> Beacons { get => beacons; set => beacons = value as RadarBeacon[]; }
        public bool Stopped { get => stopped; set => stopped = value; }
        public bool Foreground { get => foreground; set => foreground = value; }
        public RadarRegion Country { get => country; set => country = value; }
        public RadarRegion State { get => state; set => state = value; }
        public RadarRegion DMA { get => dma; set => dma = value; }
        public RadarRegion PostalCode { get => postalCode; set => postalCode = value; }
        public IEnumerable<RadarChain> NearbyPlaceChains { get => nearbyPlaceChains; set => nearbyPlaceChains = value as RadarChain[]; }
        public IEnumerable<RadarSegment> Segments { get => segments; set => segments = value as RadarSegment[]; }
        public IEnumerable<RadarChain> TopChains { get => topChains; set => topChains = value as RadarChain[]; }
        public RadarLocationSource Source { get => source; set => source = value; }
        public RadarTrip Trip { get => trip; set => trip = value; }
        public RadarFraud Fraud { get => fraud; set => fraud = value; }
        public bool Debug { get => debug; set => debug = value; }
    }
}