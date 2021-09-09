using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HowManyTimes.Views;
using HowManyTimes.Services;
using HowManyTimes.Models;
using System.Collections.Generic;

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

        protected override async void OnStart()
        {
            Category categories = await DBService.GetCategory(2);
            string s = categories.ToString();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
