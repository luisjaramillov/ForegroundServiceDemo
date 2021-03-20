using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Gms.Location;
using System;
using System.Linq;
using ForegroundServiceDemo.Services;
using ForegroundServiceDemo.Helpers;
using ForegroundServiceDemo.Models;

namespace ForegroundServiceDemo.Droid.Listeners
{
    public class FusedLocationProviderCallback : LocationCallback
    {
        readonly RestApiClient _api = new RestApiClient();
        public override void OnLocationResult(LocationResult result)
        {
            try
            {
                using var otherRealm = DatabaseHelper.GetDatabaseInstance();

                if (result.LastLocation != null)
                {
                    otherRealm.Write(() =>
                    {
                        otherRealm.Add(new AlertRealm
                        {
                            Id = "1",
                            Event = "location",
                            Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            Latitude = result.LastLocation.Latitude,
                            Longitude = result.LastLocation.Longitude
                        });
                    });
                }
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
    }
}