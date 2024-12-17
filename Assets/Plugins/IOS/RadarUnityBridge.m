// #import "RadarUnityBridge.h"
#import <RadarSDK/Radar.h>
#import <RadarSDK/RadarVerifiedLocationToken.h>
#import <RadarSDK/CustomVerifiedDelegate.h>

#ifdef __cplusplus
extern "C" {
#endif

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


    void Radar_trackVerifiedWithCompletionHandler(int requestId, CompletionHandlerPtrOnDict handler)
    {
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
        CustomVerifiedDelegate *delegate = [CustomVerifiedDelegate sharedInstance];
        [delegate setTokenUpdatedCallback:callback];
        [Radar setVerifiedDelegate:delegate];
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
