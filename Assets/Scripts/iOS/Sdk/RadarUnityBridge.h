#import <RadarSDK/Radar.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*_Nullable CompletionHandlerPtrOnDict)(int requestId, const char* _Nullable statusStr, const char* _Nullable dict);

void Radar_initializeWithPublishableKey(const char* _Nullable publishableKey);
void Radar_setUserId(const char* _Nullable userId);
void Radar_trackVerifiedWithCompletionHandler(int requestId, CompletionHandlerPtrOnDict _Nullable handler);


#ifdef __cplusplus
}
#endif
