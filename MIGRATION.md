# Migration Guide

## 1.0.0 to x.x.x
- Every method, field and event has been rewritten to conform with other Radar SDKs and C# best practices. Please refer to `RadarExample.cs` for updated usage examples.
- `SetVerifiedReceiver()` has been replaced by the `TokenUpdated` event.
- `UserId` and `Metadata` are now properties that you can `get` and `set` (`Metadata` is currently `set`-only).
- Asynchronous methods now use `async`/`await` instead of callbacks.
- Any opinionated functionality (cached calls, timeouts, etc..) have been removed from the wrapper. If you were relying on these, please implement them in your own codebase.