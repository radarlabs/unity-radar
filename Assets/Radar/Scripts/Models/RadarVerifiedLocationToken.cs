namespace RadarSDK
{
    /// <summary>
    /// Represents a user's verified location.
    /// Check out the <a href="https://radar.com/documentation/fraud">fraud documentation</a> for more information.
    /// </summary>
    [System.Serializable]
    public class RadarVerifiedLocationToken
    {
        public User User;
        public bool Passed { get; set; }
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
        public long ExpiresIn { get; set; }


        public override string ToString()
        {
            return $"Token: {Token}, Passed: {Passed}, ExpiresAt: {ExpiresAt}, ExpiresIn: {ExpiresIn}";
        }

    }
}