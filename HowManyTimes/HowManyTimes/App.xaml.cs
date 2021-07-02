using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HowManyTimes.Views;

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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
