using ForegroundServiceDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ForegroundServiceDemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPositionPage : ContentPage
    {
        public ReportPositionPage()
        {
            InitializeComponent();
            BindingContext = new ReportPositionViewModel();
        }
    }
}