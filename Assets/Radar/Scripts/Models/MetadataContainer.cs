// using UnityEngine;
// using System.Collections.Generic;

// namespace RadarSDK
// {
//     [CreateAssetMenu(fileName = "MetadataContainer", menuName = "RadarSDK/Create Metadata Container")]
//     public class MetadataContainer : ScriptableObject
//     {
//         [System.Serializable]
//         public class MetadataEntry
//         {
//             public string Key;
//             public object Value;
//         }

//         public List<MetadataEntry> Entries = new List<MetadataEntry>();

//         public Dictionary<string, object> ToDictionary()
//         {
//             var dictionary = new Dictionary<string, object>();
//             foreach (var entry in Entries)
//             {
//                 // if (!dictionary.ContainsKey(entry.Key))
//                 // {
//                 dictionary[entry.Key] = entry.Value;
//                 // }
//             }
//             return dictionary;
//         }

//     }
// }