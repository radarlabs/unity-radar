### Scripts overview

This section provides an overview of the main scripts in the Radar SDK for Unity, detailing each script's role and its relationships with other components. This will help you quickly understand how to integrate and utilize the Radar SDK in your Unity project.

#### 1. `RadarExample.cs`

> **Description**:  
> Main script used in demo scene. Demonstrates how to initialize and configure the Radar SDK in Unity. This script provides a basic setup and can be used as a starting template for integrating Radar SDK functionality.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Initializes the Radar SDK using `RadarSettingsData.cs`.                                         | - Works with `Radar.cs` to perform core SDK operations.                                           |
| - Sets up callbacks and configurations required for tracking operations.                       | - Initializes `RadarErrorHandler.cs` for centralized error handling.                                 |

#### 2. `Radar.cs`

> **Description**:  
> The main class for interacting with the Radar SDK, `Radar.cs` bridges Unity and the Radar service, handling core setup, user identification, and tracking operations.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Contains methods for initializing the SDK, setting user ID, and managing tracking.           | - Called directly by `RadarExample.cs` and `RadarUsageExample.cs` to execute SDK operations.                             |
| - Routes platform-specific calls to `AndroidAdapter.cs` or `iOSAdapter.cs` based on the platform.    | - Delegates platform-specific calls to `AndroidAdapter.cs` and `iOSAdapter.cs`.                |

#### 3. `AndroidAdapter.cs`

> **Description**:  
> Adapter class for Android, bridging Unity and the Radar SDK’s Android `.aar` library. Ensures compatibility with Android-specific integration requirements.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Manages Android-specific calls for initialization, tracking, and stopping tracking.          | - Works with `Radar.cs` to perform Android-specific SDK operations.                                  |
| - Provides platform-specific implementation to support Android features.                       | - Complements `iOSAdapter.cs` to provide cross-platform support.                                     |

#### 4. `iOSAdapter.cs`

> **Description**:  
> Adapter class for iOS, bridging Unity and the Radar SDK’s iOS `.xcframework` library. Ensures Radar SDK functionality works seamlessly on iOS devices.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Manages iOS-specific calls for initialization, user ID setup, and tracking.                  | - Complements `AndroidAdapter.cs` to provide cross-platform support.                                 |
| - Implements `Radar.cs` methods for iOS.                                                       | - Receives method calls from `Radar.cs` for iOS operations.                                          |

#### 5. `RadarUsageExample.cs`

> **Description**:  
> Example script showing how to use each method available in the Radar SDK, providing a practical reference for developers integrating Radar into their Unity project.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Demonstrates initializing and configuring the SDK, setting user ID, and using tracking.      | - Utilizes `Radar.cs` for SDK interactions.                                                          |
| - Shows error handling via `RadarErrorHandler.cs`.                                                | - Sets up error handling with `RadarErrorHandler.cs` to manage potential errors.                  |

### Script Interactions and Flow

1. **Initialization**:
   - `RadarExample.cs` initializes the SDK by calling `RadarSDKManager.Initialize()` and sets up configurations.
   - `RadarSDKManager.cs` initializes `Radar.cs` and delegates platform-specific initialization to `AndroidAdapter.cs` or `iOSAdapter.cs`.

2. **Platform-Specific Operations**:
   - `Radar.cs` routes SDK calls to `AndroidAdapter.cs` or `iOSAdapter.cs` depending on the platform.

3. **Error Handling**:
   - Errors encountered during SDK usage are sent to `RadarErrorHandler.cs` through a callback set up in `Radar.cs`.
   - `RadarErrorHandler.cs` logs errors and optionally displays messages to the user.

4. **Example Implementation**:
   - `RadarUsageExample.cs` provides a comprehensive example of SDK functions, making it easy for developers to integrate and use the Radar SDK in their projects.

### Summary

This architecture enables robust cross-platform support within Unity. With `Radar.cs` handling core SDK functionality and `AndroidAdapter.cs`/`iOSAdapter.cs` managing platform-specific operations, The example scripts (`RadarExample.cs` and `RadarUsageExample.cs`) illustrate how to integrate these features, making it easy for developers to get started.


