using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Views;
using FloatingView.Lib;
using Newtonsoft.Json;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Helpers.Utils;
using Uri = Android.Net.Uri;

namespace WoWonder.Frameworks.Floating
{
    public class FloatingObject
    {
        public string PageId { set; get; }
        public string GroupId { set; get; }
        public string UserId { set; get; }
        public string Avatar { set; get; }
        public string ChatType { set; get; } 
        public string ChatColor { set; get; } 
        public string Name { set; get; } 
        public string LastSeen { set; get; } 
        public string LastSeenUnixTime { set; get; } 
        public string MessageCount { set; get; } 
    }

    public class InitFloating
    {
        private static Activity ActivityContext;
        public static readonly int ChatHeadDataRequestCode = 5599;
        public static FloatingObject FloatingObject;

        public InitFloating()
        {
            try
            {
                SettingsPrefFragment.SChatHead = CanDrawOverlays(Application.Context);
                ActivityContext = MainApplication.Activity;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public void FloatingShow(FloatingObject userData)
        {
            try
            {
                if (!SettingsPrefFragment.SChatHead)
                    return;

                FloatingObject = userData;
                
                if (CanDrawOverlays(Application.Context))
                {
                    StartFloatingViewService(Application.Context, userData);
                    return;
                }

                //Intent intent = new Intent(Settings.ActionManageOverlayPermission, Uri.Parse("package:" + ActivityContext.PackageName));
                //ActivityContext.StartActivityForResult(intent, ChatHeadDataRequestCode);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public bool CheckPermission()
        {
            try
            {
                if (CanDrawOverlays(Application.Context))
                    return true; 

                return false; 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }
         
        public void OpenManagePermission()
        {
            try
            {
                if (CanDrawOverlays(Application.Context))
                    return;
                  
                Intent intent = new Intent(Settings.ActionManageOverlayPermission, Uri.Parse("package:" + Application.Context.PackageName));
                ActivityContext.StartActivityForResult(intent, ChatHeadDataRequestCode);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace); 
            }
        }
         
        private void StartFloatingViewService(Context context , FloatingObject userData)
        {
            try
            {
                // *** You must follow these rules when obtain the cutout(FloatingViewManager.findCutoutSafeArea) ***
                try
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    {
                        // 1. 'windowLayoutInDisplayCutoutMode' do not be set to 'never'
                        if (ActivityContext?.Window.Attributes.LayoutInDisplayCutoutMode == LayoutInDisplayCutoutMode.Never)
                        {
                            //Toast.MakeText(Application.Context, "windowLayoutInDisplayCutoutMode' do not be set to 'never" , ToastLength.Short).Show();
                            //throw new Exception("'windowLayoutInDisplayCutoutMode' do not be set to 'never'");
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
                 
                if (ChatHeadService.RunService)
                    return;

                // launch service 
                Intent intent = new Intent(context, typeof(ChatHeadService));

                if (ActivityContext != null)
                    intent.PutExtra(ChatHeadService.ExtraCutoutSafeArea, FloatingViewManager.FindCutoutSafeArea(ActivityContext));

                intent.PutExtra("UserData", JsonConvert.SerializeObject(userData));
                ContextCompat.StartForegroundService(context, intent);

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
         
        public static bool CanDrawOverlays(Context context)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.M) return true;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.OMr1)
                {
                    return Settings.CanDrawOverlays(context);
                }

                if (Settings.CanDrawOverlays(context)) return true;
                try
                {
                    var mgr = (IWindowManager)context.GetSystemService(Context.WindowService);
                    if (mgr == null) return false; //getSystemService might return null 
                    View viewToAdd = new View(context);
                    var paramsParams = new WindowManagerLayoutParams(0, 0, Build.VERSION.SdkInt >= BuildVersionCodes.O ? WindowManagerTypes.ApplicationOverlay : WindowManagerTypes.SystemAlert, WindowManagerFlags.NotTouchable | WindowManagerFlags.NotFocusable, Format.Transparent);
                    viewToAdd.LayoutParameters = (paramsParams);
                    mgr.AddView(viewToAdd, paramsParams);
                    mgr.RemoveView(viewToAdd);
                    return true;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }

    }
}