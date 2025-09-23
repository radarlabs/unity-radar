using UnityEngine;
using System.Collections.Generic;

namespace RadarSDK
{
    [CreateAssetMenu(fileName = "RadarSettings", menuName = "Radar/Create Radar Settings")]
    public class RadarSettingsData : ScriptableObject
    {
        [Tooltip("Unique identifier for the user, required for tracking purposes")]
        public string userId = "DefaultUserId";

        [Tooltip("Put your test publishable key here. Used in Development Builds")]
        public string testPublishableKey = "prj_test_pk_0000000000000000000000000000000000000000";

        [Tooltip("Put your live publishable key here. Used in Release Builds")]
        public string livePublishableKey = "prj_live_pk_0000000000000000000000000000000000000000";

        [Tooltip("Option to add an extension to the userId depending on the platform (e.g., '_Android')")]
        public bool addUserIdExtension = true;

        [Tooltip("Enable debugging to show logs in the console")]
        public bool enableDebugging = true;

        [Tooltip("Toggle to enable or disable beacon usage in tracking")]
        public bool useBeacons = true;

        [Tooltip("Interval in seconds for tracking updates")]
        public int trackingInterval = 60;

        // [Tooltip("Metadata container to store/pass additional information")]
        // public MetadataContainer metadata;
    }
}
