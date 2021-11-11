using HowManyTimes.ViewModels;
using System;
using Xamarin.Forms;
using System.Collections.Generic;
using HowManyTimes.Models;
using HowManyTimes.Services;
using System.Linq;
using System.Threading.Tasks;

namespace HowManyTimes.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();

            mainHeader.BackgroundColor = Color.Transparent;
        }

        /// <summary>
        /// Handler for PLUS button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnNewCounterClicked(object sender, EventArgs e)
        {
            if (newCounter.IsVisible)
            {
                newCounter.FadeTo(0, 1500);
                newCounter.IsVisible = false;
            }
            else
            {
                newCounter.IsVisible = true;
                newCounter.FadeTo(1, 1500);
            }
        }

        public async void OnSwipped(object sender, EventArgs e)
        {
            // TODO: rework to use Command and bind it to ViewModel
            await Application.Current.MainPage.Navigation.PushAsync(new DetailPage(), true);
        }

        public async void OnSearchTapped(object sender, EventArgs e)
        {
            // hide if shown and show if hidden
            searchFrame.IsVisible = !searchFrame.IsVisible;

            // set icon color
            switch(searchFrame.IsVisible)
            {
                case true:
                    searchIcon.TextColor = Color.FromHex("#FF8920");
                    break;
                case false:
                    searchIcon.TextColor = Color.FromHex("#6F6F6F");
                    break;
            }
        }
    }
}
