using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace HowManyTimes.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnNewCounterClicked(object sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(async () => await AnimateMe());
        }

        private async Task AnimateMe()
        {
            new Animation
        {
            { 0.5, 0.8, new Animation(v => hashImage.FadeTo(0)) },
            { 0.99, 1, new Animation(v => hashImage.IsVisible = false) }
        }.Commit(this, "leftArrowTransitionAnimation", 60, 350, Easing.Linear);

            await Task.Delay(750);

            await Application.Current.MainPage.Navigation.PushAsync(new DetailPage(), false);
        }
    }
}
