using Realms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ForegroundServiceDemo.Helpers
{
    public static class DatabaseHelper
    {
        public static Realm GetDatabaseInstance()
        {
            var config = new RealmConfiguration() { SchemaVersion = ulong.Parse($"{Math.Abs(AppInfo.Version.Major)}{Math.Abs(AppInfo.Version.Minor)}{Math.Abs(AppInfo.Version.Revision)}") };
            return Realm.GetInstance(config);
        }
        public static Task<Realm> GetDatabaseInstanceAsync()
        {
            var config = new RealmConfiguration() { SchemaVersion = ulong.Parse($"{Math.Abs(AppInfo.Version.Major)}{Math.Abs(AppInfo.Version.Minor)}{Math.Abs(AppInfo.Version.Revision)}") };
            return Realm.GetInstanceAsync(config);
        }
    }
}
