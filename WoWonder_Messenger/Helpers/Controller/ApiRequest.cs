using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Widget;
using Bumptech.Glide;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using WoWonder.Activities.Authentication;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Activities.Tab.Services;
using WoWonder.Frameworks.Floating;
using WoWonder.Frameworks.onesignal;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.User;
using WoWonderClient.Requests;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Exception = System.Exception;
using Profile = Xamarin.Facebook.Profile;

namespace WoWonder.Helpers.Controller
{
    public static class ApiRequest
    {
        //############# DONT'T MODIFY HERE #############

        //Main API URLS
        //*********************************************************
        private static readonly string ApiSendAgoraCallAction = Client.WebsiteUrl + "/app_api.php?application=phone&type=call_agora_actions";
        private static readonly string ApiCreateAgoraCallEvent = Client.WebsiteUrl + "/app_api.php?application=phone&type=create_agora_call";
        private static readonly string ApiCheckForAgoraAnswer = Client.WebsiteUrl + "/app_api.php?application=phone&type=check_agora_for_answer";

        private static readonly string ApiGetSearchGif = "https://api.giphy.com/v1/gifs/search?api_key=b9427ca5441b4f599efa901f195c9f58&limit=45&rating=g&q=";
        private static readonly string ApiGeTrendingGif = "https://api.giphy.com/v1/gifs/trending?api_key=b9427ca5441b4f599efa901f195c9f58&limit=45&rating=g";
        private static readonly string ApiGetTimeZone = "http://ip-api.com/json/";
        //########################## Client ##########################
         
        public static async Task<string> GetTimeZoneAsync()
        {
            try
            {
                if (AppSettings.AutoCodeTimeZone)
                { 
                    var response = await RestHttp.Client.GetAsync(ApiGetTimeZone);
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TimeZoneObject>(json);
                    return data?.Timezone;
                }
                else
                {
                    return AppSettings.CodeTimeZone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return AppSettings.CodeTimeZone;
            } 
        }

        #region Call


        /// ###############################
        /// Agora framework Api 

        public static async Task<string> Send_Agora_Call_Action_Async(string answerType, string callId)
        {
            try
            {
              
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user_id", UserDetails.UserId),
                    new KeyValuePair<string, string>("answer_type", answerType),
                    new KeyValuePair<string, string>("call_id", callId),
                    new KeyValuePair<string, string>("s", UserDetails.AccessToken)
                });

                var response = await RestHttp.Client.PostAsync(ApiSendAgoraCallAction, formContent);
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                string apiStatus = data["status"].ToString();
                return apiStatus;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        public static async Task<string> Check_Agora_Call_Answer_Async(string callId, string callType)
        {
            try
            {
                
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user_id", UserDetails.UserId),
                    new KeyValuePair<string, string>("call_id", callId),
                    new KeyValuePair<string, string>("call_type", callType),
                    new KeyValuePair<string, string>("s", UserDetails.AccessToken)
                });

                var response = await RestHttp.Client.PostAsync(ApiCheckForAgoraAnswer, formContent);
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                string apiStatus = data["api_status"]?.ToString();
                if (apiStatus == "200")
                {
                    string callstatus = data["call_status"]?.ToString();
                    return callstatus;
                }

                return "400";
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        public static async Task<Classes.CallUser> Create_Agora_Call_Event_Async(string recipientId, string callType)
        {
            try
            {
                
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user_id", UserDetails.UserId),
                    new KeyValuePair<string, string>("recipient_id", recipientId),
                    new KeyValuePair<string, string>("call_type", callType),
                    new KeyValuePair<string, string>("s", UserDetails.AccessToken)
                });

                var response = await RestHttp.Client.PostAsync(ApiCreateAgoraCallEvent, formContent);
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                string apiStatus = data["status"].ToString();
                if (apiStatus == "200")
                {
                    Classes.CallUser vidodata = new Classes.CallUser();

                    if (data.ContainsKey("id"))
                        vidodata.Id = data["id"]?.ToString();
                    if (data.ContainsKey("room_name"))
                        vidodata.RoomName = data["room_name"]?.ToString();

                    vidodata.Type = callType;

                    return vidodata;
                }

                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        /// ###############################

        #endregion

        public static async Task<ObservableCollection<GifGiphyClass.Datum>> SearchGif(string searchKey, string offset)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return new ObservableCollection<GifGiphyClass.Datum>();
                }
                else
                { 
                    var response = await RestHttp.Client.GetAsync(ApiGetSearchGif + searchKey + "&offset=" + offset);
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<GifGiphyClass>(json);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (data.meta.Status == 200)
                        {
                            return new ObservableCollection<GifGiphyClass.Datum>(data.Data);
                        }
                        else
                        {
                            return new ObservableCollection<GifGiphyClass.Datum>();
                        }
                    }
                    else
                    {
                        return new ObservableCollection<GifGiphyClass.Datum>();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return new ObservableCollection<GifGiphyClass.Datum>();
            }
        }

        public static async Task<ObservableCollection<GifGiphyClass.Datum>> TrendingGif(string offset)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    return new ObservableCollection<GifGiphyClass.Datum>();
                }
                else
                { 
                    var response = await RestHttp.Client.GetAsync(ApiGeTrendingGif + "&offset=" + offset);
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<GifGiphyClass>(json);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (data.meta.Status == 200)
                        {
                            return new ObservableCollection<GifGiphyClass.Datum>(data.Data);
                        }
                        else
                        {
                            return new ObservableCollection<GifGiphyClass.Datum>();
                        }
                    }
                    else
                    {
                        return new ObservableCollection<GifGiphyClass.Datum>();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return new ObservableCollection<GifGiphyClass.Datum>();
            }
        }

        public static async Task<GetSiteSettingsObject.ConfigObject> GetSettings_Api(Activity context)
        {
            if (Methods.CheckConnectivity())
            {
                await SetLangUserAsync(context).ConfigureAwait(false);

                (var apiStatus, dynamic respond) = await Current.GetSettings();

                if (apiStatus != 200 || !(respond is GetSiteSettingsObject result) || result.Config == null)
                    return Methods.DisplayReportResult(context, respond);

                ListUtils.SettingsSiteList = result.Config;

                AppSettings.OneSignalAppId = result.Config.AndroidMPushId;
                OneSignalNotification.RegisterNotificationDevice();

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateSettings(result.Config);
                dbDatabase.Dispose();
                  
                //Products Categories
                var listProducts = result.Config.ProductsCategories?.Select(cat => new Classes.Categories
                {
                    CategoriesId = cat.Key,
                    CategoriesName = Methods.FunString.DecodeString(cat.Value),
                    CategoriesColor = "#ffffff",
                    SubList = new List<SubCategories>()
                }).ToList();

                ListUtils.ListCategoriesProducts.Clear();
                if (listProducts?.Count > 0)
                    ListUtils.ListCategoriesProducts = new ObservableCollection<Classes.Categories>(listProducts);

                if (result.Config.ProductsSubCategories?.SubCategoriesList?.Count > 0)
                {
                    //Sub Categories Products
                    foreach (var sub in result.Config.ProductsSubCategories?.SubCategoriesList)
                    {
                        var subCategories = result.Config.ProductsSubCategories?.SubCategoriesList?.FirstOrDefault(a => a.Key == sub.Key).Value;
                        if (subCategories?.Count > 0)
                        {
                            var cat = ListUtils.ListCategoriesProducts.FirstOrDefault(a => a.CategoriesId == sub.Key);
                            if (cat != null)
                            {
                                foreach (var pairs in subCategories)
                                {
                                    cat.SubList.Add(pairs);
                                }
                            }
                        }
                    }
                }
                   
                try
                {
                    if (AppSettings.SetApisReportMode)
                    { 
                        if (ListUtils.ListCategoriesProducts.Count == 0)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(context, "ReportMode", "List Categories Products Not Found, Please check api get_site_settings ", "Close");
                        } 
                    }
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }

                return result.Config;

            }
            else
            {
                Toast.MakeText(context, context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                return null;
            }
        }
         
        private static async Task SetLangUserAsync(Activity context)
        {
            try
            {
                if (string.IsNullOrEmpty(Current.AccessToken) || !AppSettings.SetLangUser)
                    return;

                string lang;
                if (UserDetails.LangName.Contains("en"))
                    lang = "english";
                else if (UserDetails.LangName.Contains("ar"))
                    lang = "arabic";
                else if (UserDetails.LangName.Contains("de"))
                    lang = "german";
                else if (UserDetails.LangName.Contains("el"))
                    lang = "greek";
                else if (UserDetails.LangName.Contains("es"))
                    lang = "spanish";
                else if (UserDetails.LangName.Contains("fr"))
                    lang = "french";
                else if (UserDetails.LangName.Contains("it"))
                    lang = "italian";
                else if (UserDetails.LangName.Contains("ja"))
                    lang = "japanese";
                else if (UserDetails.LangName.Contains("nl"))
                    lang = "dutch";
                else if (UserDetails.LangName.Contains("pt"))
                    lang = "portuguese";
                else if (UserDetails.LangName.Contains("ro"))
                    lang = "romanian";
                else if (UserDetails.LangName.Contains("ru"))
                    lang = "russian";
                else if (UserDetails.LangName.Contains("sq"))
                    lang = "albanian";
                else if (UserDetails.LangName.Contains("sr"))
                    lang = "serbian";
                else if (UserDetails.LangName.Contains("tr"))
                    lang = "turkish";
                else
                    //lang = string.IsNullOrEmpty(UserDetails.LangName) ? AppSettings.Lang : "";
                    return;

                var dataPrivacy = new Dictionary<string, string>
                {
                    {"language", lang}
                };

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dataPrivacy) });
                else
                    Toast.MakeText(context, context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();

                await Task.Delay(0);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
          
        public static async Task Get_MyProfileData_Api(Activity context)
        {
            if (Methods.CheckConnectivity())
            {
                (int apiStatus, var respond) = await RequestsAsync.Global.Get_User_Data(UserDetails.UserId , "user_data,followers,following");
                if (apiStatus == 200)
                {
                    if (respond is GetUserDataObject result)
                    {
                        UserDetails.Avatar = result.UserData.Avatar;
                        UserDetails.Cover = result.UserData.Cover;
                        UserDetails.Username = result.UserData.Username;
                        UserDetails.FullName = result.UserData.Name;
                        UserDetails.Email = result.UserData.Email;

                        ListUtils.MyProfileList = new ObservableCollection<UserDataObject> {result.UserData};

                        context.RunOnUiThread(() =>
                        {
                            try
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                                // user_data
                                if (result.UserData != null)
                                {
                                    //Insert Or Update All data user_data
                                    dbDatabase.Insert_Or_Update_To_MyProfileTable(result.UserData); 
                                }

                                if (result.Following.Count > 0)
                                {
                                    //Insert Or Update All data Groups
                                    dbDatabase.Insert_Or_Replace_MyContactTable(new ObservableCollection<UserDataObject>(result.Following));
                                }

                                if (result.Followers.Count > 0)
                                {
                                    //Insert Or Update All data Groups
                                    dbDatabase.Insert_Or_Replace_MyFollowersTable(new ObservableCollection<UserDataObject>(result.Followers));
                                }

                                dbDatabase.Dispose();
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                            }
                        });
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////
        private static bool RunLogout;

        public static async void Delete(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Delete");

                    context?.RunOnUiThread(() =>
                    {
                        Methods.Path.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();

                        Runtime.GetRuntime().RunFinalization();
                        Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();
                         
                        context.StopService(new Intent(context, typeof(ScheduledApiService))); 
                        context.StopService(new Intent(context, typeof(ChatHeadService)));
                          
                        MainSettings.SharedData.Edit().Clear().Commit();
                        MainSettings.LastPosition.Edit().Clear().Commit();

                        Intent intent = new Intent(context, typeof(LoginActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                        context.Finish();
                    });

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public static async void Logout(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Logout");

                    context?.RunOnUiThread(() =>
                    {
                        Methods.Path.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();

                        Runtime.GetRuntime().RunFinalization();
                        Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        context.StopService(new Intent(context, typeof(ScheduledApiService)));
                        context.StopService(new Intent(context, typeof(ChatHeadService)));

                        MainSettings.SharedData.Edit().Clear().Commit();
                        MainSettings.LastPosition.Edit().Clear().Commit();

                        Intent intent = new Intent(context, typeof(LoginActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                        context.Finish();
                    });

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private static void TrimCache(Activity context)
        {
            try
            {
                File dir = context.CacheDir;
                if (dir != null && dir.IsDirectory)
                {
                    DeleteDir(dir);
                }

                context.DeleteDatabase("WoWonderMessenger.db");
                context.DeleteDatabase(SqLiteDatabase.PathCombine);

                Glide.Get(context).ClearDiskCache();
                Glide.Get(context).ClearMemory();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private static bool DeleteDir(File dir)
        {
            try
            {
                if (dir == null || !dir.IsDirectory) return dir != null && dir.Delete();
                string[] children = dir.List();
                foreach (string child in children)
                {
                    bool success = DeleteDir(new File(dir, child));
                    if (!success)
                    {
                        return false;
                    }
                }

                // The directory is now empty so delete it
                return dir.Delete();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }

        private static async Task RemoveData(string type)
        {
            try
            {
                if (type == "Logout")
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { RequestsAsync.Global.Get_Delete_Token });
                    }
                }
                else if (type == "Delete")
                {
                    if (Methods.CheckConnectivity())
                    {
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Delete_User(UserDetails.Password) });
                    }
                }

                try
                {
                    if (AppSettings.ShowGoogleLogin && LoginActivity.MGoogleApiClient != null)
                        if (Auth.GoogleSignInApi != null)
                        {
                            Auth.GoogleSignInApi.SignOut(LoginActivity.MGoogleApiClient);
                            LoginActivity.MGoogleApiClient.Disconnect();
                            LoginActivity.MGoogleApiClient = null;
                        }

                    if (AppSettings.ShowFacebookLogin)
                    {
                        var accessToken = AccessToken.CurrentAccessToken;
                        var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                        if (isLoggedIn && Profile.CurrentProfile != null)
                        {
                            LoginManager.Instance.LogOut();
                        }
                    }

                    OneSignalNotification.Un_RegisterNotificationDevice();

                    ListUtils.ClearAllList();

                    UserDetails.ClearAllValueUserDetails();
 
                    Methods.DeleteNoteOnSD();

                    TabbedMainActivity.Receiver = null;

                    GC.Collect();
                }
                catch (Exception exception)
                {
                    Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                }
                await Task.Delay(0);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }
    }
}