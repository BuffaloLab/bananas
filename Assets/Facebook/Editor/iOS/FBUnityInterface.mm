// Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
//
// You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
// copy, modify, and distribute this software in source code or binary form for use
// in connection with the web services and APIs provided by Facebook.
//
// As with any software that integrates with the Facebook platform, your use of
// this software is subject to the Facebook Developer Principles and Policies
// [http://developers.facebook.com/policy/]. This copyright notice shall be
// included in all copies or substantial portions of the software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#include "FBUnityInterface.h"

#import <FBSDKCoreKit/FBSDKCoreKit.h>
#import <FBSDKLoginKit/FBSDKLoginKit.h>
#import <FBSDKShareKit/FBSDKShareKit.h>
#import <Foundation/NSJSONSerialization.h>

#include "FBUnitySDKDelegate.h"
#include "FBUnityUtility.h"
#include "FBSDK+Internal.h"

static FBUnityInterface *_instance = [FBUnityInterface sharedInstance];

@interface FBUnityInterface()
@property (nonatomic, copy) NSString *openURLString;
@end

@implementation FBUnityInterface

#pragma mark Object Initialization

+ (FBUnityInterface *)sharedInstance
{
  return _instance;
}

+ (void)initialize {
  if(!_instance) {
    _instance = [[FBUnityInterface alloc] init];
  }
}

- (id)init
{
  if(_instance != nil) {
    return _instance;
  }

  if ((self = [super init])) {
    _instance = self;
    self.shareDialogMode = ShareDialogMode::AUTOMATIC;

    UnityRegisterAppDelegateListener(self);
  }
  return self;
}

#pragma mark - App (Delegate) Lifecycle

// didBecomeActive: and onOpenURL: are called by Unity's AppController
// because we implement <AppDelegateListener> and registered via UnityRegisterAppDelegateListener(...) above.

- (void)didFinishLaunching:(NSNotification *)notification
{
  [[FBSDKApplicationDelegate sharedInstance] application:[UIApplication sharedApplication]
                           didFinishLaunchingWithOptions:notification.userInfo];
}

- (void)didBecomeActive:(NSNotification *)notification
{
  [FBSDKAppEvents activateApp];
}

- (void)onOpenURL:(NSNotification *)notification
{
  NSURL *url = notification.userInfo[@"url"];
  BOOL isHandledByFBSDK = [[FBSDKApplicationDelegate sharedInstance] application:[UIApplication sharedApplication]
                                                                         openURL:url
                                                               sourceApplication:notification.userInfo[@"sourceApplication"]
                                                                      annotation:notification.userInfo[@"annotation"]];
  if (!isHandledByFBSDK) {
    [FBUnityInterface sharedInstance].openURLString = [url absoluteString];
  }
}

#pragma mark - Implementation

- (void)configureAppId:(const char *)appId
                cookie:(bool)cookie
               logging:(bool)logging
                status:(bool)status
  frictionlessRequests:(bool)frictionlessRequests
             urlSuffix:(const char *)urlSuffix
{
  self.useFrictionlessRequests = frictionlessRequests;

  if(appId) {
    [FBSDKSettings setAppID:[FBUnityUtility stringFromCString:appId]];
  }

  if(urlSuffix && strlen(urlSuffix) > 0) {
    [FBSDKSettings setAppURLSchemeSuffix:[FBUnityUtility stringFromCString:urlSuffix]];
  }

  [FBUnityUtility sendMessageToUnity:FBUnityMessageName_OnInitComplete userData:@{} requestId:0];
  [self tryCompleteLogin];
}

- (void)logInWithPublishPermissions:(const char *)scope
{
  [self startLogin:scope isPublishPermLogin:YES];
}

- (void)logInWithReadPermissions:(const char *)scope
{
  [self startLogin:scope isPublishPermLogin:NO];
}

- (void)startLogin:(const char *)scope isPublishPermLogin:(BOOL)isPublishPermLogin
{
  NSString *scopeStr = [FBUnityUtility stringFromCString:scope];
  NSArray *permissions = nil;
  if(scope && strlen(scope) > 0) {
    permissions = [scopeStr componentsSeparatedByString:@","];
  }

  void (^loginHandler)(FBSDKLoginManagerLoginResult *,NSError *) = ^(FBSDKLoginManagerLoginResult *result, NSError *error) {
    if (error) {
      [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnLoginComplete error:error requestId:0];
      return;
    } else if (result.isCancelled) {
      [FBUnityUtility sendCancelToUnity:FBUnityMessageName_OnLoginComplete requestId:0];
      return;
    }

    if ([self tryCompleteLogin]) {
      return;
    } else {
      [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnLoginComplete errorMessage:@"Unknown login error" requestId:0];
    }
  };

  FBSDKLoginManager *login = [[FBSDKLoginManager alloc] init];
  if (isPublishPermLogin) {
    [login logInWithPublishPermissions:permissions
                               handler:loginHandler];
  } else {
    [login logInWithReadPermissions:permissions
                            handler:loginHandler];
  }
}

- (void)logOut
{
  FBSDKLoginManager *login = [[FBSDKLoginManager alloc] init];
  [login logOut];
  [FBUnityUtility sendMessageToUnity:FBUnityMessageName_OnLogoutComplete userData:@{} requestId:0];
}

- (void)appRequestWithRequestId:(int)requestId
                        message:(const char *)message
                     actionType:(const char *)actionType
                       objectId:(const char *)objectId
                             to:(const char **)to
                       toLength:(int)toLength
                        filters:(const char *)filters
                           data:(const char *)data
                          title:(const char *)title
{
  FBSDKGameRequestContent *content = [[FBSDKGameRequestContent alloc] init];
  content.message = [FBUnityUtility stringFromCString:message];
  content.actionType = [FBUnityUtility gameRequestActionTypeFromString:[FBUnityUtility stringFromCString:actionType]];
  content.objectID = [FBUnityUtility stringFromCString:objectId];
  if(to && toLength) {
    NSMutableArray *toArray = [NSMutableArray array];
    for(int i = 0; i < toLength; i++) {
      [toArray addObject:[FBUnityUtility stringFromCString:to[i]]];
    }
    content.recipients = toArray;
  }
  content.filters = [FBUnityUtility gameRequestFilterFromString:[FBUnityUtility stringFromCString:filters]];
  content.data = [FBUnityUtility stringFromCString:data];
  content.title = [FBUnityUtility stringFromCString:title];

  FBUnitySDKDelegate *delegate = [FBUnitySDKDelegate instanceWithRequestID:requestId];
  NSError *error;
  FBSDKGameRequestDialog *dialog = [[FBSDKGameRequestDialog alloc] init];
  dialog.content = content;
  dialog.delegate = delegate;
  dialog.frictionlessRequestsEnabled = self.useFrictionlessRequests;

  if (![dialog validateWithError:&error]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnAppRequestsComplete error:error requestId:requestId];
  }
  if (![dialog show]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnAppRequestsComplete errorMessage:@"Failed to show request dialog" requestId:requestId];
  }
}

- (void)appInviteWithRequestId:(int)requestId
                    appLinkUrl:(const char *)appLinkUrl
               previewImageUrl:(const char *)previewImageUrl
{
  FBSDKAppInviteContent *content = [[FBSDKAppInviteContent alloc] init];
  content.appLinkURL = [NSURL URLWithString:[FBUnityUtility stringFromCString:appLinkUrl]];
  content.appInvitePreviewImageURL = [NSURL URLWithString:[FBUnityUtility stringFromCString:previewImageUrl]];
  FBUnitySDKDelegate *delegate = [FBUnitySDKDelegate instanceWithRequestID:requestId];
  [FBSDKAppInviteDialog showWithContent:content
                               delegate:delegate];
}


- (void)shareLinkWithRequestId:(int)requestId
                    contentURL:(const char *)contentURL
                  contentTitle:(const char *)contentTitle
            contentDescription:(const char *)contentDescription
                      photoURL:(const char *)photoURL
{
  FBSDKShareLinkContent *linkContent = [[FBSDKShareLinkContent alloc] init];

  NSString *contentUrlStr = [FBUnityUtility stringFromCString:contentURL];
  if (contentUrlStr) {
    linkContent.contentURL = [NSURL URLWithString:contentUrlStr];
  }

  NSString *contentTitleStr = [FBUnityUtility stringFromCString:contentTitle];
  if (contentTitleStr) {
    linkContent.contentTitle = contentTitleStr;
  }

  NSString *contentDescStr = [FBUnityUtility stringFromCString:contentDescription];
  if (contentDescStr) {
    linkContent.contentDescription = contentDescStr;
  }

  NSString *imageURL = [FBUnityUtility stringFromCString:photoURL];
  if (imageURL) {
    linkContent.imageURL = [NSURL URLWithString:imageURL];
  }

  [self shareContentWithRequestId:requestId
                     shareContent:linkContent
                       dialogMode:[self getDialogMode]];
}

- (void)shareFeedWithRequestId:(int)requestId
                          toId:(const char *)toID
                          link:(const char *)link
                      linkName:(const char *)linkName
                   linkCaption:(const char *)linkCaption
               linkDescription:(const char *)linkDescription
                       picture:(const char *)picture
                   mediaSource:(const char *)mediaSource
{
  FBSDKShareLinkContent *linkContent = [[FBSDKShareLinkContent alloc] init];
  NSString *contentUrlStr = [FBUnityUtility stringFromCString:link];
  if (contentUrlStr) {
    linkContent.contentURL = [NSURL URLWithString:contentUrlStr];
  }

  NSString *contentTitleStr = [FBUnityUtility stringFromCString:linkName];
  if (contentTitleStr) {
    linkContent.contentTitle = contentTitleStr;
  }

  NSString *contentDescStr = [FBUnityUtility stringFromCString:linkDescription];
  if (contentDescStr) {
    linkContent.contentDescription = contentDescStr;
  }

  NSString *imageURL = [FBUnityUtility stringFromCString:picture];
  if (imageURL) {
    linkContent.imageURL = [NSURL URLWithString:imageURL];
  }

  NSMutableDictionary *feedParameters = [[NSMutableDictionary alloc] init];
  NSString *toStr = [FBUnityUtility stringFromCString:toID];
  if (toStr) {
    [feedParameters setObject:toStr forKey:@"to"];
  }

  NSString *captionStr = [FBUnityUtility stringFromCString:linkCaption];
  if (captionStr) {
    [feedParameters setObject:captionStr forKey:@"caption"];
  }

  NSString *sourceStr = [FBUnityUtility stringFromCString:mediaSource];
  if (sourceStr) {
    [feedParameters setObject:sourceStr forKey:@"source"];
  }

  linkContent.feedParameters = feedParameters;
  [self shareContentWithRequestId:requestId
                     shareContent:linkContent
                       dialogMode:FBSDKShareDialogModeFeedWeb];
}

- (void)shareContentWithRequestId:(int)requestId
                     shareContent:(FBSDKShareLinkContent *)linkContent
                       dialogMode:(FBSDKShareDialogMode)dialogMode
{
  FBSDKShareDialog *dialog = [[FBSDKShareDialog alloc] init];
  dialog.shareContent = linkContent;
  dialog.mode = dialogMode;
  FBUnitySDKDelegate *delegate = [FBUnitySDKDelegate instanceWithRequestID:requestId];
  dialog.delegate = delegate;

  NSError *error;
  if (![dialog validateWithError:&error]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnShareLinkComplete error:error requestId:requestId];
  }
  if (![dialog show]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnShareLinkComplete errorMessage:@"Failed to show share dialog" requestId:requestId];
  }
}

- (FBSDKShareDialogMode)getDialogMode
{
  switch (self.shareDialogMode) {
    case ShareDialogMode::AUTOMATIC:
      return FBSDKShareDialogModeAutomatic;
    case ShareDialogMode::NATIVE:
      return FBSDKShareDialogModeNative;
    case ShareDialogMode::WEB:
      return FBSDKShareDialogModeWeb;
    case ShareDialogMode::FEED:
      return FBSDKShareDialogModeFeedWeb;
    default:
      NSLog(@"Unexpected dialog mode: %@", [NSNumber numberWithInt:self.shareDialogMode]);
      return FBSDKShareDialogModeAutomatic;
  }
}

- (void)showJoinAppGroupDialogWithRequestId:(int) requestId
                                    groupId:(const char *) groupId
{
  FBUnitySDKDelegate *delegate = [FBUnitySDKDelegate instanceWithRequestID:requestId];
  [FBSDKAppGroupJoinDialog showWithGroupID:[FBUnityUtility stringFromCString:groupId] delegate:delegate];
}

- (void)showCreateAppGroupDialogWithRequestId:(int) requestId
                                    groupName:(const char *) groupName
                             groupDescription:(const char *) groupDescription
                                 groupPrivacy:(const char *) groupPrivacy
{
  FBSDKAppGroupContent *content = [[FBSDKAppGroupContent alloc] init];
  content.name = [FBUnityUtility stringFromCString:groupName];
  content.groupDescription = [FBUnityUtility stringFromCString:groupDescription];

  FBSDKAppGroupPrivacy privacy;
  NSString *privacyStr = [FBUnityUtility stringFromCString:groupPrivacy];
  if ([privacyStr caseInsensitiveCompare:@"closed"] == NSOrderedSame) {
    privacy = FBSDKAppGroupPrivacyClosed;
  } else if ([privacyStr caseInsensitiveCompare:@"open"] == NSOrderedSame) {
    privacy = FBSDKAppGroupPrivacyOpen;
  } else {
    NSLog(@"Unexpced privacy type: %@", privacyStr);
    privacy = FBSDKAppGroupPrivacyClosed;
  }

  content.privacy = privacy;

  FBSDKAppGroupAddDialog *dialog = [[FBSDKAppGroupAddDialog alloc] init];
  dialog.content = content;
  dialog.delegate = [FBUnitySDKDelegate instanceWithRequestID:requestId];

  NSError *error;
  if (![dialog validateWithError:&error]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnGroupCreateComplete error:error requestId:requestId];
  }
  if (![dialog show]) {
    [FBUnityUtility sendErrorToUnity:FBUnityMessageName_OnGroupCreateComplete errorMessage:@"Failed to show group create dialog" requestId:requestId];
  }
}

- (BOOL)tryCompleteLogin
{
  FBSDKAccessToken *token = [FBSDKAccessToken currentAccessToken];
  if (token) {
    NSInteger expiration = token.expirationDate.timeIntervalSince1970;
    [FBUnityUtility sendMessageToUnity:FBUnityMessageName_OnLoginComplete
                              userData:@{
                                         @"opened" : @"true",
                                         @"access_token" : token.tokenString,
                                         @"expiration_timestamp" : [@(expiration) stringValue],
                                         @"user_id" : token.userID,
                                         @"permissions" : [token.permissions allObjects],
                                         @"granted_permissions" : [token.permissions allObjects],
                                         @"declined_permissions" : [token.declinedPermissions allObjects]
                                         }
                             requestId:0];
    return YES;
  } else {
    return NO;
  }
}

@end

#pragma mark - Actual Unity C# interface (extern C)

extern "C" {

  void iosInit(const char *_appId, bool _cookie, bool _logging, bool _status, bool _frictionlessRequests, const char *_urlSuffix, const char *_userAgentSuffix)
  {
    [[FBUnityInterface sharedInstance] configureAppId:_appId
                                               cookie:_cookie
                                              logging:_logging
                                               status:_status
                                 frictionlessRequests:_frictionlessRequests
                                            urlSuffix:_urlSuffix];
    [FBSDKSettings setUserAgentSuffix:[FBUnityUtility stringFromCString:_userAgentSuffix]];
  }

  void iosLogInWithReadPermissions(const char *scope)
  {
    [[FBUnityInterface sharedInstance] logInWithReadPermissions:scope];
  }

  void iosLogInWithPublishPermissions(const char *scope)
  {
    [[FBUnityInterface sharedInstance] logInWithPublishPermissions:scope];
  }

  void iosLogOut()
  {
    [[FBUnityInterface sharedInstance] logOut];
  }

  void iosSetShareDialogMode(int mode)
  {
    [FBUnityInterface sharedInstance].shareDialogMode = static_cast<ShareDialogMode>(mode);
  }

  void iosAppRequest(int requestId,
                     const char *message,
                     const char *actionType,
                     const char *objectId,
                     const char **to,
                     int toLength,
                     const char *filters,
                     const char **excludeIds, //not supported on mobile
                     int excludeIdsLength, //not supported on mobile
                     bool hasMaxRecipients, //not supported on mobile
                     int maxRecipients, //not supported on mobile
                     const char *data,
                     const char *title)
  {
    [[FBUnityInterface sharedInstance] appRequestWithRequestId: requestId
                                                       message: message
                                                    actionType: actionType
                                                      objectId: objectId
                                                            to: to
                                                      toLength: toLength
                                                       filters: filters
                                                          data: data
                                                         title: title];
  }

  void iosAppInvite(int requestId,
                    const char *appLinkUrl,
                    const char *previewImageUrl)
  {
    [[FBUnityInterface sharedInstance] appInviteWithRequestId:requestId
                                                   appLinkUrl:appLinkUrl
                                              previewImageUrl:previewImageUrl];
  }

  void iosGetDeepLink(int requestId)
  {
    NSString *url = [FBUnityInterface sharedInstance].openURLString ?: @"";
    NSDictionary *dict = @{
      @"deep_link" : url,
      @"callback_id" : [NSNumber numberWithInt:requestId],
    };

    NSError *serError = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&serError];
    NSString *jsonString = nil;
    if (jsonData) {
      jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }

    [FBUnityUtility sendMessageToUnity:FBUnityMessageName_OnGetDeepLinkComplete userData:dict requestId:0];

    [FBUnityInterface sharedInstance].openURLString = nil;
  }

  void iosShareLink(int requestId,
                    const char *contentURL,
                    const char *contentTitle,
                    const char *contentDescription,
                    const char *photoURL)
  {
    [[FBUnityInterface sharedInstance] shareLinkWithRequestId:requestId
                                                   contentURL:contentURL
                                                 contentTitle:contentTitle
                                           contentDescription:contentDescription
                                                     photoURL:photoURL];
  }

  void iosFeedShare(int requestId,
                    const char *toId,
                    const char *link,
                    const char *linkName,
                    const char *linkCaption,
                    const char *linkDescription,
                    const char *picture,
                    const char *mediaSource)
  {
    [[FBUnityInterface sharedInstance] shareFeedWithRequestId:requestId
                                                         toId:toId
                                                         link:link
                                                     linkName:linkName
                                                  linkCaption:linkCaption
                                              linkDescription:linkDescription
                                                      picture:picture
                                                  mediaSource:mediaSource];
  }

  void iosJoinGameGroup(int requestId, const char *groupId)
  {
    [[FBUnityInterface sharedInstance] showJoinAppGroupDialogWithRequestId:requestId groupId:groupId];
  }

  void iosCreateGameGroup(int requestId, const char *groupName, const char *groupDescription, const char *groupPrivacy)
  {
    [[FBUnityInterface sharedInstance] showCreateAppGroupDialogWithRequestId:requestId groupName:groupName groupDescription:groupDescription groupPrivacy:groupPrivacy];
  }

  void iosFBSettingsActivateApp(const char *appId)
  {
    [FBSDKAppEvents activateApp];
  }

  void iosFBAppEventsLogEvent(const char *eventName,
                              double valueToSum,
                              int numParams,
                              const char **paramKeys,
                              const char **paramVals)
  {
    NSDictionary *params =  [FBUnityUtility dictionaryFromKeys:paramKeys values:paramVals length:numParams];
    [FBSDKAppEvents logEvent:[FBUnityUtility stringFromCString:eventName] valueToSum:valueToSum parameters:params];
  }

  void iosFBAppEventsLogPurchase(double amount,
                                 const char *currency,
                                 int numParams,
                                 const char **paramKeys,
                                 const char **paramVals)
  {
    NSDictionary *params =  [FBUnityUtility dictionaryFromKeys:paramKeys values:paramVals length:numParams];
    [FBSDKAppEvents logPurchase:amount currency:[FBUnityUtility stringFromCString:currency] parameters:params];
  }

  void iosFBAppEventsSetLimitEventUsage(BOOL limitEventUsage)
  {
    [FBSDKSettings setLimitEventAndDataUsage:limitEventUsage];
  }

  char* iosFBSdkVersion()
  {
    const char* string = [[FBSDKSettings sdkVersion] UTF8String];
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
  }
}
