#import <RadarSDK/Radar.h>
#import <RadarSDK/RadarVerifiedDelegate.h>
#import <RadarSDK/RadarVerifiedLocationToken.h>

#ifdef __cplusplus
extern "C" {
#endif

    typedef void (*RadarTokenUpdatedCallback)(const char* token, BOOL passed, long expiresAt, int expiresIn);

    // Static variable to store the Unity callback
    static RadarTokenUpdatedCallback _tokenUpdatedCallback;

    @interface UnityRadarDelegate : NSObject <RadarVerifiedDelegate>
    @end

    @implementation UnityRadarDelegate

    - (void)didUpdateToken:(RadarVerifiedLocationToken *)token {
        if (_tokenUpdatedCallback && token) {
            const char *tokenString = [token.token UTF8String];
            BOOL passed = token.passed;
            long expiresAt = [token.expiresAt timeIntervalSince1970] * 1000; // Convert seconds to milliseconds
            int expiresIn = token.expiresIn;
            
            _tokenUpdatedCallback(tokenString, passed, expiresAt, expiresIn);
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
        } else if ([desiredAccuracyStr isEqualToString:@"NONE"]) {
            accuracyEnum = RadarTrackingOptionsDesiredAccuracyNone;
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


    typedef void (*RadarLocationCallback)(double latitude, double longitude, int callbackId);

    void Radar_getLocation(RadarLocationCallback callback, int callbackId) {
        [Radar getLocationWithCompletionHandler:^(RadarStatus status, CLLocation * _Nullable location, BOOL stopped) {
            if (status == RadarStatusSuccess && location) {
                double latitude = location.coordinate.latitude;
                double longitude = location.coordinate.longitude;
                callback(latitude, longitude, callbackId);  // Pass the callbackId
            } else {
                callback(-91, -181, callbackId);  // Pass invalid coordinates and callbackId on failure
            }
        }];
    }

#ifdef __cplusplus
}
#endif
