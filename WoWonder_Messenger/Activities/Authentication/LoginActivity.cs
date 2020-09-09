using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Newtonsoft.Json;
using Org.Json;
using WoWonder.Activities.Tab;
using WoWonder.Activities.WalkTroutPage;
using WoWonder.Frameworks.onesignal;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.SocialLogins;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Console = System.Console;
using Object = Java.Lang.Object;
using Task = System.Threading.Tasks.Task;

namespace WoWonder.Activities.Authentication
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode)]
    public class LoginActivity : AppCompatActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IResultCallback
    {
        #region Variables Basic

        private LinearLayout ToolBarLayout;
        private Button MButtonViewSignIn, RegisterButton, MGoogleSignIn;
        private EditText UsernameEditText, PasswordEditText;
        private LinearLayout MainLinearLayout;
        private TextView TopTittle, ForgetPass, TermsOfService, Privacy; 
        private ProgressBar ProgressBar; 
        private LoginButton BtnFbLogin;
        private ICallbackManager MFbCallManager;
        private FbMyProfileTracker ProfileTracker; 
        public static GoogleApiClient MGoogleApiClient; 
        private string TimeZone = AppSettings.CodeTimeZone;
        private bool IsActiveUser = true;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.Main);

                Client a = new Client(AppSettings.TripleDesAppServiceProvider);
                Console.WriteLine(a);

                //Get Value And Set Toolbar
                InitComponent();
                InitSocialLogins();

                //OneSignal Notification  
                //====================================== 
                if (string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.RegisterNotificationDevice();

                if (Methods.CheckConnectivity())
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> {() => ApiRequest.GetSettings_Api(this), GetTimezone});
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

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                if (MGoogleApiClient != null && MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
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

        protected override void OnDestroy()
        {
            try
            {
                ProfileTracker?.StopTracking();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                var currentNightMode = newConfig.UiMode & UiMode.NightMask;
                switch (currentNightMode)
                {
                    case UiMode.NightNo:
                        // Night mode is not active, we're using the light theme
                        AppSettings.SetTabDarkTheme = false;
                        break;
                    case UiMode.NightYes:
                        // Night mode is active, we're using dark theme
                        AppSettings.SetTabDarkTheme = true;
                        break;
                }

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
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
                //Get values
                ToolBarLayout = FindViewById<LinearLayout>(Resource.Id.toolbarLayout);
                UsernameEditText = FindViewById<EditText>(Resource.Id.usernamefield);
                PasswordEditText = FindViewById<EditText>(Resource.Id.passwordfield);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                MButtonViewSignIn = FindViewById<Button>(Resource.Id.loginButton);
                RegisterButton = FindViewById<Button>(Resource.Id.signUpButton);
                MainLinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);
                TopTittle = FindViewById<TextView>(Resource.Id.titile);
                TopTittle.Text = AppSettings.ApplicationName;
                
                ForgetPass = FindViewById<TextView>(Resource.Id.forgetpassButton);
                TermsOfService = FindViewById<TextView>(Resource.Id.secTermTextView);
                Privacy = FindViewById<TextView>(Resource.Id.secPrivacyTextView);

                ProgressBar.Visibility = ViewStates.Invisible;

                if (!AppSettings.EnableRegisterSystem)
                    RegisterButton.Visibility = ViewStates.Gone;
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
                    ToolBarLayout.Click += ToolBarLayout_Click;
                    ForgetPass.Click += ForgetPassOnClick; 
                    TermsOfService.Click += TermsOfServiceOnClick;
                    Privacy.Click += PrivacyOnClick;
                    MainLinearLayout.Click += Main_LoginPage_Click;
                    RegisterButton.Click += RegisterButton_Click;
                    MButtonViewSignIn.Click += BtnLoginOnClick;

                }
                else
                {
                    //Close Event
                    ToolBarLayout.Click -= ToolBarLayout_Click;
                    ForgetPass.Click -= ForgetPassOnClick; 
                    TermsOfService.Click -= TermsOfServiceOnClick;
                    Privacy.Click -= PrivacyOnClick;
                    MainLinearLayout.Click -= Main_LoginPage_Click;
                    RegisterButton.Click -= RegisterButton_Click;
                    MButtonViewSignIn.Click -= BtnLoginOnClick;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void ToolBarLayout_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private void InitSocialLogins()
        {
            try
            {
                //#Facebook
                if (AppSettings.ShowFacebookLogin)
                {
                    //FacebookSdk.SdkInitialize(this);

                    ProfileTracker = new FbMyProfileTracker();
                    ProfileTracker.MOnProfileChanged += ProfileTrackerOnMOnProfileChanged;
                    ProfileTracker.StartTracking();

                    BtnFbLogin = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    BtnFbLogin.Visibility = ViewStates.Visible;
                    BtnFbLogin.SetPermissions(new List<string>
                    {
                        "email",
                        "public_profile"
                    });

                    MFbCallManager = CallbackManagerFactory.Create();
                    BtnFbLogin.RegisterCallback(MFbCallManager, this);

                    //FB accessToken
                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                    string hash = Methods.App.GetKeyHashesConfigured(this);
                    Console.WriteLine(hash);
                }
                else
                {
                    BtnFbLogin = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                    BtnFbLogin.Visibility = ViewStates.Gone;
                }

                //#Google
                if (AppSettings.ShowGoogleLogin)
                {
                   MGoogleSignIn = FindViewById<Button>(Resource.Id.Googlelogin_button);
                    MGoogleSignIn.Click += GoogleSignInButtonOnClick;
                }
                else
                {
                    MGoogleSignIn = FindViewById<Button>(Resource.Id.Googlelogin_button);
                    MGoogleSignIn.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        //Login With Facebook
        private void ProfileTrackerOnMOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.MProfile != null)
                {
                    //var FbFirstName = e.MProfile.FirstName;
                    //var FbLastName = e.MProfile.LastName;
                    //var FbName = e.MProfile.Name;
                    //var FbProfileId = e.MProfile.Id;

                    var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                    var parameters = new Bundle();
                    parameters.PutString("fields", "id,name,age_range,email");
                    request.Parameters = parameters;
                    request.ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }
         
        //Login With Google
        private void GoogleSignInButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(AppSettings.ClientId)
                    .RequestScopes(new Scope(Scopes.Profile))
                    .RequestScopes(new Scope(Scopes.PlusMe))
                    .RequestScopes(new Scope(Scopes.DriveAppfolder))
                    .RequestServerAuthCode(AppSettings.ClientId)
                    .RequestProfile().RequestEmail().Build();

                MGoogleApiClient ??= new GoogleApiClient.Builder(this, this, this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
                 
                MGoogleApiClient.Connect();

                var opr = Auth.GoogleSignInApi.SilentSignIn(MGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);

                    //Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(this);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.
                    opr.SetResultCallback(new SignInResultCallback { Activity = this });
                }

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    if (!MGoogleApiClient.IsConnecting)
                        ResolveSignInError();
                    else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.GetAccounts) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.UseCredentials) == Permission.Granted)
                    {
                        if (!MGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(106);
                    }
                }
            }
            catch (Exception ex)
            {
               Methods.DisplayReportResultTrack(ex.Message + " \n  " + ex.StackTrace);
            }
        }

        #endregion

        #region Events

        public override void OnBackPressed()
        {
            try
            {
                Finish();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }

        }

        //Click Button Login
        private async void BtnLoginOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                        GetText(Resource.String.Lbl_CheckYourInternetConnection), GetText(Resource.String.Lbl_Ok));
                }
                else
                {
                    if (!string.IsNullOrEmpty(UsernameEditText.Text.Replace(" ", "")) || !string.IsNullOrEmpty(PasswordEditText.Text))
                    {
                        HideKeyboard();

                        ProgressBar.Visibility = ViewStates.Visible;
                        MButtonViewSignIn.Visibility = ViewStates.Gone;

                        if (string.IsNullOrEmpty(TimeZone))
                           await GetTimezone();

                        var (apiStatus, respond) = await RequestsAsync.Global.Get_Auth(UsernameEditText.Text.Replace(" ", ""), PasswordEditText.Text, TimeZone, UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is AuthObject auth)
                            {
                                var emailValidation = ListUtils.SettingsSiteList?.EmailValidation ?? "0";
                                if (emailValidation == "1")
                                {
                                    IsActiveUser = await CheckIsActiveUser(auth.UserId);
                                }

                                if (IsActiveUser)
                                {
                                    SetDataLogin(auth);

                                    if (AppSettings.ShowWalkTroutPage)
                                    {
                                        Intent newIntent = new Intent(this, typeof(AppIntroWalkTroutPage));
                                        newIntent.PutExtra("class", "login");
                                        StartActivity(newIntent);
                                    }
                                    else
                                    {
                                        StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                                    }

                                    ProgressBar.Visibility = ViewStates.Gone;
                                    MButtonViewSignIn.Visibility = ViewStates.Visible;
                                    FinishAffinity();
                                }
                                else
                                { 
                                    ProgressBar.Visibility = ViewStates.Gone;
                                    MButtonViewSignIn.Visibility = ViewStates.Visible;
                                }
                            }
                            else if (respond is AuthMessageObject messageObject)
                            {
                                UserDetails.Username = UsernameEditText.Text;
                                UserDetails.FullName = UsernameEditText.Text;
                                UserDetails.Password = PasswordEditText.Text;
                                UserDetails.UserId = messageObject.UserId;
                                UserDetails.Status = "Pending";
                                UserDetails.Email = UsernameEditText.Text;

                                //Insert user data to database
                                var user = new DataTables.LoginTb
                                {
                                    UserId = UserDetails.UserId,
                                    AccessToken = "",
                                    Cookie = "",
                                    Username = UsernameEditText.Text,
                                    Password = PasswordEditText.Text,
                                    Status = "Pending",
                                    Lang = "",
                                    DeviceId = UserDetails.DeviceId,
                                };
                                ListUtils.DataUserLoginList.Add(user);

                                var dbDatabase = new SqLiteDatabase();
                                dbDatabase.InsertOrUpdateLogin_Credentials(user);
                                dbDatabase.Dispose();

                                Intent newIntent = new Intent(this, typeof(VerificationCodeActivity));
                                newIntent.PutExtra("TypeCode", "TwoFactor");
                                StartActivity(newIntent);
                            }
                        }
                        else if (apiStatus == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                var errorText = error.Error.ErrorText;
                                var errorId = error.Error.ErrorId;
                                if (errorId == "3")
                                    Methods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_3), GetText(Resource.String.Lbl_Ok));
                                else if (errorId == "4")
                                    Methods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_4), GetText(Resource.String.Lbl_Ok));
                                else if (errorId == "5")
                                    Methods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_5), GetText(Resource.String.Lbl_Ok));
                                else
                                    Methods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security), errorText,
                                        GetText(Resource.String.Lbl_Ok));
                            }

                            ProgressBar.Visibility = ViewStates.Gone;
                            MButtonViewSignIn.Visibility = ViewStates.Visible;
                        }
                        else if (apiStatus == 404)
                        {
                            ProgressBar.Visibility = ViewStates.Gone;
                            MButtonViewSignIn.Visibility = ViewStates.Visible;
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Gone;
                        MButtonViewSignIn.Visibility = ViewStates.Visible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                            GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                MButtonViewSignIn.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private void PrivacyOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/privacy-policy";
                Methods.App.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private void TermsOfServiceOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/terms";
                Methods.App.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }
        private void ForgetPassOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                StartActivity(typeof(ForgetPasswordActivity)); 
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(RegisterActivity));
                OverridePendingTransition(Resource.Animation.abc_grow_fade_in_from_bottom,Resource.Animation.abc_shrink_fade_out_from_bottom);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private void Main_LoginPage_Click(object sender, EventArgs e)
        {
            try
            {
                HideKeyboard();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                Log.Debug("Login_Activity", "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);
                if (requestCode == 0)
                {
                    var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    HandleSignInResult(result);
                } 
                else
                {
                    // Logins Facebook
                    MFbCallManager.OnActivityResult(requestCode, (int)resultCode, data);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (!MGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (MGoogleApiClient.IsConnected) MGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        #endregion

        #region Social Logins

        private string FbAccessToken, GAccessToken, GServerCode;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                ProgressBar.Visibility = ViewStates.Gone;
                MButtonViewSignIn.Visibility = ViewStates.Visible;

                SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {

                ProgressBar.Visibility = ViewStates.Gone;
                MButtonViewSignIn.Visibility = ViewStates.Visible;

                // Handle exception
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error.Message, GetText(Resource.String.Lbl_Ok));

                SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                //var loginResult = result as LoginResult;
                //var id = AccessToken.CurrentAccessToken.UserId;

                ProgressBar.Visibility = ViewStates.Visible;
                MButtonViewSignIn.Visibility = ViewStates.Gone;

                SetResult(Result.Ok);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                var data = json.ToString();
                var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                //var FbEmail = result.Email;
                Console.WriteLine(result);
                 
                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FbAccessToken = accessToken.Token;

                    var (apiStatus, respond) = await RequestsAsync.Global.Get_SocialLogin(FbAccessToken, "facebook", UserDetails.DeviceId);
                    if (apiStatus == 200)
                    {
                        if (respond is AuthObject auth)
                        {
                            SetDataLogin(auth);

                            if (AppSettings.ShowWalkTroutPage)
                            {
                                Intent newIntent = new Intent(this, typeof(AppIntroWalkTroutPage));
                                newIntent.PutExtra("class", "login");
                                StartActivity(newIntent);
                            }
                            else
                            {
                                StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                            }
                            Finish();
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.Error.ErrorText;

                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));

                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error, GetText(Resource.String.Lbl_Ok));
                    }

                    ProgressBar.Visibility = ViewStates.Gone;
                    MButtonViewSignIn.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception exception)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                MButtonViewSignIn.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        #endregion

        //======================================================

        #region Google

        public void HandleSignInResult(GoogleSignInResult result)
        {
            try
            {
                Log.Debug("Login_Activity", "handleSignInResult:" + result.IsSuccess);
                if (result.IsSuccess)
                {
                    // Signed in successfully, show authenticated UI.
                    var acct = result.SignInAccount;
                    SetContentGoogle(acct);
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private void ResolveSignInError()
        {
            try
            {
                if (MGoogleApiClient.IsConnecting) return;

                var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(MGoogleApiClient);
                StartActivityForResult(signInIntent, 0);
            }
            catch (IntentSender.SendIntentException io)
            {
                //The intent was cancelled before it was sent. Return to the default
                //state and attempt to connect to get an updated ConnectionResult
                Console.WriteLine(io);
                MGoogleApiClient.Connect();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            try
            {
                var opr = Auth.GoogleSignInApi.SilentSignIn(MGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.

                    opr.SetResultCallback(new SignInResultCallback { Activity = this });
                }
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        private async void SetContentGoogle(GoogleSignInAccount acct)
        {
            try
            {
                //Successful log in hooray!!
                if (acct != null)
                {
                    ProgressBar.Visibility = ViewStates.Visible;
                    MButtonViewSignIn.Visibility = ViewStates.Gone;

                    //var GAccountName = acct.Account.Name;
                    //var GAccountType = acct.Account.Type;
                    //var GDisplayName = acct.DisplayName;
                    //var GFirstName = acct.GivenName;
                    //var GLastName = acct.FamilyName;
                    //var GProfileId = acct.Id;
                    //var GEmail = acct.Email;
                    //var GImg = acct.PhotoUrl.Path;
                    GAccessToken = acct.IdToken;
                    GServerCode = acct.ServerAuthCode;
                     
                    if (!string.IsNullOrEmpty(GAccessToken))
                    {
                        var (apiStatus, respond) = await RequestsAsync.Global.Get_SocialLogin(GAccessToken, "google", UserDetails.DeviceId);
                        if (apiStatus == 200)
                        {
                            if (respond is AuthObject auth)
                            {
                                SetDataLogin(auth);

                                if (AppSettings.ShowWalkTroutPage)
                                {
                                    Intent newIntent = new Intent(this, typeof(AppIntroWalkTroutPage));
                                    newIntent.PutExtra("class", "login");
                                    StartActivity(newIntent);
                                }
                                else
                                {
                                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                                }
                                Finish();
                            }
                        }
                        else if (apiStatus == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                var errorText = error.Error.ErrorText;

                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                            }
                        }
                        else if (apiStatus == 404)
                        {
                            var error = respond.ToString();
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), error, GetText(Resource.String.Lbl_Ok));
                        }

                        ProgressBar.Visibility = ViewStates.Gone;
                        MButtonViewSignIn.Visibility = ViewStates.Visible; 
                    }
                }
            }
            catch (Exception exception)
            {
                ProgressBar.Visibility = ViewStates.Gone;
                MButtonViewSignIn.Visibility = ViewStates.Visible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message, GetText(Resource.String.Lbl_Ok));
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            try
            {
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            try
            {
                // An unresolvable error has occurred and Google APIs (including Sign-In) will not
                // be available.
                Log.Debug("Login_Activity", "onConnectionFailed:" + result);

                //The user has already clicked 'sign-in' so we attempt to resolve all
                //errors until the user is signed in, or the cancel
                ResolveSignInError();
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        public void OnResult(Object result)
        {
            try
            {

            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        }

        #endregion

        //======================================================

        #endregion

        private void HideKeyboard()
        {
            try
            {
                var inputManager = (InputMethodManager)GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private async Task GetTimezone()
        {
            try
            {
                TimeZone = await ApiRequest.GetTimeZoneAsync();
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
            }
        }

        private async Task<bool> CheckIsActiveUser(string userId)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Global.IsActiveUser(userId);
                if (apiStatus == 200)
                {
                    if (respond is MessageObject auth)
                    {
                        Console.WriteLine(auth);
                        return true;
                    }
                }
                else if (apiStatus == 400)
                {
                    if (respond is ErrorObject error)
                    {
                        var errorText = error.Error.ErrorText;
                        var errorId = error.Error.ErrorId;
                        if (errorId == "5")
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_ThisUserNotActive), GetText(Resource.String.Lbl_Ok));
                        else if (errorId == "4")
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), GetText(Resource.String.Lbl_UserNotFound), GetText(Resource.String.Lbl_Ok));
                        else
                            Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errorText, GetText(Resource.String.Lbl_Ok));
                    }
                }
                else if (apiStatus == 404)
                {
                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), respond.ToString(), GetText(Resource.String.Lbl_Ok));
                }

                return false;
            }
            catch (Exception exception)
            {
                Methods.DisplayReportResultTrack(exception.Message + " \n  " + exception.StackTrace);
                return false;
            }
        }


        private void SetDataLogin(AuthObject auth)
        {
            try
            {
                Current.AccessToken = auth.AccessToken;

                UserDetails.Username = UsernameEditText.Text;
                UserDetails.FullName = UsernameEditText.Text;
                UserDetails.Password = PasswordEditText.Text;
                UserDetails.AccessToken = auth.AccessToken;
                UserDetails.UserId = auth.UserId;
                UserDetails.Status = "Pending";
                UserDetails.Cookie = auth.AccessToken;
                UserDetails.Email = UsernameEditText.Text;

                //Insert user data to database
                var user = new DataTables.LoginTb
                {
                    UserId = UserDetails.UserId,
                    AccessToken = UserDetails.AccessToken,
                    Cookie = UserDetails.Cookie,
                    Username = UsernameEditText.Text,
                    Password = UsernameEditText.Text,
                    Status = "Pending",
                    Lang = "",
                    DeviceId = UserDetails.DeviceId,
                    Email = UserDetails.Email,
                };
                ListUtils.DataUserLoginList.Clear();
                ListUtils.DataUserLoginList.Add(user);

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.InsertOrUpdateLogin_Credentials(user);
                dbDatabase.Dispose();

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.Get_MyProfileData_Api(this) });
            }
            catch (Exception e)
            {
                Methods.DisplayReportResultTrack(e.Message + " \n  " + e.StackTrace);
            }
        } 
    }
}