using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ForegroundServiceDemo.Droid.Services;
using ForegroundServiceDemo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(ForegroundServiceDemo.Droid.AndroidSyncService))]
namespace ForegroundServiceDemo.Droid
{
    public class AndroidSyncService: ISyncService
    {
        Intent startServiceIntent;
        
        public void StartService()
        {
            try
            {
                startServiceIntent = new Intent(Android.App.Application.Context, typeof(SyncService));
                startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
                Android.App.Application.Context.StartService(startServiceIntent);
            }
            catch (System.Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        
        public void StopService()
        {
            try
            {
                Android.App.Application.Context.StopService(startServiceIntent);
            }
            catch (System.Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
        
        public void ValidateBatteryOptimization()
        {
            try
            {
                Intent intent = new Intent();
                string packageName = Android.App.Application.Context.PackageName;
                PowerManager pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
                if (pm.IsIgnoringBatteryOptimizations(packageName))
                    intent.SetAction(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
                else
                {
                    intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                    intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                }
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (System.Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
    }
}