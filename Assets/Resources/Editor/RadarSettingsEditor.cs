using UnityEditor;
using UnityEngine;
using RadarSDK;

[CustomEditor(typeof(RadarSettings))]
public class RadarSettingsEditor : Editor
{
    private SerializedProperty userIdProp;
    private SerializedProperty trackingIntervalProp;
    private SerializedProperty useBeaconsProp;
    private SerializedProperty metadataProp;

    private void OnEnable()
    {
        // Link the serialized properties to the fields in RadarSettings
        userIdProp = serializedObject.FindProperty("userId");
        trackingIntervalProp = serializedObject.FindProperty("trackingInterval");
        useBeaconsProp = serializedObject.FindProperty("useBeacons");
        metadataProp = serializedObject.FindProperty("metadata");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object before showing the custom UI
        serializedObject.Update();

        // Custom editor UI for RadarSettings
        EditorGUILayout.LabelField("Radar SDK Settings", EditorStyles.boldLabel);

        // Input field for the userId
        EditorGUILayout.PropertyField(userIdProp, new GUIContent("User ID"));

        // Slider for tracking interval
        EditorGUILayout.IntSlider(trackingIntervalProp, 10, 3600, new GUIContent("Tracking Interval (seconds)"));

        // Toggle for using beacons
        EditorGUILayout.PropertyField(useBeaconsProp, new GUIContent("Use Beacons"));

        // Metadata section (customizable by users)
        EditorGUILayout.PropertyField(metadataProp, new GUIContent("Metadata"), true);

        // Buttons to control Radar actions from the Editor
        if (GUILayout.Button("Start User Tracking"))
        {
            ((RadarSettings)target).StartUserTracking();
        }

        if (GUILayout.Button("Start Tracking"))
        {
            ((RadarSettings)target).StartTracking();
        }

        if (GUILayout.Button("Stop Tracking"))
        {
            ((RadarSettings)target).StopTracking();
        }

        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
