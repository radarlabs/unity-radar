#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace RadarSDK.iOS.Editor
{
    public static class RadarPostProcessIOS
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            AddSslPinning(pathToBuiltProject);
        }

        private static void AddSslPinning(string pathToBuiltProject)
        {
            // Get the existing key if it exists; if not, create a new one.
            PlistElementDict getKey(PlistElementDict dict, string key)
            {
                return dict.values.ContainsKey(key) ? dict[key].AsDict() : dict.CreateDict(key);
            }

            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            var appTransportSecurity = getKey(rootDict, "NSAppTransportSecurity");
            appTransportSecurity.SetBoolean("NSAllowsArbitraryLoads", false);
            var pinnedDomains = getKey(appTransportSecurity, "NSPinnedDomains");
            var radarDomain = getKey(pinnedDomains, "api-verified.radar.io");

            // override all radar entries
            radarDomain.SetBoolean("NSIncludesSubdomains", true);
            var pinnedLeafIdentities = radarDomain.CreateArray("NSPinnedLeafIdentities");

            string spkiSha256Base64 = "15ktYXSSU2llpy7YyCgeqUKDBkjcimK/weUcec960sI=";
            for (int i = 0; i < 2; i++)
            {
                PlistElementDict spkiDict = pinnedLeafIdentities.AddDict();
                spkiDict.SetString("SPKI-SHA256-BASE64", spkiSha256Base64);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif