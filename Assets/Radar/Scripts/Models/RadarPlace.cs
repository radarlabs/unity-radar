using UnityEngine;
using System.Collections.Generic;

namespace RadarSDK
{
    /// <summary>
    /// Represents a place.
    /// </summary>
    [System.Serializable]
    public class RadarPlace
    {
        [SerializeField] private string _id;
        [SerializeField] private string name;
        [SerializeField] private string[] categories;
        [SerializeField] private RadarChain chain;
        [SerializeField] private RadarCoordinate location;
        [SerializeField] private string group;
        [SerializeField] private JSONObject metadata;
        [SerializeField] private RadarAddress address;

        public string Id { get => _id; set => _id = value; }
        public string Name { get => name; set => name = value; }
        public IEnumerable<string> Categories { get => categories; set => categories = value as string[]; }
        public RadarChain Chain { get => chain; set => chain = value; }
        public RadarCoordinate Location { get => location; set => location = value; }
        public string Group { get => group; set => group = value; }
        public JSONObject Metadata { get => metadata; set => metadata = value; }
        public RadarAddress Address { get => address; set => address = value; }
    }
}
