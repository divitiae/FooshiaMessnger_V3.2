﻿using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Load.Resource.Bitmap;
using Bumptech.Glide.Request;
using Java.IO;
using WoWonder.Helpers.Utils;

namespace WoWonder.Helpers.CacheLoaders
{
    public enum ImageStyle
    {
        CenterCrop, CircleCrop, RoundedCrop, FitCenter
    }

    public enum ImagePlaceholders
    {
        Color, Drawable
    }

    public static class GlideImageLoader
    {
        public static void LoadImage(Activity activity, string imageUri, ImageView image, ImageStyle style, ImagePlaceholders imagePlaceholders, RequestOptions options = null)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUri) || string.IsNullOrWhiteSpace(imageUri) || image == null)
                    return;

                imageUri = imageUri.Replace(" ", "");

                var newImage = Glide.With(activity);

                if (options == null)
                    options = GetOptions(style, imagePlaceholders);

                if (imageUri.Contains("no_profile_image") || imageUri.Contains("blackdefault") || imageUri.Contains("no_profile_image_circle")
                    || imageUri.Contains("ImagePlacholder") || imageUri.Contains("ImagePlacholder_circle") || imageUri.Contains("Grey_Offline") || imageUri.Contains("Green_Online")
                    || imageUri.Contains("d-cover") || imageUri.Contains("d-avatar") || imageUri.Contains("addImage")|| imageUri.Contains("Audio_File") || imageUri.Contains("Image_File"))
                {
                    if (imageUri.Contains("no_profile_image_circle") || imageUri.Contains("d-avatar"))
                        newImage.Load(Resource.Drawable.no_profile_image_circle).Apply(options).Into(image);
                    else if (imageUri.Contains("no_profile_image") || imageUri.Contains("d-avatar"))
                        newImage.Load(Resource.Drawable.no_profile_image).Apply(options).Into(image);
                    else if (imageUri.Contains("ImagePlacholder"))
                        newImage.Load(Resource.Drawable.ImagePlacholder).Apply(options).Into(image);
                    else if (imageUri.Contains("ImagePlacholder_circle"))
                        newImage.Load(Resource.Drawable.ImagePlacholder_circle).Apply(options).Into(image);
                    else if (imageUri.Contains("blackdefault"))
                        newImage.Load(Resource.Drawable.blackdefault).Apply(options).Into(image);
                    else if (imageUri.Contains("Grey_Offline"))
                        newImage.Load(Resource.Drawable.Grey_Offline).Apply(options).Into(image);
                    else if (imageUri.Contains("Green_Online"))
                        newImage.Load(Resource.Drawable.Green_Online).Apply(options).Into(image);
                    else if (imageUri.Contains("d-cover"))
                        newImage.Load(Resource.Drawable.Cover_image).Apply(options).Into(image);
                    else if (imageUri.Contains("addImage"))
                        newImage.Load(Resource.Drawable.addImage).Apply(options).Into(image);
                    else if (imageUri.Contains("Audio_File"))
                        newImage.Load(Resource.Drawable.Audio_File).Apply(options).Into(image);
                    else if (imageUri.Contains("Image_File"))
                        newImage.Load(Resource.Drawable.Image_File).Apply(options).Into(image);
                }
                else if (!string.IsNullOrEmpty(imageUri) && imageUri.Contains("http"))
                {
                    newImage.Load(imageUri).Apply(options).Into(image);
                }
                else if (!string.IsNullOrEmpty(imageUri) && (imageUri.Contains("file://") || imageUri.Contains("content://") || imageUri.Contains("/data/user/0/")))
                {
                    File file2 = new File(imageUri);
                    var photoUri = FileProvider.GetUriForFile(activity, activity.PackageName + ".fileprovider", file2);
                    //RequestOptions option = style == ImageStyle.CircleCrop ? new RequestOptions().CircleCrop() : new RequestOptions();
                    newImage.Load(photoUri).Apply(options).Into(image);
                }
                else if (imageUri.Contains("storage"))
                {
                    newImage.Load(imageUri).Apply(options).Into(image);
                }
                else
                {
                    newImage.Load(Resource.Drawable.no_profile_image).Apply(options).Into(image);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private static RequestOptions GetOptions(ImageStyle style, ImagePlaceholders imagePlaceholders)
        {
            try
            {
                RequestOptions options = new RequestOptions();

                switch (style)
                {
                    case ImageStyle.CenterCrop:
                        options = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                            .CenterCrop()
                            .SetPriority(Priority.High).Override(400)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder)
                            .Placeholder(Resource.Drawable.ImagePlacholder));
                        break;
                    case ImageStyle.FitCenter:
                        options = new RequestOptions().Apply(RequestOptions.CenterCropTransform()
                            .CenterCrop().FitCenter()
                            .SetPriority(Priority.High).Override(400)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder)
                            .Placeholder(Resource.Drawable.ImagePlacholder));
                        break;
                    case ImageStyle.CircleCrop:
                        options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                            .CenterCrop().CircleCrop()
                            .SetPriority(Priority.High).Override(300)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder_circle)
                            .Placeholder(Resource.Drawable.ImagePlacholder_circle));
                        break;
                    case ImageStyle.RoundedCrop:
                        options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                            .CenterCrop()
                            .Transform(new MultiTransformation(new CenterCrop(), new RoundedCorners(25)))
                            .SetPriority(Priority.High).Override(450)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder_circle)
                            .Placeholder(Resource.Drawable.ImagePlacholder_circle));
                        break;

                    default:
                        options.CenterCrop();
                        break;
                }

                switch (imagePlaceholders)
                {
                    case ImagePlaceholders.Color:
                        var color = Methods.FunString.RandomColor();
                        options.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                        break;
                    case ImagePlaceholders.Drawable:
                        switch (style)
                        {
                            case ImageStyle.CircleCrop:
                                options.Placeholder(Resource.Drawable.ImagePlacholder_circle).Fallback(Resource.Drawable.ImagePlacholder_circle);
                                break;
                            default:
                                options.Placeholder(Resource.Drawable.ImagePlacholder).Fallback(Resource.Drawable.ImagePlacholder);
                                break;
                        }
                        break;
                }

                return options;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return new RequestOptions().CenterCrop();
            }
        }

        public static RequestBuilder GetPreLoadRequestBuilder(Activity activityContext, string url, ImageStyle style)
        {
            try
            {
                if (url == null || string.IsNullOrEmpty(url))
                    return null;

                var options = GetOptions(style, ImagePlaceholders.Drawable);

                if (url.Contains("avatar"))
                    options.CircleCrop();

                options.SetDiskCacheStrategy(DiskCacheStrategy.All);

                return Glide.With(activityContext)
                    .Load(url)
                    .Apply(options);

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }

        }

        public static RequestOptions GetRequestOptions(ImageStyle style, ImagePlaceholders imagePlaceholders)
        {
            try
            {
                var options = new RequestOptions();


                switch (style)
                {
                    case ImageStyle.CenterCrop:
                        options.CenterCrop();
                        break;
                    case ImageStyle.FitCenter:
                        options.FitCenter();
                        break;
                    case ImageStyle.CircleCrop:
                        options.CircleCrop();
                        break;
                    case ImageStyle.RoundedCrop:
                        options.Transform(new MultiTransformation(new CenterCrop(), new RoundedCorners(25)))
                            .SetPriority(Priority.High).Override(450)
                            .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                            .Error(Resource.Drawable.ImagePlacholder_circle)
                            .Placeholder(Resource.Drawable.ImagePlacholder_circle); 
                        break;

                    default:
                        options.CenterCrop();
                        break;
                }


                switch (imagePlaceholders)
                {
                    case ImagePlaceholders.Color:
                        var color = Methods.FunString.RandomColor();
                        options.Placeholder(new ColorDrawable(Color.ParseColor(color))).Fallback(new ColorDrawable(Color.ParseColor(color)));
                        break;
                    case ImagePlaceholders.Drawable:
                        options.Placeholder(Resource.Drawable.ImagePlacholder).Fallback(Resource.Drawable.ImagePlacholder);
                        break;
                }

                return options;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return new RequestOptions();

            }

        }
         
    }
}