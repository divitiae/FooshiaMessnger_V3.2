using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using WoWonder.Activities.Authentication;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.PageChat;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using Exception = System.Exception;

namespace WoWonder.Activities
{
    [Activity(Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : AppCompatActivity
    {
        private SqLiteDatabase DbDatabase = new SqLiteDatabase();
       
        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                 
                if ((int)Build.VERSION.SdkInt < 23)
                { 
                    new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    { 
                        new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                        }, 101);
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void FirstRunExcite()
        {
            try
            {
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();
                 
                if (!string.IsNullOrEmpty(AppSettings.Lang))
                {
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    #pragma warning disable 618
                    UserDetails.LangName = (int)Build.VERSION.SdkInt < 25 ? Resources.Configuration.Locale.Language.ToLower() : Resources.Configuration.Locales.Get(0).Language.ToLower() ?? Resources.Configuration.Locale.Language.ToLower();
                    #pragma warning restore 618
                    LangController.SetApplicationLang(this, UserDetails.LangName);
                }

                var result = DbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    var settingsData = DbDatabase.GetSettings();
                    if (settingsData != null)
                        ListUtils.SettingsSiteList = settingsData;

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        ListUtils.UserList = DbDatabase.Get_LastUsersChat_List();
                    else
                        ListUtils.UserChatList = DbDatabase.GetLastUsersChatList();

                    var userId = Intent.GetStringExtra("UserID") ?? ""; 
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var type = Intent.GetStringExtra("type") ?? ""; //SendMsgProduct , OpenChat , OpenChatApp , OpenChatPage
                        OpenChat(type);
                    }
                    else
                    {
                        switch (result.Status)
                        {
                            case "Active":
                                StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                                break;
                            default:
                                StartActivity(CrossAppAuthentication() ? new Intent(this, typeof(FirstActivity)) : new Intent(this, typeof(LoginActivity)));
                                break;
                        }
                    }
                }
                else
                {
                    var userId = Intent.GetStringExtra("UserID") ?? "";
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var type = Intent.GetStringExtra("type") ?? ""; //SendMsgProduct , OpenChat , OpenChatApp , OpenChatPage
                        OpenChat(type);
                    }
                    else
                    {
                        StartActivity(CrossAppAuthentication() ? new Intent(this, typeof(FirstActivity)) : new Intent(this, typeof(LoginActivity)));
                    } 
                }

                DbDatabase.Dispose();

                if (AppSettings.ShowAdMobBanner || AppSettings.ShowAdMobInterstitial || AppSettings.ShowAdMobRewardVideo || AppSettings.ShowAdMobNative)
                    MobileAds.Initialize(this, GetString(Resource.String.admob_app_id));

                if (AppSettings.ShowFbBannerAds || AppSettings.ShowFbInterstitialAds || AppSettings.ShowFbRewardVideoAds)
                    InitializeFacebook.Initialize(this);
            }
            catch (Exception e)
            {
               
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private bool CrossAppAuthentication()
        {
            try
            {
                var loginTb = JsonConvert.DeserializeObject<DataTables.LoginTb>(Methods.ReadNoteOnSD());
                return loginTb != null && !string.IsNullOrEmpty(loginTb.AccessToken) && !string.IsNullOrEmpty(loginTb.Username);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">SendMsgProduct , OpenChat , OpenChatApp , OpenChatPage</param>
        private void OpenChat(string type)
        {
            try
            {
                Intent intent; 

                if (type == "OpenChat")
                {
                    intent = new Intent(this, typeof(ChatWindowActivity));
                    intent.PutExtra("Notifier", "Notifier");
                    intent.PutExtra("App", "Timeline");
                    intent.PutExtra("TypeChat", "user");
                    intent.PutExtra("ShowEmpty", "no");

                    //new ChatObject() { UserId = DataObject.UserData.UserId, Name = DataObject.UserData.Name , Avatar = DataObject.UserData .Avatar}

                    var itemObject = JsonConvert.DeserializeObject<ChatObject>(Intent.GetStringExtra("itemObject") ?? "");
                    if (itemObject != null)
                    {
                        intent.PutExtra("UserID", itemObject.UserId); // to_id  
                        intent.PutExtra("Name", itemObject.Name); 
                        intent.PutExtra("Avatar", itemObject.Avatar);  
                    }
                    StartActivity(intent);
                }
                else if (type == "SendMsgProduct")
                {
                    intent = new Intent(this, typeof(ChatWindowActivity));
                    intent.PutExtra("Notifier", "Notifier");
                    intent.PutExtra("App", "Timeline");
                    intent.PutExtra("TypeChat", "SendMsgProduct");
                    intent.PutExtra("ShowEmpty", "no");

                    //new ChatObject() { UserId = ProductData.Seller.UserId , Avatar = ProductData.Seller.Avatar, Name = ProductData.Seller.Name , LastMessage = new LastMessageUnion()
                    //{LastMessageClass = new MessageData() { ProductId = ProductData.Id , Product = new ProductUnion(){ProductClass = ProductData}} }} );

                    var itemObject = JsonConvert.DeserializeObject<ChatObject>(Intent.GetStringExtra("itemObject") ?? "");
                    if (itemObject != null)
                    {
                        intent.PutExtra("UserID", itemObject.UserId); // to_id  
                        intent.PutExtra("Name", itemObject.Name);  
                        intent.PutExtra("Avatar", itemObject.Avatar);

                        if (itemObject.LastMessage.LastMessageClass?.Product != null)
                        { 
                            intent.PutExtra("ProductId", itemObject.LastMessage.LastMessageClass.ProductId);
                            intent.PutExtra("ProductClass", JsonConvert.SerializeObject(itemObject.LastMessage.LastMessageClass.Product?.ProductClass));
                        }
                    }
                    StartActivity(intent);
                }
                else if (type == "OpenChatPage")
                {
                    intent = new Intent(this, typeof(PageChatWindowActivity));
                    intent.PutExtra("Notifier", "Notifier");
                    intent.PutExtra("App", "Timeline");
                    intent.PutExtra("TypeChat", "");
                    intent.PutExtra("ShowEmpty", "no");

                    //new ChatObject(){UserId = UserId , PageId = PageId , PageName = PageData.PageName , Avatar = PageData.Avatar});
                    var itemObject = JsonConvert.DeserializeObject<ChatObject>(Intent.GetStringExtra("itemObject") ?? "");
                    if (itemObject != null)
                    {
                        intent.PutExtra("UserID", itemObject.UserId); // to_id  
                        intent.PutExtra("PageId", itemObject.PageId);  
                        intent.PutExtra("PageName", itemObject.PageName);  
                        intent.PutExtra("Avatar", itemObject.Avatar); 
                    }
                    StartActivity(intent);
                }
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
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

    }
}