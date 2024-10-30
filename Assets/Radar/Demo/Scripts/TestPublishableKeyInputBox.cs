using UnityEngine;
using TMPro;
using RadarSDKBridge;

namespace RadarSDK
{
    public class TestPublishableKeyInputBox : MonoBehaviour
    {
        public TMP_InputField keyInputField;



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
            RadarServiceWrapper.Initialize();
        }

        // Called when "Reset" button is pressed
        public void ResetToDefault()
        {
            RadarSDKManager.SaveOverrideTestPublishableKey(RadarSDKManager.OriginalTestPublishableKey);
            UpdateText();
        }
    }
}