using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Views;
using Java.Lang;
using WoWonder.Helpers.Utils;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;
using String = Java.Lang.String;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace WoWonder.Adapters
{
    public class MainTabAdapter : FragmentPagerAdapter
    {
        #region Variables

        private List<SupportFragment> Fragments { get; set; }
        private List<string> FragmentNames { get; set; }

        #endregion

        public MainTabAdapter(SupportFragmentManager sfm) : base(sfm)
        {
            try
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void AddFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void ClaerFragment()
        {
            try
            {
                Fragments.Clear();
                FragmentNames.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void RemoveFragment(SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Remove(fragment);
                FragmentNames.Remove(name);
                NotifyDataSetChanged();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void InsertFragment(int index, SupportFragment fragment, string name)
        {
            try
            {
                Fragments.Insert(index, fragment);
                FragmentNames.Insert(index, name);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public override int Count => Fragments?.Count ?? 0;

        public override SupportFragment GetItem(int position)
        {
            try
            {
                return Fragments[position];
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(FragmentNames[position]);
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            try
            {
                return base.InstantiateItem(container, position);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                return null;
            }
        } 
    }
}