using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using RadarSDK;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RadarSDKBridge
{
    /// <summary>
    /// An example class that handles the initialization and setup of Radar SDK in Unity
    /// </summary>
    public class RadarExample : MonoBehaviour
    {
        #region Variables
        private static RadarExample _instance;
        [SerializeField] private string _publishableKey;

        [Header("Status Lights")]
        [SerializeField] private Image _setUserIdImage;
        [SerializeField] private Image _setMetadataImage;
        [SerializeField] private Image _trackVerifiedImage;
        [SerializeField] private Image _startTrackingImage;
        [SerializeField] private Image _stopTrackingImage;
        [SerializeField] private Image _getVerifiedLocationTokenImage;
        [SerializeField] private Image _getLocationImage;

        [Header("Test Buttons")]
        [SerializeField] private Button _setUserIdButton;
        [SerializeField] private Button _verifyTrackButton;
        [SerializeField] private Button _startTrackingButton;
        [SerializeField] private Button _stopTrackingButton;
        [SerializeField] private Button _setMetadataButton;
        [SerializeField] private Button _getVerifiedLocationTokenButton;
        [SerializeField] private Button _getLocationButton;

        [Header("Info Text")]
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _userIdText;
        [SerializeField] private Text _onTokenUpdatedText;
        [SerializeField] private Text _metadataText;
        [SerializeField] private Text _locationText;
        [SerializeField] private Text _jsonText;


        [Header("Visuals")]
        [SerializeField] private Color[] _colors;

        private Coroutine _timeLoadingCoroutine;
        private Coroutine _statusLoadingCoroutine;
        private Coroutine _callbackLoadingCoroutine;

        string userId = String.Empty;

        private readonly Color _redColor = Color.red;
        private readonly Color _orangeColor = new Color(1f, 0.65f, 0f);
        private readonly Color _greenColor = Color.green;

        bool requestBluetoothPermissions = false;

        #endregion


        private void Awake()
        {
            _instance = this;
        }


        private void Start()
        {
            if (requestBluetoothPermissions)
                RequestBluetoothPermissions();

            _getLocationButton.onClick.AddListener(() => GetLocation());
            _verifyTrackButton.onClick.AddListener(() => _ = TrackVerified());
            _startTrackingButton.onClick.AddListener(() => StartTrackingVerified());
            _stopTrackingButton.onClick.AddListener(() => StopTracking());
            _getVerifiedLocationTokenButton.onClick.AddListener(() => _ = GetVerifiedLocationToken());

            // Setup Radar SDK
            Radar.Initialize(_publishableKey);
            Radar.UserId = "test_user_unity";
            Radar.Metadata = new Dictionary<string, object> { { "test_key", "test_value" } };
            Radar.Error += status => LogManager.Instance.Log($"Error: {status}", LogType.Error);
            Radar.Log += message => LogManager.Instance.Log($"Log: {( message.Length > 20 ? message.Substring(0, 20) : message)}...", LogType.Log);
            Radar.TokenUpdated += OnTokenUpdated;
            Radar.RequestLocationPermissions();
            LogManager.Instance.Log("RadarInitializeExample Completed", LogType.Log);
        }



        void RequestBluetoothPermissions()
        {
#if UNITY_ANDROID
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    string[] permissions = new string[]
                    {
                        "android.permission.BLUETOOTH",
                        "android.permission.BLUETOOTH_ADMIN",
                        "android.permission.BLUETOOTH_CONNECT",
                        "android.permission.BLUETOOTH_SCAN"
                    };

                    currentActivity.Call("requestPermissions", new object[] { permissions, 1001 });
                }
#endif
        }


        private async void GetLocation()
        {
            SetImageColor(_getLocationImage, _redColor);
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var (status, location, stopped) = await Radar.GetLocation();
            if (status == RadarStatus.SUCCESS)
            {
                LogManager.Instance.Log($"Location received: Latitude = {location.Latitude}, Longitude = {location.Longitude}", LogType.Warning);
                _locationText.text = $"Location: {location.Latitude:N3}, {location.Longitude:N3}";
            }
            else
            {
                LogManager.Instance.Log("Failed to get location", LogType.Error);
                _locationText.text = "Location: Failed to get location";
            }

            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            SetImageColor(_getLocationImage, _greenColor);
            StopLoadingAnimation(ref _timeLoadingCoroutine);
        }


        private async Task TrackVerified()
        {
            _timeText.text = _statusText.text = _jsonText.text = "...";

            SetImageColor(_trackVerifiedImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            _onTokenUpdatedText.text = "...";

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var (status, track) = await Radar.TrackVerified();
            if (status == RadarStatus.SUCCESS)
            {
                var json = JsonUtility.ToJson(track);
                _jsonText.text = $"{JsonFormatter.FormatJson(json, _colors)}";
                SetImageColor(_trackVerifiedImage, _greenColor); // Task completed successfully

                _statusText.text = $"Status:{status.ToString()}";
            }
            else
            {
                SetImageColor(_trackVerifiedImage, _redColor); // Task failed or timed out
                _statusText.text = $"Status: {status.ToString()}";
            }

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("TrackVerified Completed", LogType.Log);
        }


        private void StartTrackingVerified()
        {
            _statusText.text = "Status: Starting Tracking...";

            SetImageColor(_startTrackingImage, _orangeColor); // Task in progress
            SetImageColor(_stopTrackingImage, _redColor);

            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            _onTokenUpdatedText.text = "...";

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            Radar.StartTrackingVerified(120, true);

            SetImageColor(_startTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _statusText.text = "Status: Started Tracking";
            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("StartTrackingVerified Completed", LogType.Log);
        }


        private void StopTracking()
        {
            _statusText.text = "Status: Stopping Tracking...";

            SetImageColor(_stopTrackingImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            Radar.StopTrackingVerified();

            SetImageColor(_stopTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _statusText.text = "Status: Stopped Tracking";
            stopWatch.Stop();
            _timeText.text = $"Time taken: {stopWatch.Elapsed.TotalSeconds:N3} seconds";

            LogManager.Instance.Log("StopTracking Completed", LogType.Log);
        }


        private async Task GetVerifiedLocationToken()
        {
            SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            SetImageColor(_getVerifiedLocationTokenImage, _orangeColor); // Task in progress
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var (status, tokenResult) = await Radar.GetVerifiedLocationToken();
            if (status == RadarStatus.SUCCESS)
            {
                LogManager.Instance.Log("Token received: " + tokenResult, LogType.Log);
                var json = JsonUtility.ToJson(tokenResult);
                _jsonText.text = $"{JsonFormatter.FormatJson(json, _colors)}";
                SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _greenColor); // Task success
            }
            else
            {
                LogManager.Instance.Log("Failed to get the token. Status: " + status, LogType.Error);
                SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
            }

            SetImageColor(_getVerifiedLocationTokenImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _timeText.text = $"Time taken: {stopWatch.Elapsed.TotalSeconds:N3} seconds";
            _statusText.text = $"Status: {status}";

            LogManager.Instance.Log("GetVerifiedLocationToken Completed", LogType.Log);
        }

        private void OnTokenUpdated(RadarVerifiedLocationToken token)
        {
            _onTokenUpdatedText.text = $"Token: " + token.Token.Substring(0, 5) + "...";

            LogManager.Instance.Log("OnTokenUpdated Callback. Token: " + token.Token.Substring(0, 5) + "...", LogType.Log);
        }


        private void StartLoadingAnimation(Text textToAnimate, ref Coroutine animationCoroutine)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine); // Stop any ongoing animation for this text
            }

            textToAnimate.text = "..."; // Set initial text
            animationCoroutine = StartCoroutine(LoadingAnimationCoroutine(textToAnimate));
        }


        private void StopLoadingAnimation(ref Coroutine animationCoroutine)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }


        private IEnumerator LoadingAnimationCoroutine(Text text)
        {
            string[] loadingDots = { ".", "..", "..." };
            int dotIndex = 0;
            char dot = '.';

            while (text.text[0] == dot)
            {
                text.text = loadingDots[dotIndex];
                dotIndex = (dotIndex + 1) % loadingDots.Length;
                yield return new WaitForSeconds(0.25f);
            }
        }


        private void SetImageColor(Image image, Color color)
        {
            if (image != null)
            {
                image.color = color;
            }
        }


        private void ResetImagesToRed()
        {
            SetImageColor(_setUserIdImage, _redColor);
            SetImageColor(_trackVerifiedImage, _redColor);
            SetImageColor(_startTrackingImage, _redColor);
            SetImageColor(_stopTrackingImage, _redColor);
            SetImageColor(_setMetadataImage, _redColor);
            SetImageColor(_getVerifiedLocationTokenImage, _redColor);
        }


        public void ClearConsole()
        {
            LogManager.Instance.ClearConsole();
        }
    }
}