### Scripts overview

This section provides an overview of the main scripts in the Radar SDK for Unity, detailing each script's role and its relationships with other components. This will help you quickly understand how to integrate and utilize the Radar SDK in your Unity project.

#### 1. `RadarExample.cs`

> **Description**:  
> Main script used in demo scene. Demonstrates how to initialize and configure the Radar SDK in Unity. This script provides a basic setup and can be used as a starting template for integrating Radar SDK functionality.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Initializes the Radar SDK using `RadarSettingsData`.                                         | - Works with `RadarServiceWrapper` to perform core SDK operations.                                   |
| - Sets up callbacks and configurations required for tracking operations.                       | - Initializes `RadarErrorHandler` for centralized error handling.                                    |

#### 2. `RadarServiceWrapper.cs`

> **Description**:  
> Provides a wrapper around the Radar SDK's core functionality, including methods for initializing the SDK, setting the user ID and metadata, and managing tracking operations. It also supports centralized error handling by delegating errors to `RadarErrorHandler`.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Wraps essential SDK methods like initialization, setting user ID, and tracking management.   | - Works with `RadarExample` to initialize and configure SDK.                               |
| - Exposes an error callback for centralized error management through `RadarErrorHandler`.      | - Routes platform-specific SDK calls to `AndroidAdapter` and `iOSAdapter` as needed.                 |

#### 3. `Radar.cs`

> **Description**:  
> The main class for interacting with the Radar SDK, `Radar.cs` bridges Unity and the Radar service, handling core setup, user identification, and tracking operations.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Contains methods for initializing the SDK, setting user ID, and managing tracking.           | - Called directly by `RadarServiceWrapper` to execute SDK operations.                                |
| - Routes platform-specific calls to `AndroidAdapter` or `iOSAdapter` based on the platform.    | - Delegates platform-specific calls to `AndroidAdapter` and `iOSAdapter`.                            |

#### 4. `AndroidAdapter.cs`

> **Description**:  
> Adapter class for Android, bridging Unity and the Radar SDK’s Android `.aar` library. Ensures compatibility with Android-specific integration requirements.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Manages Android-specific calls for initialization, tracking, and stopping tracking.          | - Works with `RadarServiceWrapper` and `Radar.cs` to perform Android-specific SDK operations.        |
| - Provides platform-specific implementation to support Android features.                       | - Complements `iOSAdapter` to provide cross-platform support.                                        |

#### 5. `iOSAdapter.cs`

> **Description**:  
> Adapter class for iOS, bridging Unity and the Radar SDK’s iOS `.xcframework` library. Ensures Radar SDK functionality works seamlessly on iOS devices.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Manages iOS-specific calls for initialization, user ID setup, and tracking.                  | - Complements `AndroidAdapter` to provide cross-platform support.                                    |
| - Implements `RadarServiceWrapper` and `Radar.cs` methods for iOS.                             | - Receives method calls from `RadarServiceWrapper` and `Radar.cs` for iOS operations.                |

#### 6. `RadarUsageExample.cs`

> **Description**:  
> Example script showing how to use each method available in the Radar SDK, providing a practical reference for developers integrating Radar into their Unity project.

| **Functionality**                                                                              | **Related Scripts**                                                                                  |
|------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| - Demonstrates initializing and configuring the SDK, setting user ID, and using tracking.      | - Utilizes `RadarServiceWrapper` for SDK interactions.                                               |
| - Shows error handling via `RadarErrorHandler`.                                                | - Sets up error handling with `RadarErrorHandler` to manage potential errors.                        |

### Script Interactions and Flow

1. **Initialization**:
   - `RadarExample` initializes the SDK by calling `RadarServiceWrapper.Initialize()` and sets up configurations.
   - `RadarServiceWrapper` initializes `Radar` and delegates platform-specific initialization to `AndroidAdapter` or `iOSAdapter`.

2. **Platform-Specific Operations**:
   - `RadarServiceWrapper` and `Radar` route SDK calls to `AndroidAdapter` or `iOSAdapter` depending on the platform.

3. **Error Handling**:
   - Errors encountered during SDK usage are sent to `RadarErrorHandler` through a callback set up in `RadarServiceWrapper`.
   - `RadarErrorHandler` logs errors and optionally displays messages to the user.

4. **Example Implementation**:
   - `RadarUsageExample` provides a comprehensive example of SDK functions, making it easy for developers to integrate and use the Radar SDK in their projects.

### Summary

This architecture enables robust cross-platform support within Unity. With `RadarServiceWrapper` handling core SDK functionality and `AndroidAdapter`/`iOSAdapter` managing platform-specific operations, The example scripts (`RadarExample` and `RadarUsageExample`) illustrate how to integrate these features, making it easy for developers to get started.


