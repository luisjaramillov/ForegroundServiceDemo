using ForegroundServiceDemo.Helpers;
using ForegroundServiceDemo.Interfaces;
using ForegroundServiceDemo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ForegroundServiceDemo.ViewModels
{
    public class ReportPositionViewModel : BaseViewModel
    {
        public ICommand IniciarCommand => new Command(async () => await ExecuteIniciarCommand());
        public ICommand DetenerCommand => new Command(() => ExecuteDetenerCommand());
        public ICommand RefresCommand => new Command(() => ExecuteRefreshCommand());

        public ObservableCollection<Alarmas> Items { get; set; }

        private bool _IsChecked;
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                SetProperty(ref _IsChecked, value);
                Preferences.Set("ReportsRealTimeLocation", value);
            }
        }

        public ReportPositionViewModel()
        {
            Title = "Reportar posición";
            Items = new ObservableCollection<Alarmas>();
        }

        private async Task ExecuteIniciarCommand()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
                await Permissions.RequestAsync<Permissions.LocationAlways>();

            status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status == PermissionStatus.Granted)
                DependencyService.Get<ISyncService>().StartService();
        }

        private void ExecuteDetenerCommand()
        {
            DependencyService.Get<ISyncService>().StopService();
        }

        private void ExecuteRefreshCommand()
        {
            var realm = DatabaseHelper.GetDatabaseInstance();
            var alarmas = realm.All<AlertRealm>();
            Items.Clear();
            foreach (var item in alarmas)
            {
                Items.Add(new Alarmas
                {
                    Event = item.Event,
                    Date = item.Date,
                    Location = $"Lat: {item.Latitude}, Lng: {item.Longitude}"
                });
            }
        }
    }

    public class Alarmas
    {
        public string Id { get; set; }
        public string Event { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
    }
}
