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
    /// <summary>
    /// An example class that handles the initialization and setup of Radar SDK in Unity
    /// </summary>
    public class RadarInitializeExample : MonoBehaviour
    {
        #region Variables
        private static RadarInitializeExample _instance;

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
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _userIdText;
        [SerializeField] private TextMeshProUGUI _onTokenUpdatedText;
        [SerializeField] private TextMeshProUGUI _metadataText;
        [SerializeField] private TextMeshProUGUI _locationText;
        [SerializeField] private TextMeshProUGUI _jsonText;

        [Header("Visuals")]
        [SerializeField] private Color[] _colors;

        private Coroutine _timeLoadingCoroutine;
        private Coroutine _statusLoadingCoroutine;
        private Coroutine _callbackLoadingCoroutine;

        string userId = String.Empty;

        private readonly Color _redColor = Color.red;
        private readonly Color _orangeColor = new Color(1f, 0.65f, 0f);
        private readonly Color _greenColor = Color.green;

        private Queue<Action> TODO = new Queue<Action>();
        int callbacksTotal = 0;

        #endregion


        private void OnValidate()
        {
            ResetImagesToRed();
            var a = JsonUtility.ToJson(new VerifiedLocationData());
            var b = JsonFormatter.FormatJson(a, _colors);
            _jsonText.text = b;
        }


        private void Awake()
        {
            _instance = this;
        }


        private IEnumerator Start()
        {
            _setUserIdButton.onClick.AddListener(() => { SetUserIdButtonHandler(); });
            _setMetadataButton.onClick.AddListener(() => SetMetadata());
            _getLocationButton.onClick.AddListener(() => GetLocation());
            _verifyTrackButton.onClick.AddListener(() => _ = TrackVerified());
            _startTrackingButton.onClick.AddListener(() => _ = StartTrackingVerified());
            _stopTrackingButton.onClick.AddListener(() => _ = StopTracking());
            _getVerifiedLocationTokenButton.onClick.AddListener(() => _ = GetVerifiedLocationToken());
            yield return StartCoroutine(RadarSDKManager.Initialize());
#if UNITY_IOS
            RadarServiceWrapper.SetVerifiedReceiver(DidUpdateToken);
#else
            RadarServiceWrapper.SetVerifiedReceiver(OnTokenUpdated);
#endif
            LogManager.Instance.Log("RadarInitializeExample Complete", LogType.Log);
        }


        void Update()
        {
            lock (TODO)
            {
                while (TODO.Count > 0)
                {
                    TODO.Dequeue().Invoke();
                }
            }
            lock (RadarServiceWrapper._mainThreadActions)
            {
                while (RadarServiceWrapper._mainThreadActions.Count > 0)
                {
                    RadarServiceWrapper._mainThreadActions.Dequeue().Invoke();
                }
            }
        }


        private async void GetLocation()
        {
            SetImageColor(_getLocationImage, _redColor);
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var location = await RadarServiceWrapper.GetLocation();

            stopWatch.Stop();

            if (location != null)
            {
                EnqueueMainThreadAction(() =>
                {
                    LogManager.Instance.Log($"Successfully got location: Latitude = {location.Value.latitude}, Longitude = {location.Value.longitude}", LogType.Warning);
                    _locationText.text = $"{location.Value.latitude}, {location.Value.longitude}";
                });
            }
            else
            {
                LogManager.Instance.Log("Failed to retrieve location", LogType.Error);
            }

            SetImageColor(_getLocationImage, _greenColor);
            StopLoadingAnimation(ref _timeLoadingCoroutine);
        }


        private void EnqueueMainThreadAction(System.Action action)
        {
            lock (TODO)
            {
                TODO.Enqueue(action);
            }
        }


        private void SetUserIdButtonHandler()
        {
            SetUserId();
        }


        private string SetUserId(string userId = RadarSDKManager.TEMP_UNIQUE_USER_ID)
        {
            LogManager.Instance.Log("SetUserId() " + userId, LogType.Log);

            _timeText.text = _userIdText.text = "...";

            SetImageColor(_setUserIdImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            if (userId == RadarSDKManager.TEMP_UNIQUE_USER_ID) userId = RadarSDKManager.UserId;
            string uniqueUserId = $"{userId}";
            if (RadarSDKManager.AddUserIdExtension)
                uniqueUserId += $"_{Enum.GetName(typeof(RuntimePlatform), Application.platform)}";
            RadarSDKManager.SetUserId(uniqueUserId);

            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);
            SetImageColor(_setUserIdImage, _greenColor); // Task completed successfully


            LogManager.Instance.Log("SetUserId()  Complete", LogType.Log);

#if UNITY_ANDROID
            userId = Radar.GetUserId();
#endif
            _userIdText.text = "UserId: " + userId;

            return userId;
        }


        private void SetMetadata(MetadataContainer metadata = null)
        {
            LogManager.Instance.Log("SetMetadata()", LogType.Log);

            if (metadata == null)
                metadata = RadarSDKManager.Metadata;
            _timeText.text = "...";

            SetImageColor(_setMetadataImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            RadarSDKManager.SetMetadata(metadata);
            // Call the extended TrackVerifiedWithFraudDetection method
            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);
            SetImageColor(_setMetadataImage, _greenColor); // Task completed successfully

            LogManager.Instance.Log("SetMetadata()  Complete", LogType.Log);

            string jsonString = JsonUtility.ToJson(metadata);
            _metadataText.text = jsonString;
        }


        private async Task TrackVerified()
        {
            LogManager.Instance.Log("TrackVerified()", LogType.Log);

            _timeText.text = _statusText.text = _jsonText.text = "...";

            SetImageColor(_trackVerifiedImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);
            StartLoadingAnimation(_onTokenUpdatedText, ref _callbackLoadingCoroutine);
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the extended TrackVerifiedWithFraudDetection method
            if (userId == String.Empty) SetUserId();

            var track = await RadarSDKManager.TrackVerifiedAsync(userId);
            if (track != null)
            {
                if (track.Value.Status == RadarStatus.SUCCESS)
                {
                    var json = JsonUtility.ToJson(track.Value.Data);
                    _jsonText.text = JsonFormatter.FormatJson(json, _colors);
                    SetImageColor(_trackVerifiedImage, _greenColor); // Task completed successfully
                }

                _statusText.text = $"Status:{track.Value.Status}";
            }
            else
            {
                SetImageColor(_trackVerifiedImage, _redColor); // Task failed or timed out
                _statusText.text = $"Timeout";
            }

            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("TrackVerified()  Complete", LogType.Log);
        }


        private async Task StartTrackingVerified()
        {
            LogManager.Instance.Log("StartTrackingVerified()", LogType.Log);

            _statusText.text = "Starting Tracking...";

            SetImageColor(_startTrackingImage, _orangeColor); // Task in progress
            SetImageColor(_stopTrackingImage, _redColor);

            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);
            StartLoadingAnimation(_onTokenUpdatedText, ref _callbackLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarSDKManager.StartTrackingVerifiedAsync(RadarSDKManager.TrackingInterval, RadarSDKManager.UseBeacons);

            SetImageColor(_startTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _statusText.text = "Started Tracking";
            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("StartTrackingVerified()  Complete", LogType.Log);
        }


        private async Task StopTracking()
        {
            LogManager.Instance.Log("StopTracking()", LogType.Log);

            _statusText.text = "Stopping Tracking...";

            SetImageColor(_stopTrackingImage, _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarSDKManager.StopTrackingAsync();

            SetImageColor(_stopTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            _statusText.text = "Stopped Tracking";
            stopWatch.Stop();
            _timeText.text = string.Format("Time taken: {0:N3} seconds", stopWatch.Elapsed.TotalSeconds);

            LogManager.Instance.Log("StopTracking()  Complete", LogType.Log);
        }


        private async Task GetVerifiedLocationToken()
        {
            LogManager.Instance.Log("Getting verified location token...", LogType.Log);

            SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _orangeColor); // Task in progress
            StartLoadingAnimation(_timeText, ref _timeLoadingCoroutine);
            StartLoadingAnimation(_statusText, ref _statusLoadingCoroutine);

            SetImageColor(_getVerifiedLocationTokenImage, _orangeColor); // Task in progress
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the RadarServiceWrapper to get the token
            var tokenResult = await RadarSDKManager.GetVerifiedLocationTokenAsync();
            if (tokenResult != null)
            {
                if (tokenResult.Value.Status == RadarStatus.SUCCESS)
                {
                    LogManager.Instance.Log("Token received: " + tokenResult.Value.Data, LogType.Log);
                    var json = JsonUtility.ToJson(tokenResult.Value.Data);
                    _jsonText.text = JsonFormatter.FormatJson(json, _colors);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _greenColor); // Task success
                }
                else
                {
                    LogManager.Instance.Log("Failed to get the token. Status: " + tokenResult.Value.Status, LogType.Error);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
                }
            }
            else
            {
                LogManager.Instance.Log("Error retrieving token.", LogType.Error);
                SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
            }

            SetImageColor(_getVerifiedLocationTokenImage, _greenColor); // Task completed successfully
            StopLoadingAnimation(ref _timeLoadingCoroutine);
            StopLoadingAnimation(ref _statusLoadingCoroutine);

            stopWatch.Stop();
            _timeText.text = $"Time taken: {stopWatch.Elapsed.TotalSeconds:N3} seconds";
            _statusText.text = $"Status:{tokenResult.Value.Status}";

            LogManager.Instance.Log("GetVerifiedLocationToken() Completed", LogType.Log);
        }


        [MonoPInvokeCallback(typeof(Action<RadarVerifiedLocationToken>))]
        public static void DidUpdateToken(RadarVerifiedLocationToken token)
        {
            _instance?.OnTokenUpdated(token);
        }


        private void OnTokenUpdated(RadarVerifiedLocationToken token)
        {
            Debug.Log("OnTokenUpdated()");
            EnqueueMainThreadAction(() =>
            {
                _onTokenUpdatedText.text = $"OnTokenUpdated Callback";
            });
            EnqueueMainThreadAction(() =>
            {
                LogManager.Instance.Log("Token updated in Unity: " + token);

                // Check if the token passed and take the necessary actions
                if (token.Passed)
                    LogManager.Instance.Log("Access granted. Token is valid.");
                else
                    LogManager.Instance.Log("Access denied. Token is invalid or expired.", LogType.Error);

                callbacksTotal += 1;
                StopLoadingAnimation(ref _callbackLoadingCoroutine);
                _onTokenUpdatedText.text = $"OnTokenUpdated Callback {callbacksTotal}. Token: " + token.Token.Substring(0, 5) + "...";

                LogManager.Instance.Log("OnTokenUpdated Callback. Token: " + token.Token.Substring(0, 5) + "...", LogType.Log);
            });
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