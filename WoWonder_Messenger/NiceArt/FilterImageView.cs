﻿using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using WoWonder.Helpers.Utils;
using WoWonder.NiceArt.Models;
using Exception = Java.Lang.Exception;
using Uri = Android.Net.Uri;

namespace WoWonder.NiceArt
{
    public class FilterImageView : ImageView
    {
        public INiceArt.IOnImageChangedListener MOnImageChangedListener;

        protected FilterImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public FilterImageView(Context context) : base(context)
        {
        }

        public FilterImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public FilterImageView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public Bitmap GetBitmap()
        {
            try
            {
                Bitmap bitmap = ((BitmapDrawable)Drawable)?.Bitmap;
                return bitmap;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;

            }
        }

        public override void SetImageBitmap(Bitmap bm)
        {
            try
            {
                base.SetImageBitmap(bm);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public override void SetImageIcon(Icon icon)
        {
            try
            {
                base.SetImageIcon(icon);

                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }


        public virtual void SetImageMatrix(Matrix matrix)
        {
            try
            {
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public override void SetImageState(int[] state, bool merge)
        {
            try
            {
                base.SetImageState(state, merge);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }


        public virtual void SetImageTintList(ColorStateList tint)
        {
            try
            {
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public virtual void SetImageTintMode(PorterDuff.Mode tintMode)
        {
            try
            {
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public override void SetImageDrawable(Drawable drawable)
        {
            try
            {
                base.SetImageDrawable(drawable);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public override void SetImageResource(int resId)
        {
            try
            {
                base.SetImageResource(resId);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }


        public override void SetImageURI(Uri uri)
        {
            try
            {
                base.SetImageURI(uri);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        public override void SetImageLevel(int level)
        {
            try
            {
                base.SetImageLevel(level);
                MOnImageChangedListener?.OnBitmapLoaded(GetBitmap());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }
    }
}