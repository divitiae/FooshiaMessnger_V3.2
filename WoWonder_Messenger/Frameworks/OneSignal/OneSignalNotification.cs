using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Com.OneSignal.Abstractions;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using OneSignal = Com.OneSignal.OneSignal;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;

namespace WoWonder.Frameworks.onesignal
{
    public static class OneSignalNotification
    {
        //Force your app to Register notifcation derictly without loading it from server (For Best Result) 
        private static string Userid;

        public static void RegisterNotificationDevice()
        {
            try
            {
                if (UserDetails.NotificationPopup)
                {
                    if (!string.IsNullOrEmpty(AppSettings.OneSignalAppId) || !string.IsNullOrWhiteSpace(AppSettings.OneSignalAppId))
                    {
                        OneSignal.Current.StartInit(AppSettings.OneSignalAppId)
                            .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived(HandleNotificationReceived)
                            .HandleNotificationOpened(HandleNotificationOpened)
                            .EndInit();
                        OneSignal.Current.IdsAvailable(IdsAvailable);
                        OneSignal.Current.RegisterForPushNotifications();
                        OneSignal.Current.SetSubscription(true);
                        AppSettings.ShowNotification = true;
                    }
                }
                else
                {
                    Un_RegisterNotificationDevice();
                } 
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        public static void Un_RegisterNotificationDevice()
        {
            try
            {
                OneSignal.Current.SetSubscription(false);
                AppSettings.ShowNotification = false;
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        private static void IdsAvailable(string userId, string pushToken)
        {
            try
            {
                UserDetails.DeviceId = userId;
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        private static void HandleNotificationReceived(OSNotification notification)
        {
            try
            {
                OSNotificationPayload payload = notification.payload;
                //Dictionary<string, object> additionalData = payload.additionalData;

                string message = payload.body;
                if (message.Contains("call") || message.Contains("Calling"))
                {
                    notification.shown = false;
                    notification.displayType = OSNotification.DisplayType.None;
                    OneSignal.Current.ClearAndroidOneSignalNotifications();
                }
            }
            catch (Exception ex)
            {

               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            try
            {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                // string message = payload.body;
                string actionId = result.action.actionID;

                if (additionalData != null)
                {
                    foreach (var item in additionalData)
                    {
                        if (item.Key == "user_id")
                        {
                            Userid = item.Value.ToString();
                        }
                    }

                    //Intent intent = new Intent(Application.Context.PackageName + ".FOO");
                    Intent intent = new Intent(Application.Context, typeof(TabbedMainActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    intent.PutExtra("UserID", Userid);
                    intent.PutExtra("Notifier", "Notifier");
                    Application.Context.StartActivity(intent);

                    if (additionalData.ContainsKey("discount"))
                    {
                        // Take user to your store..
                    }
                }

                if (actionId != null)
                {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present. 
                }
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }
    }
}