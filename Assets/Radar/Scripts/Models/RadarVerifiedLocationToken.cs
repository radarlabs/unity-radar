using System;
using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a user's verified location.
    /// Check out the <a href="https://radar.com/documentation/fraud">fraud documentation</a> for more information.
    /// </summary>
    [Serializable]
    public class RadarVerifiedLocationToken
    {
        [SerializeField] private string _id;
        [SerializeField] private RadarUser user;
        [SerializeField] private RadarEvent[] events;
        [SerializeField] private string token;
        [SerializeField] private string expiresAt;
        [SerializeField] private double expiresIn;
        [SerializeField] private bool passed;
        [SerializeField] private string[] failureReasons;

        public RadarUser User { get => user; set => user = value; }
        public IEnumerable<RadarEvent> Events { get => events; set => events = value as RadarEvent[]; }
        public string Token { get => token; set => token = value; }
        public DateTime? ExpiresAt
        {
            get => DateTime.TryParse(expiresAt, out DateTime result) ? result : (DateTime?)null;
            set => expiresAt = value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public double ExpiresIn { get => expiresIn; set => expiresIn = value; }
        public bool Passed { get => passed; set => passed = value; }
        public IEnumerable<string> FailureReasons { get => failureReasons; set => failureReasons = value as string[]; }

        public override string ToString()
        {
            return $"id: {_id}, Token: {Token?.Substring(0, Math.Min(5, Token?.Length ?? 0)) + "..."}, Passed: {Passed}, ExpiresAt: {ExpiresAt}, ExpiresIn: {ExpiresIn}";
        }
    }
}