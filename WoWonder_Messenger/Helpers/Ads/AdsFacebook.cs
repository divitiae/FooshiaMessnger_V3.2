﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WoWonder.Helpers.Utils;
using Xamarin.Facebook.Ads;

namespace WoWonder.Helpers.Ads
{
    public static class AdsFacebook 
    {
        private static int CountInterstitial;
        private static int CountRewarded;
       

        #region Banner

        public static AdView InitAdView(Activity activity ,LinearLayout adContainer)
        {
            try
            {
                if (AppSettings.ShowFbBannerAds)
                {
                    InitializeFacebook.Initialize(activity);

                    AdView adView = new AdView(activity, AppSettings.AdsFbBannerKey, AdSize.BannerHeight50);
                    // Add the ad view to your activity layout
                    adContainer.AddView(adView);

                    adView.SetAdListener(new BannerAdListener());
                    // Request an ad
                    adView.LoadAd();

                    return adView;
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        private class BannerAdListener : Java.Lang.Object, IAdListener
        {

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {

            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }
        }

        #endregion

        #region Interstitial

        public static InterstitialAd InitInterstitial(Activity activity)
        {
            try
            {
                if (AppSettings.ShowFbInterstitialAds)
                {
                    if (CountInterstitial == AppSettings.ShowAdMobInterstitialCount)
                    {
                        InitializeFacebook.Initialize(activity);

                        CountInterstitial = 0;
                        var interstitialAd = new InterstitialAd(activity, AppSettings.AdsFbInterstitialKey);

                        interstitialAd.SetAdListener(new MyInterstitialAdListener(activity, interstitialAd));
                        // Request an ad
                        interstitialAd.LoadAd();

                        return interstitialAd;
                    }

                    CountInterstitial++;
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        private class MyInterstitialAdListener : Java.Lang.Object, IInterstitialAdListener
        {
            private readonly InterstitialAd InterstitialAd;
            private readonly Activity Activity;
            public MyInterstitialAdListener(Activity activity, InterstitialAd interstitialAd)
            {
                Activity = activity;
                InterstitialAd = interstitialAd;
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    // Show the ad
                    InterstitialAd?.Show();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }

            /// <summary>
            /// Interstitial dismissed callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnInterstitialDismissed(IAd ad)
            {

            }

            /// <summary>
            /// Interstitial ad displayed callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnInterstitialDisplayed(IAd ad)
            {

            }
        }

        #endregion

        #region RewardVideo

        public static RewardedVideoAd InitRewardVideo(Activity activity)
        {
            try
            {
                if (AppSettings.ShowFbRewardVideoAds)
                {
                    if (CountRewarded == AppSettings.ShowAdMobRewardedVideoCount)
                    {
                        InitializeFacebook.Initialize(activity);

                        CountRewarded = 0;

                       var rewardVideoAd = new RewardedVideoAd(activity, AppSettings.AdsFbRewardVideoKey);

                       rewardVideoAd.SetAdListener(new MyRRewardVideoAdListener(activity, rewardVideoAd));
                       rewardVideoAd.LoadAd(); 
                        //RewardVideoAd.SetRewardData(new RewardData("YOUR_USER_ID", "YOUR_REWARD"));

                        return rewardVideoAd;
                    }

                    CountRewarded++; 
                }
                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        private class MyRRewardVideoAdListener : Java.Lang.Object, IS2SRewardedVideoAdListener
        {
            private readonly RewardedVideoAd RewardVideoAd;
            private readonly Activity Activity;
            public MyRRewardVideoAdListener(Activity activity, RewardedVideoAd rewardVideoAd)
            {
                Activity = activity;
                RewardVideoAd = rewardVideoAd;
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    // Show the ad
                    RewardVideoAd?.Show();
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }


            /// <summary>
            /// Rewarded Video Closed
            /// </summary>
            public void OnRewardedVideoClosed()
            {

            }

            /// <summary>
            /// Rewarded Video View Complete
            /// </summary>
            public void OnRewardedVideoCompleted()
            {

            }

            /// <summary>
            /// Reward Video Server Failed
            /// </summary>
            public void OnRewardServerFailed()
            {

            }

            /// <summary>
            /// Reward Video Server Succeeded
            /// </summary>
            public void OnRewardServerSuccess()
            {

            }
        }

        #endregion

        #region Native

        public static void InitNative(Activity activity , NativeAdLayout nativeAdLayout , NativeAd ad)
        {
            try
            {
                if (AppSettings.ShowFbNativeAds)
                {
                    if (ad == null)
                    {
                        var nativeAd = new NativeAd(activity, AppSettings.AdsFbNativeKey);
                        nativeAd.SetAdListener(new NativeAdListener(activity, nativeAd, nativeAdLayout));
                        // Initiate a request to load an ad.
                        nativeAd.LoadAd();
                    }
                    else
                    {
                        var holder = new AdHolder(nativeAdLayout);
                        LoadAd(activity ,holder, ad, nativeAdLayout);
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace); 
            }
        }

        private class NativeAdListener : Java.Lang.Object, INativeAdListener
        {
            private readonly NativeAd NativeAd;
            private readonly NativeAdLayout NativeAdLayout;
            private readonly Activity Activity;
            public NativeAdListener(Activity activity, NativeAd nativeAd, NativeAdLayout nativeAdLayout)
            {
                Activity = activity;
                NativeAd = nativeAd;
                NativeAdLayout = nativeAdLayout;
            }

            /// <summary>
            /// Ad clicked callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdClicked(IAd ad)
            {

            }

            /// <summary>
            /// Ad loaded callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnAdLoaded(IAd ad)
            {
                try
                {
                    if (NativeAd == null || NativeAd != ad)
                    {
                        // Race condition, load() called again before last ad was displayed
                        return;
                    }

                    if (NativeAdLayout == null)
                    {
                        return;
                    }

                    Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            NativeAdLayout.Visibility = ViewStates.Visible;

                            // Unregister last ad
                            NativeAd.UnregisterView();

                            if (NativeAdChoicesContainer != null)
                            {
                                var adOptionsView = new AdOptionsView(Activity, NativeAd, NativeAdLayout);
                                NativeAdChoicesContainer.RemoveAllViews();
                                NativeAdChoicesContainer.AddView(adOptionsView, 0);
                            }
                            
                            InflateAd(NativeAd, NativeAdLayout);
                        }
                        catch (Exception e)
                        {
                            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                        }
                    });
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad error callback
            /// </summary>
            /// <param name="ad"></param>
            /// <param name="adError"></param>
            public void OnError(IAd ad, AdError adError)
            {
                try
                {
                    var error = adError.ErrorMessage;
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            /// <summary>
            /// Ad impression logged callback
            /// </summary>
            /// <param name="ad"></param>
            public void OnLoggingImpression(IAd ad)
            {

            }

            /// <summary>
            /// on Media Downloaded
            /// </summary>
            /// <param name="p0"></param>
            public void OnMediaDownloaded(IAd p0)
            {
                 
            }

            private LinearLayout NativeAdChoicesContainer;

            private void InflateAd(NativeAd nativeAd, View adView)
            {
                try
                {
                    // Create native UI using the ad metadata.
                    var holder = new AdHolder(adView);
                    NativeAdChoicesContainer = holder.NativeAdChoicesContainer;

                    // Setting the Text
                    holder.NativeAdSocialContext.Text = nativeAd.AdSocialContext;
                    holder.NativeAdCallToAction.Text = nativeAd.AdCallToAction;
                    holder.NativeAdCallToAction.Visibility = nativeAd.HasCallToAction ? ViewStates.Visible : ViewStates.Invisible;
                    holder.NativeAdTitle.Text = nativeAd.AdvertiserName;
                    holder.NativeAdBody.Text = nativeAd.AdBodyText;
                    holder.SponsoredLabel.Text = Activity.GetText(Resource.String.sponsored);

                    // You can use the following to specify the clickable areas.
                    List<View> clickableViews = new List<View> { holder.NativeAdIcon, holder.NativeAdMedia, holder.NativeAdCallToAction };

                    nativeAd.RegisterViewForInteraction(NativeAdLayout, holder.NativeAdMedia, holder.NativeAdIcon, clickableViews);

                    // Optional: tag views
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdIcon, NativeAdBase.NativeComponentTag.AdIcon);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdTitle, NativeAdBase.NativeComponentTag.AdTitle);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdBody, NativeAdBase.NativeComponentTag.AdBody);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdSocialContext, NativeAdBase.NativeComponentTag.AdSocialContext);
                    NativeAdBase.NativeComponentTag.TagView(holder.NativeAdCallToAction, NativeAdBase.NativeComponentTag.AdCallToAction);
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            public class MediaViewListener : Java.Lang.Object, IMediaViewListener
            {
                public void OnComplete(MediaView p0)
                {
                     
                }

                public void OnEnterFullscreen(MediaView p0)
                {
                    
                }

                public void OnExitFullscreen(MediaView p0)
                {
                    
                }

                public void OnFullscreenBackground(MediaView p0)
                {
                    
                }

                public void OnFullscreenForeground(MediaView p0)
                {
                    
                }

                public void OnPause(MediaView p0)
                {
                    
                }

                public void OnPlay(MediaView p0)
                {
                    
                }

                public void OnVolumeChange(MediaView p0, float p1)
                {
                    
                }
            }
        }

        private static void LoadAd(Activity activity,  AdHolder holder , NativeAd nativeAd, NativeAdLayout adView)
        {
            try
            {
                adView.Visibility = ViewStates.Visible;

                if (holder.NativeAdChoicesContainer != null)
                {
                    var adOptionsView = new AdOptionsView(activity, nativeAd, adView);
                    holder.NativeAdChoicesContainer.RemoveAllViews();
                    holder.NativeAdChoicesContainer.AddView(adOptionsView, 0);
                }

                // Setting the Text
                holder.NativeAdSocialContext.Text = nativeAd.AdSocialContext;
                holder.NativeAdCallToAction.Text = nativeAd.AdCallToAction;
                holder.NativeAdCallToAction.Visibility = nativeAd.HasCallToAction ? ViewStates.Visible : ViewStates.Invisible;
                holder.NativeAdTitle.Text = nativeAd.AdvertiserName;
                holder.NativeAdBody.Text = nativeAd.AdBodyText;
                holder.SponsoredLabel.Text = activity.GetText(Resource.String.sponsored);

                // You can use the following to specify the clickable areas.
                List<View> clickableViews = new List<View> { holder.NativeAdIcon, holder.NativeAdMedia, holder.NativeAdCallToAction };

                nativeAd.RegisterViewForInteraction(adView, holder.NativeAdMedia, holder.NativeAdIcon, clickableViews);

                // Optional: tag views
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdIcon, NativeAdBase.NativeComponentTag.AdIcon);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdTitle, NativeAdBase.NativeComponentTag.AdTitle);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdBody, NativeAdBase.NativeComponentTag.AdBody);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdSocialContext, NativeAdBase.NativeComponentTag.AdSocialContext);
                NativeAdBase.NativeComponentTag.TagView(holder.NativeAdCallToAction, NativeAdBase.NativeComponentTag.AdCallToAction);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private class AdHolder : RecyclerView.ViewHolder
        {
            public View NativeAdLayout { get; private set; }
            public MediaView NativeAdMedia { get; private set; }
            public MediaView NativeAdIcon { get; private set; }
            public TextView NativeAdTitle { get; private set; }
            public TextView NativeAdBody { get; private set; }
            public TextView NativeAdSocialContext { get; private set; }
            public TextView SponsoredLabel { get; private set; }
            public Button NativeAdCallToAction { get; private set; }
            public LinearLayout NativeAdChoicesContainer { get; private set; }

            public AdHolder(View adLayout) : base(adLayout)
            {
                try
                {
                    NativeAdLayout = adLayout;

                    NativeAdMedia = adLayout.FindViewById<MediaView>(Resource.Id.native_ad_media);
                    NativeAdTitle = adLayout.FindViewById<TextView>(Resource.Id.native_ad_title);
                    NativeAdBody = adLayout.FindViewById<TextView>(Resource.Id.native_ad_body);
                    NativeAdSocialContext = adLayout.FindViewById<TextView>(Resource.Id.native_ad_social_context);
                    SponsoredLabel = adLayout.FindViewById<TextView>(Resource.Id.native_ad_sponsored_label);
                    NativeAdCallToAction = adLayout.FindViewById<Button>(Resource.Id.native_ad_call_to_action);
                    NativeAdIcon = adLayout.FindViewById<MediaView>(Resource.Id.native_ad_icon);
                    NativeAdChoicesContainer = adLayout.FindViewById<LinearLayout>(Resource.Id.ad_choices_container);

                    NativeAdMedia.SetListener(new NativeAdListener.MediaViewListener());
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }
        }

        #endregion
    }

    //public class MyNativeAdsManagerListener : Java.Lang.Object, NativeAdsManager.IListener
    //{
    //    private readonly NativePostAdapter NativePostAdapter;
    //    public MyNativeAdsManagerListener(NativePostAdapter nativePostAdapter)
    //    {
    //        try
    //        {
    //            NativePostAdapter = nativePostAdapter;
    //        }
    //        catch (Exception e)
    //        {
    //            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
    //        }
    //    }

    //    public void OnAdError(AdError p0)
    //    {

    //    }

    //    public void OnAdsLoaded()
    //    {
    //        try
    //        {
    //            NativeAd ad = NativePostAdapter?.mNativeAdsManager?.NextNativeAd();
    //            if (ad != null)
    //            {
    //                NativePostAdapter.MAdItems.Add(ad);
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
    //        } 
    //    }
    //}


    public static class InitializeFacebook 
    {
        public static void Initialize(Context context)
        {
            try
            {
                if (AudienceNetworkAds.IsInAdsProcess(context))
                {
                    return;
                } // else execute default application initialization code


                MapsInitializer.Initialize(context);

                AudienceNetworkAds.Initialize(context); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
    }

}