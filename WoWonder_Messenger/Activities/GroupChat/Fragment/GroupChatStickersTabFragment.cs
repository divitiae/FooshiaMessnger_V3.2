using System;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using WoWonder.Activities.ChatWindow.Adapters;
using WoWonder.Activities.StickersFragments;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.GroupChat.Fragment
{
    public class GroupChatStickersTabFragment : Android.Support.V4.App.Fragment
    {
        private TabLayout Tabs;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View mainTabPage = inflater.Inflate(Resource.Layout.Chat_StickersTab_Fragment, container, false);
                Tabs = mainTabPage.FindViewById<TabLayout>(Resource.Id.tabsSticker);
                ViewPager viewPager = mainTabPage.FindViewById<ViewPager>(Resource.Id.viewpagerSticker);
                //AppBarLayout appBarLayoutview = MainTabPage.FindViewById<AppBarLayout>(Resource.Id.appbarSticker);

                SetUpViewPager(viewPager);

                return mainTabPage;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                StickersTabAdapter adapter = new StickersTabAdapter(ChildFragmentManager);
                if (AppSettings.ShowStickerStack0)
                    adapter.AddFragment(new StickerFragment1("GroupChatWindowActivity"), "0");

                if (AppSettings.ShowStickerStack1)
                    adapter.AddFragment(new StickerFragment2("GroupChatWindowActivity"), "1");

                if (AppSettings.ShowStickerStack2)
                    adapter.AddFragment(new StickerFragment3("GroupChatWindowActivity"), "2");

                if (AppSettings.ShowStickerStack3)
                    adapter.AddFragment(new StickerFragment4("GroupChatWindowActivity"), "3");

                if (AppSettings.ShowStickerStack4)
                    adapter.AddFragment(new StickerFragment5("GroupChatWindowActivity"), "4");

                if (AppSettings.ShowStickerStack5)
                    adapter.AddFragment(new StickerFragment6("GroupChatWindowActivity"), "5");

                if (AppSettings.ShowStickerStack6)
                    adapter.AddFragment(new StickerFragment7("GroupChatWindowActivity"), "6");

                viewPager.Adapter = adapter;
                Tabs.SetupWithViewPager(viewPager);
                Tabs.SetBackgroundColor(!AppSettings.SetTabDarkTheme ? Color.ParseColor(AppSettings.StickersBarColor) : Color.ParseColor(AppSettings.StickersBarColorDark));

                if (Tabs.TabCount > 0)
                {
                    for (int i = 0; i <= Tabs.TabCount; i++)
                    {
                        var stickerReplacer = Tabs.GetTabAt(i);
                        if (stickerReplacer != null)
                        {
                            if (stickerReplacer.Text == "0")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker1).SetText("");

                            if (stickerReplacer.Text == "1")
                                stickerReplacer.SetIcon(Resource.Drawable.sticker2).SetText("");

                            if (stickerReplacer.Text == "2")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker3).SetText("");

                            if (stickerReplacer.Text == "3")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker4).SetText("");

                            if (stickerReplacer.Text == "4")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker5).SetText("");

                            if (stickerReplacer.Text == "5")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker6).SetText("");

                            if (stickerReplacer.Text == "6")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker7).SetText("");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }


        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

    }
}