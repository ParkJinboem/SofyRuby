//
//  Created by 손석영 on 2021/03/05.
//  Copyright © 2021 손석영. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

typedef void (*CompletionHandler)(BOOL isAgree);

@interface RequestTrackingAuthorizationPlugin : NSObject

@end

@implementation RequestTrackingAuthorizationPlugin

+ (void)RequestTrackingAuthorization:(CompletionHandler)completion {
    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            switch (status) {
                case ATTrackingManagerAuthorizationStatusAuthorized:
                    completion(true);
                    NSLog(@"ATTrackingManagerAuthorizationStatusAuthorized");
                    break;
                case ATTrackingManagerAuthorizationStatusDenied:
                    completion(false);
                    NSLog(@"ATTrackingManagerAuthorizationStatusDenied");
                    break;
                case ATTrackingManagerAuthorizationStatusRestricted:
                    completion(false);
                    NSLog(@"ATTrackingManagerAuthorizationStatusRestricted");
                    break;
                case ATTrackingManagerAuthorizationStatusNotDetermined:
                    completion(false);
                    NSLog(@"ATTrackingManagerAuthorizationStatusNotDetermined");
                    break;
                default:
                    completion(false);
                    break;
            }
        }];
    } else {
        completion(true);
    }
}

@end

extern "C" {
    void _RequestTrackingAuthorization (CompletionHandler completion) {
        [RequestTrackingAuthorizationPlugin RequestTrackingAuthorization:completion];
    }
}
