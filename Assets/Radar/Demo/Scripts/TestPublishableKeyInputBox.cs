using UnityEngine;
using UnityEngine.UI;
using RadarSDKBridge;

namespace RadarSDK
{
    public class TestPublishableKeyInputBox : MonoBehaviour
    {
        public InputField keyInputField;



        void Start()
        {
            UpdateText();
        }


        void UpdateText()
        {
            keyInputField.text = RadarSDKManager.TestPublishableKey;
        }

        // Called when "Save" button is pressed
        public void SaveKey()
        {
            // Get the new key from the input field
            string newKey = keyInputField.text;
            
            // Save it as an override using RadarSDKManager
            RadarSDKManager.SaveOverrideTestPublishableKey(newKey);
            
            LogManager.Instance.Log("Override TestPublishableKey saved: " + newKey, LogType.Log);
        }

        // Called when "Update" button is pressed
        public void ReInitialize()
        {
            string publishableKey = Debug.isDebugBuild ? RadarSDKManager.TestPublishableKey : RadarSDKManager.LivePublishableKey;
            Radar.Initialize(publishableKey, fraud: true);
            LogManager.Instance.Log("ReInitialized", LogType.Log);
        }

        // Called when "Reset" button is pressed
        public void ResetToDefault()
        {
            LogManager.Instance.Log("Key has been reset to default value", LogType.Log);
            RadarSDKManager.SaveOverrideTestPublishableKey(RadarSDKManager.OriginalTestPublishableKey);
            UpdateText();
        }
    }
}