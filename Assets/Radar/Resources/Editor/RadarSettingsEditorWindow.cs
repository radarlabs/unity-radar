using UnityEditor;
using UnityEngine;
using RadarSDK;
using System.IO;

public class RadarSettingsEditorWindow : EditorWindow
{
    private RadarSettingsData radarSettings;
    private string settingsPath;
    private string metadataPath;

    private Editor metadataEditor;

    [MenuItem("Radar/Settings")]
    public static void ShowWindow()
    {
        GetWindow<RadarSettingsEditorWindow>("Radar SDK Settings");
    }


    private void OnEnable()
    {
        string path = GetScriptFolderPath();

        // Set the settings path relative to the "Settings" folder
        settingsPath = Path.Combine(path, "RadarSettings.asset");
        metadataPath = Path.Combine(path, "MetadataContainer.asset");

        // Load the RadarSettings asset, or create one if it doesn't exist
        radarSettings = AssetDatabase.LoadAssetAtPath<RadarSettingsData>(settingsPath);
        if (radarSettings == null)
        {
            CreateSettingsAsset();
        }
    }


    private string GetScriptFolderPath()
    {
        // Get the script path using AssetDatabase
        string scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));

        // Get the directory of the script
        string scriptDirectory = Path.GetDirectoryName(scriptFilePath);

        // Move two directories back
        string twoDirectoriesBack = Directory.GetParent(Directory.GetParent(scriptDirectory).FullName).FullName;

        // Convert the full path back to a relative path that Unity understands
        int assetsIndex = twoDirectoriesBack.IndexOf("Assets");
        string relativePath = twoDirectoriesBack.Substring(assetsIndex);

        // Ensure the Resources folder exists
        string resourcesFolderPath = Path.Combine(relativePath, "Resources");
        if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
        {
            AssetDatabase.CreateFolder(relativePath, "Resources");
        }

        // Ensure the SO folder exists inside Resources
        string soFolderPath = Path.Combine(resourcesFolderPath, "Settings");
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            AssetDatabase.CreateFolder(resourcesFolderPath, "Settings");
        }

        // Return the path to the SO folder
        return soFolderPath;
    }



    private void OnGUI()
    {
        if (radarSettings == null) return;

        GUILayout.Label("Radar SDK Settings", EditorStyles.boldLabel); // Header

        radarSettings.userId = EditorGUILayout.TextField("User ID", radarSettings.userId);

        radarSettings.trackingInterval = EditorGUILayout.IntSlider("Tracking Interval (seconds)", radarSettings.trackingInterval, 10, 3600);

        radarSettings.useBeacons = EditorGUILayout.Toggle("Use Beacons", radarSettings.useBeacons);

        EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);

        // Check if radarSettings.metadata is null, if so create a new one
        if (radarSettings.metadata == null)
        {
            if (GUILayout.Button("Create Metadata"))
            {
                radarSettings.metadata = CreateInstance<MetadataContainer>();
                AssetDatabase.CreateAsset(radarSettings.metadata, metadataPath);
                AssetDatabase.SaveAssets();
                Debug.Log("New MetadataContainer created and assigned.");
            }
        }
        else
        {
            // If metadata exists, create an editor for it and display its fields
            if (metadataEditor == null || metadataEditor.target != radarSettings.metadata)
            {
                metadataEditor = Editor.CreateEditor(radarSettings.metadata);
            }

            if (metadataEditor != null)
            {
                metadataEditor.OnInspectorGUI();
            }
        }

        if (GUILayout.Button("Save Settings"))
        {
            SaveSettings();
        }

        EditorUtility.SetDirty(radarSettings); // Apply changes to the ScriptableObject
    }

    private void CreateSettingsAsset()
    {
        radarSettings = CreateInstance<RadarSettingsData>();
        AssetDatabase.CreateAsset(radarSettings, settingsPath);
        AssetDatabase.SaveAssets();
    }

    private void SaveSettings()
    {
        EditorUtility.SetDirty(radarSettings);
        AssetDatabase.SaveAssets();
        Debug.Log("Radar SDK settings saved.");
    }
}
