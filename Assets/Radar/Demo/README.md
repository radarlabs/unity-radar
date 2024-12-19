## Demo scene

The Demo Scene in this Unity SDK provides a hands-on way to test each function call with visual feedback and live status updates. The interface is divided into three main sections: [**Top Panel**](#top-panel), [**Left Panel**](#left-panel), and [**Right Panel**](#right-panel).

### Overview

Each function in the Demo Scene is represented by a button that triggers the respective SDK call. Status lights beside each button give instant feedback on the operation's result:

- 🔴**Red**: Operation failed
- 🟡**Orange**: Operation started
- 🟢**Green**: Operation succeeded

You can also change the publishable key directly in the build. These are some additional operations:

- **Save**: Saves key in Player Prefs so that the next time it will have a new key
- **Update**: Reinitializes the SDK by calling `Radar.Initialize()`
- **Reset**: Resets the key to default and reinitializes the SDK

<div align="center">
    <img src="https://images.ctfassets.net/f2vbu16fzuly/4YCMfKPVMoND2zOXn7iMxT/2f591a4efa957853f0f3c37f2da4c32e/Image1.PNG" alt="Demo Scene Status Lights" style="width: 50%;">
</div>


### Interface layout

#### Top Panel
The top panel displays key information and status updates for each function call:
- **Status Text**: Shows the current status of the last operation (e.g., `Success`, `Timeout`, or `Failed`).
- **Time Taken**: Displays the time taken for the last operation.
- **User ID Text**: Displays the current User ID.
- **Callback Received Text**: Shows `_onTokenUpdatedText`, indicating that a callback has been received (specific to token updates).
- **Metadata Text**: Displays any metadata set during the session.
- **Location Text**: Displays the last known location.

#### Left panel
The left panel shows the JSON data retrieved from the latest verified tracking operation. This data is dynamically updated when the `Track Verified` function is called. The process works as follows:

```csharp
var track = await RadarSDKManager.TrackVerifiedAsync(userId);
if (track != null)
{
    if (track.Value.Status == RadarStatus.SUCCESS)
    {
        var json = JsonUtility.ToJson(track.Value.Data);
        _jsonText.text = JsonFormatter.FormatJson(json, _colors);
        SetImageColor(_trackVerifiedImage, _greenColor); // Task completed successfully
    }

    _statusText.text = $"Status: {track.Value.Status}";
}
else
{
    SetImageColor(_trackVerifiedImage, _redColor); // Task failed or timed out
    _statusText.text = "Timeout";
}
```
#### Right Panel

The right panel contains logs managed by the `LogManager.cs` script. This log section categorizes messages by type:

- **Log**: General operational messages.
- **Attention**: Important events that may need user attention. (Highlighted in orange)
- **Warning**: Issues that could potentially affect performance or functionality. (Highlighted in yellow)
- **Error**: Critical issues that need immediate attention. (Highlighted in red)

Each log entry shows vital information about the SDK's runtime behavior, making it easier to debug and monitor operations.

##### Functionality

The Demo Scene includes the following buttons and their associated functionality:

- **Set User ID**: Calls `SetUserIdButtonHandler()`.
- **Set Metadata**: Calls `SetMetadata()`.
- **Get Location**: Calls `GetLocation()`.
- **Verify Track**: Calls `TrackVerified()`, verifying location tracking with a status update.
- **Start Tracking**: Calls `StartTrackingVerified()`, initiating background location tracking.
- **Stop Tracking**: Calls `StopTracking()`, stopping background tracking.
- **Get Verified Location Token**: Calls `GetVerifiedLocationToken()`, retrieving a token for verifying location data.

Each button is connected to its respective function through event listeners:

```csharp
private IEnumerator Start()
{
    _setUserIdButton.onClick.AddListener(() => { SetUserIdButtonHandler(); });
    _setMetadataButton.onClick.AddListener(() => SetMetadata());
    _getLocationButton.onClick.AddListener(() => GetLocation());
    _verifyTrackButton.onClick.AddListener(() => _ = TrackVerified());  
    _startTrackingButton.onClick.AddListener(() => _ = StartTrackingVerified());
    _stopTrackingButton.onClick.AddListener(() => _ = StopTracking());
    _getVerifiedLocationTokenButton.onClick.AddListener(() => _ = GetVerifiedLocationToken());
    // ...
}
```