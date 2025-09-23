using System;
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
        [SerializeField] string _id;
        [SerializeField] RadarUser user;
        [SerializeField] string token;
        [SerializeField] string expiresAt;
        [SerializeField] long expiresIn;


        public RadarUser User { get => user; set => user = value; }
        public bool Passed { get; set; }
        public string Token { get => token; set => token = value; }
        public DateTime ExpiresAt
        {
            get => DateTime.TryParse(expiresAt, out DateTime result) ? result : DateTime.MinValue;
            set => expiresAt = value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public long ExpiresIn { get => expiresIn; set => expiresIn = value; }


        public override string ToString()
        {
            return $"id: {_id}, Token: {Token.Substring(0, 5) + "..."}, Passed: {Passed}, ExpiresAt: {ExpiresAt}, ExpiresIn: {ExpiresIn}";
        }

    }
}