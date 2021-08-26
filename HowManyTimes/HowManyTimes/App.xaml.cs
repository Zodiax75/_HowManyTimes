using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HowManyTimes.Views;
using HowManyTimes.Services;

namespace HowManyTimes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
            Device.SetFlags(new string[] { "Shapes_Experimental" });

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            _ = DBService.Database;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
