using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using WoWonder.Activities.ChatWindow.Adapters;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.StickersFragments
{
    public class StickerFragment4 : Android.Support.V4.App.Fragment
    {
        private View TabPage;
        private RecyclerView StickerRecyclerView;
        private GridLayoutManager MLayoutManager;
        private StickerRecylerAdapter.StickerAdapter StickerAdapter;
        private readonly string Type;
        public StickerFragment4(string type = "ChatWindowActivity")
        {
            Type = type;
        }
         
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                TabPage = inflater.Inflate(Resource.Layout.StickerFragment, container, false);
                StickerRecyclerView = (RecyclerView) TabPage.FindViewById(Resource.Id.stickerRecyler1);
                // Stickers.StickerList1

                MLayoutManager = new GridLayoutManager(Activity.ApplicationContext, AppSettings.StickersOnEachRow,LinearLayoutManager.Vertical, false);
                StickerRecyclerView.SetLayoutManager(MLayoutManager);
                StickerAdapter = new StickerRecylerAdapter.StickerAdapter(Activity,Stickers.StickerList4);

                StickerRecyclerView.SetAdapter(StickerAdapter);
                StickerItemClickListener clickListener = new StickerItemClickListener(Activity, Type, StickerAdapter);
                Console.WriteLine(clickListener);
                return TabPage;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
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