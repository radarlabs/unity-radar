#import <RadarSDK/Radar.h>
#import <RadarSDK/RadarDelegate.h>
#import <RadarSDK/RadarVerifiedDelegate.h>
#import <RadarSDK/RadarVerifiedLocationToken.h>

#ifdef __cplusplus
extern "C" {
#endif

    typedef void (*RadarLogCallback)(const char* message);
    typedef void (*RadarErrorCallback)(const char* statusStr);
    typedef void (*RadarTokenUpdatedCallback)(const char* jsonData);

    static RadarLogCallback _logCallback;
    static RadarErrorCallback _errorCallback;
    static RadarTokenUpdatedCallback _tokenUpdatedCallback;

    @interface UnityRadarDelegate : NSObject <RadarDelegate, RadarVerifiedDelegate>
    @end

    @implementation UnityRadarDelegate

    - (void)didReceiveEvents:(NSArray<RadarEvent *> *)events user:(RadarUser *)user {
        // TODO: Implement
    }

    - (void)didUpdateLocation:(CLLocation *)location user:(RadarUser *)user {
        // TODO: Implement
    }

    - (void)didUpdateClientLocation:(CLLocation *)location stopped:(BOOL)stopped source:(RadarLocationSource)source {
        // TODO: Implement
    }

    - (void)didFailWithStatus:(RadarStatus)status {
        if (_errorCallback) {
            _errorCallback([[Radar stringForStatus:status] UTF8String]);
        }
    }

    - (void)didLogMessage:(NSString *)message {
        if (_logCallback) {
            _logCallback([message UTF8String]);
        }
    }

    - (void)didUpdateToken:(RadarVerifiedLocationToken *)token {
        if (_tokenUpdatedCallback && token) {
            NSDictionary *dict = [token dictionaryValue];
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
            if (!error) {
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                _tokenUpdatedCallback([jsonString UTF8String]);
            } // todo: surface error
        }
    }

    @end

    static UnityRadarDelegate *unityRadarDelegate;


    typedef void (*TokenHandlerPtr)(int requestId, const char* statusStr, const char* tokenStr);
    typedef void (*LocationHandlerPtr)(int requestId, const char* statusStr, const char* locationStr, bool stopped);
    typedef void (*TrackOnceHandlerPtr)(int requestId, const char* statusStr, const char* locationStr, const char* eventsStr, const char* userStr);


    void Radar_initializeWithPublishableKey(const char* publishableKey) {
        [Radar initializeWithPublishableKey:[NSString stringWithUTF8String:publishableKey]];
    }


    void Radar_setUserId(const char* userId) {
        [Radar setUserId:[NSString stringWithUTF8String:userId]];
    }


    void Radar_trackVerified(
        int requestId,
        TokenHandlerPtr handler,
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
            const char *tokenStr = NULL;

            if (status == RadarStatusSuccess && token != nil)
            {
                NSDictionary *dict = [token dictionaryValue];
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
                if (!error)
                {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    tokenStr = [jsonString UTF8String];
                }
            }

            handler(requestId, statusStr, tokenStr);
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


    void Radar_getVerifiedLocationToken(int requestId, TokenHandlerPtr handler) {
        [Radar getVerifiedLocationToken:^(RadarStatus status, RadarVerifiedLocationToken * _Nullable token) {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *tokenStr = NULL;

            if (status == RadarStatusSuccess && token != nil) {
                NSDictionary *dict = [token dictionaryValue];
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                if (!error) {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    tokenStr = [jsonString UTF8String];
                }
            }

            // Call the C# callback handler
            handler(requestId, statusStr, tokenStr);
        }];
    }


    void Radar_setDelegateCallbacks(RadarLogCallback logCallback, RadarErrorCallback errorCallback, RadarTokenUpdatedCallback tokenUpdatedCallback) {
        _logCallback = logCallback;
        _errorCallback = errorCallback;
        _tokenUpdatedCallback = tokenUpdatedCallback;

        if (!unityRadarDelegate) {
            unityRadarDelegate = [[UnityRadarDelegate alloc] init];
            [Radar setDelegate:unityRadarDelegate];
            [Radar setVerifiedDelegate:unityRadarDelegate];
        }

    }


    void Radar_getLocation(int requestId, LocationHandlerPtr handler) {
        [Radar getLocationWithCompletionHandler:^(RadarStatus status, CLLocation * _Nullable location, BOOL stopped) {

            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *locationStr = NULL;

            if (status == RadarStatusSuccess && location != nil) {
                NSArray *coordinates = @[@(location.coordinate.longitude), @(location.coordinate.latitude)];
                NSDictionary *dict = @{
                    @"coordinates": coordinates,
                };
                NSError *error;
                NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                if (!error) {
                    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                    locationStr = [jsonString UTF8String];
                }
            }

            handler(requestId, statusStr, locationStr, stopped);
        }];
    }

    void Radar_trackOnce(
        int requestId,
        TrackOnceHandlerPtr handler,
        const char* desiredAccuracy,
        bool beacons
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

        [Radar trackOnceWithCompletionHandler:^(RadarStatus status, CLLocation * _Nullable location, NSArray<RadarEvent *> * _Nullable events, RadarUser * _Nullable user)
        {
            const char *statusStr = [[Radar stringForStatus:status] UTF8String];
            const char *locationStr = NULL;
            const char *eventsStr = NULL;
            const char *userStr = NULL;

            if (status == RadarStatusSuccess)
            {
                if (location != nil)
                {
                    NSArray *coordinates = @[@(location.coordinate.longitude), @(location.coordinate.latitude)];
                    NSDictionary *dict = @{
                        @"coordinates": coordinates,
                    };
                    NSError *error;
                    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                    if (!error) {
                        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                        locationStr = [jsonString UTF8String];
                    }
                }

                if (events != nil && events.count > 0)
                {
                    NSMutableArray *eventsArray = [NSMutableArray array];
                    for (RadarEvent *event in events) {
                        NSDictionary *eventDict = [event dictionaryValue];
                        [eventsArray addObject:eventDict];
                    }
                    NSError *error;
                    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:eventsArray options:0 error:&error];

                    if (!error) {
                        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                        eventsStr = [jsonString UTF8String];
                    }
                }

                if (user != nil)
                {
                    NSDictionary *dict = [user dictionaryValue];
                    NSError *error;
                    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];

                    if (!error) {
                        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                        userStr = [jsonString UTF8String];
                    }
                }
            }

            handler(requestId, statusStr, locationStr, eventsStr, userStr);
        }];
    }

#ifdef __cplusplus
}
#endif
