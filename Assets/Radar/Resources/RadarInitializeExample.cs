using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using RadarSDK;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace RadarSDKBridge
{
    public class RadarInitializeExample : MonoBehaviour
    {
        private static RadarInitializeExample _instance;

        #region Variables

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

        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _userId;
        [SerializeField] private TextMeshProUGUI _onTokenUpdatedText;
        [SerializeField] private TextMeshProUGUI _text;


        [SerializeField, TextArea] private string _jso;
        [SerializeField] private Color[] _colors;

        private Coroutine _timeLoadingCoroutine;
        private Coroutine _statusLoadingCoroutine;
        private Coroutine _callbackLoadingCoroutine;

        string userId = String.Empty;

        private readonly Color _redColor = Color.red;
        private readonly Color _orangeColor = new Color(1f, 0.65f, 0f); // Orange
        private readonly Color _greenColor = Color.green;

        private Queue<System.Action> TODO = new Queue<System.Action>();
        int callbacksTotal = 0;

        #endregion


        private void OnValidate()
        {
            ResetImagesToRed();
            var a = JsonUtility.ToJson(new VerifiedLocationData());
            var b = JsonFormatter.FormatJson(a, _colors);
            _text.text = _jso = b;
        }


        private void Awake()
        {
            _instance = this;
        }


        private void Start()
        {
            _setUserIdButton.onClick.AddListener(() => { SetUserIdButtonHandler(); });
            _setMetadataButton.onClick.AddListener(() => SetMetadata());
            _getLocationButton.onClick.AddListener(() => GetLocation());
            _verifyTrackButton.onClick.AddListener(() => _ = TrackVerified());
            _startTrackingButton.onClick.AddListener(() => _ = StartTrackingVerified());
            _stopTrackingButton.onClick.AddListener(() => _ = StopTracking());
            _getVerifiedLocationTokenButton.onClick.AddListener(() => _ = GetVerifiedLocationToken());

            RadarSDKManager.Initialize();

            RadarServiceWrapper.Initialize();
#if UNITY_IOS
            RadarServiceWrapper.SetVerifiedReceiver(DidUpdateToken);
#else
            RadarServiceWrapper.SetVerifiedReceiver(OnTokenUpdated);
#endif
            LogManager.Instance.Log("RadarInitializeExample Start ---end", LogType.Log);
        }


        void Update()
        {
            lock (TODO)
            {
                while (TODO.Count > 0)
                {
                    TODO.Dequeue()();
                }
            }
        }


        private async void GetLocation()
        {
            SetImageColor(_getLocationImage, _redColor);
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var location = await RadarServiceWrapper.GetLocation();

            if (location != null)
            {
                LogManager.Instance.Log($"Successfully got location: Latitude = {location.Value.latitude}, Longitude = {location.Value.longitude}", LogType.Warning);
            }
            else
            {
                LogManager.Instance.Log("Failed to retrieve location", LogType.Error);
            }
            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            SetImageColor(_getLocationImage, _greenColor);
            StopLoadingAnimation(ref _timeLoadingCoroutine);

        }


        private void SetUserIdButtonHandler()
        {
            SetUserId();
        }


        private string SetUserId(string userId = RadarSDKManager.TEMP_UNIQUE_USER_ID)
        {
            LogManager.Instance.Log("SetUserId() " + userId, LogType.Log);
            _time.text = _status.text = _text.text = _userId.text = string.Empty + "...";

            SetImageColor(_setUserIdImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            if (userId == RadarSDKManager.TEMP_UNIQUE_USER_ID) userId = RadarSDKManager.UserId;

            var uniqueUserId = $"{userId}_{Enum.GetName(typeof(RuntimePlatform), Application.platform)}";
            Radar.SetUserId(uniqueUserId);

            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);
            SetImageColor(_setUserIdImage, _greenColor); // Task completed successfully


            LogManager.Instance.Log("SetUserId()  ---end", LogType.Log);
            Debug.Log("SetUserId()  ---end");

#if UNITY_ANDROID
            userId = Radar.GetUserId();
            _userId.text = "UserId: " + userId;
#endif
            return userId;
        }


        private void SetMetadata(MetadataContainer metadata = null)
        {
            LogManager.Instance.Log("SetMetadata()", LogType.Log);
            Debug.Log("SetMetadata()");
            if (metadata == null)
                metadata = RadarSDKManager.Metadata;
            _time.text = _status.text = _text.text = _userId.text = string.Empty + "...";

            SetImageColor(_setMetadataImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            Radar.SetMetadata(metadata);
            // Call the extended TrackVerifiedWithFraudDetection method
            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);
            SetImageColor(_setMetadataImage, _greenColor); // Task completed successfully

            LogManager.Instance.Log("SetMetadata()  ---end", LogType.Log);
            Debug.Log("SetMetadata()  ---end");
        }


        private async Task TrackVerified()
        {
            LogManager.Instance.Log("TrackVerified()", LogType.Log);
            Debug.Log("TrackVerified()");
            _time.text = _status.text = _text.text = string.Empty + "...";

            SetImageColor(_trackVerifiedImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);
            StartLoadingAnimation(_onTokenUpdatedText, ref _callbackLoadingCoroutine);
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the extended TrackVerifiedWithFraudDetection method
            if (userId == String.Empty) SetUserId();

            var track = await RadarServiceWrapper.TrackVerified(userId);
            if (track != null)
            {
                if (track.Value.Status == RadarStatus.SUCCESS)
                {
                    var json = JsonUtility.ToJson(track.Value.Data);
                    _text.text = JsonFormatter.FormatJson(json, _colors);
                    SetImageColor(_trackVerifiedImage, _greenColor); // Task completed successfully
                }

                _status.text = $"Status:{track.Value.Status}";
            }
            else
            {
                SetImageColor(_trackVerifiedImage, _redColor); // Task failed or timed out
                _status.text = $"Timeout";
            }

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("TrackVerified()  ---end", LogType.Log);
            Debug.Log("TrackVerified()  ---end");
        }


        private async Task StartTrackingVerified()
        {
            LogManager.Instance.Log("StartTracking()", LogType.Log);
            Debug.Log("StartTracking()");
            _status.text = "Starting Tracking...";

            SetImageColor(_startTrackingImage, _orangeColor); // Task in progress
            SetImageColor(_stopTrackingImage, _redColor);

            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);
            StartLoadingAnimation(_onTokenUpdatedText, ref _callbackLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarServiceWrapper.StartTrackingVerified(RadarSDKManager.TrackingInterval, RadarSDKManager.UseBeacons);

            SetImageColor(_startTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _status.text = "Started Tracking";
            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("StartTracking()  ---end", LogType.Log);
            Debug.Log("StartTracking()  ---end");
        }


        private async Task StopTracking()
        {
            LogManager.Instance.Log("StopTracking()", LogType.Log);
            Debug.Log("StopTracking()");
            _status.text = "Stopping Tracking...";

            SetImageColor(_stopTrackingImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarServiceWrapper.StopTracking();

            SetImageColor(_stopTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _status.text = "Stopped Tracking";
            stopWatch.Stop();
            _time.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("StopTracking()  ---end", LogType.Log);
            Debug.Log("StopTracking()  ---end");
        }



        private async Task GetVerifiedLocationToken()
        {
            LogManager.Instance.Log("Getting verified location token...", LogType.Log);
            Debug.Log("Getting verified location token...");
            SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _orangeColor); // Task in progress
            StartLoadingAnimation(_time, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_status, ref _statusLoadingCoroutine);

            SetImageColor(_getVerifiedLocationTokenImage, _orangeColor); // Task in progress
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the RadarServiceWrapper to get the token
            var tokenResult = await RadarServiceWrapper.GetVerifiedLocationToken();
            if (tokenResult != null)
            {
                if (tokenResult.Value.Status == RadarStatus.SUCCESS)
                {
                    LogManager.Instance.Log("Token received: " + tokenResult.Value.Data, LogType.Log);
                    Debug.Log("Token received: " + tokenResult.Value.Data);
                    var json = JsonUtility.ToJson(tokenResult.Value.Data);
                    _text.text = JsonFormatter.FormatJson(json, _colors);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _greenColor); // Task success
                }
                else
                {
                    LogManager.Instance.Log("Failed to get the token. Status: " + tokenResult.Value.Status, LogType.Error);
                    Debug.LogError("Failed to get the token. Status: " + tokenResult.Value.Status);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
                }
            }
            else
            {
                LogManager.Instance.Log("Error retrieving token.", LogType.Error);
                Debug.LogError("Error retrieving token.");
                SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
            }

            SetImageColor(_getVerifiedLocationTokenImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _time.text = $"Time taken: {stopWatch.Elapsed.TotalSeconds:N3} seconds";

            LogManager.Instance.Log("GetVerifiedLocationToken() completed.", LogType.Log);
            Debug.Log("GetVerifiedLocationToken() completed.");
        }


        [MonoPInvokeCallback(typeof(Action<RadarVerifiedLocationToken>))]
        public static void DidUpdateToken(RadarVerifiedLocationToken token)
        {
            _instance?.OnTokenUpdated(token);
        }

        private void OnTokenUpdated(RadarVerifiedLocationToken token)
        {
            Debug.Log("Token updated in Unity: " + token);

            // Check if the token passed and take the necessary actions
            if (token.Passed)
            {
                Debug.Log("Access granted. Token is valid.");
                // You can send the token to the server or allow access to features
            }
            else
            {
                Debug.Log("Access denied. Token is invalid or expired.");
                // Show error or restrict access to features
            }
            lock (TODO)
            {
                TODO.Enqueue(() =>
                {
                    // your code here
                    callbacksTotal += 1;
                    _onTokenUpdatedText.text = $"OnTokenUpdated Callback {callbacksTotal}. Token: " + token.Token.Substring(0, 5) + "...";
                    LogManager.Instance.Log("OnTokenUpdated Callback. Token: " + token.Token.Substring(0, 5) + "...", LogType.Log);
                    StopLoadingAnimation(ref _callbackLoadingCoroutine);
                });
            }

        }


        private void StartLoadingAnimation(TextMeshProUGUI textToAnimate, ref Coroutine animationCoroutine)
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


        private IEnumerator LoadingAnimationCoroutine(TextMeshProUGUI text)
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
    }
}