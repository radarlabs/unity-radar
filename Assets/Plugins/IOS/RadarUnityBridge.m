#import <RadarSDK/Radar.h>
#import <RadarSDK/RadarVerifiedDelegate.h>
#import <RadarSDK/RadarVerifiedLocationToken.h>

#ifdef __cplusplus
extern "C" {
#endif

    typedef void (*RadarTokenUpdatedCallback)(const char* jsonData);

    // Static variable to store the Unity callback
    static RadarTokenUpdatedCallback _tokenUpdatedCallback;

    @interface UnityRadarDelegate : NSObject <RadarVerifiedDelegate>
    @end

    @implementation UnityRadarDelegate

    - (void)didUpdateToken:(RadarVerifiedLocationToken *)token {
        if (_tokenUpdatedCallback && token) {
            NSDictionary *dict = [token dictionaryValue];
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
            if (!error) {
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                _tokenUpdatedCallback([jsonString UTF8String]);
            }
        }
    }

    @end

    static UnityRadarDelegate *unityRadarDelegate;


    typedef void (*CompletionHandlerPtrOnDict)(int requestId, const char* statusStr, const char* jsonStr);


    void Radar_initializeWithPublishableKey(const char* publishableKey) {
        [Radar initializeWithPublishableKey:[NSString stringWithUTF8String:publishableKey]];
    }


    void Radar_setUserId(const char* userId) {
        [Radar setUserId:[NSString stringWithUTF8String:userId]];
    }


    void Radar_getVerifiedLocationTokenWithCompletionHandler(int requestId, CompletionHandlerPtrOnDict handler) {
        [Radar getVerifiedLocationToken:^(RadarStatus status, RadarVerifiedLocationToken * _Nullable token) {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *jsonStr = NULL;

            if (status == RadarStatusSuccess && token != nil) {
                NSDictionary *dict = [token dictionaryValue];
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                if (!error) {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    jsonStr = [jsonString UTF8String];
                }
            }

            handler(requestId, statusStr, jsonStr);
        }];
    }


    void Radar_trackVerifiedWithCompletionHandler(
        int requestId,
        CompletionHandlerPtrOnDict handler,
        const char* desiredAccuracy
    )
    {
        NSString *desiredAccuracyStr = [NSString stringWithUTF8String:desiredAccuracy];
        RadarTrackingOptionsDesiredAccuracy accuracyEnum;

        // Map the string to the corresponding enum value
        if ([desiredAccuracyStr isEqualToString:@"HIGH"]) {
            accuracyEnum = RadarTrackingOptionsDesiredAccuracyHigh;
        } else if ([desiredAccuracyStr isEqualToString:@"LOW"]) {
            accuracyEnum = RadarTrackingOptionsDesiredAccuracyLow;
        } else {
            accuracyEnum = RadarTrackingOptionsDesiredAccuracyMedium; // Default to MEDIUM
        }

        [Radar trackVerifiedWithCompletionHandler:^(RadarStatus status, RadarVerifiedLocationToken * _Nullable token)
        {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *jsonStr = NULL;

            if (status == RadarStatusSuccess && token != nil)
            {
                NSDictionary *dict = [token dictionaryValue];
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
                if (!error)
                {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    jsonStr = [jsonString UTF8String];
                }
            }

            handler(requestId, statusStr, jsonStr);
        }];
    }

    // string Radar_getMetadata() {
    //     NSDictionary *metadata = [Radar getMetadata];
    //     if (metadata == nil) {
    //         return NULL;
    //     }
    //     NSError *error;
    //     NSData *jsonData = [NSJSONSerialization dataWithJSONObject:metadata options:0 error:&error];
    //     if (error) {
    //         return NULL;
    //     }
    //     NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    //     return [jsonString UTF8String];
    // }

    void Radar_setMetadata(const char* jsonMetadata) {
        NSString *metadataStr = [NSString stringWithUTF8String:jsonMetadata];
        NSData *data = [metadataStr dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        NSDictionary *metadata = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
        if (!error) {
            [Radar setMetadata:metadata];
        }
    }


    void Radar_startTrackingVerified(double interval, bool beacons) {
        [Radar startTrackingVerifiedWithInterval:interval beacons:beacons];
    }


    void Radar_stopTrackingVerified() {
        [Radar stopTrackingVerified];
    }


    const char* Radar_getUserId() {
        NSString *userId = [Radar getUserId];
        if (userId == nil) {
            return NULL;
        }
        return [userId UTF8String];
    }


    void Radar_getVerifiedLocationToken(int requestId, CompletionHandlerPtrOnDict handler) {
        [Radar getVerifiedLocationToken:^(RadarStatus status, RadarVerifiedLocationToken * _Nullable token) {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *jsonStr = NULL;

            if (status == RadarStatusSuccess && token != nil) {
                NSDictionary *dict = [token dictionaryValue];
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                if (!error) {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    jsonStr = [jsonString UTF8String];
                }
            }

            // Call the C# callback handler
            handler(requestId, statusStr, jsonStr);
        }];
    }


    void Radar_setVerifiedDelegate(RadarTokenUpdatedCallback callback) {
        _tokenUpdatedCallback = callback;

        if (!unityRadarDelegate) {
            unityRadarDelegate = [[UnityRadarDelegate alloc] init];
        }

        [Radar setVerifiedDelegate:unityRadarDelegate];
    }


    typedef void (*RadarLocationCallback)(const char* status, double latitude, double longitude, bool stopped, int callbackId);
    void Radar_getLocation(RadarLocationCallback callback, int callbackId) {
        [Radar getLocationWithCompletionHandler:^(RadarStatus status, CLLocation * _Nullable location, BOOL stopped) {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const BOOL stoppedBool = stopped;
            if (status == RadarStatusSuccess) {
                double latitude = location.coordinate.latitude;
                double longitude = location.coordinate.longitude;
                callback(statusStr, latitude, longitude, stoppedBool, callbackId);
            } else {
                callback(statusStr, 0, 0, false, callbackId);  // Pass invalid coordinates and callbackId on failure
            }
        }];
    }

#ifdef __cplusplus
}
#endif
