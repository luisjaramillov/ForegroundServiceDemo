using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using ForegroundServiceDemo.Droid.Implementations;
using ForegroundServiceDemo.Droid.Listeners;
using ForegroundServiceDemo.Helpers;
using ForegroundServiceDemo.Models;
using ForegroundServiceDemo.Services;
using Realms;
using Xamarin.Essentials;
using AndroidApp = Android.App.Application;

namespace ForegroundServiceDemo.Droid.Services
{
    [Service]
    public class SyncService : Android.App.Service, Android.Locations.ILocationListener
    {
        bool isStarted;
        Handler handler;
        Action runnable;
        const string channelId = "sincronizacion";
        const string channelName = "Servicio de sincronización";
        const string channelDescription = "Notificaciones emitidas por el servicio de sincronización.";
        readonly AndroidNotificationManager notificationManager = new AndroidNotificationManager();
        NotificationManager manager;
        bool channelInitialized = false;
        Timer LocalizationTimer;
        readonly RestApiClient _api = new RestApiClient();
        string user;
        private bool _IsInitialized = false;
        private LocationManager _locationManager;
        FusedLocationProviderClient _locationService;
        FusedLocationProviderCallback _providerCallback;

        public override void OnCreate()
        {
            base.OnCreate();
            handler = new Handler();
            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
            {
                try
                {
                    bool ReportsRealTimeLocation = Preferences.Get("ReportsRealTimeLocation", false);
                    if (ReportsRealTimeLocation)
                    {
                        GPS_StatusChange();
                    }
                    else
                    {
                        if (LocalizationTimer is null)
                        {
                            try
                            {
                                //Solicita la localizacion cada minuto
                                LocalizationTimer = new Timer
                                {
                                    Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
                                };
                                LocalizationTimer.Elapsed += (sender, args) => { ShareLocation(user); };
                                LocalizationTimer.Start();
                            }
                            catch (Exception ex)
                            {
                                //Crashes.TrackError(ex);
                            }
                        }
                    }
                    if (!_IsInitialized)
                    {
                        Connectivity.ConnectivityChanged += async (sender, args) => { await Connectivity_ConnectivityChanged(sender, args); };
                        _IsInitialized = true;
                    }
                    Intent i = new Intent(Constants.NOTIFICATION_BROADCAST_ACTION);
                    i.PutExtra(Constants.BROADCAST_MESSAGE_KEY, "Mensaje");
                    Android.Support.V4.Content.LocalBroadcastManager.GetInstance(this).SendBroadcast(i);
                    handler.PostDelayed(runnable, (long)TimeSpan.FromMinutes(3).TotalMilliseconds);
                }
                catch (Exception e)
                {
                    //Crashes.TrackError(e);
                }
            });
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (intent.Action.Equals(Constants.ACTION_START_SERVICE) && !isStarted)
                {
                    RegisterForegroundService();
                    handler.Post(runnable);
                    isStarted = true;
                }
                // This tells Android not to restart the service if it is killed to reclaim resources.
                return StartCommandResult.Sticky;
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
                return StartCommandResult.RedeliverIntent;
            }
        }
        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }
        public override void OnDestroy()
        {
            try
            {
                // Stop the handler.
                handler.RemoveCallbacks(runnable);
                LocalizationTimer.Stop();
                // Remove the notification from the status bar.
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Cancel(Constants.SERVICE_RUNNING_NOTIFICATION_ID);

                isStarted = false;
                base.OnDestroy();
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        void RegisterForegroundService()
        {
            try
            {
                if (!channelInitialized)
                {
                    CreateNotificationChannel();
                }
                //Android.Net.Uri soundUri = Android.Net.Uri.Parse($"{ContentResolver.SchemeAndroidResource}://{AndroidApp.Context.PackageName}/{Resource.Raw.notification}");
                var notification = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                    .SetContentTitle("ForegroundServiceDemo")
                    .SetContentText("Servicio de Sincronización")
                    .SetSmallIcon(Resource.Drawable.icon_about)
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    //.SetSound(soundUri)
                    .SetOngoing(true)
                    .Build();
                // Enlist this instance of the service as a foreground service
                StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            try
            {
                var notificationIntent = new Intent(this, typeof(MainActivity));
                notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
                notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
                notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

                var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
                return pendingIntent;
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
                return null;
            }
        }
        void CreateNotificationChannel()
        {
            try
            {
                manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelNameJava = new Java.Lang.String(channelName);
                    var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.High)
                    {
                        Description = channelDescription
                    };
                    channel.EnableVibration(true);
                    manager.CreateNotificationChannel(channel);
                }
                channelInitialized = true;
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        async Task Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {
                    // Aquí puedes reiniciar tu proceso de subida o sicronización
                }
                catch (Exception ex)
                {
                    //Crashes.TrackError(ex);
                }
            }
            if (current == NetworkAccess.None)
            {
                try
                {
                    var realm = DatabaseHelper.GetDatabaseInstance();
                    try
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();
                        if (location != null)
                        {

                            realm.Write(() =>
                            {
                                realm.Add(new AlertRealm
                                {
                                    Id = "1",
                                    Event = "Sin acceso a internet",
                                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Latitude = location.Latitude,
                                    Longitude = location.Longitude
                                });
                            });
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx)
                    {
                        //Crashes.TrackError(fnsEx);
                    }
                    catch (FeatureNotEnabledException fneEx)
                    {
                        //Crashes.TrackError(fneEx);
                    }
                    catch (PermissionException pEx)
                    {
                        //Crashes.TrackError(pEx);
                    }
                    catch (Exception ex)
                    {
                        //Crashes.TrackError(ex);
                    }
                }
                catch (Exception ex)
                {
                    //Crashes.TrackError(ex);
                }
            }
        }
        private void ShareLocation(string User)
        {
            try
            {
                using var otherRealm = DatabaseHelper.GetDatabaseInstance();
                var location = Geolocation.GetLastKnownLocationAsync().GetAwaiter().GetResult();
                if (location != null)
                {
                    otherRealm.Write(() =>
                    {
                        otherRealm.Add(new AlertRealm
                        {
                            Id = User,
                            Event = "location",
                            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Latitude = location.Latitude,
                            Longitude = location.Longitude
                        });
                    });
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        
        void GPS_StatusChange()
        {
            try
            {
                if (_locationService is null)
                {
                    _locationManager = (LocationManager)GetSystemService(Context.LocationService);
                    Criteria criteriaForLocationService = new Criteria
                    {
                        Accuracy = Accuracy.Fine
                    };
                    IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);
                    if (acceptableLocationProviders.Any())
                    {
                        _locationManager.RequestLocationUpdates(acceptableLocationProviders.First(), 0, 0, this);
                    }
                    _locationService = LocationServices.GetFusedLocationProviderClient(this);
                    var locationRequest = new LocationRequest();
                    locationRequest.SetInterval(180000);
                    locationRequest.SetFastestInterval(90000);
                    locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
                    _providerCallback = new FusedLocationProviderCallback();
                    _locationService.RequestLocationUpdates(locationRequest, _providerCallback, Looper.MyLooper());
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        public void OnProviderDisabled(string provider)
        {
            if (provider.Equals("gps"))
            {
                notificationManager.ScheduleNotification("ForegroundServiceDemo", "Active la localización.");
                try
                {
                    var realm = DatabaseHelper.GetDatabaseInstance();
                    realm.Write(() =>
                    {
                        realm.Add(new AlertRealm
                        {
                            Id = "1",
                            Event = "Geolocalización apagada",
                            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Latitude = 0,
                            Longitude = 0
                        });
                    });
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    Console.WriteLine(fnsEx.Message);
                }
                catch (FeatureNotEnabledException fneEx)
                {
                    Console.WriteLine(fneEx.Message);
                }
                catch (PermissionException pEx)
                {
                    Console.WriteLine(pEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void OnProviderEnabled(string provider)
        {
            if (provider.Equals("gps"))
                notificationManager.CancelNotification();
        }
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {
            //throw new NotImplementedException();
        }
    }

    class LatLng
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
}