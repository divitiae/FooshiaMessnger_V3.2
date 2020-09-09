using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Widget;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Adapters;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using MessageData = WoWonder.Helpers.Model.MessageDataExtra;

namespace WoWonder.Helpers.Controller
{
    public static class MessageController
    {
        //############# DON'T  MODIFY HERE #############
        private static ChatObject Datauser;
        private static UserDataObject UserData;
        private static GetUsersListObject.User DataUserChat;

        private static ChatWindowActivity WindowActivity;

        private static TabbedMainActivity GlobalContext;
        //========================= Functions =========================
        public static async Task SendMessageTask(ChatWindowActivity windowActivity, string userId, string messageHashId, string text = "", string contact = "", string filePath = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string productId = "", string lat = "", string lng = "")
        {
            try
            {
                WindowActivity = windowActivity;
                if (windowActivity.DataUser != null)
                    Datauser = windowActivity.DataUser;
                else if (windowActivity.UserData != null)
                    UserData = windowActivity.UserData;
                else if (windowActivity.DataUserChat != null)
                    DataUserChat = windowActivity.DataUserChat;

                GlobalContext = TabbedMainActivity.GetInstance();

                StartApiService(userId, messageHashId, text, contact, filePath, imageUrl, stickerId, gifUrl, productId, lat, lng);

                await Task.Delay(0);
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        private static void StartApiService(string userId, string messageHashId, string text = "", string contact = "", string filePath = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string productId = "", string lat = "", string lng = "")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(WindowActivity, WindowActivity?.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SendMessage(userId, messageHashId, text, contact, filePath, imageUrl, stickerId, gifUrl, productId, lat, lng)});
        }

        private static async Task SendMessage(string userId, string messageHashId, string text = "", string contact = "", string filePath = "", string imageUrl = "", string stickerId = "", string gifUrl = "", string productId = "", string lat = "", string lng = "")
        {
            var (apiStatus, respond) = await RequestsAsync.Message.Send_Message(userId, messageHashId, text, contact, filePath, imageUrl, stickerId, gifUrl, productId, lat, lng);
            if (apiStatus == 200)
            {
                if (respond is SendMessageObject result)
                {
                    UpdateLastIdMessage(result);
                }
            }
            else Methods.DisplayReportResult(WindowActivity, respond);
        }

        public static async void UpdateLastIdMessage(SendMessageObject chatMessages)
        {
            try
            { 
                foreach (var messageInfo in chatMessages.MessageData)
                { 
                    MessageData m = new MessageData
                    {
                        Id = messageInfo.Id,
                        FromId = messageInfo.FromId,
                        GroupId = messageInfo.GroupId,
                        ToId = messageInfo.ToId,
                        Text = messageInfo.Text,
                        Media = messageInfo.Media,
                        MediaFileName = messageInfo.MediaFileName,
                        MediaFileNames = messageInfo.MediaFileNames,
                        Time = messageInfo.Time,
                        Seen = messageInfo.Seen,
                        DeletedOne = messageInfo.DeletedOne,
                        DeletedTwo = messageInfo.DeletedTwo,
                        SentPush = messageInfo.SentPush,
                        NotificationId = messageInfo.NotificationId,
                        TypeTwo = messageInfo.TypeTwo,
                        Stickers = messageInfo.Stickers,
                        TimeText = messageInfo.TimeText,
                        Position = messageInfo.Position,
                        ModelType = messageInfo.ModelType,
                        FileSize = messageInfo.FileSize,
                        MediaDuration = messageInfo.MediaDuration,
                        MediaIsPlaying = messageInfo.MediaIsPlaying,
                        ContactNumber = messageInfo.ContactNumber,
                        ContactName = messageInfo.ContactName,
                        ProductId = messageInfo.ProductId,
                        MessageUser = messageInfo.MessageUser,
                        Product = messageInfo.Product,
                        MessageHashId = messageInfo.MessageHashId,
                        Lat = messageInfo.Lat,
                        Lng = messageInfo.Lng,
                        SendFile = false,
                    };

                    var typeModel = Holders.GetTypeModel(m);
                    if (typeModel == MessageModelType.None)
                        continue;

                    var message = await WoWonderTools.MessageFilter(messageInfo.ToId, m, typeModel, true); 
                    message.ModelType = typeModel;

                    AdapterModelsClassMessage checker = WindowActivity?.MAdapter?.DifferList?.FirstOrDefault(a => a.MesData.Id == messageInfo.MessageHashId);
                    if (checker != null)
                    {
                        //checker.TypeView = typeModel;
                        checker.MesData = message;
                        checker.Id = Java.Lang.Long.ParseLong(message.Id);
                        checker.TypeView = typeModel;

                        checker.MesData.Id = message.Id;
                        checker.MesData.FromId = message.FromId;
                        checker.MesData.GroupId = message.GroupId;
                        checker.MesData.ToId = message.ToId;
                        checker.MesData.Text = message.Text;
                        checker.MesData.Media = message.Media;
                        checker.MesData.MediaFileName = message.MediaFileName;
                        checker.MesData.MediaFileNames = message.MediaFileNames;
                        checker.MesData.Time = message.Time;
                        checker.MesData.Seen = message.Seen;
                        checker.MesData.DeletedOne = message.DeletedOne;
                        checker.MesData.DeletedTwo = message.DeletedTwo;
                        checker.MesData.SentPush = message.SentPush;
                        checker.MesData.NotificationId = message.NotificationId;
                        checker.MesData.TypeTwo = message.TypeTwo;
                        checker.MesData.Stickers = message.Stickers;
                        checker.MesData.TimeText = message.TimeText;
                        checker.MesData.Position = message.Position;
                        checker.MesData.ModelType = message.ModelType;
                        checker.MesData.FileSize = message.FileSize;
                        checker.MesData.MediaDuration = message.MediaDuration;
                        checker.MesData.MediaIsPlaying = message.MediaIsPlaying;
                        checker.MesData.ContactNumber = message.ContactNumber;
                        checker.MesData.ContactName = message.ContactName;
                        checker.MesData.ProductId = message.ProductId;
                        checker.MesData.MessageUser = message.MessageUser;
                        checker.MesData.Product = message.Product;
                        checker.MesData.MessageHashId = message.MessageHashId;
                        checker.MesData.Lat = message.Lat;
                        checker.MesData.Lng = message.Lng;
                        checker.MesData.SendFile = false;
                        
                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var updaterUser = GlobalContext?.LastChatTab?.MAdapter?.ChatList?.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(GlobalContext.LastChatTab.MAdapter.ChatList.FirstOrDefault(x => x.UserId == message.ToId));
                                if (index > -1)
                                {
                                    updaterUser.LastMessage.LastMessageClass.Text = typeModel switch
                                    {
                                        MessageModelType.RightGif => WindowActivity?.GetText(Resource.String.Lbl_SendGifFile),
                                        MessageModelType.RightText => !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : WindowActivity?.GetText(Resource.String.Lbl_SendMessage),
                                        MessageModelType.RightSticker => WindowActivity?.GetText(Resource.String.Lbl_SendStickerFile), 
                                        MessageModelType.RightContact => WindowActivity?.GetText(Resource.String.Lbl_SendContactnumber),
                                        MessageModelType.RightFile => WindowActivity?.GetText(Resource.String.Lbl_SendFile),
                                        MessageModelType.RightVideo => WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile),
                                        MessageModelType.RightImage => WindowActivity?.GetText(Resource.String.Lbl_SendImageFile),
                                        MessageModelType.RightAudio => WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile),
                                        MessageModelType.RightMap => WindowActivity?.GetText(Resource.String.Lbl_SendLocationFile),
                                        _ => updaterUser.LastMessage.LastMessageClass.Text
                                    };

                                    GlobalContext.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastChatTab?.MAdapter?.ChatList.Move(index, 0);
                                            GlobalContext?.LastChatTab?.MAdapter?.NotifyItemMoved(index, 0);
                                            GlobalContext?.LastChatTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                                        }
                                    });
                                    
                                    SqLiteDatabase dbSqLite = new SqLiteDatabase();
                                    //Update All data users to database
                                    dbSqLite.Insert_Or_Update_LastUsersChat(GlobalContext, new ObservableCollection<ChatObject>() { updaterUser });
                                    dbSqLite.Dispose();
                                }
                            }
                            else
                            {
                                //insert new user  
                                var data = ConvertData(checker.MesData);
                                if (data != null)
                                {
                                    GlobalContext?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastChatTab.MAdapter.ChatList.Insert(0, data);
                                            GlobalContext?.LastChatTab.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastChatTab.MRecycler.ScrollToPosition(GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(data));
                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                                        }
                                    });

                                    //Update All data users to database
                                    //dbDatabase.Insert_Or_Update_LastUsersChat(new ObservableCollection<GetUsersListObject.User>
                                    //{
                                    //    data
                                    //});
                                }
                            }
                        }
                        else
                        {
                            var updaterUser = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.IndexOf(GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.FirstOrDefault(x => x.UserId == message.ToId));
                                if (index > -1)
                                {
                                    updaterUser.LastMessage.Text = typeModel switch
                                    {
                                        MessageModelType.RightGif => WindowActivity?.GetText(Resource.String.Lbl_SendGifFile),
                                        MessageModelType.RightText => !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : WindowActivity?.GetText(Resource.String.Lbl_SendMessage),
                                        MessageModelType.RightSticker => WindowActivity?.GetText(Resource.String.Lbl_SendStickerFile),
                                        MessageModelType.RightContact => WindowActivity?.GetText(Resource.String.Lbl_SendContactnumber),
                                        MessageModelType.RightFile => WindowActivity?.GetText(Resource.String.Lbl_SendFile),
                                        MessageModelType.RightVideo => WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile),
                                        MessageModelType.RightImage => WindowActivity?.GetText(Resource.String.Lbl_SendImageFile),
                                        MessageModelType.RightAudio => WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile),
                                        MessageModelType.RightMap => WindowActivity?.GetText(Resource.String.Lbl_SendLocationFile),
                                        _ => updaterUser.LastMessage.Text
                                    };

                                    GlobalContext.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser.Move(index, 0);
                                            GlobalContext?.LastMessagesTab?.MAdapter?.NotifyItemMoved(index, 0);
                                            GlobalContext?.LastMessagesTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                                        }
                                    });
                                    SqLiteDatabase dbSqLite = new SqLiteDatabase();
                                    //Update All data users to database
                                    dbSqLite.Insert_Or_Update_LastUsersChat(GlobalContext, new ObservableCollection<GetUsersListObject.User>() { updaterUser });
                                    dbSqLite.Dispose();
                                }
                            }
                            else
                            {
                                //insert new user  
                                var data = ConvertDataChat(checker.MesData);
                                if (data != null)
                                {
                                    GlobalContext?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastMessagesTab?.MAdapter.MLastMessagesUser.Insert(0, data);
                                            GlobalContext?.LastMessagesTab?.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastMessagesTab?.MRecycler.ScrollToPosition(GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.IndexOf(data));
                                        }
                                        catch (Exception e)
                                        {
                                            Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                                        }
                                    });

                                    //Update All data users to database
                                    //dbDatabase.Insert_Or_Update_LastUsersChat(new ObservableCollection<GetUsersListObject.User>
                                    //{
                                    //    data
                                    //});
                                }
                            }
                        }
                    
                        //checker.Media = media;
                        //Update All data users to database
                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_Or_Update_To_one_MessagesTable(checker.MesData);
                        dbDatabase.Dispose();

                        GlobalContext?.RunOnUiThread(() =>
                        {
                            try
                            {
                                //Update data RecyclerView Messages.
                                if (message.ModelType == MessageModelType.RightSticker || message.ModelType == MessageModelType.RightImage || message.ModelType == MessageModelType.RightMap || message.ModelType == MessageModelType.RightVideo)
                                    WindowActivity?.Update_One_Messages(checker.MesData);

                                if (SettingsPrefFragment.SSoundControl)
                                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                            }
                            catch (Exception e)
                            {
                                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                            }
                        }); 
                    }
                }

                Datauser = null;
                DataUserChat = null;
                UserData = null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private static GetUsersListObject.User ConvertDataChat(MessageData ms)
        {
            try
            {
                if (DataUserChat != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = DataUserChat.Username,
                        Avatar = DataUserChat.Avatar,
                        Cover = DataUserChat.Cover,
                        LastseenTimeText = DataUserChat.LastseenTimeText,
                        Lastseen = DataUserChat.Lastseen,
                        Url = DataUserChat.Url,
                        Name = DataUserChat.Name,
                        LastseenUnixTime = DataUserChat.LastseenUnixTime,
                        ChatColor = AppSettings.MainColor,
                        Verified = DataUserChat.Verified,
                        LastMessage = new GetUsersListObject.LastMessage()
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers,
                            Lat = ms.Lat,
                            Lng = ms.Lng,
                            ProductId = ms.ProductId,  
                        }
                    };
                    return user;
                }

                if (Datauser != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = Datauser.Username,
                        Avatar = Datauser.Avatar,
                        Cover = Datauser.Cover,
                        Lastseen = Datauser.Lastseen,
                        Url = Datauser.Url,
                        Name = Datauser.Name,
                        LastseenUnixTime = Datauser.LastseenUnixTime,
                        Verified = Datauser.Verified,
                        LastMessage = new GetUsersListObject.LastMessage
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers,
                            Lat = ms.Lat,
                            Lng = ms.Lng,
                            ProductId = ms.ProductId,
                        }
                    };
                    return user;
                }
                if (UserData != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = UserData.Username,
                        Avatar = UserData.Avatar,
                        Cover = UserData.Cover,
                        LastseenTimeText = UserData.LastseenTimeText,
                        Lastseen = UserData.Lastseen,
                        Url = UserData.Url,
                        Name = UserData.Name,
                        LastseenUnixTime = UserData.LastseenUnixTime,
                        ChatColor = AppSettings.MainColor,
                        Verified = UserData.Verified,
                        LastMessage = new GetUsersListObject.LastMessage()
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers,
                            Lat = ms.Lat,
                            Lng = ms.Lng,
                            ProductId = ms.ProductId,
                        }
                    };
                    return user;
                }

                return DataUserChat;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }

        private static ChatObject ConvertData(MessageData ms)
        {
            try
            {
                if (Datauser != null)
                {
                    ChatObject user = new ChatObject
                    {
                        UserId = ms.ToId,
                        Username = Datauser.Username,
                        Avatar = Datauser.Avatar,
                        Cover = Datauser.Cover,
                        Lastseen = Datauser.Lastseen,
                        Url = Datauser.Url,
                        Name = Datauser.Name,
                        LastseenUnixTime = Datauser.LastseenUnixTime,
                        Verified = Datauser.Verified,
                        LastMessage = ms
                    };
                    user.LastMessage.LastMessageClass.ChatColor = ms.ChatColor ?? AppSettings.MainColor;

                    return user;
                }
                if (UserData != null)
                {
                    ChatObject user = new ChatObject
                    {
                        UserId = ms.ToId,
                        Username = UserData.Username,
                        Avatar = UserData.Avatar,
                        Cover = UserData.Cover,
                        Lastseen = UserData.Lastseen,
                        Url = UserData.Url,
                        Name = UserData.Name,
                        LastseenUnixTime = UserData.LastseenUnixTime,
                        Verified = UserData.Verified,
                        LastMessage = ms,
                    };
                    user.LastMessage.LastMessageClass.ChatColor = ms.ChatColor ?? AppSettings.MainColor;
                    return user;
                }

                return null;
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
                return null;
            }
        }
          
        public static void UpdateRecyclerLastMessageView(GetUsersListObject.LastMessage result, GetUsersListObject.User user, int index, TabbedMainActivity context)
        {
            try
            {
                if (IsImageExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendImageFile);
                }
                else if (IsVideoExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile);
                }
                else if (IsAudioExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile);
                }
                else if (IsFileExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendFile);
                }
                else if (result.MediaFileName.Contains(".gif") || result.MediaFileName.Contains(".GIF"))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendGifFile);
                }
                else
                {
                    result.Text = Methods.FunString.DecodeString(result.Text);
                }

                context.RunOnUiThread(() =>
                {
                    try
                    {
                        switch (AppSettings.LastChatSystem)
                        {
                            case SystemApiGetLastChat.New:
                                context.LastChatTab?.MAdapter?.NotifyItemChanged(index);
                                break;
                            default:
                                context.LastMessagesTab?.MAdapter?.NotifyItemChanged(index);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace); 
                    } 
                });

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                //Update All data users to database
                dbDatabase.Insert_Or_Update_LastUsersChat(GlobalContext,new ObservableCollection<GetUsersListObject.User>() { user });
                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);

            }
        }

        private static readonly string[] ImageValidExtensions = { ".jpg", ".bmp", ".gif", ".png", ".jpeg", ".tif" };
        private static readonly string[] VideoValidExtensions = { ".mp4", ".avi", ".mov", ".flv", ".wmv", ".divx", ".mpeg", ".mpeg2" };
        private static readonly string[] AudioValidExtensions = { ".mp3", ".wav", ".aiff", ".pcm", ".wmv" };
        private static readonly string[] FilesValidExtensions = { ".zip", ".pdf", ".doc", ".xml", ".txt" };

        public static bool IsImageExtension(string text)
        {
            return ImageValidExtensions.Any(text.Contains);
        }

        public static bool IsVideoExtension(string text)
        {
            return VideoValidExtensions.Any(text.Contains);
        }
        public static bool IsAudioExtension(string text)
        {
            return AudioValidExtensions.Any(text.Contains);
        }

        public static bool IsFileExtension(string text)
        {
            return FilesValidExtensions.Any(text.Contains);
        }
    }
}