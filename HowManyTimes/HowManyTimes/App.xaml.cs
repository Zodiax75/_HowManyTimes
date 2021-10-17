using HowManyTimes.Services;
using HowManyTimes.Views;
using System;
using Xamarin.Forms;
using HowManyTimes.Models;

namespace HowManyTimes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // set log message repository
            LogService.LogRepository = Shared.LogRepository.Console;

            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
            Device.SetFlags(new string[] { "Shapes_Experimental" });

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
