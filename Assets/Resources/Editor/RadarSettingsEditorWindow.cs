using UnityEditor;
using UnityEngine;
using RadarSDK;

public class RadarSettingsEditorWindow : EditorWindow
{
    private RadarSettingsData radarSettings;
    private string settingsPath = "Assets/RadarSettings.asset";

    private Editor metadataEditor;

    [MenuItem("RadarSDK/Settings")]
    public static void ShowWindow()
    {
        GetWindow<RadarSettingsEditorWindow>("Radar SDK Settings");
    }

    private void OnEnable()
    {
        // Load the RadarSettings asset, or create one if it doesn't exist
        radarSettings = AssetDatabase.LoadAssetAtPath<RadarSettingsData>(settingsPath);
        if (radarSettings == null)
        {
            CreateSettingsAsset();
        }
    }

    private void OnGUI()
    {
        if (radarSettings == null) return;

        // Header
        GUILayout.Label("Radar SDK Settings", EditorStyles.boldLabel);

        // User ID
        radarSettings.userId = EditorGUILayout.TextField("User ID", radarSettings.userId);

        // Tracking Interval
        radarSettings.trackingInterval = EditorGUILayout.IntSlider("Tracking Interval (seconds)", radarSettings.trackingInterval, 10, 3600);

        // Use Beacons
        radarSettings.useBeacons = EditorGUILayout.Toggle("Use Beacons", radarSettings.useBeacons);

        // Metadata
        EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);

        // Check if radarSettings.metadata is null, if so create a new one
        if (radarSettings.metadata == null)
        {
            if (GUILayout.Button("Create Metadata"))
            {
                radarSettings.metadata = CreateInstance<MetadataContainer>();
                AssetDatabase.CreateAsset(radarSettings.metadata, "Assets/MetadataContainer.asset");
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

        // Save the modified settings
        if (GUILayout.Button("Save Settings"))
        {
            SaveSettings();
        }

        // Apply changes to the ScriptableObject
        EditorUtility.SetDirty(radarSettings);
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
