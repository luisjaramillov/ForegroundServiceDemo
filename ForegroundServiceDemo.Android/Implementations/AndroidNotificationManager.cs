using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using ForegroundServiceDemo.EventsArgs;
using ForegroundServiceDemo.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(ForegroundServiceDemo.Droid.Implementations.AndroidNotificationManager))]
namespace ForegroundServiceDemo.Droid.Implementations
{
    public class AndroidNotificationManager : INotificationManager
    {
        const string channelId = "notificacionLocal";
        const string channelName = "Aplicación";
        const string channelDescription = "Notificaciones emitidas por la aplicación.";
        const int pendingIntentId = 0;

        public const string TitleKey = "title";
        public const string MessageKey = "message";

        int messageId = -1;
        NotificationManager manager;

        public event EventHandler NotificationReceived;

        public void Initialize()
        {
            CreateNotificationChannel();
        }

        public int ScheduleNotification(string title, string message)
        {
            try
            {
                CreateNotificationChannel();

                messageId++;

                Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
                intent.PutExtra(TitleKey, title);
                intent.PutExtra(MessageKey, message);

                PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);
                NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                    .SetContentIntent(pendingIntent)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.icon_about))
                    .SetSmallIcon(Resource.Drawable.icon_about);
                if (Preferences.Get("CanSound", true) && Preferences.Get("CanVibrate", true))
                    builder.SetDefaults((int)NotificationDefaults.All);
                else if (Preferences.Get("CanSound", true))
                    builder.SetDefaults((int)NotificationDefaults.Sound);
                else if (Preferences.Get("CanVibrate", true))
                    builder.SetDefaults((int)NotificationDefaults.Vibrate);
                var notification = builder.Build();
                manager.Notify(messageId, notification);

                return messageId;
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
                return -1;
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            try
            {
                var args = new NotificationEventArgs()
                {
                    Title = title,
                    Message = message,
                };
                NotificationReceived?.Invoke(null, args);
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        public void RebuildNotificationChannel()
        {
            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
            manager.DeleteNotificationChannel(channelId);
            CreateNotificationChannel();
        }

        public void CancelNotification()
        {
            try
            {
                if (manager is null)
                {
                    CreateNotificationChannel();
                }
                manager.CancelAll();
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
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
                    var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                    {
                        Description = channelDescription
                    };
                    channel.EnableVibration(true);
                    manager.CreateNotificationChannel(channel);
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
    }
}