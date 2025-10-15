using System;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents timezone information.
    /// </summary>
    [System.Serializable]
    public class RadarTimeZone
    {
        [SerializeField] private string name;
        [SerializeField] private string code;
        [SerializeField] private string currentTime;
        [SerializeField] private int utcOffset;
        [SerializeField] private int dstOffset;

        public string Name { get => name; set => name = value; }
        public string Code { get => code; set => code = value; }
        public DateTime? CurrentTime
        {
            get => DateTime.TryParse(currentTime, out DateTime result) ? result : (DateTime?)null;
            set => currentTime = value?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
        public int UtcOffset { get => utcOffset; set => utcOffset = value; }
        public int DstOffset { get => dstOffset; set => dstOffset = value; }
    }
}
