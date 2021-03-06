using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using JP.ShTs.StoriesProgressView;
using Newtonsoft.Json;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Story;
using WoWonderClient.Requests;
using Exception = System.Exception;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Story
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ViewStoryActivity : AppCompatActivity, StoriesProgressView.IStoriesListener, View.IOnTouchListener, MediaPlayer.IOnCompletionListener, MediaPlayer.IOnPreparedListener
    {
        #region Variables Basic

        private ImageView StoryImageView, UserImageView;
        private VideoView StoryVideoView;
        private string UserId = "", StoryId = "";
        private StoriesProgressView StoriesProgress;
        private GetUserStoriesObject.StoryObject DataStories;
        private View ReverseView, SkipView;
        private TextView CaptionStoryTextView, UsernameTextView, LastSeenTextView, DeleteIconView;
        private int Counter;
        private long PressTime;
        private readonly long Limit = 500L;
        private Toolbar Toolbar;
        private TabbedMainActivity GlobalContext;
        private LinearLayout StoryaboutLayout;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.LayoutStable;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                // Create your application here
                SetContentView(Resource.Layout.View_Story_Layout);

                UserId = Intent.GetStringExtra("UserId");

                GlobalContext = TabbedMainActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                LoadData();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
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

        protected override void OnDestroy()
        {
            try
            {
                // Very important !
                StoriesProgress.Destroy();

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                StoryaboutLayout = FindViewById<LinearLayout>(Resource.Id.storyaboutLayout);
                StoryImageView = FindViewById<ImageView>(Resource.Id.imagstoryDisplay);
                StoriesProgress = FindViewById<StoriesProgressView>(Resource.Id.stories);
                CaptionStoryTextView = FindViewById<TextView>(Resource.Id.storyaboutText);
                UserImageView = FindViewById<ImageView>(Resource.Id.imageAvatar);
                UsernameTextView = FindViewById<TextView>(Resource.Id.username);
                LastSeenTextView = FindViewById<TextView>(Resource.Id.time);
                DeleteIconView = FindViewById<TextView>(Resource.Id.DeleteIcon);
                ReverseView = FindViewById<View>(Resource.Id.reverse);
                SkipView = FindViewById<View>(Resource.Id.skip);

                StoriesProgress.Visibility = ViewStates.Visible;

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, DeleteIconView, FontAwesomeIcon.TrashAlt);

                ReverseView.SetOnTouchListener(this);
                SkipView.SetOnTouchListener(this);

                InitVideoView();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void InitVideoView()
        {
            try
            {
                StoryVideoView = FindViewById<VideoView>(Resource.Id.VideoView);

                StoryVideoView.SetOnPreparedListener(this);
                StoryVideoView.SetOnCompletionListener(this);
                StoryVideoView.SetAudioAttributes(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Movie).Build());
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
        private void InitToolbar()
        {
            try
            {
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    Toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(Toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    DeleteIconView.Click += DeleteIconViewOnClick;
                    ReverseView.Click += ReverseViewOnClick;
                    SkipView.Click += SkipViewOnClick;
                }
                else
                {
                    DeleteIconView.Click -= DeleteIconViewOnClick;
                    ReverseView.Click -= ReverseViewOnClick;
                    SkipView.Click -= SkipViewOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Events

        //delete story
        private async void DeleteIconViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StoriesProgress.Pause();

                if (StoryVideoView.IsPlaying)
                    StoryVideoView.Pause();

                if (Methods.CheckConnectivity())
                {
                    (int respondCode, var respond) = await RequestsAsync.Story.Delete_Story(StoryId).ConfigureAwait(false);
                    if (respondCode == 200)
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                var story = GlobalContext.LastStoriesTab.MAdapter?.StoryList?.FirstOrDefault(a => a.UserId == UserId);
                                if (story == null) return;
                                var item = story.Stories.FirstOrDefault(q => q.Id == StoryId);
                                if (item != null)
                                {
                                    story.Stories.Remove(item);

                                    GlobalContext.LastStoriesTab.MAdapter.NotifyItemChanged(GlobalContext.LastStoriesTab.MAdapter.StoryList.IndexOf(story));

                                    if (story.Stories.Count == 0)
                                    {
                                        GlobalContext.LastStoriesTab.MAdapter?.StoryList.Remove(story);
                                        GlobalContext.LastStoriesTab.MAdapter.NotifyDataSetChanged();
                                        GlobalContext.LastStoriesTab.ShowEmptyPage();
                                    }
                                }
                                Finish();
                            }
                            catch (Exception exception)
                            {
                                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                            }
                        });

                        Toast.MakeText(this, GetString(Resource.String.Lbl_Deleted), ToastLength.Short).Show();
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        }

        private void SkipViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StoriesProgress.Skip();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private void ReverseViewOnClick(object sender, EventArgs e)
        {
            try
            {
                StoriesProgress.Reverse();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }


        #endregion
         
        private async void LoadData()
        {
            try
            {
                DataStories = JsonConvert.DeserializeObject<GetUserStoriesObject.StoryObject>(Intent.GetStringExtra("DataItem"));
                if (DataStories != null)
                {
                    GlideImageLoader.LoadImage(this, DataStories.Avatar, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                    DeleteIconView.Visibility = DataStories.Stories[0].IsOwner ? ViewStates.Visible : ViewStates.Invisible;

                    int count = DataStories.Stories.Count;
                    StoriesProgress.Visibility = ViewStates.Visible;
                    StoriesProgress.SetStoriesCount(count); // <- set stories
                    StoriesProgress.SetStoriesListener(this); // <- set listener 
                    //StoriesProgress.SetStoryDuration(10000L); // <- set a story duration   

                    var fistStory = DataStories.Stories.FirstOrDefault();
                    if (fistStory != null)
                    {
                        StoriesProgress.SetStoriesCountWithDurations(DataStories.DurationsList.ToArray());

                        await SetStory(fistStory);

                        StoriesProgress.StartStories(); // <- start progress 
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private string MediaFile;

        private async Task SetStory(GetUserStoriesObject.StoryObject.Story story)
        {
            try
            {
                StoryId = story.Id;

                if (story.IsOwner)
                {
                    UsernameTextView.Text = WoWonderTools.GetNameFinal(DataStories) + "  " + Methods.Time.ReplaceTime(story.TimeText);
                    LastSeenTextView.Text = GetText(Resource.String.Lbl_SeenBy) + " " + story.ViewCount;
                }
                else
                {
                    UsernameTextView.Text = WoWonderTools.GetNameFinal(DataStories);
                    LastSeenTextView.Text = story.TimeText;
                }
                 
                //image and video
                MediaFile = !story.Thumbnail.Contains("avatar") && story.Videos.Count == 0
                    ? story.Thumbnail
                    : story.Videos[0].Filename;

                if (StoryVideoView == null)
                    InitVideoView();

                string caption = "";
                if (!string.IsNullOrEmpty(story.Description))
                    caption = story.Description;
                else if (!string.IsNullOrEmpty(story.Title))
                    caption = story.Title;

                if (string.IsNullOrEmpty(caption) || string.IsNullOrWhiteSpace(caption))
                {
                    StoryaboutLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    StoryaboutLayout.Visibility = ViewStates.Visible;
                    CaptionStoryTextView.Text = Methods.FunString.DecodeString(caption);
                }

                if (StoryVideoView == null)
                    InitVideoView();

                var type = Methods.AttachmentFiles.Check_FileExtension(MediaFile);
                if (type == "Video")
                {
                    //Show a progress
                    //RunOnUiThread(() => { try { AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading)); }catch (Exception e) { Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace); } });

                    var fileName = MediaFile.Split('/').Last();
                    MediaFile = WoWonderTools.GetFile(DateTime.Now.Day.ToString(), Methods.Path.FolderDiskStory, fileName, MediaFile);

                    StoryImageView.Visibility = ViewStates.Gone;
                    StoryVideoView.Visibility = ViewStates.Visible;
                    if (MediaFile.Contains("http"))
                    {
                        StoryVideoView.SetVideoURI(Uri.Parse(MediaFile));
                        StoryVideoView.Start();
                    }
                    else
                    {
                        var file = Uri.FromFile(new File(MediaFile));
                        StoryVideoView.SetVideoPath(file.Path);
                        StoryVideoView.Start();
                    }

                    await Task.Delay(500);
                }
                else
                {
                    StoryImageView.Visibility = ViewStates.Visible;
                    StoryVideoView.Visibility = ViewStates.Gone;

                    if (MediaFile.Contains("http"))
                        Glide.With(this).Load(MediaFile).Apply(new RequestOptions()).Into(StoryImageView);
                    else  
                        Glide.With(this).Load(new File(MediaFile)).Apply(new RequestOptions()).Into(StoryImageView);

                    // GlideImageLoader.LoadImage(this,story.MediaFile, StoryImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
        public void OnPrepared(MediaPlayer mp)
        {
            try
            {
                //RunOnUiThread(() => { AndHUD.Shared.Dismiss(this); });

                StoryVideoView.Start();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            try
            {
                mp.Release();
                StoryVideoView?.StopPlayback();
                StoryVideoView = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public async void OnNext()
        {
            try
            {
                StoryVideoView?.StopPlayback();
                StoryVideoView = null;

                if (Counter + 1 > DataStories.Stories.Count)
                {
                    OnComplete();
                    return;
                }

                var dataStory = DataStories.Stories[++Counter];
                if (dataStory != null)
                {
                    await SetStory(dataStory);
                }
                else
                {
                    OnComplete();
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public async void OnPrev()
        {
            try
            {
                if (Counter - 1 < 0) return;
                var dataStory = DataStories.Stories[--Counter];
                if (dataStory != null)
                {
                    await SetStory(dataStory);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public void OnComplete()
        {
            try
            {
                AdsGoogle.Ad_Interstitial(this);
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    PressTime = CurrentTimeMillis();
                    StoriesProgress.Pause();

                    return false;

                case MotionEventActions.Up:
                    long now = CurrentTimeMillis();
                    StoriesProgress.Resume();

                    return Limit < now - PressTime;
            }

            return false;
        }

    }
}