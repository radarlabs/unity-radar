using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using RadarSDK;
using UnityEngine.UI;
using TMPro;


namespace RadarSDKBridge
{
    public class RadarInitializeExample : MonoBehaviour
    {
        const string TEMP_UNIQUE_USER_ID = "TEST_uniqueUserId_001";

        [SerializeField] private Image _setUserIdImage;
        [SerializeField] private Image _setMetadataImage;
        [SerializeField] private Image _trackVerifiedImage;
        [SerializeField] private Image _startTrackingImage;
        [SerializeField] private Image _stopTrackingImage;
        [SerializeField] private Image _getVerifiedLocationTokenImage;

        [SerializeField] private Button _setUserIdButton;
        [SerializeField] private Button _verifyTrackButton;
        [SerializeField] private Button _startTrackingButton;
        [SerializeField] private Button _stopTrackingButton;
        [SerializeField] private Button _setMetadataButton;
        [SerializeField] private Button _getVerifiedLocationTokenButton;

        [SerializeField] private TextMeshProUGUI _time;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _userId;
        [SerializeField] private TextMeshProUGUI _onTokenUpdatedText;

        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField, TextArea] private string _jso;
        [SerializeField] private Color[] _colors;

        private Coroutine _loadingCoroutine;
        string userId = String.Empty;

        private readonly Color _redColor = Color.red;
        private readonly Color _orangeColor = new Color(1f, 0.65f, 0f); // Orange
        private readonly Color _greenColor = Color.green;

        MetadataContainer exampleMetadata;


        private void OnValidate()
        {
            ResetImagesToRed();
            var a = JsonUtility.ToJson(new VerifiedLocationData());
            var b = JsonFormatter.FormatJson(a, _colors);
            _text.text = _jso = b;
        }


        private void Start()
        {
            _status.text = "";
            _text.text = "";

            exampleMetadata = new MetadataContainer
            {
                someNumber = 999
            };

            _setUserIdButton.onClick.AddListener(() => SetUserId());
            _setMetadataButton.onClick.AddListener(() => SetMetadata());
            _verifyTrackButton.onClick.AddListener(() => _ = TrackVerified());
            _startTrackingButton.onClick.AddListener(() => _ = StartTrackingVerified());
            _stopTrackingButton.onClick.AddListener(() => _ = StopTracking());
            _getVerifiedLocationTokenButton.onClick.AddListener(() => _ = GetVerifiedLocationToken());

            RadarServiceWrapper.Initialize();
            RadarServiceWrapper.SetVerifiedReceiver(OnTokenUpdated);
        }


        private string SetUserId(string userId = TEMP_UNIQUE_USER_ID)
        {
            Debug.Log("SetUserId()");
            _time.text = _status.text = _text.text = _userId.text = string.Empty + "...";

            SetImageColor(_setUserIdImage, _orangeColor); // Task in progress
            StartLoadingAnimation();

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            var uniqueUserId = $"{TEMP_UNIQUE_USER_ID}_{Enum.GetName(typeof(RuntimePlatform), Application.platform)}";
            Radar.SetUserId(uniqueUserId);
            stopWatch.Stop();

            StopLoadingAnimation();
            SetImageColor(_setUserIdImage, _greenColor); // Task completed successfully

            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = string.Format("Time: {0:N3} seconds", totalSeconds);
            Debug.Log("SetUserId()  ---end");
            userId = Radar.GetUserId();
            _userId.text = "UserId: " + userId;
            return userId;
        }


        private void SetMetadata(MetadataContainer metadata = null)
        {
            Debug.Log("SetMetadata()");
            if (metadata == null)
                metadata = exampleMetadata;
            _time.text = _status.text = _text.text = _userId.text = string.Empty + "...";

            SetImageColor(_setMetadataImage, _orangeColor); // Task in progress
            StartLoadingAnimation();

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            Radar.SetMetadata(metadata);
            // Call the extended TrackVerifiedWithFraudDetection method
            stopWatch.Stop();

            StopLoadingAnimation();
            SetImageColor(_setMetadataImage, _greenColor); // Task completed successfully

            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = string.Format("Time: {0:N3} seconds", totalSeconds);
            Debug.Log("SetMetadata()  ---end");
            userId = Radar.GetUserId();
            _userId.text = "UserId: " + userId;
        }


        private async Task TrackVerified()
        {
            Debug.Log("TrackVerified()");
            _time.text = _status.text = _text.text = string.Empty + "...";

            SetImageColor(_trackVerifiedImage, _orangeColor); // Task in progress
            StartLoadingAnimation();

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

            StopLoadingAnimation();

            stopWatch.Stop();
            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = string.Format("Time: {0:N3} seconds", totalSeconds);
            Debug.Log("TrackVerified()  ---end");
        }


        private async Task StartTrackingVerified()
        {
            Debug.Log("StartTracking()");
            _status.text = "Starting Tracking...";

            SetImageColor(_startTrackingImage, _orangeColor); // Task in progress
            StartLoadingAnimation();

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarServiceWrapper.StartTrackingVerified(60, true); // 60 seconds interval, with beacons

            SetImageColor(_startTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation();

            _status.text = "Started Tracking";
            stopWatch.Stop();
            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = string.Format("Start Tracking Time: {0:N3} seconds", totalSeconds);
            Debug.Log("StartTracking()  ---end");
        }


        private async Task StopTracking()
        {
            Debug.Log("StopTracking()");
            _status.text = "Stopping Tracking...";

            SetImageColor(_stopTrackingImage, _orangeColor); // Task in progress
            StartLoadingAnimation();

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await RadarServiceWrapper.StopTracking();

            SetImageColor(_stopTrackingImage, _greenColor); // Task completed successfully
            StopLoadingAnimation();

            _status.text = "Stopped Tracking";
            stopWatch.Stop();
            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = string.Format("Stop Tracking Time: {0:N3} seconds", totalSeconds);
            Debug.Log("StopTracking()  ---end");
        }



        private async Task GetVerifiedLocationToken()
        {
            Debug.Log("Getting verified location token...");

            SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _orangeColor); // Task in progress
            StartLoadingAnimation();

            SetImageColor(_getVerifiedLocationTokenImage, _orangeColor); // Task in progress
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the RadarServiceWrapper to get the token
            var tokenResult = await RadarServiceWrapper.GetVerifiedLocationToken();
            if (tokenResult != null)
            {
                if (tokenResult.Value.Status == RadarStatus.SUCCESS)
                {
                    Debug.Log("Token received: " + tokenResult.Value.Data);
                    var json = JsonUtility.ToJson(tokenResult.Value.Data);
                    _text.text = JsonFormatter.FormatJson(json, _colors);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _greenColor); // Task success
                }
                else
                {
                    Debug.LogError("Failed to get the token. Status: " + tokenResult.Value.Status);
                    SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
                }
            }
            else
            {
                Debug.LogError("Error retrieving token.");
                SetImageColor(_getVerifiedLocationTokenImage.GetComponent<Image>(), _redColor); // Task failed
            }

            SetImageColor(_getVerifiedLocationTokenImage, _greenColor); // Task completed successfully
            StopLoadingAnimation();

            stopWatch.Stop();
            var totalSeconds = stopWatch.Elapsed.TotalSeconds;
            _time.text = $"Time: {totalSeconds:N3} seconds";

            Debug.Log("GetVerifiedLocationToken() completed.");
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
            _onTokenUpdatedText.text = "OnTokenUpdated Callback!!!";
        }



        private void StartLoadingAnimation()
        {
            _status.text = _time.text = "...";
            _loadingCoroutine = StartCoroutine(LoadingAnimationCoroutine());
        }

        private void StopLoadingAnimation()
        {
            if (_loadingCoroutine != null)
            {
                StopCoroutine(_loadingCoroutine);
                _loadingCoroutine = null;
            }
        }

        private IEnumerator LoadingAnimationCoroutine()
        {
            string[] loadingDots = { ".", "..", "..." };
            int dotIndex = 0;
            char dot = '.';

            while (_status.text[0] == dot && _time.text[0] == dot)
            {
                _status.text = _time.text = loadingDots[dotIndex];
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