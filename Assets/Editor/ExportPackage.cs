using UnityEditor;
using UnityEngine;

public class ExportPackage
{
    [MenuItem("Tools/Export Package")]
    public static void Run()
    {
        string packagePath = "RadarUnitySDK.unitypackage";
        string[] assetPaths = new string[]
        {
            "Assets/ExternalDependencyManager",   // Include relevant paths
            "Assets/Plugins",
            "Assets/Radar",
            "Assets/Settings",
            "Assets/TextMesh Pro",
            "Packages/com.unity.textmeshpro"
        };

        AssetDatabase.ExportPackage(assetPaths, packagePath, ExportPackageOptions.Recurse);
        Debug.Log("Unity package exported successfully to: " + packagePath);
    }
}
