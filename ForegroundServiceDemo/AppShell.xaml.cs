using ForegroundServiceDemo.ViewModels;
using ForegroundServiceDemo.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ForegroundServiceDemo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ReportPositionPage), typeof(ReportPositionPage));
            Routing.RegisterRoute(nameof(UploadPhotoPage), typeof(UploadPhotoPage));
        }

    }
}
