#import "RadarUnityBridge.h"
#import <RadarSDK/Radar.h>
#import <RadarSDK/RadarVerifiedLocationToken.h>

void Radar_initializeWithPublishableKey(const char* publishableKey) {
 [Radar initializeWithPublishableKey:[NSString stringWithUTF8String:publishableKey]];
}
void Radar_setUserId(const char* userId) {
 [Radar setUserId:[NSString stringWithUTF8String:userId]];
}

void Radar_trackVerifiedWithCompletionHandler(int requestId, CompletionHandlerPtrOnDict _Nullable handler)
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
