#import <Foundation/Foundation.h>
#import "Radar.h"  // Radar SDK to access RadarVerifiedDelegate protocol

typedef void (*RadarTokenUpdatedCallback)(const char* token, BOOL passed, long expiresAt, int expiresIn);

@interface CustomVerifiedDelegate : NSObject <RadarVerifiedDelegate>

@property(nonatomic) RadarTokenUpdatedCallback callback;

+(instancetype)sharedInstance;
-(void)setTokenUpdatedCallback:(RadarTokenUpdatedCallback)callback;

@end
