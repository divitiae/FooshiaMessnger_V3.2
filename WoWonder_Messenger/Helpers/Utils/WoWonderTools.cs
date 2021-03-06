﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Support.V4.Content;
using AutoMapper;
using Bumptech.Glide;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Request.Transition;
using Java.Lang;
using Newtonsoft.Json;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.DefaultUser;
using WoWonder.Helpers.Model;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using Exception = System.Exception;
using Path = System.IO.Path;

namespace WoWonder.Helpers.Utils
{
    public static class WoWonderTools
    {
        public static string GetNameFinal(UserDataObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.Name) && !string.IsNullOrWhiteSpace(dataUser.Name))
                    return Methods.FunString.DecodeString(dataUser.Name);

                if (!string.IsNullOrEmpty(dataUser.Username) && !string.IsNullOrWhiteSpace(dataUser.Username))
                    return Methods.FunString.DecodeString(dataUser.Username);

                return Methods.FunString.DecodeString(dataUser.Username);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return Methods.FunString.DecodeString(dataUser.Username);
            }
        }

        public static string GetAboutFinal(UserDataObject dataUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataUser.About) && !string.IsNullOrWhiteSpace(dataUser.About))
                    return Methods.FunString.DecodeString(dataUser.About);

                return Application.Context.Resources.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.ApplicationName;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return Application.Context.Resources.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.ApplicationName;
            }
        }
         
        public static void OpenProfile(Activity activity, string userId, UserDataObject item = null, string namePage = "")
        {
            try
            {
                if (userId != UserDetails.UserId)
                {
                    var intent = new Intent(activity, typeof(UserProfileActivity));
                    if (item != null) intent.PutExtra("UserObject", JsonConvert.SerializeObject(item));
                    intent.PutExtra("UserId", userId);
                    intent.PutExtra("NamePage", namePage);
                    activity.StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(activity, typeof(MyProfileActivity));
                    activity.StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public static (string, string) GetCurrency(string idCurrency)
        {
            try
            {
                if (AppSettings.CurrencyStatic) return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);

                string currency;
                bool success = int.TryParse(idCurrency, out var number);
                if (success)
                {
                    Console.WriteLine("Converted '{0}' to {1}.", idCurrency, number);
                    currency = ListUtils.SettingsSiteList?.CurrencyArray.CurrencyList[number] ?? "USD";
                }
                else
                {
                    Console.WriteLine("Attempted conversion of '{0}' failed.", idCurrency ?? "<null>");
                    currency = idCurrency;
                }

                if (ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList != null)
                {
                    string currencyIcon = currency?.ToUpper() switch
                    {
                        "USD" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$"
                            : "$",
                        "Jpy" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy ?? "¥"
                            : "¥",
                        "EUR" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur ?? "€"
                            : "€",
                        "TRY" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try ?? "₺"
                            : "₺",
                        "GBP" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp ?? "£"
                            : "£",
                        "RUB" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub ?? "$"
                            : "$",
                        "PLN" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln ?? "zł"
                            : "zł",
                        "ILS" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils ?? "₪"
                            : "₪",
                        "BRL" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl ?? "R$"
                            : "R$",
                        "INR" => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr ?? "₹"
                            : "₹",
                        _ => !string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd)
                            ? ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$"
                            : "$"
                    };

                    return (currency, currencyIcon);
                }

                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return (AppSettings.CurrencyCodeStatic, AppSettings.CurrencyIconStatic);
            }
        }

        public static List<string> GetCurrencySymbolList()
        {
            try
            {
                var arrayAdapter = new List<string>();

                if (ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList != null)
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Usd ?? "$");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Jpy ?? "¥");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Eur ?? "€");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Try ?? "₺");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Gbp ?? "£");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Rub ?? "$");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Pln ?? "zł");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Ils ?? "₪");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Brl ?? "R$");

                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr))
                        arrayAdapter.Add(ListUtils.SettingsSiteList?.CurrencySymbolArray.CurrencyList.Inr ?? "₹");
                }

                return arrayAdapter;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return new List<string>();
            }
        }

        public static bool GetStatusOnline(int lastSeen, string isShowOnline)
        {
            try
            {
                string time = Methods.Time.TimeAgo(lastSeen, false);
                bool status = isShowOnline?.ToLower() == "on" && time == Methods.Time.LblJustNow;
                return status;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }


        public static bool CheckAllowedFileSharingInServer(string type)
        {
            try
            {
                if (type == "File")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.FileSharing) && ListUtils.SettingsSiteList?.FileSharing == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Video")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.VideoUpload) && ListUtils.SettingsSiteList?.VideoUpload == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Audio")
                {
                    if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.AudioUpload) && ListUtils.SettingsSiteList?.AudioUpload == "1")
                    {
                        // Allowed
                        return true;
                    }
                }
                else if (type == "Image")
                {
                    // Allowed
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }

        public static bool CheckMimeTypesWithServer(string path)
        {
            try
            {
                var allowedExtenstionStatic = "jpg,png,jpeg,gif,mp4,m4v,webm,flv,mov,mpeg,mp3,wav";
                var fileName = path.Split('/').Last();
                var fileNameWithExtension = fileName.Split('.').Last();

                if (!string.IsNullOrEmpty(ListUtils.SettingsSiteList?.MimeTypes))
                {
                    var allowedExtenstion = ListUtils.SettingsSiteList?.AllowedExtenstion; //jpg,png,jpeg,gif,mkv,docx,zip,rar,pdf,doc,mp3,mp4,flv,wav,txt,mov,avi,webm,wav,mpeg
                    var mimeTypes = ListUtils.SettingsSiteList?.MimeTypes; //video/mp4,video/mov,video/mpeg,video/flv,video/avi,video/webm,audio/wav,audio/mpeg,video/quicktime,audio/mp3,image/png,image/jpeg,image/gif,application/pdf,application/msword,application/zip,application/x-rar-compressed,text/pdf,application/x-pointplus,text/css

                    var getMimeType = MimeTypeMap.GetMimeType(fileNameWithExtension);

                    if (allowedExtenstion.Contains(fileNameWithExtension) && mimeTypes.Contains(getMimeType))
                    {
                        var type = Methods.AttachmentFiles.Check_FileExtension(path);

                        var check = CheckAllowedFileSharingInServer(type);
                        if (check)  // Allowed
                            return true;
                    }
                }

                //just this Allowed : >> jpg,png,jpeg,gif,mp4,m4v,webm,flv,mov,mpeg,mp3,wav
                if (allowedExtenstionStatic.Contains(fileNameWithExtension))
                    return true;

                return false;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return false;
            }
        }

        public static async Task<MessageDataExtra> MessageFilter(string id, MessageData item, MessageModelType modelType , bool showStar = false)
        {
            try
            {
                if (item == null)
                    return null;

                item.Media ??= "";
                item.Stickers ??= "";

                item.Text ??= "";

                item.Stickers = item.Stickers.Replace(".mp4", ".gif");

                item.Text = Methods.FunString.DecodeString(item.Text);

                item.TimeText = Methods.Time.TimeAgo(int.Parse(item.Time));

                item.ModelType = modelType;

                if (item.FromId == UserDetails.UserId) // right
                    item.Position = "right";
                else if (item.ToId == UserDetails.UserId) // left
                    item.Position = "left";

                if (item.Position == "right" && string.IsNullOrEmpty(item.ChatColor) || item.ChatColor != ChatWindowActivity.MainChatColor)
                    item.ChatColor = ChatWindowActivity.MainChatColor;

                if (showStar && ChatWindowActivity.GetInstance()?.StartedMessageList?.Count > 0)
                {
                    //SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    //item.IsStarted = dbDatabase.IsStartedMessages(item.Id);
                    //dbDatabase.Dispose();
                    var cec = ChatWindowActivity.GetInstance()?.StartedMessageList?.FirstOrDefault(a => a.Id == Long.ParseLong(item.Id));
                    item.IsStarted = cec != null;
                }

                if (modelType == MessageModelType.LeftProduct || modelType == MessageModelType.RightProduct)
                {
                    string imageUrl = item.Product?.ProductClass?.Images[0]?.Image ?? "";
                    var fileName = imageUrl.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDcimImage, fileName, imageUrl);
                }
                else if (modelType == MessageModelType.LeftGif || modelType == MessageModelType.RightGif)
                {
                    //https://media1.giphy.com/media/l0ExncehJzexFpRHq/200.gif?cid=b4114d905d3e926949704872410ec12a&rid=200.gif
                    string imageUrl = "";
                    if (!string.IsNullOrEmpty(item.Stickers))
                        imageUrl = item.Stickers;
                    else if (!string.IsNullOrEmpty(item.Media))
                        imageUrl = item.Media;
                    else if (!string.IsNullOrEmpty(item.MediaFileName))
                        imageUrl = item.MediaFileName;

                    string[] fileName = imageUrl.Split(new[] { "/", "200.gif?cid=", "&rid=200" }, StringSplitOptions.RemoveEmptyEntries);
                    var lastFileName = fileName.Last();
                    var name = fileName[3] + lastFileName;

                    item.Media = GetFile(id, Methods.Path.FolderDiskGif, name, imageUrl);
                }
                else if (modelType == MessageModelType.LeftText || modelType == MessageModelType.RightText)
                {
                    //return item;
                }
                else if (modelType == MessageModelType.LeftMap || modelType == MessageModelType.RightMap)
                {
                    LatLng latLng = new LatLng(Convert.ToDouble(item.Lat), Convert.ToDouble(item.Lng));

                    var addresses = await ReverseGeocodeCurrentLocation(latLng);
                    if (addresses != null)
                    {
                        var deviceAddress = addresses.GetAddressLine(0);

                        string imageUrlMap = "https://maps.googleapis.com/maps/api/staticmap?";
                        //imageUrlMap += "center=" + item.CurrentLatitude + "," + item.CurrentLongitude;
                        imageUrlMap += "center=" + deviceAddress;
                        imageUrlMap += "&zoom=13";
                        imageUrlMap += "&scale=2";
                        imageUrlMap += "&size=150x150";
                        imageUrlMap += "&maptype=roadmap";
                        imageUrlMap += "&key=" + Application.Context.GetText(Resource.String.google_maps_key);
                        imageUrlMap += "&format=png";
                        imageUrlMap += "&visual_refresh=true";
                        imageUrlMap += "&markers=size:small|color:0xff0000|label:1|" + deviceAddress;

                        item.MessageMap = imageUrlMap;
                    } 
                }
                else if (modelType == MessageModelType.LeftImage || modelType == MessageModelType.RightImage)
                {
                    var fileName = item.Media.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDcimImage, fileName, item.Media);
                }
                else if (modelType == MessageModelType.LeftAudio || modelType == MessageModelType.RightAudio)
                {
                    var fileName = item.Media.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDcimSound, fileName, item.Media);

                    if (string.IsNullOrEmpty(item.MediaDuration) || item.MediaDuration == "00:00")
                    { 
                        var duration = GetDuration(item.Media);
                        item.MediaDuration = Methods.AudioRecorderAndPlayer.GetTimeString(duration);
                    }  
                }
                else if (modelType == MessageModelType.LeftContact || modelType == MessageModelType.RightContact)
                {
                    if (item.Text.Contains("{&quot;Key&quot;") || item.Text.Contains("{key:") || item.Text.Contains("{key:^qu") || item.Text.Contains("{^key:^qu") || item.Text.Contains("{Key:") || item.Text.Contains("&quot;"))
                    {
                        string[] stringSeparators = { "," };
                        var name = item.Text.Split(stringSeparators, StringSplitOptions.None);
                        var stringName = name[0].Replace("{key:", "").Replace("{Key:", "").Replace("Value:", "").Replace("{", "").Replace("}", "");
                        var stringNumber = name[1].Replace("{key:", "").Replace("{Key:", "").Replace("Value:", "").Replace("{", "").Replace("}", "");
                        item.ContactName = stringName;
                        item.ContactNumber = stringNumber;
                    }
                }
                else if (modelType == MessageModelType.LeftVideo || modelType == MessageModelType.RightVideo)
                {
                    var fileName = item.Media.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDcimVideo, fileName, item.Media);
                    var fileNameWithoutExtension = fileName.Split('.').First();

                    var bitmapImage = Methods.MultiMedia.Retrieve_VideoFrame_AsBitmap(Application.Context, item.Media);
                    if (bitmapImage != null)
                    {
                        item.ImageVideo = Methods.Path.FolderDcimVideo + id + "/" + fileNameWithoutExtension + ".png"; 
                        Methods.MultiMedia.Export_Bitmap_As_Image(bitmapImage, fileNameWithoutExtension, Methods.Path.FolderDcimVideo + id + "/");
                    }
                    else
                    {
                        item.ImageVideo = "";

                        Glide.With(Application.Context)
                            .AsBitmap()
                            .Load(item.Media) // or URI/path
                            .Into(new MySimpleTarget(id, item)); 
                    }
                }
                else if (modelType == MessageModelType.LeftSticker || modelType == MessageModelType.RightSticker)
                {
                    var fileName = item.Media.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDiskSticker, fileName, item.Media);
                }
                else if (modelType == MessageModelType.LeftFile || modelType == MessageModelType.RightFile)
                {
                    var fileName = item.Media.Split('/').Last();
                    item.Media = GetFile(id, Methods.Path.FolderDcimFile, fileName, item.Media);
                }

                var db = Mapper.Map<MessageDataExtra>(item);
                return db;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                var db = Mapper.Map<MessageDataExtra>(item);
                return db;
            }
        }
         
        #region Location >> BindMap

        public static async Task<Address> ReverseGeocodeCurrentLocation(LatLng latLng)
        {
            try
            {
                #pragma warning disable 618
                var locale = (int)Build.VERSION.SdkInt < 25 ? Application.Context.Resources.Configuration.Locale : Application.Context.Resources.Configuration.Locales.Get(0) ?? Application.Context.Resources.Configuration.Locale;
                #pragma warning restore 618

                Geocoder geocode = new Geocoder(Application.Context, locale);

                var addresses = await geocode.GetFromLocationAsync(latLng.Latitude, latLng.Longitude, 2); // Here 1 represent max location result to returned, by documents it recommended 1 to 5
                if (addresses.Count > 0)
                {
                    //string address = addresses[0].GetAddressLine(0); // If any additional address line present than only, check with max available address lines by getMaxAddressLineIndex()
                    //string city = addresses[0].Locality;
                    //string state = addresses[0].AdminArea;
                    //string country = addresses[0].CountryName;
                    //string postalCode = addresses[0].PostalCode;
                    //string knownName = addresses[0].FeatureName; // Only if available else return NULL 
                    return addresses.FirstOrDefault();
                }
                else
                {
                    //Error Message  
                    //Toast.MakeText(MainActivity, MainActivity.GetText(Resource.String.Lbl_Error_DisplayAddress), ToastLength.Short).Show();
                    return null;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        #endregion
         
        // Functions Save Images
        private static void SaveFile(string id, string folder, string fileName, string url)
        {
            try
            {
                if (url.Contains("http"))
                {
                    string folderDestination = folder + id + "/";

                    string filePath = Path.Combine(folderDestination);
                    string mediaFile = filePath + "/" + fileName;

                    if (!File.Exists(mediaFile))
                    {
                        WebClient webClient = new WebClient();

                        webClient.DownloadDataAsync(new Uri(url));
                        webClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                File.WriteAllBytes(mediaFile, e.Result);
                            }
                            catch (Exception exception)
                            {
                                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                            }
                        };
                    }
                } 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        // Functions file from folder
        public static string GetFile(string id, string folder, string filename, string url)
        {
            try
            {
                string folderDestination = folder + id + "/";

                if (!Directory.Exists(folderDestination))
                {
                    if (folder == Methods.Path.FolderDiskStory)
                        Directory.Delete(folder, true);

                    Directory.CreateDirectory(folderDestination);
                }

                string imageFile = Methods.MultiMedia.GetMediaFrom_Gallery(folderDestination, filename);
                if (imageFile == "File Dont Exists")
                {
                    //This code runs on a new thread, control is returned to the caller on the UI thread.
                    Task.Factory.StartNew(() => { SaveFile(id, folder, filename, url); });
                    return url;
                }
                else
                {
                    return imageFile;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return url;
            }
        }
         
        private class MySimpleTarget : CustomTarget
        {
            private readonly string Id;
            private readonly MessageData Item;
            public MySimpleTarget(string id, MessageData item)
            {
                try
                {
                    Id = id;
                    Item = item;
                }
                catch (Exception e)
                {
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            public override void OnResourceReady(Java.Lang.Object resource, ITransition transition)
            {
                try
                {
                    if (Item == null) return;
                
                    var fileName = Item.Media.Split('/').Last();
                    var fileNameWithoutExtension = fileName.Split('.').First();

                    var pathImage = Methods.Path.FolderDcimVideo + Id + "/" + fileNameWithoutExtension + ".png";

                    var videoImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + Id, fileNameWithoutExtension + ".png");
                    if (videoImage == "File Dont Exists")
                    {
                        if (resource is Bitmap bitmap)
                        {
                            Methods.MultiMedia.Export_Bitmap_As_Image(bitmap, fileNameWithoutExtension, Methods.Path.FolderDcimVideo + Id + "/");

                            Java.IO.File file2 = new Java.IO.File(pathImage);
                            var photoUri = FileProvider.GetUriForFile(Application.Context, Application.Context.PackageName + ".fileprovider", file2);
                                 
                            Item.ImageVideo = photoUri.ToString();
                        }
                    }
                    else
                    {
                     
                        Java.IO.File file2 = new Java.IO.File(pathImage);
                        var photoUri = FileProvider.GetUriForFile(Application.Context, Application.Context.PackageName + ".fileprovider", file2);

                        Item.ImageVideo = photoUri.ToString();
                    }
                }
                catch (Exception e)
                {
                    if (Item != null) Item.ImageVideo = "";
                    Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                }
            }

            public override void OnLoadCleared(Drawable p0) { }
        }

        public static string GetDuration(string mediaFile)
        {
            try
            {
                string duration;
                MediaMetadataRetriever retriever;
                if (mediaFile.Contains("http"))
                {
                    retriever = new MediaMetadataRetriever();
                    if ((int)Build.VERSION.SdkInt >= 14)
                        retriever.SetDataSource(mediaFile, new Dictionary<string, string>());
                    else
                        retriever.SetDataSource(mediaFile);

                    duration = retriever.ExtractMetadata(MetadataKey.Duration); //time In Millisec 
                    retriever.Release();
                }
                else
                {
                    var file = Android.Net.Uri.FromFile(new Java.IO.File(mediaFile));
                    retriever = new MediaMetadataRetriever();
                    //if ((int)Build.VERSION.SdkInt >= 14)
                    //    retriever.SetDataSource(file.Path, new Dictionary<string, string>());
                    //else
                    retriever.SetDataSource(file.Path);

                    duration = retriever.ExtractMetadata(MetadataKey.Duration); //time In Millisec 
                    retriever.Release();
                }

                return duration;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return "0";
            }
        }

        public static async Task GetSharedFiles(string id)
        {
            try
            {
                var imagePath = Methods.Path.FolderDcimImage + id;
                var stickerPath = Methods.Path.FolderDiskSticker + id;
                var gifPath = Methods.Path.FolderDiskGif + id;
                var soundsPath = Methods.Path.FolderDcimSound + id;
                var videoPath = Methods.Path.FolderDcimVideo + id;
                var otherPath = Methods.Path.FolderDcimFile + id;

                //Check for folder if exists
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                if (!Directory.Exists(stickerPath))
                    Directory.CreateDirectory(stickerPath);

                if (!Directory.Exists(gifPath))
                    Directory.CreateDirectory(gifPath);

                if (!Directory.Exists(soundsPath))
                    Directory.CreateDirectory(soundsPath);

                if (!Directory.Exists(videoPath))
                    Directory.CreateDirectory(videoPath);

                if (!Directory.Exists(otherPath))
                    Directory.CreateDirectory(otherPath);

                var imageFiles = new DirectoryInfo(imagePath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                var stickerFiles = new DirectoryInfo(stickerPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                var gifFiles = new DirectoryInfo(gifPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                var soundsFiles = new DirectoryInfo(soundsPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                var videoFiles = new DirectoryInfo(videoPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                var otherFiles = new DirectoryInfo(otherPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();

                if (imageFiles.Count > 0)
                {
                    foreach (var dir in from file in imageFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "Image",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = file.FullName,
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (stickerFiles.Count > 0)
                {
                    foreach (var dir in from file in stickerFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "Sticker",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = file.FullName,
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (gifFiles.Count > 0)
                {
                    foreach (var dir in from file in gifFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "Gif",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = file.FullName,
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (soundsFiles.Count > 0)
                {
                    foreach (var dir in from file in soundsFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "Sounds",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = "Audio_File",
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (videoFiles.Count > 0)
                {
                    foreach (var dir in from file in videoFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "Video",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = file.FullName,
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (otherFiles.Count > 0)
                {
                    foreach (var dir in from file in otherFiles
                                        let check = ListUtils.ListSharedFiles.FirstOrDefault(a => a.FileName.Contains(file.Name))
                                        where check == null
                                        select new Classes.SharedFile
                                        {
                                            FileType = "File",
                                            FileName = file.Name,
                                            FileDate = file.LastWriteTime.Millisecond.ToString(),
                                            FilePath = file.FullName,
                                            ImageExtra = "Image_File",
                                            FileExtension = file.Extension,
                                        })
                    {
                        ListUtils.ListSharedFiles.Add(dir);
                    }
                }

                if (ListUtils.ListSharedFiles.Count > 0)
                {
                    //Last 50 File
                    List<Classes.SharedFile> orderByDateList = ListUtils.ListSharedFiles.OrderBy(T => T.FileDate).Take(50).ToList();
                    ListUtils.LastSharedFiles = new ObservableCollection<Classes.SharedFile>(orderByDateList);
                }

                await Task.Delay(0);
                Console.WriteLine(ListUtils.ListSharedFiles);
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }
    }
}