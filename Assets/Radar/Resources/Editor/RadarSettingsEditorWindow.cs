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

        settingsPath = Path.Combine(path, "RadarSettings.asset");
        metadataPath = Path.Combine(path, "MetadataContainer.asset");

        radarSettings = AssetDatabase.LoadAssetAtPath<RadarSettingsData>(settingsPath);
        if (radarSettings == null)
        {
            CreateSettingsAsset();
        }
    }

    private string GetScriptFolderPath()
    {
        string scriptFilePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        string scriptDirectory = Path.GetDirectoryName(scriptFilePath);
        string twoDirectoriesBack = Directory.GetParent(Directory.GetParent(scriptDirectory).FullName).FullName;
        int assetsIndex = twoDirectoriesBack.IndexOf("Assets");
        string relativePath = twoDirectoriesBack.Substring(assetsIndex);

        string resourcesFolderPath = Path.Combine(relativePath, "Resources");
        if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
        {
            AssetDatabase.CreateFolder(relativePath, "Resources");
        }

        string soFolderPath = Path.Combine(resourcesFolderPath, "Settings");
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            AssetDatabase.CreateFolder(resourcesFolderPath, "Settings");
        }

        return soFolderPath;
    }

    private void OnGUI()
    {
        if (radarSettings == null) return;

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.cyan }
        };

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            normal = { textColor = Color.white }
        };

        GUIStyle greyLabelStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = Color.gray }
        };

        GUIStyle saveButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.green }
        };

        GUILayout.Space(10);
        GUILayout.Label("Radar SDK Settings", headerStyle);
        GUILayout.Space(10);

        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("General Settings", sectionStyle);

        radarSettings.userId = EditorGUILayout.TextField(new GUIContent("User ID", "Unique identifier for the user, required for tracking purposes."), radarSettings.userId);

        EditorGUILayout.BeginHorizontal();
        radarSettings.addUserIdExtension = EditorGUILayout.Toggle(new GUIContent("Add Extension", "Option to add an extension to the userId (e.g., '_Android')."), radarSettings.addUserIdExtension);

        // Display the full user ID with extension if addUserIdExtension is checked
        if (radarSettings.addUserIdExtension)
        {
            string fullUserId = $"{radarSettings.userId}_{Application.platform}";
            EditorGUILayout.LabelField(fullUserId, greyLabelStyle, GUILayout.Width(200));
        }
        
        EditorGUILayout.EndHorizontal();

        radarSettings.enableDebugging = EditorGUILayout.Toggle(new GUIContent("Enable Debugging", "Enable debugging to show logs in the console."), radarSettings.enableDebugging);

        radarSettings.testPublishableKey = EditorGUILayout.TextField(new GUIContent("Test Publishable Key", "Test mode publishable key, used for testing purposes."), radarSettings.testPublishableKey);

        radarSettings.livePublishableKey = EditorGUILayout.TextField(new GUIContent("Live Publishable Key", "Live mode publishable key, used in production for live tracking."), radarSettings.livePublishableKey);

        radarSettings.trackingInterval = EditorGUILayout.IntSlider(new GUIContent("Tracking Interval (seconds)", "Interval in seconds for tracking updates."), radarSettings.trackingInterval, 10, 3600);

        radarSettings.useBeacons = EditorGUILayout.Toggle(new GUIContent("Use Beacons", "Toggle to enable or disable beacon usage in tracking."), radarSettings.useBeacons);

        EditorGUILayout.EndVertical();

        GUILayout.Space(15);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Separator line

        // Metadata Section
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Metadata", sectionStyle);

        // Display metadata preview if it exists
        if (radarSettings.metadata != null)
        {
            // Display metadata contents as a read-only preview
            string metadataPreview = JsonUtility.ToJson(radarSettings.metadata, true);
            EditorGUILayout.LabelField(metadataPreview, greyLabelStyle, GUILayout.MaxHeight(100));
        }

        // Edit metadata button
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
            if (metadataEditor == null || metadataEditor.target != radarSettings.metadata)
            {
                metadataEditor = Editor.CreateEditor(radarSettings.metadata);
            }

            if (GUILayout.Button("Edit Metadata", GUILayout.Height(30)))
            {
                Selection.activeObject = radarSettings.metadata;
            }
        }

        EditorGUILayout.EndVertical();

        GUILayout.Space(15);

        // Save button
        if (GUILayout.Button("Save Settings", saveButtonStyle))
        {
            SaveSettings();
        }

        GUILayout.Space(10);

        // Create label for the clickable link
        GUILayout.Label("Unity SDK Documentation", EditorStyles.linkLabel);

        // Add cursor change to indicate clickable area
        var rect = GUILayoutUtility.GetLastRect();
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

        // Detect mouse click on the label
        if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
        {
            Application.OpenURL("https://radar.com/documentation/sdk/unity"); // Replace with your URL
        }

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
