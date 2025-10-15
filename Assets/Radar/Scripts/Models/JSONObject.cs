using System.Collections.Generic;
using UnityEngine;

namespace RadarSDK
{
    /// <summary>
    /// Represents a JSON object for metadata storage.
    /// </summary>
    [System.Serializable]
    public class JSONObject : Dictionary<string, object>
    {
        public JSONObject() : base() { }
        public JSONObject(IDictionary<string, object> dictionary) : base(dictionary) { }
    }
}
