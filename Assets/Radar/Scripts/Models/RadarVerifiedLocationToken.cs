namespace RadarSDK
{
    public class RadarVerifiedLocationToken
    {
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